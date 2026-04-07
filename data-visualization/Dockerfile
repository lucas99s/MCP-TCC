FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend-build
WORKDIR /src
COPY backend/src/Api/ExperimentAnalytics.Api.csproj backend/src/Api/
RUN dotnet restore backend/src/Api/ExperimentAnalytics.Api.csproj
COPY backend/src/Api/ backend/src/Api/
RUN dotnet publish backend/src/Api/ExperimentAnalytics.Api.csproj -c Release -o /out/backend

FROM node:22-bookworm-slim AS frontend-build
WORKDIR /src/frontend
COPY frontend/package.json frontend/package-lock.json* ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
RUN apt-get update \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:5080
ENV API_BASE_URL=http://127.0.0.1:5080/api
ENV NEXT_PUBLIC_API_BASE_URL=http://127.0.0.1:5080/api
ENV HOSTNAME=0.0.0.0
ENV PORT=3000

COPY --from=backend-build /out/backend /app/backend
COPY --from=frontend-build /src/frontend/.next/standalone /app/frontend
COPY --from=frontend-build /src/frontend/.next/static /app/frontend/.next/static
COPY docker/start.sh /app/start.sh

RUN chmod +x /app/start.sh

EXPOSE 3000
EXPOSE 5080

ENTRYPOINT ["/app/start.sh"]
