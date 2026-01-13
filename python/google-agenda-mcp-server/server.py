import os
import json
import logging
from datetime import datetime, timedelta
from mcp.server import FastMCP
from typing import Optional
from google.oauth2 import service_account
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from dotenv import load_dotenv
import pytz

load_dotenv()

# Configurar logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

DEFAULT_CALENDAR_ID = os.getenv("GOOGLE_CALENDAR_ID", "primary")
DEFAULT_TIMEZONE = os.getenv("TIMEZONE", "America/Sao_Paulo")  # Timezone padrão para interpretação de datas sem TZ

try:
    LOCAL_TZ = pytz.timezone(DEFAULT_TIMEZONE)
    logger.info(f"Timezone configurado: {DEFAULT_TIMEZONE}")
except pytz.exceptions.UnknownTimeZoneError:
    logger.warning(f"Timezone inválido '{DEFAULT_TIMEZONE}', usando America/Sao_Paulo")
    LOCAL_TZ = pytz.timezone("America/Sao_Paulo")
    DEFAULT_TIMEZONE = "America/Sao_Paulo"

server = FastMCP(name="google-agenda-mcp-server", host="0.0.0.0", port=3333, stateless_http=True)

def format_datetime_for_api(dt: datetime, assume_local: bool = True) -> str:
    """Converte datetime para string RFC3339 compatível com Google Calendar API.
    
    Args:
        dt: objeto datetime para converter
        assume_local: Se True e dt não tem timezone, assume timezone local configurado.
                     Se False, assume UTC (comportamento anterior).
        
    Returns:
        String no formato RFC3339 (ISO 8601 com timezone)
    """
    if dt.tzinfo is None:
        if assume_local:
            # Adiciona timezone local para evitar confusão
            dt = LOCAL_TZ.localize(dt)
            logger.debug(f"Data sem timezone interpretada como {DEFAULT_TIMEZONE}: {dt.isoformat()}")
        else:
            # Comportamento legado: assume UTC
            return dt.isoformat() + 'Z'
    return dt.isoformat()

def get_calendar_service():
    """Inicializa o serviço do Google Calendar exclusivamente usando variáveis de ambiente."""
    scopes = ['https://www.googleapis.com/auth/calendar']
    
    service_account_info = os.getenv("GOOGLE_SERVICE_ACCOUNT_INFO")
    
    if not service_account_info:
        raise ValueError(
            "A variável de ambiente GOOGLE_SERVICE_ACCOUNT_INFO não foi encontrada. "
            "Certifique-se de definir o JSON da Service Account como uma string no seu arquivo .env ou ambiente."
        )
    
    try:
        info = json.loads(service_account_info)
        creds = service_account.Credentials.from_service_account_info(info, scopes=scopes)
        service = build('calendar', 'v3', credentials=creds)

        # Se um calendarId customizado foi configurado, garante que ele esteja presente na 
        # calendarList da service account. Se não estiver, será inserido.
        try:
            calendar_id = DEFAULT_CALENDAR_ID
            if calendar_id and calendar_id != 'primary':
                try:
                    service.calendarList().get(calendarId=calendar_id).execute()
                except HttpError as he:
                    status = getattr(he.resp, 'status', None)
                    if status == 404 or str(status) == '404':
                        service.calendarList().insert(body={'id': calendar_id}).execute()
                    else:
                        raise
        except Exception as ex:
            print(f"Aviso: não foi possível garantir a presença do calendarId '{calendar_id}' na calendarList: {str(ex)}") # type: ignore
            pass

        return service
    except json.JSONDecodeError as e:
        logger.error(f"JSON inválido em GOOGLE_SERVICE_ACCOUNT_INFO: {str(e)}")
        raise RuntimeError("A variável GOOGLE_SERVICE_ACCOUNT_INFO não contém um JSON válido.")
    except Exception as e:
        logger.error(f"Erro ao inicializar Google Calendar service: {type(e).__name__}: {str(e)}")
        raise RuntimeError(f"Erro ao inicializar o serviço do Google Calendar: {str(e)}")

@server.tool()
def ping() -> str:
    """Ferramenta de teste para validar o servidor MCP."""
    return "pong pong"

