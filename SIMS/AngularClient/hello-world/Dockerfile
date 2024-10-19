# Stage 1: Build Angular App with Node 18.x
FROM node:18-alpine as builder
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

# add app
COPY . /usr/src/app

# install deps and cli
RUN npm install
RUN npm install -g @angular/cli

# add .bin to $PATH
ENV PATH /usr/src/app/node_modules/.bin:$PATH

# build app
RUN ng build

# Stage 1, nginx, copy & serve app
FROM nginx:alpine
COPY ./nginx.conf /etc/nginx/nginx.conf

COPY --from=builder /usr/src/app/dist/hello-world/browser /usr/share/nginx/html/
# COPY nginx.conf /etc/nginx/conf.d/default.conf