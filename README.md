# YCompany Auto Claims

## Overview

This README outlines the installation and configuration steps for setting up Minikube, Docker, and Kubernetes on WSL for the YCompany Auto Claims project.

## Table of Contents

- [Minikube and Docker Installation on WSL](#minikube-and-docker-installation-on-wsl)
- [Starting Minikube](#starting-minikube)
- [Windows Configuration](#windows-configuration)
- [Docker Setup](#docker-setup)
- [Kubernetes Deployment](#kubernetes-deployment)
- [Other Commands](#other-commands)
- [Optional: Installing Chrome on WSL](#optional-installing-chrome-on-wsl)
- [Links](#links)

## Minikube and Docker Installation on WSL

For a seamless installation of Minikube and Docker on WSL, refer to this article:

[WSL2: Seamlessly Install Ubuntu OS, Docker, and Kubernetes on Windows](https://medium.com/@dudu.zbeda_13698/wsl2-seamlessly-install-ubuntu-os-docker-and-kubernetes-on-windows-for-a-development-environment-13ce936a275c)

## Starting Minikube

To start Minikube, run the following command:

```bash
minikube start
```

## Windows Configuration

### Edit Hosts File

Update the `hosts` file with the Minikube IP address:

1. Open the `hosts` file located at:
   ```bash
   C:\Windows\System32\drivers\etc\hosts
   ```
2. Add the following line, replacing `192.168.49.2` with your actual Minikube IP address:
   ```bash
   192.168.49.2 ycompany.local
   ```

## Docker Setup

### Set Docker Environment

Run the following command to set the Docker environment:

```bash
eval $(minikube docker-env)
```

### Build Docker Images

Build the Docker images for each service:

```bash
cd path/to/document-service
docker build -t document-service:latest .

cd path/to/auth-service
docker build -t auth-service:latest .

cd path/to/claims-service
docker build -t claims-service:latest .

cd path/to/workshop-service
docker build -t workshop-service:latest .

cd path/to/claims-processing-app
docker build -t claims-processing-app:latest .
```

### Update Hosts File (WSL)

Add the Minikube IP address to the WSL hosts file:

```bash
echo "$(minikube ip) api.ycompany.local" | sudo tee -a /etc/hosts
echo "$(minikube ip) customer.ycompany.local" | sudo tee -a /etc/hosts
echo "$(minikube ip) internal.ycompany.local" | sudo tee -a /etc/hosts
```

## Kubernetes Deployment

### Apply Kubernetes Configuration

Apply the Kubernetes configuration:

```bash
kubectl apply -f k8s/
```

## Other Commands

### Delete Pods

Delete pods by label:

```bash
kubectl delete pod -l app=ycompany-api
kubectl delete pod -l app=claims-processing-app
```

### Get Pods

Get all pods:

```bash
kubectl get pod -A
```

### Restart Deployments

Restart deployments:

```bash
kubectl rollout restart deployment claims-processing-app
kubectl rollout restart deployment ycompany-api
kubectl rollout restart deployment -n ingress-nginx ingress-nginx-controller
```

### List Services

List all services:

```bash
minikube service --all
```

### Execute Shell Command

Execute a shell command in a specific pod:

```bash
kubectl exec -n default ycompany-api-bf67d7-hsrzt -it -- /bin/bash
```

## Optional: Installing Chrome on WSL

### Mandatory: Installing Chrome on WSL (When Firewall is Turned On)

If your development environment has the firewall turned on, installing Chrome is mandatory. Follow these steps:

1. **Download Chrome**:

   ```bash
   wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
   ```

2. **Install Chrome**:

   If you encounter the following error:

   ```bash
   E: Invalid operation install./google-chrome-stable_current_amd64.deb
   ```

   Use this command instead:

   ```bash
   sudo dpkg -i ./google-chrome-stable_current_amd64.deb
   ```

3. **Run Chrome**:

   After installation, run Chrome using:

   ```bash
   google-chrome
   ```

### Optional: Installing Chrome on WSL (When Firewall is Turned Off)

If the firewall is off, you can install Chrome, but it’s not mandatory.

```bash
sudo apt -y install ./google-chrome-stable_current_amd64.deb
```

## Links

- **Customer App**: [http://customer.ycompany.local/](http://customer.ycompany.local/)
- **Internal App**: [http://internal.ycompany.local/](http://internal.ycompany.local/)
- **Swagger Auth**: [http://api.ycompany.local/auth/swagger/index.html](http://api.ycompany.local/auth/swagger/index.html)
- **Swagger Claims**: [http://api.ycompany.local/claims/swagger/index.html](http://api.ycompany.local/claims/swagger/index.html)
- **Swagger Workshop**: [http://api.ycompany.local/workshop/swagger/index.html](http://api.ycompany.local/workshop/swagger/index.html)
- **Swagger Document**: [http://api.ycompany.local/documents/swagger/index.html](http://api.ycompany.local/documents/swagger/index.html)
```
