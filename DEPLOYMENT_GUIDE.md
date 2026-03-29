# Pizza API - OpenShift Deployment Guide

## Prerequisites

### Required Tools

- **oc CLI** - OpenShift command-line interface (v4.10+)
- **Git** - Source code management
- **Access** - OpenShift cluster with push permissions

### Cluster Requirements

- Namespace with resource quota (CPU, Memory, Storage)
- Internal container registry access
- Network connectivity for Git repository fetch

---

## Step-by-Step Deployment

### Step 1: Prepare Your Repository

This guide assumes your code is in a Git repository accessible to the OpenShift cluster.

```bash
# Clone or navigate to your repository
cd /path/to/MyContainerApp

# Verify files are committed
git status
```

### Step 2: Configure BuildConfig

The BuildConfig is configured for **manual trigger** (no automatic builds on Git push).

Update [Deployment/openshift/01-buildconfig.yaml](Deployment/openshift/01-buildconfig.yaml) with your repository:

```yaml
source:
  type: Git
  git:
    uri: https://github.com/YOUR-ORG/YOUR-REPO.git # <- Update this
    ref: main # <- Update if different branch
```

### Step 3: Create Namespace (Optional)

```bash
# Create a dedicated namespace
oc create namespace pizza-app

# Switch to the namespace
oc project pizza-app
```

If deploying to existing namespace:

```bash
oc project your-existing-namespace
```

### Step 4: Apply Configuration

Apply all OpenShift manifests in order:

```bash
# Option 1: Apply all at once
oc apply -f Deployment/openshift/

# Option 2: Apply individually for better control
oc apply -f Deployment/openshift/01-buildconfig.yaml
oc apply -f Deployment/openshift/02-imagestream.yaml
oc apply -f Deployment/openshift/06-configmap.yaml
```

**Expected Output:**

```
imagestreamimport.image.openshift.io/pizza-api created
buildconfig.build.openshift.io/pizza-api created
configmap/pizza-api-config created
```

### Step 5: Trigger Build

Manually start the build (since manual trigger is enabled):

```bash
# Start the build from Git source
oc start-build pizza-api

# View build logs
oc logs -f bc/pizza-api

# Or view specific build logs
oc logs build/pizza-api-1 -f
```

**Expected Output (after build completes):**

```
Successfully pushed image-registry.openshift-image-registry.svc:5000/default/pizza-api:latest
Push successful
```

### Step 6: Deploy Application

After image is built and pushed to registry, apply deployment manifests:

```bash
# Apply DeploymentConfig, Service, and Route
oc apply -f Deployment/openshift/03-deploymentconfig.yaml
oc apply -f Deployment/openshift/04-service.yaml
oc apply -f Deployment/openshift/05-route.yaml
```

**Expected Output:**

```
deploymentconfig.apps.openshift.io/pizza-api created
service/pizza-api created
route.route.openshift.io/pizza-api created
```

### Step 7: Verify Deployment

Check that all components are ready:

```bash
# Check pods are running
oc get pods -l app=pizza-api
# Expected: 2 pods in "Running" state with "1/1" ready

# Check service
oc get svc pizza-api
# Expected: Service with ClusterIP
oc describe svc pizza-api

# Check route
oc get routes
# Expected: Route with hostname
oc describe route pizza-api

# Get the public Route URL
ROUTE_URL=$(oc get route pizza-api -o jsonpath='{.spec.host}')
echo "API URL: http://$ROUTE_URL"
```

### Step 8: Test the API

```bash
# Health check
curl http://$ROUTE_URL/health

# Get all pizzas (should be empty)
curl http://$ROUTE_URL/api/pizzas

# Create a pizza
curl -X POST http://$ROUTE_URL/api/pizzas \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Pepperoni",
    "price": 13.99,
    "description": "Spicy pepperoni pizza"
  }'

# Get all pizzas
curl http://$ROUTE_URL/api/pizzas
```

### Step 9: View Logs

```bash
# View logs from specific pod
POD=$(oc get pods -l app=pizza-api -o jsonpath='{.items[0].metadata.name}')
oc logs $POD

# Stream logs in real-time
oc logs -f $POD

# View logs from all pods with label
oc logs -l app=pizza-api --tail=50 -f
```

### Step 10: Scale Application

```bash
# Scale to 3 replicas
oc scale dc/pizza-api --replicas=3

# Verify scaling
oc get pods -l app=pizza-api

# Scale down
oc scale dc/pizza-api --replicas=1
```

---

## Subsequent Deployments (Updates)

When you make code changes and need to redeploy:

```bash
# 1. Commit code to Git
git add .
git commit -m "Feature: Add new pizza variant"
git push origin main

# 2. Trigger a new build (manually)
oc start-build pizza-api

# 3. Monitor build
oc logs -f bc/pizza-api

# 4. Deployment auto-triggers (DeploymentConfig has ImageChange trigger)
# View rolling deployment status
oc rollout status dc/pizza-api

# 5. Verify new pods are running
oc get pods -l app=pizza-api
```

---

## Troubleshooting

### Build Failures

**Problem**: Build pod shows error

```bash
# View build details
oc describe build pizza-api-1

# View build logs with more context
oc logs build/pizza-api-1 -f

# Common causes:
# - Git repository not accessible
# - Dockerfile syntax error
# - Missing NuGet packages during restore
```

**Solution**:

