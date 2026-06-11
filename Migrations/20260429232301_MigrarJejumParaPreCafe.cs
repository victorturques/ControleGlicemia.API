using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGlicemia.API.Migrations
{
    /// <inheritdoc />
    public partial class MigrarJejumParaPreCafe : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.Sql("UPDATE `RegistrosGlicose` SET `MomentoMedicao` = 1 WHERE `MomentoMedicao` = 0;");
            }

        protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.Sql("UPDATE `RegistrosGlicose` SET `MomentoMedicao` = 0 WHERE `MomentoMedicao` = 1;");
            }
    }
}
