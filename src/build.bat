@echo off
echo [== Compiling XenOS ==]
dotnet build
if %ERRORLEVEL% NEQ 0 (goto error)
goto eof

:error
pause
exit

:eof
echo [== DONE ==]