- Verify Git URI in BuildConfig is correct
- Check Git access token/SSH keys
- Validate Dockerfile syntax
- Check build pod resource limits

### Deployment Failures

**Problem**: Pods not transitioning to Running state

```bash
# Check pod status
oc get pods pizza-api-xxx -o yaml

# View pod events
oc describe pod pizza-api-xxx

# Check container logs
oc logs pizza-api-xxx

# Common causes:
# - CrashLoopBackOff: Application start error
# - ImagePullBackOff: Image not found or registry access
# - Pending: Resource not available
```

**Solution**:

- Check application logs for startup errors
- Verify image was pushed to registry: `oc get images | grep pizza-api`
- Check resource requests vs cluster availability: `oc top nodes`
- Verify service account permissions

### Health Check Failures

**Problem**: Pod shows Ready=0/1, failing readiness probe

```bash
# Check exact probe failure
oc describe pod pizza-api-xxx | grep -A 5 "Readiness"

# Manually test health endpoint
POD_IP=$(oc get pod pizza-api-xxx -o jsonpath='{.status.podIP}')
oc run -it test --image=curlimages/curl --restart=Never -- curl http://$POD_IP:8080/health
```

**Solution**:

- Verify `/health` endpoint is implemented and accessible
- Check network policies aren't blocking internal traffic
- Increase initialDelaySeconds if startup is slow
- Review application logs for configuration errors

### Access Issues

**Problem**: Cannot reach API via Route URL

```bash
# Verify route exists and is configured
oc get routes

# Test internal service access from pod
oc run -it test --image=alpine --restart=Never -- \
  wget -O- http://pizza-api:80/api/pizzas

# Check DNS resolution
oc run -it test --image=alpine --restart=Never -- nslookup pizza-api

# Test from route using debug pod
oc run -it debug --image=curlimages/curl --restart=Never -- \
  curl http://pizza-api-default.apps.openshift.com/health
```

**Solution**:

- Verify Service port matches container port (8080)
- Ensure Route targets correct Service
- Check network policies aren't blocking external access
- Verify firewall rules allow traffic to Route

---

## Configuration Management

### ConfigMap Updates

Update application configuration without rebuilding:

```bash
# Edit ConfigMap
oc edit configmap pizza-api-config

# Or apply updated manifest
oc apply -f Deployment/openshift/06-configmap.yaml

# Rolling restart to pick up new config
oc rollout restart dc/pizza-api
```

### Environment Variables

The DeploymentConfig currently uses:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:8080`
- `LOG_LEVEL` from ConfigMap (if configured)

To add more environment variables, edit `03-deploymentconfig.yaml`:

```yaml
env:
  - name: YOUR_VAR
    value: "your_value"
  - name: YOUR_SECRET
    valueFrom:
      secretKeyRef:
        name: your-secret
        key: your-key
```

---

## Cleanup

To remove the application from OpenShift:

```bash
# Delete all resources in one command
oc delete all -l app=pizza-api

# Or delete specific resources
oc delete route pizza-api
oc delete svc pizza-api
oc delete dc pizza-api
oc delete is pizza-api
oc delete bc pizza-api
oc delete configmap pizza-api-config

# Delete namespace (if created)
oc delete namespace pizza-app
```

---

## Monitoring & Observability

### View Resource Usage

```bash
# CPU and Memory usage by pod
oc top pods

# Top nodes in cluster
oc top nodes

# Node resource details
oc describe node <node-name>
```

### View Events

```bash
# Recent cluster events
oc get events

# Events for specific resource
oc describe dc pizza-api
```

### Access Logs

```bash
# Application logs from all pods
oc logs -l app=pizza-api --tail=100 -f

# Previous logs (from crashed pod)
oc logs pizza-api-xxx --previous

# Logs from all containers in pod
oc logs pizza-api-xxx --all-containers=true
```

---

## Production Checklist

- [ ] Repository Git URI is accurate and accessible
- [ ] All secrets/credentials are stored in OpenShift Secrets (not in code)
- [ ] Resource requests/limits are appropriate for workload
- [ ] Health check endpoints are accessible
- [ ] Logging is configured and centralized
- [ ] AutoScaling is enabled (if needed)
- [ ] Network policies are configured
- [ ] Backup/restore procedures are documented
- [ ] Monitoring and alerting are set up
- [ ] All tests pass before deployment

---

## Next Steps

1. **Set up CI/CD Pipeline**: Integrate with Jenkins or GitHub Actions for automated builds
2. **Enable Auto-Scaling**: Configure HorizontalPodAutoscaler for automatic scaling
3. **Persistent Storage**: If data persistence needed, add PersistentVolume and PersistentVolumeClaim
4. **Secrets Management**: Use OpenShift Secrets for sensitive data (API keys, passwords)
5. **Monitoring**: Integrate with Prometheus, Grafana, or OpenShift monitoring
6. **Ingress/TLS**: Configure TLS certificates for HTTPS access
7. **RBAC**: Set up proper authorization policies

---

## Support & Documentation

- [OpenShift Documentation](https://docs.openshift.com/)
- [BuildConfig Reference](https://docs.openshift.com/container-platform/latest/builds/creating-build-inputs.html)
- [DeploymentConfig Reference](https://docs.openshift.com/container-platform/latest/deployments/what-are-deployments.html)
- [Route Configuration](https://docs.openshift.com/container-platform/latest/networking/routes/index.html)
