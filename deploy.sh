#!/bin/bash

# Pizza API - OpenShift Deployment Script
# This script automates the deployment of the Pizza API to OpenShift

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
NAMESPACE=${1:-default}
REGISTRY=${2:-docker.io}
IMAGE_NAME=${3:-pizza-api}
IMAGE_TAG=${4:-latest}

echo -e "${YELLOW}═══════════════════════════════════════════════════════════${NC}"
echo -e "${YELLOW}Pizza API - OpenShift Deployment${NC}"
echo -e "${YELLOW}═══════════════════════════════════════════════════════════${NC}"
echo ""
echo -e "Namespace: ${GREEN}${NAMESPACE}${NC}"
echo -e "Registry: ${GREEN}${REGISTRY}${NC}"
echo -e "Image: ${GREEN}${IMAGE_NAME}:${IMAGE_TAG}${NC}"
echo ""

# Step 1: Create namespace (if it doesn't exist)
echo -e "${YELLOW}Step 1: Creating/Checking namespace...${NC}"
if oc get namespace ${NAMESPACE} 2>/dev/null; then
    echo -e "${GREEN}✓${NC} Namespace ${NAMESPACE} already exists"
else
    echo -e "${GREEN}✓${NC} Creating namespace ${NAMESPACE}"
    oc create namespace ${NAMESPACE}
fi

# Switch to target namespace
oc project ${NAMESPACE}
echo ""

# Step 2: Create secret for image pull (if using private registry)
echo -e "${YELLOW}Step 2: Setting up image pull secret (optional)${NC}"
if [ ! -z "$REGISTRY_SECRET" ]; then
    echo -e "${GREEN}✓${NC} Creating image pull secret"
    # Uncomment and configure if needed
    # oc create secret docker-registry regcred \
    #   --docker-server=${REGISTRY} \
    #   --docker-username=${REGISTRY_USERNAME} \
    #   --docker-password=${REGISTRY_PASSWORD}
fi
echo -e "${GREEN}✓${NC} Skipping image pull secret (using default)"
echo ""

# Step 3: Apply ConfigMap
echo -e "${YELLOW}Step 3: Creating ConfigMap...${NC}"
oc apply -f Deployment/openshift/06-configmap.yaml
echo -e "${GREEN}✓${NC} ConfigMap created/updated"
echo ""

# Step 4: Apply BuildConfig
echo -e "${YELLOW}Step 4: Creating BuildConfig...${NC}"
oc apply -f Deployment/openshift/01-buildconfig.yaml
echo -e "${GREEN}✓${NC} BuildConfig created/updated"
echo ""

# Step 5: Apply ImageStream
echo -e "${YELLOW}Step 5: Creating ImageStream...${NC}"
oc apply -f Deployment/openshift/02-imagestream.yaml
echo -e "${GREEN}✓${NC} ImageStream created/updated"
echo ""

# Step 6: Start build
echo -e "${YELLOW}Step 6: Starting build...${NC}"
BUILD_ID=$(oc start-build pizza-api -o jsonpath='{.metadata.name}')
echo -e "${GREEN}✓${NC} Build started: ${BUILD_ID}"
echo ""

# Step 7: Wait for build to complete
echo -e "${YELLOW}Step 7: Waiting for build to complete...${NC}"
oc logs -f build/${BUILD_ID}
echo -e "${GREEN}✓${NC} Build completed"
echo ""

# Step 8: Apply DeploymentConfig
echo -e "${YELLOW}Step 8: Creating DeploymentConfig...${NC}"
oc apply -f Deployment/openshift/03-deploymentconfig.yaml
echo -e "${GREEN}✓${NC} DeploymentConfig created/updated"
echo ""

# Step 9: Apply Service
echo -e "${YELLOW}Step 9: Creating Service...${NC}"
oc apply -f Deployment/openshift/04-service.yaml
echo -e "${GREEN}✓${NC} Service created/updated"
echo ""

# Step 10: Apply Route
echo -e "${YELLOW}Step 10: Creating Route...${NC}"
oc apply -f Deployment/openshift/05-route.yaml
echo -e "${GREEN}✓${NC} Route created/updated"
echo ""

# Step 11: Wait for deployment to be ready
echo -e "${YELLOW}Step 11: Waiting for deployment to be ready...${NC}"
oc rollout status dc/pizza-api --timeout=5m
echo -e "${GREEN}✓${NC} Deployment is ready"
echo ""

# Step 12: Display deployment information
echo -e "${YELLOW}Step 12: Deployment Information${NC}"
echo ""

ROUTE_HOST=$(oc get route pizza-api -o jsonpath='{.spec.host}' 2>/dev/null || echo "N/A")
echo -e "Route URL: ${GREEN}http://${ROUTE_HOST}${NC}"
echo ""

echo -e "${YELLOW}Pod Status:${NC}"
oc get pods -l app=pizza-api
echo ""

echo -e "${YELLOW}Service Status:${NC}"
oc get svc pizza-api
echo ""

echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}Deployment completed successfully!${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo ""
echo -e "Test the API:"
echo -e "  ${GREEN}curl http://${ROUTE_HOST}/api/pizzas${NC}"
echo ""
