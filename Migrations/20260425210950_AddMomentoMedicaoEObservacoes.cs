using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGlicemia.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMomentoMedicaoEObservacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RealizadaEm",
                table: "Refeicoes",
                newName: "DataHora");

            migrationBuilder.AddColumn<int>(
                name: "MomentoMedicao",
                table: "RegistrosGlicose",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "RegistrosGlicose",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Refeicoes",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Refeicoes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Refeicoes",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MomentoMedicao",
                table: "RegistrosGlicose");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "RegistrosGlicose");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Refeicoes");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Refeicoes");

            migrationBuilder.RenameColumn(
                name: "DataHora",
                table: "Refeicoes",
                newName: "RealizadaEm");

            migrationBuilder.UpdateData(
                table: "Refeicoes",
                keyColumn: "Descricao",
                keyValue: null,
                column: "Descricao",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Refeicoes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
