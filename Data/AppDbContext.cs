using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<RegistroGlicose> RegistrosGlicose { get; set; }
    public DbSet<Medicamento> Medicamentos { get; set; }
    public DbSet<Refeicao> Refeicoes { get; set; }
    public DbSet<RegistroDiario> RegistrosDiarios { get; set; }

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

        // Email único
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasMaxLength(255);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Índices de consulta por usuário + data
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