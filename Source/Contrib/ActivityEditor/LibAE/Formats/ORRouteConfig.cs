﻿// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

/// This module ...
/// 
/// Author: Stéfan Paitoni
/// Updates : 
/// 

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ORTS.Common;
using MSTS;
using LibAE.Common;
using MSTS.Formats;
using MSTS.Parsers;

namespace LibAE.Formats
{
    /// <summary>
    /// ORRouteConfig is the main class to access the OpenRail specific data for a route.  These data complete the MSTS one in terms of Station
    /// and Station's connectors to track.
    /// The data are saved in json file into the main repository of the route.
    /// </summary>

    public class ORRouteConfig
    {
        [JsonProperty("FileName")]
        public string FileName;
        [JsonProperty("RoutePath")]
        public string RoutePath { get; protected set; }
        [JsonProperty("GlobalItem")]
        public List<GlobalItem> routeItems;    //  Only the items linked to the route Metadata
        [JsonProperty("RouteName")]
        public string RouteName { get; protected set; }

        [JsonIgnore]
        public List<GlobalItem> AllItems { get; protected set; }       //  All the items, include the activity items, exclude the MSTS Item, not saved
        [JsonIgnore]
        public bool toSave = false;
        [JsonIgnore]
        public AETraveller traveller { get; protected set; }
        [JsonIgnore]
        AETraveller searchTraveller;
        [JsonIgnore]
        public MSTSBase TileBase { get; set; }
        [JsonIgnore]
        public int a;

        /// <summary>
        /// The class constructor, but, don't use it.  Prefer to use the static method 'LoadConfig' wich return this object
        /// </summary>
        public ORRouteConfig()
        {
            AllItems = new List<GlobalItem>();
            routeItems = new List<GlobalItem>();
            RouteName = "";
            TileBase = new MSTSBase();
        }

        /// <summary>
        /// SetTileBase is used to initialize the TileBase for the route.  This information is then used to 'reduce' the value of the
        /// MSTS Coordinate wich are too big to be correctly shown in the editor
        /// </summary>
        /// <param name="tileBase"></param>
        public void SetTileBase(MSTSBase tileBase)
        {
            TileBase = tileBase;
        }

        public List<StationItem> GetStationItem()
        {
            List<StationItem> stationList = new List<StationItem>();
            foreach (var item in routeItems)
            {
                if (typeof(StationItem) == item.GetType() || item.typeItem == (int)TypeItem.STATION_ITEM)
                {
                    stationList.Add((StationItem)item);
                }
            }
            return stationList;
        }

        /// <summary>
        /// Use this function to add a new item into the 'AllItems' list.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(GlobalItem item)
        {
            if (item == null)
                return;
            //if (!(sideItem is PathEventItem) && !(sideItem is SideStartItem))
            if (item.asMetadata)
            {
                if (routeItems.IndexOf(item) < 0)
                    routeItems.Add(item);
            }
            if (AllItems.IndexOf(item) < 0)
                AllItems.Add(item);
            toSave = true;
            if (item.GetType() == typeof(StationItem))
            {
                foreach (StationAreaItem SAItem in ((StationItem)item).stationArea)
                {
                    AllItems.Add(SAItem);
                }
            }
        }

        /// <summary>
        /// Used to remove a connector item from the 'AllItem' list. 
        /// </summary>
        /// <param name="item"></param>
        public void RemoveConnectorItem(GlobalItem item)
        {
            if (item.GetType() == typeof(StationAreaItem))
            {
                AllItems.Remove(item);
                routeItems.Remove(item);
            }
        }

        /// <summary>
        /// Used to remove an item from all list of items: AllItems, RouteItems
        /// </summary>
        /// <param name="item">the item to remove</param>
        public void RemoveItem(GlobalItem item)
        {
            if (item.GetType() == typeof(StationItem))
            {
                foreach (var point in ((StationItem)item).stationArea)
                {
                    RemoveConnectorItem(point);
                }
                ((StationItem)item).stationArea.Clear();
            }
            AllItems.Remove(item);
            routeItems.Remove(item);
            toSave = true;
        }

        public GlobalItem Index(int cnt)
        {
            return AllItems[cnt];
        }

        public bool SaveConfig()
        {
            foreach (var item in AllItems)
            {
                item.Unreduce(TileBase);
            }
            return SerializeJSON();
        }

        public void ReduceItems()
        {
            foreach (var item in AllItems)
            {
                item.Reduce(TileBase);
            }

        }

        static public ORRouteConfig LoadConfig(string fileName, string path, TypeEditor interfaceType)
        {
            string completeFileName = Path.Combine(path, fileName);
            ORRouteConfig loaded = DeserializeJSON(completeFileName, interfaceType);
            return loaded;
        }

