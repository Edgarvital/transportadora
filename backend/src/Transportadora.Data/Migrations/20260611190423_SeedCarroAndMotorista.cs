using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Transportadora.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCarroAndMotorista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Motorista",
                columns: new[] { "ID", "Documento", "Nome" },
                values: new object[,]
                {
                    { 1, "12345678901", "João Silva" },
                    { 2, "98765432109", "Maria Souza" }
                });

            migrationBuilder.InsertData(
                table: "Veiculo",
                columns: new[] { "ID", "Modelo", "Placa" },
                values: new object[,]
                {
                    { 1, "Scania R450", "ABC-1234" },
                    { 2, "Mercedes-Benz Actros", "XYZ-9876" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Motorista",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Motorista",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Veiculo",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veiculo",
                keyColumn: "ID",
                keyValue: 2);
        }
    }
}
