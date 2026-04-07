# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables for PostgreSQL connection
# These should be overridden when running the container
ENV POSTGRES_HOST=localhost
ENV POSTGRES_PORT=5432
ENV POSTGRES_DATABASE=
ENV POSTGRES_USERNAME=
ENV POSTGRES_PASSWORD=
ENV POSTGRES_TIMEOUT=30
ENV POSTGRES_MAX_POOL_SIZE=100

# ASP.NET Core configuration
ENV ASPNETCORE_URLS=http://+:6099
EXPOSE 6099

ENTRYPOINT ["./postgre_mcp_server"]