        public bool SerializeJSON()
        {
            if (FileName == null || FileName.Length <= 0)
                return false;
            string completeFileName = Path.Combine(RoutePath, FileName);

            JsonSerializer serializer = new JsonSerializer();
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter wr = new StreamWriter(completeFileName))
            {
                using (JsonWriter writer = new JsonTextWriter(wr))
                {
                    serializer.Serialize(writer, this);
                }
            }
            return true;
        }

        static public ORRouteConfig DeserializeJSON(string fileName, TypeEditor interfaceType)
        {
            ORRouteConfig p;

            fileName += ".cfg.json";
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(fileName))
                {
                    ORRouteConfig orRouteConfig = JsonConvert.DeserializeObject<ORRouteConfig>((string)sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    p = orRouteConfig;
                    
                    foreach (var item in p.routeItems)
                    {
                        p.AllItems.Add(item);
                        item.alignEdition(interfaceType, null);
                        if (item.GetType() == typeof(StationItem))
                        {
                            if (((StationItem)item).stationArea.Count > 0)
                            {
                                foreach (var item2 in ((StationItem)item).stationArea)
                                {
                                    ((StationAreaItem)item2).alignEdition(interfaceType, item);
                                }
                                ((StationItem)item).areaCompleted = true;
                            }
                        }
                        else if (item.GetType() == typeof(AEBufferItem))
                        {
                        }
                    }
                    //orRouteConfig.ReduceItems();
                }

            }
            catch (IOException)
            {
                p = new ORRouteConfig();
                p.FileName = Path.GetFileName(fileName);
                p.RoutePath = Path.GetDirectoryName(fileName);
                p.RouteName = "";
                p.toSave = true;

            }
            return p;
        }

        public void SetTraveller(TSectionDatFile TSectionDat, TDBFile TDB)
        {
            TrackNode[] TrackNodes = TDB.TrackDB.TrackNodes;
            traveller = new AETraveller(TSectionDat, TDB);
            foreach (var item in routeItems)
            {
                if (item.GetType() == typeof(StationItem))
                {
                    ((StationItem)item).setTraveller(traveller);
                }
            }

        }

        public void StartSearchPath(TrackPDP startPoint)
        {
            searchTraveller = new AETraveller(this.traveller);
            searchTraveller.place((int)startPoint.TileX, (int)startPoint.TileZ, startPoint.X, startPoint.Z);
        }

        public TrackPDP SearchNextPathNode(TrackPDP endPoint)
        {
            TrItem trItem = null;
            TrackPDP newNode = null;
            trItem = searchTraveller.MoveToNextItem(AllItems, (int)endPoint.TileX, (int)endPoint.TileZ, endPoint.X, endPoint.Z);
            if (trItem != null)
            {
                //newNode = new TrackPDP(trItem);
            }

            return newNode;
        }

        public GlobalItem FindMetadataItem(PointF point, double snapSize, MSTSItems aeItems)
        {
            double positiveInfinity = double.PositiveInfinity;
            double actualDist = double.PositiveInfinity;
            GlobalItem item = null;

            //  First we check only for items except StationItem
            foreach (GlobalItem item2 in AllItems)
            {
                if (item2.GetType() == typeof(StationItem))
                    continue;
                if (!item2.IsEditable() && !item2.IsMovable() && !item2.IsRotable())
                    continue;
                item2.SynchroLocation();
                positiveInfinity = item2.FindItem(point, snapSize, actualDist, aeItems);
                if ((((item != null) && (positiveInfinity <= actualDist)) && ((positiveInfinity == 0.0) || item2.isItSeen())) || (item == null))
                {
                    actualDist = positiveInfinity;
                    item = item2;
                }
            }
            if ((item == null) || (actualDist == double.PositiveInfinity))
            {
                foreach (GlobalItem item2 in AllItems)
                {
                    item2.SynchroLocation();
                    positiveInfinity = item2.FindItem(point, snapSize, actualDist, aeItems);
                    if ((((item != null) && (positiveInfinity <= actualDist)) && ((positiveInfinity == 0.0) || item2.isItSeen())) || (item == null))
                    {
                        actualDist = positiveInfinity;
                        item = item2;
                    }
                }
                if ((item == null) || (actualDist == double.PositiveInfinity))
                {
                    return null;
                }

            }
            item.isSeen = true;
            return item;
        }

        //  Used only in RunActivity
        public StationItem SearchByLocation(WorldLocation location)
        {
            MSTSCoord place = new MSTSCoord(location);
            List<StationItem> listStation = GetStationItem();
            foreach (var item in listStation)
            {
                if (item.IsInStation(place))
                {
                    return item;
                }
            }
            return null;
        }
    }
}