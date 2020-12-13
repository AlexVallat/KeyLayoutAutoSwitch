Unicode true
SetCompressor lzma

!include "MUI2.nsh"
!include "DotNetChecker.nsh"
!include "nsProcess.nsh"

!define INSTALL_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\KeyLayoutAutoSwitch.exe"
!define HOMEPAGE "https://github.com/AlexVallat/KeyLayoutAutoSwitch"
!define EXE_FILENAME "KeyLayoutAutoSwitch.exe"
!define SOURCE_PATH "..\KeyLayoutAutoSwitch\bin\Release"
!define UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\KeyLayoutAutoSwitch"  

Name "KeyLayoutAutoSwitch"
OutFile "../Releases/KeyLayoutAutoSwitch-Install.exe"
InstallDir "$PROGRAMFILES64\KeyLayoutAutoSwitch"
InstallDirRegKey HKLM "${INSTALL_DIR_REGKEY}" ""

!insertmacro MUI_PAGE_DIRECTORY
!define MUI_COMPONENTSPAGE_NODESC
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\${EXE_FILENAME}"
!define MUI_FINISHPAGE_RUN_TEXT "Configure KeyLayoutAutoSwitch now"
!define MUI_FINISHPAGE_LINK "http://keylayoutautoswitch.byalexv.co.uk"
!define MUI_FINISHPAGE_LINK_LOCATION "http://keylayoutautoswitch.byalexv.co.uk"
!insertmacro MUI_PAGE_FINISH

Section "KeyLayoutAutoSwitch Program Files" SecCore
	SectionIn RO
	SetOutPath "$INSTDIR"
	!insertmacro CheckNetFramework 472
	
	File "${SOURCE_PATH}\${EXE_FILENAME}"
	File "${SOURCE_PATH}\CommandLine.dll"
	File "${SOURCE_PATH}\ObjectListView.dll"
SectionEnd

Section "Add icon to Start Menu"
	CreateShortCut "$SMPROGRAMS\KeyLayoutAutoSwitch.lnk" "$INSTDIR\${EXE_FILENAME}"
SectionEnd
Section "Start with Windows"
	CreateShortCut "$SMSTARTUP\KeyLayoutAutoSwitch.lnk" "$INSTDIR\${EXE_FILENAME}" "--minimized"
SectionEnd

!macro Language Culture
	Section "-Lang-${Culture}"
		SetOutPath "$INSTDIR\${Culture}"
		File "${SOURCE_PATH}\${Culture}\KeyLayoutAutoSwitch.resources.dll"
		SetOutPath "$INSTDIR"
	SectionEnd
	Section "-un.Lang-${Culture}"
		Delete "$INSTDIR\${Culture}\KeyLayoutAutoSwitch.resources.dll"
		RMDir "$INSTDIR\${Culture}"
	SectionEnd
!macroend

#!insertmacro Language "qps-ploc"
!insertmacro Language "bg"

Section -Post
	WriteUninstaller "$INSTDIR\uninstall.exe"
	WriteRegStr HKLM "${INSTALL_DIR_REGKEY}" "" "$INSTDIR\${EXE_FILENAME}"

	WriteRegStr HKLM "${UNINST_KEY}" "DisplayName" "$(^Name)"
	WriteRegStr HKLM "${UNINST_KEY}" "UninstallString" "$INSTDIR\uninstall.exe"
	WriteRegStr HKLM "${UNINST_KEY}" "DisplayIcon" "$INSTDIR\${EXE_FILENAME}"
	WriteRegStr HKLM "${UNINST_KEY}" "Publisher" "Alex Vallat"
	WriteRegStr HKLM "${UNINST_KEY}" "HelpLink" "${HOMEPAGE}"
	WriteRegDword HKLM "${UNINST_KEY}" "NoModify" "1"
	WriteRegDword HKLM "${UNINST_KEY}" "NoRepair" "1"
	
	SectionGetSize ${SecCore} $0
	IntFmt $0 "0x%08X" $0
	WriteRegDWORD HKLM "${UNINST_KEY}" "EstimatedSize" "$0"
SectionEnd

# Uninstall

!insertmacro MUI_UNPAGE_COMPONENTS
!insertmacro MUI_UNPAGE_INSTFILES

Section "un.KeyLayoutAutoSwitch Program Files"
	SectionIn RO
	
	${nsProcess::FindProcess} "${EXE_FILENAME}" $R0
	StrCmp $R0 0 0 endclose
	DetailPrint "Closing KeyLayoutAutoSwitch..."
	${nsProcess::CloseProcess} "${EXE_FILENAME}" $R0
	# Try a few times to wait for it to close
	${nsProcess::FindProcess} "${EXE_FILENAME}" $R0
	StrCmp $R0 0 0 endclose
	${nsProcess::FindProcess} "${EXE_FILENAME}" $R0
	StrCmp $R0 0 0 endclose
	${nsProcess::FindProcess} "${EXE_FILENAME}" $R0
	StrCmp $R0 0 0 endclose
	DetailPrint "Unable to close KeyLayoutAutoSwitch"
	endclose:
	${nsProcess::Unload}
	
	Delete "$SMPROGRAMS\KeyLayoutAutoSwitch.lnk"
	Delete "$SMSTARTUP\KeyLayoutAutoSwitch.lnk"

	Delete "$INSTDIR\${EXE_FILENAME}"
	Delete "$INSTDIR\CommandLine.dll"
	Delete "$INSTDIR\ObjectListView.dll"
	
	Delete "$INSTDIR\uninstall.exe"
	RMDir "$INSTDIR"
	
	DeleteRegKey HKLM "${UNINST_KEY}"
SectionEnd

Section /o "un.Remove all settings"
	RMDir /r "$APPDATA\KeyLayoutAutoSwitch"
	
	DeleteRegKey HKLM "${INSTALL_DIR_REGKEY}"
SectionEnd

!insertmacro MUI_LANGUAGE "English"