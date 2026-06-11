using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportesSoft_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdConsumoUnidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfSucursalLocal");

            migrationBuilder.DropTable(
                name: "ContConsumoUnidades");

            migrationBuilder.DropTable(
                name: "ContKilometrajeUnidad");

            migrationBuilder.DropTable(
                name: "ContMantenimientosCab");

            migrationBuilder.DropTable(
                name: "ContMantenimientosDet");

            migrationBuilder.DropTable(
                name: "ContOperadoresCat");

            migrationBuilder.DropTable(
                name: "ContPolizasReg");

            migrationBuilder.DropTable(
                name: "ContPreciosDiesel");

            migrationBuilder.DropTable(
                name: "ContRemolquesCat");

            migrationBuilder.DropTable(
                name: "ContTiposPolizas");

            migrationBuilder.DropTable(
                name: "ContUnidadesCat");

            migrationBuilder.DropTable(
                name: "ContViajes");

            migrationBuilder.DropTable(
                name: "EstadosCat");

            migrationBuilder.DropTable(
                name: "MunicipiosCat");

            migrationBuilder.DropTable(
                name: "RolesCat");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "EmpresasCat");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "EmpresasCat");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "EmpresasCat");
        }
    }
}
