# PostgreSQL MCP Server

Um servidor MCP (Model Context Protocol) para PostgreSQL construído com .NET 8 e ASP.NET Core. Este servidor fornece ferramentas para explorar e interagir com bancos de dados PostgreSQL através do protocolo MCP.

## Características

- **Execução de SQL**: Execute queries SQL direto no banco PostgreSQL
- **Exploração de Schema**: Liste tabelas e explore a estrutura do banco
- **Configuração via Variáveis de Ambiente**: Seguro e flexível
- **Banco de Dados Externo**: Conecte a qualquer PostgreSQL (local ou remoto)
- **Docker Ready**: Suporte para Docker e Docker Compose
- **HTTP Transport**: API HTTP compatível com MCP Protocol

> Observação sobre "self-contained": a aplicação pode ser publicada como self-contained (`dotnet publish -r <RID> --self-contained true`) para não requerer runtime instalado no host. O repositório não inclui um binário self-contained por padrão.

## Pré-requisitos

- .NET 8 SDK (para desenvolvimento local)
- Docker (recomendado para deployment)
- PostgreSQL (local, remoto ou na nuvem)

## Configuração

### Variáveis de Ambiente

O servidor requer as seguintes variáveis de ambiente:

**Obrigatórias:**
- `POSTGRES_DATABASE`: Nome do banco de dados
- `POSTGRES_USERNAME`: Usuário do banco
- `POSTGRES_PASSWORD`: Senha do banco

**Opcionais:**
- `POSTGRES_HOST`: Host do banco (padrão: `localhost`)
  - Para banco local no host com Docker: `host.docker.internal` (Windows/macOS) ou `172.17.0.1` (Linux)
- `POSTGRES_PORT`: Porta do banco (padrão: `5432`)
- `POSTGRES_TIMEOUT`: Timeout de conexão em segundos (padrão: `30`)
- `POSTGRES_MAX_POOL_SIZE`: Tamanho máximo do pool de conexões (padrão: `100`)

Veja o arquivo ` .env.example` para um template completo com as variáveis mínimas.

Exemplo mínimo (`.env.example`):

```
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DATABASE=mydb
POSTGRES_USERNAME=postgres
POSTGRES_PASSWORD=changeme
```

## Como Executar

### 1) Desenvolvimento Local (sem Docker)

O projeto carrega automaticamente um arquivo `.env` no diretório de trabalho usando o pacote `DotNetEnv` se o arquivo existir. Portanto, no diretório do projeto basta:

```bash
cp .env.example .env   # ajustar valores
dotnet run
```

### 2) Docker Compose

Coloque o arquivo ` .env` no mesmo diretório de `docker-compose.yml` para que o Compose faça interpolação das variáveis, ou use `env_file` no serviço.

Exemplos:

```bash
# Reconstruir e iniciar
docker compose build --no-cache
docker compose --env-file .env up -d

# alternativa compatível com versões antigas
# docker-compose --env-file .env up -d
```

Ou no `docker-compose.yml`:

```yaml
services:
  mcp-server:
    env_file: .env
    # ...
```

Ver logs do serviço:

```bash
docker compose logs -f mcp-server
```

### Verificação rápida

Após iniciar, verifique a disponibilidade:

```bash
curl http://localhost:6099/
```

## Segurança

- **Nunca** commite o arquivo ` .env` com credenciais reais. Mantenha ` .env` no `.gitignore` e mantenha ` .env.example` no repositório.
- Em produção prefira secrets managers (Azure Key Vault, AWS Secrets Manager), `docker secrets` ou Kubernetes Secrets.
- Valide e sanitize todas as queries SQL para prevenir SQL injection.
- Use TLS e autenticação para o endpoint HTTP em produção.

## Ferramentas MCP Disponíveis

- `TestConnection` — testa a conexão com o banco
- `ListTables` — lista tabelas do schema público
- `GetTableSchema` — retorna schema de uma tabela
- `ExecuteSQL` — executa uma query SQL arbitrária (cuidado com injeção)

## Estrutura do Projeto (resumido)

```
postgre-mcp-server/
  Configuration/
    PostgresConfiguration.cs
  Services/
    PostgresService.cs
  Tools/
    AdminTools.cs
    ExploreDbTools.cs
  Program.cs
  Dockerfile
  docker-compose.yml
  .env.example
  init-db.sql
```

---

**Nota:** Este projeto está configurado para conectar a bancos PostgreSQL externos. Não inclui um container PostgreSQL embutido por padrão.
