@echo off
setlocal

:: ============================================================
:: WORKPING - PUBLISH SCRIPT
:: Deploy: https://atlink.asia/workping
:: Chay script nay tai thu muc goc: F:\VibleCoding\WorkPing
:: ============================================================

set SOLUTION_DIR=%~dp0
set BACKEND_DIR=%SOLUTION_DIR%Backend\EmployeeManagement.Api\EmployeeManagement.Api
set FRONTEND_DIR=%SOLUTION_DIR%Frontend\employee-management
set PUBLISH_DIR=%SOLUTION_DIR%publish

echo.
echo ============================================================
echo  WORKPING PUBLISH SCRIPT
echo  Output: %PUBLISH_DIR%
echo ============================================================
echo.

:: Xoa thu muc publish cu
if exist "%PUBLISH_DIR%" (
    echo [1/5] Xoa thu muc publish cu...
    rmdir /s /q "%PUBLISH_DIR%"
)
mkdir "%PUBLISH_DIR%"

:: ============================================================
:: BUOC 1: BUILD FRONTEND (Angular)
:: ============================================================
echo [2/5] Build Angular (production)...
cd /d "%FRONTEND_DIR%"

call npm install --silent
if %errorlevel% neq 0 (
    echo [LOI] npm install that bai!
    exit /b 1
)

call npx ng build --configuration production
if %errorlevel% neq 0 (
    echo [LOI] Angular build that bai!
    exit /b 1
)

echo [OK] Angular build thanh cong.

:: ============================================================
:: BUOC 2: PUBLISH BACKEND (ASP.NET Core)
:: ============================================================
echo [3/5] Publish .NET API (production)...
cd /d "%BACKEND_DIR%"

dotnet publish -c Release -r win-x64 --self-contained false -o "%PUBLISH_DIR%" /p:EnvironmentName=Production
if %errorlevel% neq 0 (
    echo [LOI] dotnet publish that bai!
    exit /b 1
)

echo [OK] .NET publish thanh cong.

:: ============================================================
:: BUOC 3: COPY ANGULAR -> wwwroot
:: ============================================================
echo [4/5] Copy Angular dist vao wwwroot...

set FE_DIST=%FRONTEND_DIR%\dist\employee-management\browser
set WWWROOT=%PUBLISH_DIR%\wwwroot

if not exist "%WWWROOT%" mkdir "%WWWROOT%"

xcopy /e /i /y "%FE_DIST%\*" "%WWWROOT%\"
if %errorlevel% neq 0 (
    echo [LOI] Copy Angular dist that bai!
    exit /b 1
)

echo [OK] Copy Angular dist hoan tat.

:: ============================================================
:: BUOC 4: TAO web.config CHO IIS
:: ============================================================
echo [5/5] Tao web.config cho IIS...

(
echo ^<?xml version="1.0" encoding="utf-8"?^>
echo ^<configuration^>
echo   ^<system.webServer^>
echo     ^<handlers^>
echo       ^<add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified"/^>
echo     ^</handlers^>
echo     ^<aspNetCore processPath="dotnet"
echo                 arguments=".\EmployeeManagement.Api.dll"
echo                 stdoutLogEnabled="true"
echo                 stdoutLogFile=".\Logs\stdout"
echo                 hostingModel="inprocess"^>
echo       ^<environmentVariables^>
echo         ^<environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production"/^>
echo       ^</environmentVariables^>
echo     ^</aspNetCore^>
echo     ^<staticContent^>
echo       ^<mimeMap fileExtension=".webmanifest" mimeType="application/manifest+json"/^>
echo     ^</staticContent^>
echo     ^<rewrite^>
echo       ^<rules^>
echo         ^<rule name="API Routes" stopProcessing="true"^>
echo           ^<match url="^api/.*"/^>
echo           ^<action type="None"/^>
echo         ^</rule^>
echo         ^<rule name="Angular SPA" stopProcessing="true"^>
echo           ^<match url=".*"/^>
echo           ^<conditions logicalGrouping="MatchAll"^>
echo             ^<add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true"/^>
echo             ^<add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true"/^>
echo           ^</conditions^>
echo           ^<action type="Rewrite" url="/workping/index.html"/^>
echo         ^</rule^>
echo       ^</rules^>
echo     ^</rewrite^>
echo   ^</system.webServer^>
echo ^</configuration^>
) > "%PUBLISH_DIR%\web.config"

echo [OK] web.config da tao.

:: ============================================================
:: TAO THU MUC LOGS
:: ============================================================
if not exist "%PUBLISH_DIR%\Logs" mkdir "%PUBLISH_DIR%\Logs"

:: ============================================================
:: HOAN TAT
:: ============================================================
echo.
echo ============================================================
echo  PUBLISH HOAN TAT!
echo  Thu muc output: %PUBLISH_DIR%
echo.
echo  HUONG DAN DEPLOY LEN IIS:
echo  1. Copy toan bo noi dung thu muc [publish] len server
echo  2. Tren IIS: Add Application voi alias [workping]
echo     Physical path: ^<duong dan den thu muc publish^>
echo  3. Application Pool: No Managed Code, Integrated pipeline
echo  4. Tao file appsettings.json tren server (khong co trong publish)
echo  5. Dam bao da cai .NET 8 Hosting Bundle tren server
echo  6. Restart IIS site
echo ============================================================
echo.

endlocal
pause