@server.tool()
def list_events(max_results: int = 10) -> str:
    """Lista os próximos eventos da agenda.

    Args:
        max_results: número máximo de eventos a retornar.
    """
    try:
        service = get_calendar_service()
        now = datetime.utcnow().isoformat() + 'Z'

        calendar_id = DEFAULT_CALENDAR_ID

        events_result = service.events().list(
            calendarId=calendar_id,
            timeMin=now,
            maxResults=max_results,
            singleEvents=True,
            orderBy='startTime'
        ).execute()
        
        events = events_result.get('items', [])
        if not events:
            return "Nenhum evento encontrado."

        lines = ["Próximos eventos:"]
        for event in events:
            start = event['start'].get('dateTime', event['start'].get('date'))
            event_id = event.get('id')
            lines.append(f"- {start}: {event.get('summary', '(Sem título)')} (ID: {event_id})")
        
        return "\n".join(lines)
    except HttpError as e:
        logger.error(f"Erro HTTP ao listar eventos: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao listar eventos: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao acessar a agenda: {type(e).__name__}: {str(e)}"

@server.tool()
def quick_add_event(summary: str, duration_minutes: int = 30) -> str:
    """Adiciona um evento rápido na agenda começando AGORA no timezone local configurado.

    Args:
        summary: título do evento.
        duration_minutes: duração em minutos (padrão: 30).
        
    Nota:
        O evento será criado no timezone: {tz}
        Exemplo: Se agora são 15:30 em São Paulo, o evento começa às 15:30 (horário local).
    """.format(tz=DEFAULT_TIMEZONE)
    try:
        service = get_calendar_service()
        start = datetime.now(LOCAL_TZ)
        end = start + timedelta(minutes=duration_minutes)

        calendar_id = DEFAULT_CALENDAR_ID

        event = {
            'summary': summary,
            'start': {'dateTime': format_datetime_for_api(start)},
            'end': {'dateTime': format_datetime_for_api(end)},
        }

        created_event = service.events().insert(calendarId=calendar_id, body=event).execute()
        logger.info(f"Evento rápido criado: {created_event.get('id')}")
        return f"Evento criado com sucesso: {created_event.get('htmlLink')}"
    except HttpError as e:
        logger.error(f"Erro HTTP ao criar evento rápido: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao criar evento rápido: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao criar evento: {type(e).__name__}: {str(e)}"

@server.tool()
def add_event_at(summary: str, start_iso: str, duration_minutes: int = 60) -> str:
    """Cria um evento em uma data/hora específica.

    Args:
        summary: título do evento.
        start_iso: data/hora inicial em ISO 8601. IMPORTANTE sobre timezone:
            - COM timezone explícito: "2026-01-12T15:00:00-03:00" ou "2026-01-12T15:00:00Z"
              → Usa exatamente o timezone especificado
            - SEM timezone: "2026-01-12T15:00:00"
              → Interpreta como horário LOCAL ({tz})
              → Exemplo: 23:00 será 23:00 em São Paulo, NÃO em UTC!
        duration_minutes: duração em minutos (padrão: 60).
        
    Exemplos corretos:
        - "2026-01-12T23:00:00" → 23h no horário de {tz}
        - "2026-01-12T23:00:00-03:00" → 23h UTC-3 (explícito)
        - "2026-01-12T23:00:00Z" → 23h UTC (20h em Brasília)
    """.format(tz=DEFAULT_TIMEZONE)
    try:
        service = get_calendar_service()

        # parse start
        try:
            start_dt = datetime.fromisoformat(start_iso)
        except Exception as e:
            logger.warning(f"Formato de data inválido: {start_iso}")
            return f"start_iso inválido. Use formato ISO 8601.\nExemplos:\n- Com timezone: 2026-01-12T23:00:00-03:00\n- Sem timezone (usa {DEFAULT_TIMEZONE}): 2026-01-12T23:00:00"

        # Log para debug de timezone
        if start_dt.tzinfo is None:
            logger.info(f"Data sem timezone '{start_iso}' interpretada como {DEFAULT_TIMEZONE}")
        else:
            logger.info(f"Data com timezone explícito: {start_iso}")

        # normalize to RFC3339 string
        start_str = format_datetime_for_api(start_dt, assume_local=True)
        end_dt = start_dt + timedelta(minutes=duration_minutes)
        end_str = format_datetime_for_api(end_dt, assume_local=True)

        calendar_id = DEFAULT_CALENDAR_ID

        event = {
            'summary': summary,
            'start': {'dateTime': start_str},
            'end': {'dateTime': end_str},
        }

        created = service.events().insert(calendarId=calendar_id, body=event).execute()
        logger.info(f"Evento criado em data específica: {created.get('id')}")
        return f"Evento criado com sucesso: {created.get('htmlLink')} (id: {created.get('id')})"
    except HttpError as e:
        logger.error(f"Erro HTTP ao criar evento: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao criar evento em data específica: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao criar evento: {type(e).__name__}: {str(e)}"

@server.tool()
def edit_event(event_id: str, summary: Optional[str] = None, start_iso: Optional[str] = None, duration_minutes: Optional[int] = None) -> str:
    """Edita um evento existente pelo ID.

    Args:
        event_id: ID do evento (obtido via list_events)
        summary: novo título (opcional)
        start_iso: nova data/hora inicial (opcional). Veja add_event_at para exemplos de timezone.
            - SEM timezone: interpretado como {tz}
            - COM timezone: usa o timezone especificado
        duration_minutes: nova duração em minutos (opcional)
        
    Forneça pelo menos um dos campos para atualizar: `summary`, `start_iso` ou `duration_minutes`.
    """.format(tz=DEFAULT_TIMEZONE)
    try:
        service = get_calendar_service()
        calendar_id = DEFAULT_CALENDAR_ID

        event = service.events().get(calendarId=calendar_id, eventId=event_id).execute()

        if summary is not None:
            event['summary'] = summary

        # determine start and end
        if start_iso is not None:
            try:
                new_start = datetime.fromisoformat(start_iso)
            except Exception as e:
                logger.warning(f"Formato de data inválido em edit_event: {start_iso}")
                return f"start_iso inválido. Use formato ISO 8601.\nSem timezone usa {DEFAULT_TIMEZONE}."

            # Log timezone usado
            if new_start.tzinfo is None:
                logger.info(f"Edit: data sem timezone '{start_iso}' interpretada como {DEFAULT_TIMEZONE}")
            
            start_str = format_datetime_for_api(new_start, assume_local=True)

            # compute end
            if duration_minutes is not None:
                new_end = new_start + timedelta(minutes=duration_minutes)
            else:
                # try keep original duration if available
                orig_start = event['start'].get('dateTime')
                orig_end = event['end'].get('dateTime')
                if orig_start and orig_end:
                    try:
                        orig_s = datetime.fromisoformat(orig_start.replace('Z', '+00:00'))
                        orig_e = datetime.fromisoformat(orig_end.replace('Z', '+00:00'))
                        delta = orig_e - orig_s
                        new_end = new_start + delta
                    except Exception:
                        new_end = new_start + timedelta(minutes=60)
                else:
                    new_end = new_start + timedelta(minutes=60)

            end_str = format_datetime_for_api(new_end, assume_local=True)

            event['start'] = {'dateTime': start_str}
            event['end'] = {'dateTime': end_str}

        elif duration_minutes is not None:
            # only duration provided: shift end based on existing start
            orig_start = event['start'].get('dateTime')
            if not orig_start:
                return "Não foi possível atualizar duração: evento não tem start com dateTime"
            try:
                orig_s = datetime.fromisoformat(orig_start.replace('Z', '+00:00'))
            except Exception as e:
                logger.warning(f"Formato de start original inválido: {orig_start}")
                return "Formato de start original do evento inválido"
            new_end = orig_s + timedelta(minutes=duration_minutes)
            end_str = format_datetime_for_api(new_end, assume_local=False)  # Preserva timezone original
            event['end'] = {'dateTime': end_str}

        
        updated = service.events().update(calendarId=calendar_id, eventId=event_id, body=event).execute()
        logger.info(f"Evento atualizado: {event_id}")
        return f"Evento atualizado com sucesso: {updated.get('htmlLink')} (id: {updated.get('id')})"
    except HttpError as e:
        logger.error(f"Erro HTTP ao editar evento {event_id}: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao editar evento {event_id}: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao editar evento: {type(e).__name__}: {str(e)}"

@server.tool()
def delete_event(event_id: str) -> str:
    """Deleta um evento pelo ID."""
    try:
        service = get_calendar_service()
        calendar_id = DEFAULT_CALENDAR_ID
        service.events().delete(calendarId=calendar_id, eventId=event_id).execute()
        logger.info(f"Evento deletado: {event_id}")
        return f"Evento {event_id} removido com sucesso."
    except HttpError as e:
        logger.error(f"Erro HTTP ao deletar evento {event_id}: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao deletar evento {event_id}: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao deletar evento: {type(e).__name__}: {str(e)}"


@server.tool()
def get_event(event_id: str) -> str:
    """Retorna detalhes de um evento pelo ID (summary, start, end, reminders)."""
    try:
        service = get_calendar_service()
        calendar_id = DEFAULT_CALENDAR_ID
        event = service.events().get(calendarId=calendar_id, eventId=event_id).execute()

        summary = event.get('summary')
        start = event.get('start', {}).get('dateTime', event.get('start', {}).get('date'))
        end = event.get('end', {}).get('dateTime', event.get('end', {}).get('date'))
        lines = [f"id: {event.get('id')}", f"summary: {summary}", f"start: {start}", f"end: {end}"]
        logger.info(f"Evento recuperado: {event_id}")
        return "\n".join(lines)
    except HttpError as e:
        logger.error(f"Erro HTTP ao recuperar evento {event_id}: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao recuperar evento {event_id}: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao recuperar evento: {type(e).__name__}: {str(e)}"


@server.tool()
def get_calendar_settings() -> str:
    """Retorna configurações do calendário."""
    try:
        service = get_calendar_service()
        calendar_id = DEFAULT_CALENDAR_ID

        cal_list = service.calendarList().get(calendarId=calendar_id).execute()
        access_role = cal_list.get('accessRole')

        info = {
            'calendarId': calendar_id,
            'accessRole': access_role,
        }
        logger.info(f"Configurações do calendário recuperadas para: {calendar_id}")
        return json.dumps(info, indent=2)
    except HttpError as e:
        logger.error(f"Erro HTTP ao recuperar configurações: {e.resp.status} - {e.error_details}")
        return f"Erro na API do Google Calendar: {e.resp.status} - {e.error_details}"
    except Exception as e:
        logger.error(f"Erro inesperado ao recuperar configurações: {type(e).__name__}: {str(e)}")
        return f"Erro inesperado ao recuperar configurações: {type(e).__name__}: {str(e)}"

if __name__ == "__main__":
    server.run(transport="streamable-http")
