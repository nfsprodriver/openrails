;Open Rails installer include file
;17-Apr-2014
;Chris Jakeman

; Included from "OpenRails from DVD\OpenRails from DVD.iss" or from "OpenRails from download\OpenRails from download.iss"

#define MyAppName "Open Rails"
#include "Version.iss"
#define MyAppPublisher "Open Rails"
#define MyAppManualName "Open Rails manual"
#define MyAppSourceName "Download Open Rails source code"
#define MyAppBugName "Report a bug in Open Rails"

#define MyAppURL "http://openrails.org"
#define MyAppSourceURL "http://openrails.org/download/source/"
#define MyAppSupportURL "http://launchpad.net/or"

#define MyAppExeName "OpenRails.exe"
#define MyAppManual "Documentation\Manual.pdf"

#define MyAppProgPath "..\..\..\Open Rails\Program"
#define MyAppDocPath "..\..\..\Open Rails\Documentation"


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, use Tools > Generate GUID.)
AppId={{94E15E08-869D-4B69-B8D7-8C82075CB51C} ; Generated for OpenRails pre-v1.0
AppName         ={#MyAppName}
AppVersion      ={#MyAppVersion}
AppVerName      ={#MyAppName} {#MyAppVersion}
AppPublisher    ={#MyAppPublisher}
AppPublisherURL ={#MyAppURL}
AppSupportURL   ={#MyAppSupportURL}
AppUpdatesURL   ={#MyAppURL}
DefaultDirName  ={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons    =yes
LicenseFile     ={#MyAppProgPath}\Copying.txt
InfoAfterFile   =..\Readme.txt
OutputDir       =Output
OutputBaseFilename={#OutputBaseFilename}
Compression     =lzma
SolidCompression=yes
Uninstallable   =yes
UninstallDisplayIcon={app}\{#MyAppExeName}
; Windows XP SP2
MinVersion      =5.1sp2

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "greek"; MessagesFile: "compiler:Languages\Greek.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "serbiancyrillic"; MessagesFile: "compiler:Languages\SerbianCyrillic.isl"
Name: "serbianlatin"; MessagesFile: "compiler:Languages\SerbianLatin.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
; The game itself
Source: {#MyAppProgPath}\*; Excludes: Readme*.txt; DestDir: {app}; Flags: ignoreversion recursesubdirs
Source: ..\Readme.txt; DestDir: {app}; Flags: ignoreversion
Source: {#MyAppDocPath}\*; DestDir: {app}\Documentation; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppManualName}"; Filename: "{app}\{#MyAppManual}"
Name: "{group}\{#MyAppSourceName}"; Filename: "{#MyAppSourceURL}"
Name: "{group}\{#MyAppBugName}"; Filename: "{#MyAppSupportURL}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; StatusMsg: "Installing Open Rails ..."; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent