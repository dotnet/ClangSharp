@ECHO OFF
pwsh.exe -NoLogo -NoProfile -ExecutionPolicy ByPass -Command "& """%~dp0build.ps1""" -ci %*"
EXIT /B %ERRORLEVEL%
