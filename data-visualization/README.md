# Data Visualization

Base inicial de um sistema analítico dividido em:

- `backend`: ASP.NET Core Web API para agregações, filtros e paginação
- `frontend`: Next.js para dashboard, exploração e detalhe de execução

## Abrir no Visual Studio

O backend possui solution:

- [DataVisualization.Backend.sln](.\MCP-TCC\data-visualization\backend\data-visualization.sln)

Ela referencia o projeto:

- [ExperimentAnalytics.Api.csproj](.\MCP-TCC\data-visualization\backend\src\Api\ExperimentAnalytics.Api.csproj)

Também foi adicionado o profile de debug em:

- [launchSettings.json](.\MCP-TCC\data-visualization\backend\src\Api\Properties\launchSettings.json)

Com isso você pode abrir a solution no Visual Studio, definir o projeto web como startup e usar `F5` normalmente em `http://localhost:5080/swagger`.

## Como rodar localmente

### Backend

```powershell
cd C:\{...}\MCP-TCC\data-visualization\backend\src\Api
dotnet run
```

Opcionalmente configure `ConnectionStrings__Experiments`.

### Frontend

```powershell
cd C:\{...}\MCP-TCC\data-visualization\frontend
npm install
npm run dev
```

Para usar a API real:

```powershell
$env:NEXT_PUBLIC_API_BASE_URL="http://localhost:5080/api"
$env:NEXT_PUBLIC_USE_MOCKS="false"
```

## Docker

Foi configurado um container único com frontend e backend juntos:

- [Dockerfile](.\MCP-TCC\data-visualization\Dockerfile)
- [docker-compose.yml](.\MCP-TCC\data-visualization\docker-compose.yml)
- [start.sh](.\MCP-TCC\data-visualization\docker\start.sh)

## Variáveis de ambiente

O `docker-compose` usa variáveis carregadas de um arquivo `.env`, evitando deixar credenciais versionadas.

Arquivo de exemplo:

- [\.env.example](.\MCP-TCC\data-visualization\.env.example)

As variáveis principais são:

- `EXPERIMENTS_CONNECTION_STRING`
- `FRONTEND_PORT`
- `BACKEND_PORT`
- `INTERNAL_API_BASE_URL`
- `NEXT_PUBLIC_API_BASE_URL`
- `USE_MOCKS`
- `NEXT_PUBLIC_USE_MOCKS`

## Subir com Docker

```powershell
cd C:\{...}\MCP-TCC\data-visualization
copy .env.example .env
docker compose up --build
```

Depois disso:

- Frontend: `http://localhost:3000`
- Backend: `http://localhost:5080/swagger`

Se você alterar `FRONTEND_PORT` ou `BACKEND_PORT` no `.env`, use as portas correspondentes no navegador.