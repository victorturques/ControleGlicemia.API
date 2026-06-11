using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleGlicemia.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGlicemiaMetasToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GlicemiaMaxima",
                table: "Users",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GlicemiaMinima",
                table: "Users",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlicemiaMaxima",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GlicemiaMinima",
                table: "Users");
        }
    }
}
