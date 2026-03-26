using intranet_mcp_server.Entities;
using Microsoft.EntityFrameworkCore;

namespace intranet_mcp_server.Data
{
    public class AppDbContext : DbContext
    {
        // Schema constants
        public const string SchemaHr        = "hr";
        public const string SchemaProjects  = "projects";
        public const string SchemaFinance   = "finance";
        public const string SchemaTimesheet = "timesheet";

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Department>        Departments        => Set<Department>();
        public DbSet<Position>          Positions          => Set<Position>();
        public DbSet<Employee>          Employees          => Set<Employee>();
        public DbSet<Project>           Projects           => Set<Project>();
        public DbSet<ProjectAllocation> ProjectAllocations => Set<ProjectAllocation>();
        public DbSet<ProjectTask>       ProjectTasks       => Set<ProjectTask>();
        public DbSet<LeaveRequest>      LeaveRequests      => Set<LeaveRequest>();
        public DbSet<TimeEntry>         TimeEntries        => Set<TimeEntry>();
        public DbSet<VacationBalance>   VacationBalances   => Set<VacationBalance>();
        public DbSet<PayrollSummary>    PayrollSummaries   => Set<PayrollSummary>();
        public DbSet<TimeBankBalance>   TimeBankBalances   => Set<TimeBankBalance>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── HR ───────────────────────────────────────────────────

            modelBuilder.Entity<Department>(e =>
            {
                e.ToTable("Departments", SchemaHr);
                e.HasKey(d => d.Id);
                e.Property(d => d.Name).IsRequired().HasMaxLength(100);
                e.Property(d => d.Description).HasMaxLength(500);
            });

            modelBuilder.Entity<Position>(e =>
            {
                e.ToTable("Positions", SchemaHr);
                e.HasKey(p => p.Id);
                e.Property(p => p.Title).IsRequired().HasMaxLength(100);
                e.Property(p => p.Level).HasMaxLength(50);
            });

            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("Employees", SchemaHr);
                e.HasKey(emp => emp.Id);
                e.Property(emp => emp.FullName).IsRequired().HasMaxLength(150);
                e.Property(emp => emp.Email).IsRequired().HasMaxLength(150);
                e.Property(emp => emp.RegistrationNumber).IsRequired().HasMaxLength(50);
                e.Property(emp => emp.Status).HasMaxLength(30);

                e.HasOne(emp => emp.Department)
                    .WithMany()
                    .HasForeignKey(emp => emp.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(emp => emp.Position)
                    .WithMany()
                    .HasForeignKey(emp => emp.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Auto-referência: gestor (sem navigation property)
                e.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(emp => emp.ManagerId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LeaveRequest>(e =>
            {
                e.ToTable("LeaveRequests", SchemaHr);
                e.HasKey(lr => lr.Id);
                e.Property(lr => lr.Type).IsRequired().HasMaxLength(50);
                e.Property(lr => lr.Status).IsRequired().HasMaxLength(20);

                e.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(lr => lr.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(lr => lr.ApprovedById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VacationBalance>(e =>
            {
                e.ToTable("VacationBalances", SchemaHr);
                e.HasKey(vb => vb.Id);

                e.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(vb => vb.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── PROJECTS ───────────────────────────────────────────────────

            modelBuilder.Entity<Project>(e =>
            {
                e.ToTable("Projects", SchemaProjects);
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(150);
                e.Property(p => p.Code).IsRequired().HasMaxLength(30);
                e.Property(p => p.Description).HasMaxLength(500);
                e.Property(p => p.Status).HasMaxLength(30);

                e.HasOne(p => p.Department)
                    .WithMany()
                    .HasForeignKey(p => p.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.ProjectManager)
                    .WithMany()
                    .HasForeignKey(p => p.ProjectManagerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProjectAllocation>(e =>
            {
                e.ToTable("ProjectAllocations", SchemaProjects);
                e.HasKey(pa => pa.Id);
                e.Property(pa => pa.RoleOnProject).HasMaxLength(100);

                e.HasOne(pa => pa.Employee)
                    .WithMany()
                    .HasForeignKey(pa => pa.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pa => pa.Project)
                    .WithMany()
                    .HasForeignKey(pa => pa.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProjectTask>(e =>
            {
                e.ToTable("ProjectTasks", SchemaProjects);
                e.HasKey(pt => pt.Id);
                e.Property(pt => pt.Title).IsRequired().HasMaxLength(200);
                e.Property(pt => pt.Description).HasMaxLength(1000);
                e.Property(pt => pt.Status).HasMaxLength(30);
                e.Property(pt => pt.Priority).HasMaxLength(20);

                e.HasOne(pt => pt.Project)
                    .WithMany()
                    .HasForeignKey(pt => pt.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pt => pt.AssignedEmployee)
                    .WithMany()
                    .HasForeignKey(pt => pt.AssignedEmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── TIMESHEET ───────────────────────────────────────────────────

            modelBuilder.Entity<TimeEntry>(e =>
            {
                e.ToTable("TimeEntries", SchemaTimesheet);
                e.HasKey(te => te.Id);
                e.Property(te => te.Description).HasMaxLength(500);
                e.Property(te => te.EntryType).HasMaxLength(50);

                e.HasOne(te => te.Employee)
                    .WithMany()
                    .HasForeignKey(te => te.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(te => te.Project)
                    .WithMany()
                    .HasForeignKey(te => te.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                // ApprovedById sem navigation property
                e.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(te => te.ApprovedById)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TimeBankBalance>(e =>
            {
                e.ToTable("TimeBankBalances", SchemaTimesheet);
                e.HasKey(tb => tb.Id);

                e.HasOne(tb => tb.Employee)
                    .WithMany()
                    .HasForeignKey(tb => tb.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── FINANCE ───────────────────────────────────────────────────

            modelBuilder.Entity<PayrollSummary>(e =>
            {
                e.ToTable("PayrollSummaries", SchemaFinance);
                e.HasKey(ps => ps.Id);
                e.Property(ps => ps.GrossSalary).HasPrecision(18, 2);
                e.Property(ps => ps.Deductions).HasPrecision(18, 2);
                e.Property(ps => ps.NetSalary).HasPrecision(18, 2);
                e.Property(ps => ps.Status).HasMaxLength(20);

                e.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(ps => ps.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
