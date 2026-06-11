using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Transportadora.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Endereco",
                columns: new[] { "ID", "Bairro", "CEP", "Cidade", "Complemento", "Logradouro", "Numero", "UF" },
                values: new object[,]
                {
                    { 1, "Centro", "00000001", "São Paulo", null, "Rua Admin", "1", "SP" },
                    { 2, "Centro", "00000002", "São Paulo", null, "Rua Atendente", "2", "SP" },
                    { 3, "Centro", "00000003", "São Paulo", null, "Rua Cliente", "3", "SP" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "ID", "Documento", "Email", "Endereco_ID", "Nome", "Role_ID", "SenhaHash", "Situacao" },
                values: new object[,]
                {
                    { 1, "00000000000", "admin@admin.com", 1, "Administrador Master", 3, "9B968201AC99BBE67CEC3DF02251FAA383BE2B040732D00B0EF07C1600D97963", "Ativo" },
                    { 2, "11111111111", "atendente@atendente.com", 2, "Atendente Padrão", 2, "9B968201AC99BBE67CEC3DF02251FAA383BE2B040732D00B0EF07C1600D97963", "Ativo" },
                    { 3, "22222222222", "cliente@cliente.com", 3, "Cliente Fictício", 1, "9B968201AC99BBE67CEC3DF02251FAA383BE2B040732D00B0EF07C1600D97963", "Ativo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
