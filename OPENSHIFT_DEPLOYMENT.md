# OpenShift Deployment Instructions - Step by Step

## Your Configuration

- OpenShift API URL: `https://api.rm3.7wse.p1.openshiftapps.com:6443`
- Project/Namespace: `mohanramphp-dev` (existing Red Hat Developer Sandbox project)
- Application Name: `pizza-api`
- GitHub Repository: `https://github.com/mohanramphp/MyContainerApp.git`

---

## Step 1: Login to OpenShift Cluster

Run this command in your terminal (PowerShell or Command Prompt):

```powershell
oc login https://api.rm3.7wse.p1.openshiftapps.com:6443
```

**When prompted:**

- Username: Enter your OpenShift username
- Password: Enter your OpenShift password

**Alternative using token (if you have a token):**

```powershell
oc login --token=YOUR_TOKEN_HERE --server=https://api.rm3.7wse.p1.openshiftapps.com:6443
```

**Verify login succeeded:**

```powershell
oc whoami
```

You should see your username returned.

---

## Step 2: Switch to Existing Project

Switch to your existing Red Hat Developer Sandbox project:

```powershell
oc project mohanramphp-dev
```

**Verify you're in the correct project:**

```powershell
oc project
```

Output should be: `Using project "mohanramphp-dev"`

**Note:** Red Hat Developer Sandbox trial version only allows one project. Your existing `mohanramphp-dev` project will be used for all resources.

---

## Step 3: Apply OpenShift Manifests

Navigate to your project root and apply all manifests to `mohanramphp-dev` project:

```powershell
cd f:\mohan-work\poc\openshift\MyContainerApp
oc apply -f Deployment/openshift/ -n mohanramphp-dev
```

**Or if you're already in the project (from Step 2):**

```powershell
oc apply -f Deployment/openshift/
```

This will create:

- BuildConfig (builds Docker image from GitHub)
- ImageStream (internal image registry)
- DeploymentConfig (pod deployment configuration)
- Service (internal routing)
- Route (external HTTP access)
- ConfigMap (configuration management)

**Verify all resources were created:**

```powershell
oc get buildconfig
oc get imagestream
oc get deploymentconfig
oc get service
oc get route
oc get configmap
```

You should see `pizza-api` listed in each resource type.

---

## Step 4: Trigger Manual Build

Start the Docker build from your GitHub repository:

```powershell
oc start-build pizza-api
```

Expected output: `build "pizza-api-1" started`

---

## Step 5: Monitor Build Progress

Watch the build logs in real-time:

```powershell
oc logs -f bc/pizza-api
```

**What to look for:**

- ✅ `Cloning "https://github.com/mohanramphp/MyContainerApp.git"` — GitHub cloned successfully
- ✅ `Restoring NuGet packages...` — Dependencies restored
- ✅ `Building solution...` — .NET build running
- ✅ `Publishing...` — Application published
- ✅ `Push successful` — Image pushed to OpenShift registry
- ⏹️ Build completes (may take 3-5 minutes)

**If you see errors:**

- Check GitHub URL is correct: `oc get bc pizza-api -o yaml | grep uri`
- Verify GitHub repository is accessible (public or has access credentials)
- Check Dockerfile syntax: `oc describe bc pizza-api`

**Exit log monitoring:** Press `Ctrl+C`

---

## Step 6: Verify Deployment

Check if pods are running:

```powershell
oc get pods
```

**Expected output:**

```
NAME                    READY   STATUS    RESTARTS   AGE
pizza-api-1-xxxxx       1/1     Running   0          2m
pizza-api-1-xxxxx       1/1     Running   0          1m (second replica)
```

**If pods are not running:**

```powershell
oc describe pod pizza-api-1-xxxxx # replace xxxxx with actual pod name
oc logs pizza-api-1-xxxxx
```

**Verify deployment config:**

```powershell
oc get dc pizza-api
oc describe dc pizza-api
```

---

## Step 7: Get the Application URL

Retrieve the public Route URL:

```powershell
oc get route pizza-api -o jsonpath={.spec.host}
```

**Save this URL for later testing!** It will look like:

