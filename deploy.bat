@echo off
REM Pizza API - Windows Deployment Helper Script
REM This script helps with deploying to OpenShift from Windows

setlocal enabledelayedexpansion

REM Color codes (Windows 10+)
for /F %%A in ('echo prompt $H ^| cmd') do set "BS=%%A"

set "GREEN="
set "YELLOW="
set "NC="

REM Configuration
set NAMESPACE=%1
if "!NAMESPACE!"=="" set NAMESPACE=default

set REGISTRY=%2
if "!REGISTRY!"=="" set REGISTRY=docker.io

set IMAGE_NAME=%3
if "!IMAGE_NAME!"=="" set IMAGE_NAME=pizza-api

set IMAGE_TAG=%4
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

echo.
echo =====================================================================
echo Pizza API - OpenShift Deployment (Windows)
echo =====================================================================
echo.
echo Namespace: !NAMESPACE!
echo Registry: !REGISTRY!
echo Image: !IMAGE_NAME!:!IMAGE_TAG!
echo.

REM Step 1: Check if oc CLI is installed
echo Step 1: Checking OpenShift CLI...
where oc >nul 2>&1
if errorlevel 1 (
    echo Error: 'oc' CLI not found. Please install OpenShift CLI.
    exit /b 1
)
echo [OK] OpenShift CLI found
echo.

REM Step 2: Check if connected to cluster
echo Step 2: Checking cluster connection...
oc cluster-info >nul 2>&1
if errorlevel 1 (
    echo Error: Not connected to OpenShift cluster. Please login first.
    echo Run: oc login ^<cluster-url^>
    exit /b 1
)
echo [OK] Connected to OpenShift cluster
echo.

REM Step 3: Create namespace
echo Step 3: Setting up namespace...
oc get namespace !NAMESPACE! >nul 2>&1
if errorlevel 1 (
    echo Creating namespace !NAMESPACE!...
    oc create namespace !NAMESPACE!
) else (
    echo Namespace !NAMESPACE! already exists
)
oc project !NAMESPACE!
echo [OK] Namespace ready
echo.

REM Step 4: Apply manifests
echo Step 4: Applying OpenShift manifests...
echo Applying ConfigMap...
oc apply -f Deployment\openshift\06-configmap.yaml
echo Applying BuildConfig...
oc apply -f Deployment\openshift\01-buildconfig.yaml
echo Applying ImageStream...
oc apply -f Deployment\openshift\02-imagestream.yaml
echo [OK] Configuration applied
echo.

REM Step 5: Trigger build
echo Step 5: Starting build...
for /f "tokens=*" %%A in ('oc start-build pizza-api -o jsonpath="{.metadata.name}"') do set BUILD_ID=%%A
echo Build ID: !BUILD_ID!
echo Waiting for build to complete (this may take several minutes)...
oc logs -f build/!BUILD_ID!
echo [OK] Build completed
echo.

REM Step 6: Apply deployment manifests
echo Step 6: Applying deployment manifests...
echo Applying DeploymentConfig...
oc apply -f Deployment\openshift\03-deploymentconfig.yaml
echo Applying Service...
oc apply -f Deployment\openshift\04-service.yaml
echo Applying Route...
oc apply -f Deployment\openshift\05-route.yaml
echo [OK] Deployment manifests applied
echo.

REM Step 7: Wait for rollout
echo Step 7: Waiting for deployment to be ready...
oc rollout status dc/pizza-api --timeout=5m
echo [OK] Deployment is ready
echo.

REM Step 8: Display information
echo =====================================================================
echo Deployment Status
echo =====================================================================
echo.

echo Pods:
oc get pods -l app=pizza-api
echo.

echo Service:
oc get svc pizza-api
echo.

echo Route:
oc get routes pizza-api
echo.

for /f "tokens=*" %%A in ('oc get route pizza-api -o jsonpath="{.spec.host}"') do set ROUTE_HOST=%%A

echo =====================================================================
echo Deployment completed successfully!
echo =====================================================================
echo.
echo API URL: http://!ROUTE_HOST!
echo Test: curl http://!ROUTE_HOST!/api/pizzas
echo.
