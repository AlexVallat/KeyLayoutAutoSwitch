@echo off
echo Enter new locale, optionally including culture (for exampe en-US, or fr):
set /p locale=
cd %~dp0%
md %locale%
copy qps-ploc\*.qps-ploc.restext %locale%\????????????????????????????????????????????????????????????????????????????????.%locale%.restext