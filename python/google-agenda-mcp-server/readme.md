# Google Agenda MCP Server

Um servidor MCP (Model Context Protocol) para gerenciar eventos do Google Calendar com Python.
Este servidor fornece ferramentas para criar, editar, excluir e consultar eventos de calendário através de IA.

## 🚀 Funcionalidades

- **Gerenciamento completo de eventos**: criar, editar, excluir e listar eventos
- **Suporte a timezone**: interpretação inteligente de datas com timezone local configurável
- **Eventos rápidos**: criação instantânea de eventos com início imediato
- **Consultas flexíveis**: buscar eventos futuros e detalhes específicos
- **Integração com IA**: ferramentas otimizadas para uso com assistentes de IA

## 📋 Pré-requisitos

### 1. Ambiente de Desenvolvimento

- **Python 3.8+** para desenvolvimento local
- **Docker Desktop** para execução em container (opcional, mas recomendado)

### 2. Configuração do Google Cloud

#### 2.1 Criar Projeto e Service Account

1. Acesse o [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um novo projeto ou selecione um existente
3. Navegue até **IAM & Admin** > **Service Accounts**
4. Clique em **Create Service Account**:
   - Nome: `calendar-mcp-server` (ou outro de sua preferência)
   - ID: será gerado automaticamente (formato: `nome@projeto.iam.gserviceaccount.com`)
   - Clique em **Create and Continue**
   - **Role**: não é necessário atribuir roles para este projeto
   - Clique em **Done**

#### 2.2 Habilitar API do Google Calendar

1. No Google Cloud Console, vá para **APIs & Services** > **Library**
2. Busque por "Google Calendar API"
3. Clique em **Enable**

#### 2.3 Criar Chaves de Acesso

1. Vá para **APIs & Services** > **Credentials**
2. Na lista de Service Accounts, clique na conta que você criou
3. Vá para a aba **Keys**
4. Clique em **Add Key** > **Create new key**
5. Selecione o formato **JSON**
6. Clique em **Create**

> **⚠️ Importante**: O arquivo JSON será baixado automaticamente. Guarde-o em local seguro, pois contém as credenciais de acesso. Este arquivo será usado para configurar a variável de ambiente `GOOGLE_SERVICE_ACCOUNT_INFO`.

### 3. Configuração do Google Calendar

Para que a Service Account tenha acesso ao seu calendário pessoal:

#### 3.1 Compartilhar o Calendário

1. Acesse [Google Calendar](https://calendar.google.com)
2. Na barra lateral esquerda, localize seu calendário em "Meus calendários"
3. Passe o mouse sobre o calendário e clique nos três pontos (⋮)
4. Selecione **Configurações e compartilhamento**

#### 3.2 Adicionar a Service Account

1. Role até a seção **Compartilhar com pessoas específicas**
2. Clique em **Adicionar pessoas**
3. Cole o email da Service Account (ex: `calendar-mcp-server@seu-projeto.iam.gserviceaccount.com`)
4. Selecione a permissão **Fazer alterações em eventos**
5. Clique em **Enviar**

#### 3.3 Obter o ID do Calendário (Opcional)

Por padrão, o servidor usa o calendário primário. Para usar um calendário específico:

1. Ainda nas **Configurações do calendário**
2. Role até **Integrar calendário**
3. Copie o **ID do calendário** (formato: `nome@gmail.com` ou string alfanumérica)
4. Use este ID na variável de ambiente `GOOGLE_CALENDAR_ID`

> **✅ Pronto!** Sua Service Account agora tem permissão para gerenciar eventos no seu calendário.

## ⚙️ Configuração

### 1. Variáveis de Ambiente

Crie um arquivo `.env` baseado no `.env.example`:

```bash
cp .env.example .env
```

Configure as seguintes variáveis:

```env
# Conteúdo do arquivo JSON da Service Account (em uma única linha)
GOOGLE_SERVICE_ACCOUNT_INFO='{"type":"service_account","project_id":"...","private_key_id":"...","private_key":"...","client_email":"...","client_id":"...","auth_uri":"...","token_uri":"...","auth_provider_x509_cert_url":"...","client_x509_cert_url":"..."}'

# ID do calendário (use "primary" para o calendário principal ou o ID específico)
GOOGLE_CALENDAR_ID=primary

# Timezone padrão para interpretação de datas sem timezone explícito
DEFAULT_TIMEZONE=America/Sao_Paulo
```

> **💡 Dica**: Para converter o JSON em uma única linha, você pode usar:
> ```bash
> # Linux/Mac
> cat sua-chave.json | jq -c
> 
> # Windows PowerShell
> (Get-Content sua-chave.json -Raw) -replace '\s+', ' '
> ```

## 🐳 Execução com Docker

### Construir e Executar

```bash
docker-compose up --build
```

O servidor estará disponível em: `http://localhost:3333`

### Parar o Servidor

```bash
docker-compose down
```

## 🐍 Execução Local (Python)

### Instalar Dependências

```bash
pip install -r requirements.docker.txt
```

### Executar o Servidor

```bash
python server.py
```

O servidor estará disponível em: `http://0.0.0.0:3333`

## 📝 Notas sobre Timezone

O servidor suporta interpretação inteligente de timezones:

- **Datas COM timezone explícito**: usa o timezone especificado
  - `2026-01-15T14:30:00-03:00` → 14h30 UTC-3
  - `2026-01-15T14:30:00Z` → 14h30 UTC

- **Datas SEM timezone**: assume o timezone configurado em `DEFAULT_TIMEZONE`
  - `2026-01-15T14:30:00` → 14h30 no timezone local (ex: America/Sao_Paulo)

**Timezones válidos**: Use nomes da [base IANA](https://en.wikipedia.org/wiki/List_of_tz_database_time_zones)
- Exemplos: `America/Sao_Paulo`, `America/New_York`, `Europe/London`, `Asia/Tokyo`

## 🔍 Troubleshooting

### Erro: "GOOGLE_SERVICE_ACCOUNT_INFO não foi encontrada"
- Verifique se o arquivo `.env` existe e está no diretório raiz do projeto
- Certifique-se de que o JSON está em uma única linha e entre aspas simples

### Erro: "Calendar not found" ou "404"
- Verifique se compartilhou o calendário com a Service Account
- Confirme se o `GOOGLE_CALENDAR_ID` está correto (ou use `"primary"`)

### Erro: "Invalid credentials"
- Verifique se o JSON da Service Account está correto e completo
- Certifique-se de que a API do Google Calendar está habilitada no projeto

### Eventos aparecem em horário errado
- Verifique a configuração de `DEFAULT_TIMEZONE`
- Use timezone explícito nas datas ISO: `2026-01-15T14:30:00-03:00`

## 📄 Licença

Este projeto é de código aberto e está disponível sob a licença MIT.