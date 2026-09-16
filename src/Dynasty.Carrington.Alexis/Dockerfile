# syntax=docker/dockerfile:1
# Build context must be the repository root, e.g.:
#   docker build -f src/Dynasty.Carrington.Alexis/Dockerfile .

FROM node:22-alpine AS deps
WORKDIR /app
COPY src/Dynasty.Carrington.Alexis/package.json src/Dynasty.Carrington.Alexis/package-lock.json ./
RUN npm ci

FROM node:22-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY src/Dynasty.Carrington.Alexis/ .
RUN npm run build

FROM node:22-alpine AS final
WORKDIR /app
ENV NODE_ENV=production
ENV PORT=3000
ENV HOSTNAME=0.0.0.0
EXPOSE 3000
COPY --from=build /app/public ./public
COPY --from=build /app/.next/standalone ./
COPY --from=build /app/.next/static ./.next/static
ENTRYPOINT ["node", "server.js"]
