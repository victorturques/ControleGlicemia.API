using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGlicemia.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndUserEmailUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrosGlicose_UserId",
                table: "RegistrosGlicose");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosDiarios_UserId",
                table: "RegistrosDiarios");

            migrationBuilder.DropIndex(
                name: "IX_Refeicoes_UserId",
                table: "Refeicoes");

            migrationBuilder.DropIndex(
                name: "IX_Medicamentos_UserId",
                table: "Medicamentos");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosGlicose_UserId_MedidoEm",
                table: "RegistrosGlicose",
                columns: new[] { "UserId", "MedidoEm" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_RegistrosGlicose_MomentoMedicao",
                table: "RegistrosGlicose",
                sql: "MomentoMedicao >= 1 AND MomentoMedicao <= 7");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_UserId_Data",
                table: "RegistrosDiarios",
                columns: new[] { "UserId", "Data" });

            migrationBuilder.CreateIndex(
                name: "IX_Refeicoes_UserId_DataHora",
                table: "Refeicoes",
                columns: new[] { "UserId", "DataHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_UserId_TomadoEm",
                table: "Medicamentos",
                columns: new[] { "UserId", "TomadoEm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosGlicose_UserId_MedidoEm",
                table: "RegistrosGlicose");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RegistrosGlicose_MomentoMedicao",
                table: "RegistrosGlicose");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosDiarios_UserId_Data",
                table: "RegistrosDiarios");

            migrationBuilder.DropIndex(
                name: "IX_Refeicoes_UserId_DataHora",
                table: "Refeicoes");

            migrationBuilder.DropIndex(
                name: "IX_Medicamentos_UserId_TomadoEm",
                table: "Medicamentos");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosGlicose_UserId",
                table: "RegistrosGlicose",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosDiarios_UserId",
                table: "RegistrosDiarios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Refeicoes_UserId",
                table: "Refeicoes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_UserId",
                table: "Medicamentos",
                column: "UserId");
        }
    }
}