```
pizza-api-mohanramphp-dev.apps.rm3.7wse.p1.openshiftapps.com
```

---

## Step 8: Test Health Endpoint

Test if the API is responding:

```powershell
$routeUrl = oc get route pizza-api -o jsonpath={.spec.host}
curl -k https://$routeUrl/health
```

**Expected response:**

```
Healthy
```

If you get `curl: (60) SSL certificate problem`, you can ignore it for now or use HTTP:

```powershell
curl http://$routeUrl/health
```

---

## Step 9: Test API Endpoints

Create a pizza (test POST):

```powershell
$routeUrl = oc get route pizza-api -o jsonpath={.spec.host}
$pizzaData = @{
    name = "Margherita"
    price = 12.99
    description = "Classic Italian pizza"
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://$routeUrl/api/pizzas" `
    -Method POST `
    -ContentType "application/json" `
    -Body $pizzaData `
    -SkipCertificateCheck | Select-Object StatusCode, Content
```

**Expected response:** Status Code 201 with pizza object

Get all pizzas (test GET):

```powershell
$routeUrl = oc get route pizza-api -o jsonpath={.spec.host}
Invoke-WebRequest -Uri "https://$routeUrl/api/pizzas" `
    -Method GET `
    -SkipCertificateCheck | Select-Object StatusCode, Content
```

---

## Step 10: View Application Logs

Check logs from running pods:

```powershell
oc logs -f dc/pizza-api
```

This shows Serilog output from your application.

---

## Troubleshooting

### Build Failed

```powershell
oc describe bc pizza-api
oc logs bc/pizza-api-1  # or -2, -3, etc. for latest builds
```

### Pod Crashes

```powershell
oc describe pod pizza-api-1-xxxxx
oc logs pizza-api-1-xxxxx --previous  # if pod restarted
```

### Service Not Accessible

```powershell
oc get svc pizza-api
oc describe svc pizza-api
oc get endpoints pizza-api
```

### Route Not Showing

```powershell
oc get route
oc describe route pizza-api
```

---

## Next Steps

After successful deployment:

1. **Update api-tests.http** with your Route URL:
   - Open `api-tests.http`
   - Replace `@baseUrl = http://localhost:5000` with `@baseUrl = https://[your-route-url]`
   - Run REST Client requests against production API

2. **Monitor in OpenShift Console:**
   - Go to your OpenShift console
   - Navigate to Workloads → Deployments → pizza-api
   - View metrics, logs, events, and pod status

3. **Scale the Application:**

   ```powershell
   oc scale dc pizza-api --replicas=3
   ```

4. **Configure HTTPS:**
   The Route is already set up for HTTPS. For production, configure edge termination:
   ```powershell
   oc edit route pizza-api
   ```

---

## Important Notes

- ⚠️ The DeploymentConfig will auto-redeploy if the image changes (ImageChange trigger)
- 📝 Logs are available for 24-48 hours in OpenShift
- 🔄 Use `oc rollout latest dc/pizza-api` to manually trigger deployment
- 🔐 Never commit .kube/config file to git (contains credentials)

---

## Quick Reference Commands

```powershell
# Login
oc login https://api.rm3.7wse.p1.openshiftapps.com:6443

# Switch to existing Red Hat Developer Sandbox project
oc project mohanramphp-dev

# Apply manifests
oc apply -f Deployment/openshift/

# Start build
oc start-build pizza-api

# Watch logs
oc logs -f bc/pizza-api

# View pods
oc get pods

# Get Route URL
oc get route pizza-api -o jsonpath={.spec.host}

# View deployment
oc describe dc pizza-api

# Scale
oc scale dc pizza-api --replicas=2

# Delete all
oc delete all -l app=pizza-api
```

---

**Your Reference Information:**

- API URL: https://api.rm3.7wse.p1.openshiftapps.com:6443
- Project: mohanramphp-dev (Red Hat Developer Sandbox)
- Build Command: `oc start-build pizza-api`
- Logs: `oc logs -f bc/pizza-api` (build) or `oc logs -f dc/pizza-api` (app)
- Route Command: `oc get route pizza-api -o jsonpath={.spec.host}`
