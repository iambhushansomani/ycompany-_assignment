FROM mirror.gcr.io/library/node:20 as node-base
FROM mirror.gcr.io/library/nginx:alpine as nginx-base

FROM node-base as build

RUN npm install -g @angular/cli

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build -- --verbose

# FROM nginx-base
# COPY --from=build /app/dist/claims-processing-app /usr/share/nginx/html
# EXPOSE 80
# CMD ["nginx", "-g", "daemon off;"]

CMD ["npm", "start", "--", "--host", "0.0.0.0", "--disable-host-check"]