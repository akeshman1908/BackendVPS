@echo off
REM Stop eventuele draaiende backend-processen
taskkill /F /IM dotnet.exe >nul 2>&1

REM Ga naar de backend-map
cd /d C:\Users\Administrator\Desktop\PortfolioTracker\backend

REM Start de backend met dotnet
dotnet Backend.dll
