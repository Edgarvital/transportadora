using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Transportadora.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CEP", "Complemento", "Logradouro", "Numero" },
                values: new object[] { "00000001", null, "Rua Admin", "1" });

            migrationBuilder.InsertData(
                table: "Endereco",
                columns: new[] { "ID", "Bairro", "CEP", "Cidade", "Complemento", "Logradouro", "Numero", "UF" },
                values: new object[,]
                {
                    { 2, "Centro", "00000002", "São Paulo", null, "Rua Atendente", "2", "SP" },
                    { 3, "Centro", "00000003", "São Paulo", null, "Rua Cliente", "3", "SP" }
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2,
                column: "Endereco_ID",
                value: 2);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 3,
                column: "Endereco_ID",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Endereco",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CEP", "Complemento", "Logradouro", "Numero" },
                values: new object[] { "00000000", "Matriz", "Sede da Transportadora", "S/N" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 2,
                column: "Endereco_ID",
                value: 1);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "ID",
                keyValue: 3,
                column: "Endereco_ID",
                value: 1);
        }
    }
}
