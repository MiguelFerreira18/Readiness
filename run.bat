@echo off

cd /d "%~dp0"

if /I "%~1"=="docker" goto start_docker
if /I "%~1"=="--d" goto start_docker
if /I "%~1"=="-d" goto start_docker
goto check_migrations

:start_docker
docker compose up -d
echo Waiting for db to come alive
timeout /t 10 /nobreak >nul
echo Done
if errorlevel 1 exit /b %errorlevel%

:check_migrations
if not exist "Migrations\" (
    dotnet ef migrations add initMigration
    if errorlevel 1 exit /b %errorlevel%
)

dotnet run
