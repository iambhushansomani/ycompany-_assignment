# Enable Ingress controller addon in Minikube
minikube addons enable ingress
minikube addons enable ingress-dns

# Wait for the Ingress controller to be ready
echo "Waiting for Ingress controller to be ready..."
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=90s
eval $(minikube docker-env)
cd ../document-service
docker build -t document-service:latest .
cd ../auth-service
docker build -t auth-service:latest .
cd ../claims-service
docker build -t claims-service:latest .
cd ../workshop-service
docker build -t workshop-service:latest .
cd ../claims-processing-app
docker build -t claims-processing-app:latest .

echo "$(minikube ip) internal.ycompany.local" | sudo tee -a /etc/hosts
echo "$(minikube ip) customer.ycompany.local" | sudo tee -a /etc/hosts
echo "$(minikube ip) api.ycompany.local" | sudo tee -a /etc/hosts


cd ../k8s
kubectl apply -f .

kubectl rollout restart deployment claims-processing-app
kubectl rollout restart deployment document-service
kubectl rollout restart deployment auth-service
kubectl rollout restart deployment workshop-service
kubectl rollout restart deployment claims-service

 curl https://raw.githubusercontent.com/helm/helm/master/scripts/get-helm-3 | bash

#  # Create monitoring namespace
# kubectl create namespace monitoring

# # Add Helm repositories
# helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
# helm repo add grafana https://grafana.github.io/helm-charts
# helm repo update

# # Install Prometheus
# helm install prometheus prometheus-community/prometheus \
#     --namespace monitoring \
#     --set server.persistentVolume.enabled=false \
#     --set alertmanager.persistentVolume.enabled=false

# # Install Grafana
# helm install grafana grafana/grafana \
#     --namespace monitoring \
#     --set persistence.enabled=false \
#     --set adminPassword=admin \
#     --values - <<EOF
# datasources:
#   datasources.yaml:
#     apiVersion: 1
#     datasources:
#     - name: Prometheus
#       type: prometheus
#       url: http://prometheus-server.monitoring.svc.cluster.local
#       access: proxy
#       isDefault: true
# EOF

# # Wait for pods to be ready
# echo "Waiting for Prometheus and Grafana pods to be ready..."
# kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=prometheus -n monitoring --timeout=300s
# kubectl wait --for=condition=ready pod -l app.kubernetes.io/name=grafana -n monitoring --timeout=300s

# # Instead of using minikube service, use port-forwarding
# echo "Use the following commands in separate terminals to access Prometheus and Grafana:"
# echo "kubectl port-forward -n monitoring svc/prometheus-server 9090:80"
# echo "kubectl port-forward -n monitoring svc/grafana 3000:80"

# echo "Prometheus will be available at: http://localhost:9090"
# echo "Grafana will be available at: http://localhost:3000"
# echo "Grafana default credentials - Username: admin, Password: admin"

