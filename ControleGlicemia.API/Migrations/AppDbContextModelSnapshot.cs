using System;
using ControleGlicemia.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ControleGlicemia.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("ControleGlicemia.API.Models.Medicamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<double>("Dose")
                        .HasColumnType("REAL");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TomadoEm")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "TomadoEm");

                    b.ToTable("Medicamentos");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.Refeicao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataHora")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Descricao")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Observacoes")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "DataHora");

                    b.ToTable("Refeicoes");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.RegistroDiario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Data")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Observacoes")
                        .HasMaxLength(1000)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "Data");

                    b.ToTable("RegistrosDiarios");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.RegistroGlicose", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("MedidoEm")
                        .HasColumnType("TEXT");

                    b.Property<int>("MomentoMedicao")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Observacoes")
                        .HasMaxLength(300)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Valor")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "MedidoEm");

                    b.ToTable("RegistrosGlicose");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.TokenBlacklistEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("BlacklistedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenJti")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TokenBlacklist");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<double>("GlicemiaMaxima")
                        .HasColumnType("REAL");

                    b.Property<double>("GlicemiaMinima")
                        .HasColumnType("REAL");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("RefreshTokenExpiry")
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("SenhaHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.Medicamento", b =>
                {
                    b.HasOne("ControleGlicemia.API.Models.User", "User")
                        .WithMany("Medicamentos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.Refeicao", b =>
                {
                    b.HasOne("ControleGlicemia.API.Models.User", "User")
                        .WithMany("Refeicoes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.RegistroDiario", b =>
                {
                    b.HasOne("ControleGlicemia.API.Models.User", "User")
                        .WithMany("RegistrosDiarios")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.RegistroGlicose", b =>
                {
                    b.HasOne("ControleGlicemia.API.Models.User", "User")
                        .WithMany("RegistrosGlicose")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ControleGlicemia.API.Models.User", b =>
                {
                    b.Navigation("Medicamentos");

                    b.Navigation("Refeicoes");

                    b.Navigation("RegistrosDiarios");

                    b.Navigation("RegistrosGlicose");
                });
#pragma warning restore 612, 618
        }
    }
}
