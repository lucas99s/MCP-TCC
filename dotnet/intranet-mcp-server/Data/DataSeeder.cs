using intranet_mcp_server.Entities;
using Microsoft.EntityFrameworkCore;

namespace intranet_mcp_server.Data;

/// <summary>
/// Seeds the database with realistic, coherent sample data for the MCP experiment.
/// Safe to run multiple times — skips seeding if data already exists.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Guard: skip if already seeded
        if (await db.Departments.AnyAsync()) return;

        // ── 1. DEPARTMENTS ───────────────────────────────────────────────────
        var departments = new List<Department>
        {
            new() { Name = "Tecnologia da Informação", Description = "Responsável pelo desenvolvimento e manutenção de sistemas internos e infraestrutura de TI." },
            new() { Name = "Recursos Humanos",          Description = "Gestão de pessoas, recrutamento, benefícios e cultura organizacional." },
            new() { Name = "Financeiro",                Description = "Controle orçamentário, folha de pagamento, contabilidade e relatórios financeiros." },
            new() { Name = "Comercial",                 Description = "Vendas, atendimento ao cliente e gestão de contratos comerciais." },
            new() { Name = "Operações",                 Description = "Logística, suporte operacional e gestão de processos internos." },
        };
        db.Departments.AddRange(departments);
        await db.SaveChangesAsync();

        // ── 2. POSITIONS ─────────────────────────────────────────────────────
        var positions = new List<Position>
        {
            new() { Title = "Desenvolvedor de Software",  Level = "Junior"      },
            new() { Title = "Desenvolvedor de Software",  Level = "Mid"         },
            new() { Title = "Desenvolvedor de Software",  Level = "Senior"      },
            new() { Title = "Arquiteto de Software",      Level = "Especialista" },
            new() { Title = "Gerente de TI",              Level = "Gestor"      },
            new() { Title = "Analista de RH",             Level = "Mid"         },
            new() { Title = "Gerente de RH",              Level = "Gestor"      },
            new() { Title = "Analista Financeiro",        Level = "Senior"      },
            new() { Title = "Gerente Financeiro",         Level = "Gestor"      },
            new() { Title = "Analista Comercial",         Level = "Mid"         },
            new() { Title = "Gerente Comercial",          Level = "Gestor"      },
            new() { Title = "Analista de Operações",      Level = "Junior"      },
            new() { Title = "Gerente de Operações",       Level = "Gestor"      },
        };
        db.Positions.AddRange(positions);
        await db.SaveChangesAsync();

        // shortcuts
        var dTI       = departments[0]; var dRH  = departments[1];
        var dFin      = departments[2]; var dCom = departments[3];
        var dOps      = departments[4];

        var pDevJr    = positions[0];  var pDevMid  = positions[1];
        var pDevSr    = positions[2];  var pArq     = positions[3];
        var pGerTI    = positions[4];  var pAnRH    = positions[5];
        var pGerRH    = positions[6];  var pAnFin   = positions[7];
        var pGerFin   = positions[8];  var pAnCom   = positions[9];
        var pGerCom   = positions[10]; var pAnOps   = positions[11];
        var pGerOps   = positions[12];

        // ── 3. EMPLOYEES (managers first, then reports) ──────────────────────

        // --- Gestores (ManagerId = null) ---
        var gerTI  = new Employee { FullName = "Carlos Mendes",    Email = "carlos.mendes@empresa.com",   RegistrationNumber = "EMP001", HireDate = new DateTime(2015, 3, 10, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1980, 6, 22, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo", DepartmentId = dTI.Id,  PositionId = pGerTI.Id,  ManagerId = null };
        var gerRH  = new Employee { FullName = "Fernanda Costa",   Email = "fernanda.costa@empresa.com",  RegistrationNumber = "EMP002", HireDate = new DateTime(2016, 7, 1, 0, 0, 0, DateTimeKind.Utc),   BirthDate = new DateTime(1982, 9, 14, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo", DepartmentId = dRH.Id,  PositionId = pGerRH.Id,  ManagerId = null };
        var gerFin = new Employee { FullName = "Roberto Alves",    Email = "roberto.alves@empresa.com",   RegistrationNumber = "EMP003", HireDate = new DateTime(2014, 1, 15, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1978, 3, 5, 0, 0, 0, DateTimeKind.Utc),   Status = "ativo", DepartmentId = dFin.Id, PositionId = pGerFin.Id, ManagerId = null };
        var gerCom = new Employee { FullName = "Patricia Souza",   Email = "patricia.souza@empresa.com",  RegistrationNumber = "EMP004", HireDate = new DateTime(2017, 4, 20, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1985, 11, 30, 0, 0, 0, DateTimeKind.Utc), Status = "ativo", DepartmentId = dCom.Id, PositionId = pGerCom.Id, ManagerId = null };
        var gerOps = new Employee { FullName = "Marcos Oliveira",  Email = "marcos.oliveira@empresa.com", RegistrationNumber = "EMP005", HireDate = new DateTime(2013, 8, 5, 0, 0, 0, DateTimeKind.Utc),   BirthDate = new DateTime(1977, 2, 18, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo", DepartmentId = dOps.Id, PositionId = pGerOps.Id, ManagerId = null };

        db.Employees.AddRange(gerTI, gerRH, gerFin, gerCom, gerOps);
        await db.SaveChangesAsync();

        // --- Colaboradores (apontam para seus gestores) ---
        var employees = new List<Employee>
        {
            // TI
            new() { FullName = "Ana Lima",        Email = "ana.lima@empresa.com",        RegistrationNumber = "EMP006", HireDate = new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1995, 7, 10, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dTI.Id,  PositionId = pDevJr.Id,  ManagerId = gerTI.Id },
            new() { FullName = "Bruno Santos",    Email = "bruno.santos@empresa.com",    RegistrationNumber = "EMP007", HireDate = new DateTime(2019, 5, 15, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1993, 4, 25, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dTI.Id,  PositionId = pDevMid.Id, ManagerId = gerTI.Id },
            new() { FullName = "Juliana Ferreira",Email = "juliana.ferreira@empresa.com",RegistrationNumber = "EMP008", HireDate = new DateTime(2018, 9, 10, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1991, 1, 8, 0, 0, 0, DateTimeKind.Utc),   Status = "ativo",    DepartmentId = dTI.Id,  PositionId = pDevSr.Id,  ManagerId = gerTI.Id },
            new() { FullName = "Rafael Moreira",  Email = "rafael.moreira@empresa.com",  RegistrationNumber = "EMP009", HireDate = new DateTime(2017, 11, 20, 0, 0, 0, DateTimeKind.Utc),BirthDate = new DateTime(1988, 5, 17, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dTI.Id,  PositionId = pArq.Id,    ManagerId = gerTI.Id },
            new() { FullName = "Camila Rocha",    Email = "camila.rocha@empresa.com",    RegistrationNumber = "EMP010", HireDate = new DateTime(2021, 3, 8, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1997, 8, 3, 0, 0, 0, DateTimeKind.Utc),   Status = "afastado", DepartmentId = dTI.Id,  PositionId = pDevJr.Id,  ManagerId = gerTI.Id },
            // RH
            new() { FullName = "Thiago Barbosa",  Email = "thiago.barbosa@empresa.com",  RegistrationNumber = "EMP011", HireDate = new DateTime(2019, 6, 3, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1990, 12, 21, 0, 0, 0, DateTimeKind.Utc), Status = "ativo",    DepartmentId = dRH.Id,  PositionId = pAnRH.Id,   ManagerId = gerRH.Id },
            new() { FullName = "Larissa Nunes",   Email = "larissa.nunes@empresa.com",   RegistrationNumber = "EMP012", HireDate = new DateTime(2022, 1, 17, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1996, 3, 29, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dRH.Id,  PositionId = pAnRH.Id,   ManagerId = gerRH.Id },
            // Financeiro
            new() { FullName = "Diego Carvalho",  Email = "diego.carvalho@empresa.com",  RegistrationNumber = "EMP013", HireDate = new DateTime(2018, 4, 22, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1989, 9, 11, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dFin.Id, PositionId = pAnFin.Id,  ManagerId = gerFin.Id },
            new() { FullName = "Isabela Martins", Email = "isabela.martins@empresa.com", RegistrationNumber = "EMP014", HireDate = new DateTime(2020, 8, 5, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1994, 6, 15, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dFin.Id, PositionId = pAnFin.Id,  ManagerId = gerFin.Id },
            // Comercial
            new() { FullName = "Lucas Teixeira",  Email = "lucas.teixeira@empresa.com",  RegistrationNumber = "EMP015", HireDate = new DateTime(2021, 7, 12, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1993, 10, 4, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dCom.Id, PositionId = pAnCom.Id,  ManagerId = gerCom.Id },
            new() { FullName = "Mariana Vieira",  Email = "mariana.vieira@empresa.com",  RegistrationNumber = "EMP016", HireDate = new DateTime(2020, 11, 9, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1992, 2, 7, 0, 0, 0, DateTimeKind.Utc),   Status = "ativo",    DepartmentId = dCom.Id, PositionId = pAnCom.Id,  ManagerId = gerCom.Id },
            // Operações
            new() { FullName = "Felipe Gomes",    Email = "felipe.gomes@empresa.com",    RegistrationNumber = "EMP017", HireDate = new DateTime(2022, 5, 23, 0, 0, 0, DateTimeKind.Utc), BirthDate = new DateTime(1998, 7, 19, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dOps.Id, PositionId = pAnOps.Id,  ManagerId = gerOps.Id },
            new() { FullName = "Amanda Freitas",  Email = "amanda.freitas@empresa.com",  RegistrationNumber = "EMP018", HireDate = new DateTime(2023, 2, 6, 0, 0, 0, DateTimeKind.Utc),  BirthDate = new DateTime(1999, 5, 28, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",    DepartmentId = dOps.Id, PositionId = pAnOps.Id,  ManagerId = gerOps.Id },
        };
        db.Employees.AddRange(employees);
        await db.SaveChangesAsync();

        // Shortcut vars for employees
        var ana  = employees[0];
        var bruno = employees[1];
        var juliana = employees[2];
        var rafael = employees[3];
        var thiago = employees[5];
        var lucas = employees[9];
        var felipe = employees[11];
        var amanda = employees[12];

        // ── 4. VACATION BALANCES ─────────────────────────────────────────────
        var allEmployees = new List<Employee> { gerTI, gerRH, gerFin, gerCom, gerOps }.Concat(employees).ToList();
        var vacations = allEmployees.Select(e => new VacationBalance
        {
            EmployeeId    = e.Id,
            ReferenceYear = 2025,
            EarnedDays    = 30,
            UsedDays      = new Random(e.Id).Next(0, 25),
            ExpiryDate    = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
        }).ToList();

        // fix RemainingDays
        vacations.ForEach(v => v.RemainingDays = v.EarnedDays - v.UsedDays);
        db.VacationBalances.AddRange(vacations);
        await db.SaveChangesAsync();

        // ── 5. LEAVE REQUESTS ────────────────────────────────────────────────
        var leaveRequests = new List<LeaveRequest>
        {
            new() { EmployeeId = ana.Id,     Type = "férias",              StartDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = new DateTime(2025, 7, 30, 0, 0, 0, DateTimeKind.Utc),  Status = "aprovado",  RequestedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),  ApprovedById = gerTI.Id },
            new() { EmployeeId = bruno.Id,   Type = "folga",               StartDate = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),   EndDate = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),   Status = "aprovado",  RequestedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),  ApprovedById = gerTI.Id },
            new() { EmployeeId = juliana.Id, Type = "licença médica",      StartDate = new DateTime(2025, 10, 10, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2025, 10, 17, 0, 0, 0, DateTimeKind.Utc), Status = "aprovado",  RequestedAt = new DateTime(2025, 10, 9, 0, 0, 0, DateTimeKind.Utc), ApprovedById = gerTI.Id },
            new() { EmployeeId = thiago.Id,  Type = "férias",              StartDate = new DateTime(2025, 12, 20, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),  Status = "aprovado",  RequestedAt = new DateTime(2025, 11, 15, 0, 0, 0, DateTimeKind.Utc),ApprovedById = gerRH.Id  },
            new() { EmployeeId = lucas.Id,   Type = "ausência justificada",StartDate = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc),  Status = "pendente",  RequestedAt = new DateTime(2026, 1, 18, 0, 0, 0, DateTimeKind.Utc), ApprovedById = gerCom.Id },
            new() { EmployeeId = felipe.Id,  Type = "folga",               StartDate = new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2026, 2, 14, 0, 0, 0, DateTimeKind.Utc),  Status = "rejeitado", RequestedAt = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc), ApprovedById = gerOps.Id },
            new() { EmployeeId = amanda.Id,  Type = "férias",              StartDate = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc),  Status = "pendente",  RequestedAt = new DateTime(2026, 2, 20, 0, 0, 0, DateTimeKind.Utc), ApprovedById = gerOps.Id },
        };
        db.LeaveRequests.AddRange(leaveRequests);
        await db.SaveChangesAsync();

        // ── 6. PROJECTS ──────────────────────────────────────────────────────
        var projects = new List<Project>
        {
            new() { Name = "Portal do Colaborador",    Code = "PROJ-001", Description = "Desenvolvimento do portal interno de autoatendimento para funcionários.",        StartDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc), Status = "concluido",   DepartmentId = dTI.Id,  ProjectManagerId = gerTI.Id  },
            new() { Name = "Migração para a Nuvem",    Code = "PROJ-002", Description = "Migração da infraestrutura on-premise para serviços de nuvem AWS.",              StartDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = null,                                                  Status = "ativo",       DepartmentId = dTI.Id,  ProjectManagerId = rafael.Id },
            new() { Name = "CRM Comercial",            Code = "PROJ-003", Description = "Implantação de sistema de CRM para gestão do relacionamento com clientes.",      StartDate = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),  Status = "ativo",       DepartmentId = dCom.Id, ProjectManagerId = gerCom.Id },
            new() { Name = "Automação de Folha",       Code = "PROJ-004", Description = "Automação do processo de geração e distribuição da folha de pagamento.",         StartDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = new DateTime(2025, 6, 30, 0, 0, 0, DateTimeKind.Utc),  Status = "concluido",   DepartmentId = dFin.Id, ProjectManagerId = gerFin.Id },
            new() { Name = "Plataforma de Treinamento",Code = "PROJ-005", Description = "Criação de plataforma e-learning para capacitação e desenvolvimento de equipes.",StartDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),   EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc), Status = "ativo",       DepartmentId = dRH.Id,  ProjectManagerId = gerRH.Id  },
        };
        db.Projects.AddRange(projects);
        await db.SaveChangesAsync();

        var pPortal   = projects[0]; var pCloud = projects[1];
        var pCRM      = projects[2]; var pTrein = projects[4];

        // ── 7. PROJECT ALLOCATIONS ───────────────────────────────────────────
        var allocations = new List<ProjectAllocation>
        {
            // Portal do Colaborador
            new() { EmployeeId = ana.Id,     ProjectId = pPortal.Id, RoleOnProject = "Desenvolvedora Frontend",  StartDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc) },
            new() { EmployeeId = bruno.Id,   ProjectId = pPortal.Id, RoleOnProject = "Desenvolvedor Backend",    StartDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc) },
            new() { EmployeeId = juliana.Id, ProjectId = pPortal.Id, RoleOnProject = "Tech Lead",                StartDate = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),  EndDate = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc) },
            // Migração para a Nuvem
            new() { EmployeeId = rafael.Id,  ProjectId = pCloud.Id,  RoleOnProject = "Arquiteto de Solução",     StartDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = null },
            new() { EmployeeId = bruno.Id,   ProjectId = pCloud.Id,  RoleOnProject = "Desenvolvedor Backend",    StartDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = null },
            new() { EmployeeId = juliana.Id, ProjectId = pCloud.Id,  RoleOnProject = "Engenheira de Software Sr",StartDate = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = null },
            // CRM Comercial
            new() { EmployeeId = lucas.Id,   ProjectId = pCRM.Id,    RoleOnProject = "Analista de Negócios",     StartDate = new DateTime(2025, 3, 15, 0, 0, 0, DateTimeKind.Utc),  EndDate = null },
            new() { EmployeeId = ana.Id,     ProjectId = pCRM.Id,    RoleOnProject = "Desenvolvedora Frontend",  StartDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc),   EndDate = null },
            // Plataforma de Treinamento
            new() { EmployeeId = thiago.Id,  ProjectId = pTrein.Id,  RoleOnProject = "Analista de Conteúdo",     StartDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),   EndDate = null },
            new() { EmployeeId = ana.Id,     ProjectId = pTrein.Id,  RoleOnProject = "Desenvolvedora Frontend",  StartDate = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),   EndDate = null },
        };
        db.ProjectAllocations.AddRange(allocations);
        await db.SaveChangesAsync();

        // ── 8. PROJECT TASKS ─────────────────────────────────────────────────
        var tasks = new List<ProjectTask>
        {
            // Portal do Colaborador (concluído)
            new() { ProjectId = pPortal.Id, AssignedEmployeeId = ana.Id,     Title = "Desenvolver tela de login",         Description = "Implementar autenticação SSO com Azure AD.",             Status = "concluido",    Priority = "Alta",   DueDate = new DateTime(2025, 2, 28, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pPortal.Id, AssignedEmployeeId = bruno.Id,   Title = "Criar API de funcionários",         Description = "Endpoint REST para consulta de dados do colaborador.",   Status = "concluido",    Priority = "Alta",   DueDate = new DateTime(2025, 3, 31, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pPortal.Id, AssignedEmployeeId = juliana.Id, Title = "Code review e testes de integração",Description = "Revisão do código e execução dos testes E2E.",           Status = "concluido",    Priority = "Média",  DueDate = new DateTime(2025, 11, 30, 0, 0, 0, DateTimeKind.Utc)},
            // Migração para a Nuvem (ativo)
            new() { ProjectId = pCloud.Id,  AssignedEmployeeId = rafael.Id,  Title = "Definir arquitetura AWS",           Description = "Desenhar diagrama e escolher serviços AWS necessários.", Status = "concluido",    Priority = "Alta",   DueDate = new DateTime(2025, 7, 15, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pCloud.Id,  AssignedEmployeeId = bruno.Id,   Title = "Migrar banco de dados para RDS",    Description = "Exportar, transformar e importar dados no Amazon RDS.",  Status = "em andamento", Priority = "Alta",   DueDate = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pCloud.Id,  AssignedEmployeeId = juliana.Id, Title = "Configurar pipelines CI/CD",        Description = "Configurar GitHub Actions para deploy automático.",      Status = "em andamento", Priority = "Média",  DueDate = new DateTime(2026, 4, 30, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pCloud.Id,  AssignedEmployeeId = rafael.Id,  Title = "Testes de carga e performance",     Description = "Validar capacidade da nova infraestrutura em nuvem.",    Status = "aberto",       Priority = "Média",  DueDate = new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc) },
            // CRM Comercial
            new() { ProjectId = pCRM.Id,    AssignedEmployeeId = lucas.Id,   Title = "Mapeamento de processos comerciais",Description = "Levantar e documentar os fluxos do time comercial.",     Status = "concluido",    Priority = "Alta",   DueDate = new DateTime(2025, 4, 30, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pCRM.Id,    AssignedEmployeeId = ana.Id,     Title = "Personalizar interface do CRM",     Description = "Adaptar dashboard e campos do CRM ao processo interno.", Status = "em andamento", Priority = "Média",  DueDate = new DateTime(2026, 4, 15, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pCRM.Id,    AssignedEmployeeId = lucas.Id,   Title = "Treinamento da equipe comercial",   Description = "Conduzir sessões de treinamento para o time de vendas.", Status = "aberto",       Priority = "Baixa",  DueDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc) },
            // Plataforma de Treinamento
            new() { ProjectId = pTrein.Id,  AssignedEmployeeId = thiago.Id,  Title = "Definir catálogo de cursos",        Description = "Mapear necessidades de treinamento por área.",           Status = "concluido",    Priority = "Alta",   DueDate = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc) },
            new() { ProjectId = pTrein.Id,  AssignedEmployeeId = ana.Id,     Title = "Desenvolver módulo de cursos",      Description = "Implementar tela de listagem e reprodução de conteúdo.", Status = "em andamento", Priority = "Alta",   DueDate = new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Utc) },
        };
        db.ProjectTasks.AddRange(tasks);
        await db.SaveChangesAsync();

        // ── 9. TIME ENTRIES ──────────────────────────────────────────────────
        var timeEntries = new List<TimeEntry>();

        void AddEntries(int empId, int projId, int approvedBy, IEnumerable<(DateTime date, double hours, string type, string desc)> entries)
        {
            foreach (var (date, hours, type, desc) in entries)
                timeEntries.Add(new TimeEntry { EmployeeId = empId, ProjectId = projId, EntryDate = date, HoursWorked = hours, EntryType = type, Description = desc, ApprovedById = approvedBy });
        }

        // Ana — Portal + CRM + Treinamento
        AddEntries(ana.Id, pPortal.Id, gerTI.Id, new[]
        {
            (new DateTime(2025, 11, 4, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Desenvolvimento de tela de perfil do colaborador"),
            (new DateTime(2025, 11, 5, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Integração com API de RH"),
            (new DateTime(2025, 11, 6, 0, 0, 0, DateTimeKind.Utc),  4.0, "normal", "Correção de bugs reportados em homologação"),
            (new DateTime(2025, 11, 7, 0, 0, 0, DateTimeKind.Utc),  2.0, "extra",  "Deploy em produção fora do horário"),
        });
        AddEntries(ana.Id, pCRM.Id, gerTI.Id, new[]
        {
            (new DateTime(2026, 1, 6, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Customização do dashboard do CRM"),
            (new DateTime(2026, 1, 7, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Implementação de filtros avançados"),
            (new DateTime(2026, 1, 8, 0, 0, 0, DateTimeKind.Utc),  6.0, "normal", "Testes unitários do módulo CRM"),
        });
        AddEntries(ana.Id, pTrein.Id, gerTI.Id, new[]
        {
            (new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Criação de protótipos do módulo de cursos"),
            (new DateTime(2026, 2, 4, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Desenvolvimento do player de vídeo"),
        });

        // Bruno — Portal + Cloud
        AddEntries(bruno.Id, pPortal.Id, gerTI.Id, new[]
        {
            (new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Desenvolvimento de endpoint de autenticação"),
            (new DateTime(2025, 10, 2, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Implementação de cache Redis para sessões"),
            (new DateTime(2025, 10, 3, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Documentação da API com Swagger"),
        });
        AddEntries(bruno.Id, pCloud.Id, gerTI.Id, new[]
        {
            (new DateTime(2026, 1, 13, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Configuração do Amazon RDS PostgreSQL"),
            (new DateTime(2026, 1, 14, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Script de migração de dados legados"),
            (new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Validação da integridade dos dados migrados"),
            (new DateTime(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc), 4.0, "extra",  "Correção emergencial de falha na migração"),
        });

        // Rafael — Cloud
        AddEntries(rafael.Id, pCloud.Id, gerTI.Id, new[]
        {
            (new DateTime(2025, 6, 3, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Workshop de definição da arquitetura AWS"),
            (new DateTime(2025, 6, 4, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Documentação do diagrama de infraestrutura"),
            (new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc), 8.0, "normal", "Planejamento dos testes de carga"),
            (new DateTime(2026, 2, 11, 0, 0, 0, DateTimeKind.Utc), 6.0, "normal", "Execução de testes com Apache JMeter"),
        });

        // Lucas — CRM
        AddEntries(lucas.Id, pCRM.Id, gerCom.Id, new[]
        {
            (new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Entrevistas com equipe comercial para mapeamento"),
            (new DateTime(2025, 4, 2, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Documentação dos processos AS-IS"),
            (new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc),  8.0, "normal", "Proposição de melhorias e processos TO-BE"),
        });

        db.TimeEntries.AddRange(timeEntries);
        await db.SaveChangesAsync();

        // ── 10. PAYROLL SUMMARIES ────────────────────────────────────────────
        var payrolls = new List<PayrollSummary>();

        void AddPayrolls(int empId, decimal gross, decimal deductions)
        {
            for (int month = 1; month <= 12; month++)
            {
                payrolls.Add(new PayrollSummary
                {
                    EmployeeId     = empId,
                    ReferenceMonth = month,
                    ReferenceYear  = 2025,
                    GrossSalary    = gross,
                    Deductions     = deductions,
                    NetSalary      = gross - deductions,
                    PaymentDate    = new DateTime(2025, month, 5, 0, 0, 0, DateTimeKind.Utc),
                    Status         = "pago"
                });
            }
            // Janeiro/2026
            payrolls.Add(new PayrollSummary
            {
                EmployeeId     = empId,
                ReferenceMonth = 1,
                ReferenceYear  = 2026,
                GrossSalary    = gross,
                Deductions     = deductions,
                NetSalary      = gross - deductions,
                PaymentDate    = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                Status         = "pago"
            });
        }

        AddPayrolls(gerTI.Id,  22000m, 5500m);
        AddPayrolls(gerRH.Id,  18000m, 4500m);
        AddPayrolls(gerFin.Id, 20000m, 5000m);
        AddPayrolls(gerCom.Id, 19000m, 4750m);
        AddPayrolls(gerOps.Id, 17000m, 4250m);
        AddPayrolls(ana.Id,     6500m, 1300m);
        AddPayrolls(bruno.Id,   8500m, 1870m);
        AddPayrolls(juliana.Id,12000m, 2880m);
        AddPayrolls(rafael.Id, 15000m, 3750m);
        AddPayrolls(lucas.Id,   9000m, 2070m);

        db.PayrollSummaries.AddRange(payrolls);
        await db.SaveChangesAsync();
    }
}
