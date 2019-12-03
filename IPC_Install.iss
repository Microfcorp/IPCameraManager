; Скрипт создан через Мастер Inno Setup Script.
; ИСПОЛЬЗУЙТЕ ДОКУМЕНТАЦИЮ ДЛЯ ПОДРОБНОСТЕЙ ИСПОЛЬЗОВАНИЯ INNO SETUP!

#define MyAppName "IPCamera Manager"
#define MyAppVersion "1.5"
#define MyAppPublisher "Microf Corporation"
#define MyAppURL "http://microfcorp.ml/"
#define MyAppExeName "IPCamera.exe"

[ISFormDesigner]
WizardForm=FF0A005457495A415244464F524D0030104504000054504630F10B5457697A617264466F726D0A57697A617264466F726D0C436C69656E744865696768740368010B436C69656E74576964746803F1010C4578706C696369744C65667402000B4578706C69636974546F7002000D4578706C6963697457696474680301020E4578706C69636974486569676874038E010D506978656C73506572496E636802600A54657874486569676874020D00F10C544E65774E6F7465626F6F6B0D4F757465724E6F7465626F6F6B00F110544E65774E6F7465626F6F6B506167650B57656C636F6D65506167650D4578706C69636974576964746803F1010E4578706C696369744865696768740339010000F110544E65774E6F7465626F6F6B5061676509496E6E6572506167650D4578706C69636974576964746803F1010E4578706C6963697448656967687403390100F10C544E65774E6F7465626F6F6B0D496E6E65724E6F7465626F6F6B00F110544E65774E6F7465626F6F6B506167650B4C6963656E7365506167650D4578706C69636974576964746803A1010E4578706C6963697448656967687403ED000000F110544E65774E6F7465626F6F6B506167650D53656C656374446972506167650D4578706C69636974576964746803A1010E4578706C6963697448656967687403ED000000F110544E65774E6F7465626F6F6B506167651653656C65637450726F6772616D47726F7570506167650D4578706C69636974576964746803A1010E4578706C6963697448656967687403ED000000F110544E65774E6F7465626F6F6B506167650F53656C6563745461736B73506167650D4578706C69636974576964746803A1010E4578706C6963697448656967687403ED0000000000F110544E65774E6F7465626F6F6B506167650C46696E6973686564506167650D4578706C69636974576964746803F1010E4578706C6963697448656967687403390100F110544E6577436865636B4C697374426F780752756E4C697374044C65667403D80103546F7003310105576964746802050648656967687402020C4578706C696369744C65667403D8010B4578706C69636974546F700331010D4578706C69636974576964746802050E4578706C6963697448656967687402020000F10F544E6577526164696F427574746F6E074E6F526164696F044C65667403D80103546F7003C90005576964746802050648656967687402010C4578706C696369744C65667403D8010B4578706C69636974546F7003C9000D4578706C69636974576964746802050E4578706C6963697448656967687402010000F10F544E6577526164696F427574746F6E08596573526164696F044C65667403D00103546F7003B300055769647468020D064865696768740201074F6E436C69636B070D596573526164696F436C69636B0C4578706C696369744C65667403D0010B4578706C69636974546F7003B3000D4578706C696369745769647468020D0E4578706C6963697448656967687402010000000000

[Setup]
; Примечание: Значение AppId идентифицирует это приложение.
; Не используйте одно и тоже значение в разных установках.
; (Для генерации значения GUID, нажмите Инструменты | Генерация GUID.)
AppId={{2E34B61C-A021-4F8B-8F44-F833B8A39831}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\IPCameraManager
DefaultGroupName=IPCamera
AllowNoIcons=yes
OutputDir=C:\Users\Лехап\OneDrive\Документы\IPC
OutputBaseFilename=IPCamera_Install
Compression=lzma
SolidCompression=yes

[Languages]
Name: english; MessagesFile: compiler:Languages\English.isl
Name: russian; MessagesFile: compiler:Languages\Russian.isl

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked
Name: quicklaunchicon; Description: {cm:CreateQuickLaunchIcon}; GroupDescription: {cm:AdditionalIcons}; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\IPCamera.exe; DestDir: {app}; Flags: ignoreversion
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\IPCamera.exe; DestDir: {app}; Flags: ignoreversion
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\convert.exe; DestDir: {app}; Flags: ignoreversion
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\ffmpeg.exe; DestDir: {app}; Flags: ignoreversion
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\ffplay.exe; DestDir: {app}; Flags: ignoreversion
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\HtmlAgilityPack.dll; DestDir: {app}; Flags: ignoreversion
; Примечание: Не используйте "Flags: ignoreversion" для системных файлов
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\DBFile.dll; DestDir: {app}; Flags: IgnoreVersion
Source: C:\Users\Лехап\OneDrive\Документы\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\IPCameraDBParser.exe; DestDir: {app}; Flags: IgnoreVersion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\cvextern.dll; DestDir: {app}; Flags: ignoreversion; Tasks: 
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\Emgu.CV.UI.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\Emgu.CV.UI.GL.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\Emgu.CV.World.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\haarcascade_eye.xml; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\haarcascade_frontalface_alt.xml; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\haarcascade_frontalface_default.xml; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\opencv_ffmpeg341_64.dll; DestDir: {app}; Flags: ignoreversion
Source: ..\Visual Studio 2015\Projects\IPCamera\IPCamera\bin\Debug\ZedGraph.dll; DestDir: {app}; Flags: ignoreversion

[Icons]
Name: {group}\{#MyAppName}; Filename: {app}\{#MyAppExeName}
Name: {group}\{cm:ProgramOnTheWeb,{#MyAppName}}; Filename: {#MyAppURL}
Name: {group}\{cm:UninstallProgram,{#MyAppName}}; Filename: {uninstallexe}
Name: {commondesktop}\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: desktopicon
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}; Filename: {app}\{#MyAppExeName}; Tasks: quicklaunchicon

[Run]
Filename: {app}\{#MyAppExeName}; Description: {cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}; Flags: nowait postinstall skipifsilent

[Code]

procedure RedesignWizardForm;
begin
  with WizardForm.RunList do
  begin
    Left := ScaleX(472);
    Top := ScaleY(305);
    Width := ScaleX(5);
    Height := ScaleY(2);
  end;

  with WizardForm.NoRadio do
  begin
    Left := ScaleX(472);
    Top := ScaleY(201);
    Width := ScaleX(5);
    Height := ScaleY(1);
  end;

  with WizardForm.YesRadio do
  begin
    Left := ScaleX(464);
    Top := ScaleY(179);
    Width := ScaleX(13);
    Height := ScaleY(1);
  end;

{ ReservationBegin }
  // Вы можете добавить ваш код здесь.

{ ReservationEnd }
end;
// Не изменять эту секцию. Она создана автоматически.
{ RedesignWizardFormEnd } // Не удалять эту строку!

procedure InitializeWizard();
begin
  RedesignWizardForm;
end;
