# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Tool version: "Detailed" (Version A) or "Generic" (Version B)
ENV ToolVersion=Detailed

# PostgreSQL connection string
# Uses ASP.NET Core environment variable override convention for ConnectionStrings:Default
ENV ConnectionStrings__Default=Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres

# ASP.NET Core configuration
ENV ASPNETCORE_URLS=http://+:6099
EXPOSE 6099

ENTRYPOINT ["./intranet_mcp_server"]
