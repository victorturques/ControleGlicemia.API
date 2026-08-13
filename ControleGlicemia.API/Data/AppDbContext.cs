using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public override int SaveChanges()
    {
        AtualizarAuditTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AtualizarAuditTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AtualizarAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AtualizarAuditTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AtualizarAuditTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                var criadoProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CriadoEm");
                if (criadoProp?.CurrentValue is DateTime dt && dt == default)
                    criadoProp.CurrentValue = now;
            }

            if (entry.State == EntityState.Modified)
            {
                var atualizadoProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "AtualizadoEm");
                if (atualizadoProp != null)
                    atualizadoProp.CurrentValue = now;
            }
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RegistroGlicose> RegistrosGlicose { get; set; }
    public DbSet<Medicamento> Medicamentos { get; set; }
    public DbSet<Refeicao> Refeicoes { get; set; }
    public DbSet<RegistroDiario> RegistrosDiarios { get; set; }
    public DbSet<TokenBlacklistEntry> TokenBlacklist { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistroGlicose>()
            .HasOne(g => g.User)
            .WithMany(u => u.RegistrosGlicose)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Medicamento>()
            .HasOne(m => m.User)
            .WithMany(u => u.Medicamentos)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Refeicao>()
            .HasOne(r => r.User)
            .WithMany(u => u.Refeicoes)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RegistroDiario>()
            .HasOne(r => r.User)
            .WithMany(u => u.RegistrosDiarios)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        SoftDeleteQueryFilter.Apply<User>(modelBuilder);
        SoftDeleteQueryFilter.Apply<RegistroGlicose>(modelBuilder);
        SoftDeleteQueryFilter.Apply<Medicamento>(modelBuilder);
        SoftDeleteQueryFilter.Apply<Refeicao>(modelBuilder);
        SoftDeleteQueryFilter.Apply<RegistroDiario>(modelBuilder);

        modelBuilder.Entity<RegistroGlicose>()
            .HasIndex(r => new { r.UserId, r.MedidoEm });

        modelBuilder.Entity<RegistroDiario>()
            .HasIndex(r => new { r.UserId, r.Data });

        modelBuilder.Entity<Medicamento>()
            .HasIndex(m => new { m.UserId, m.TomadoEm });

        modelBuilder.Entity<Refeicao>()
            .HasIndex(r => new { r.UserId, r.DataHora });

        base.OnModelCreating(modelBuilder);
    }
}

public static class SoftDeleteQueryFilter
{
    public static void Apply<T>(ModelBuilder modelBuilder) where T : class, ISoftDeletable
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.DeletedAt == null);
    }
}