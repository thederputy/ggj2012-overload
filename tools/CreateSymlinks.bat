@echo off

if [%1]==[] (
	echo.Enter the Dropbox directory to link to e.g. C:\DropBox\Overload
	set /P DROPBOX_DIR=
) else (
	set DROPBOX_DIR=%1
)

REM remove any quotes or trailing backslash
set DROPBOX_DIR=%DROPBOX_DIR:"=%
if “%DROPBOX_DIR:~-1%”==”\” set DROPBOX_DIR=%DROPBOX_DIR:~0,-1%

if not exist "%DROPBOX_DIR%" (
	echo Error The directory does not seem to exist
	goto ERROR
)

set SOLUTION_FILE="*.sln"
REM in case the bat was run from the buildscripts folder
if not exist "%SOLUTION_FILE%" cd ..\..

if not exist "%SOLUTION_FILE%" (
	echo Can't find Solution file, please make sure the script is being run from the root of the project
	goto ERROR
)

set BIN_DIR=%DROPBOX_DIR%\builds
set BIN_LINK=builds


rmdir "%BIN_LINK%"
if ERRORLEVEL 1 goto ERROR
mklink /J "%BIN_LINK%" "%BIN_DIR%"
if ERRORLEVEL 1 goto ERROR

goto EOF

:ERROR
pause

:EOF