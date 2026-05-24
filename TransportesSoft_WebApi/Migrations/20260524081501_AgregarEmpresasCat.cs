using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportesSoft_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEmpresasCat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpresasCat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RFC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaveAcceso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasCat", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpresasCat");

            migrationBuilder.DropTable(
                name: "UsuariosCat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContClientesCat",
                table: "ContClientesCat");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "ContClientesCat");

            migrationBuilder.RenameTable(
                name: "ContClientesCat",
                newName: "lClientes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lClientes",
                table: "lClientes",
                column: "id_Client");
        }
    }
}
