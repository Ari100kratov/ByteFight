FROM node:22-alpine AS build
WORKDIR /app

RUN corepack enable
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile

COPY . .
ARG VITE_API_URL=/api
ARG VITE_GAME_HUB_URL=/game-runtime-hub
ENV VITE_API_URL=$VITE_API_URL
ENV VITE_GAME_HUB_URL=$VITE_GAME_HUB_URL
RUN pnpm build

FROM nginx:1.27-alpine AS final
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
