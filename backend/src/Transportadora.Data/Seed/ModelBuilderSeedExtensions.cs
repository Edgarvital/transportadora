using Microsoft.EntityFrameworkCore;
using Transportadora.Models.Entities;
using Transportadora.Models.Enums;
using System.Security.Cryptography;
using System.Text;

namespace Transportadora.Data.Seed;

public static class ModelBuilderSeedExtensions
{
    private static string GenerateSeedHash(string password)
    {
        var saltedPassword = $"{password}|TransportadoraMvpSalt";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword));
        return Convert.ToHexString(bytes).ToUpper(); // Garantindo maiúsculas
    }

    public static void SeedDefaults(this ModelBuilder modelBuilder)
    {
        // 1. Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Nome = RoleTipo.Cliente },
            new Role { Id = 2, Nome = RoleTipo.Atendente },
            new Role { Id = 3, Nome = RoleTipo.Admin }
        );

        // 2. Endereços
        modelBuilder.Entity<Endereco>().HasData(
            new Endereco { Id = 1, Cep = "00000001", Logradouro = "Rua Admin", Numero = "1", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP" },
            new Endereco { Id = 2, Cep = "00000002", Logradouro = "Rua Atendente", Numero = "2", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP" },
            new Endereco { Id = 3, Cep = "00000003", Logradouro = "Rua Cliente", Numero = "3", Bairro = "Centro", Cidade = "São Paulo", Uf = "SP" }
        );

        // 3. Usuários
        string passwordHash = GenerateSeedHash("password");

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Nome = "Administrador Master", Documento = "00000000000", Email = "admin@admin.com", SenhaHash = passwordHash, RoleId = 3, EnderecoId = 1, Situacao = SituacaoUser.Ativo },
            new User { Id = 2, Nome = "Atendente Padrão", Documento = "11111111111", Email = "atendente@atendente.com", SenhaHash = passwordHash, RoleId = 2, EnderecoId = 2, Situacao = SituacaoUser.Ativo },
            new User { Id = 3, Nome = "Cliente Fictício", Documento = "22222222222", Email = "cliente@cliente.com", SenhaHash = passwordHash, RoleId = 1, EnderecoId = 3, Situacao = SituacaoUser.Ativo }
        );

        // 4. Veículos e Motoristas (Novos recursos da Fase 4a)
        modelBuilder.Entity<Veiculo>().HasData(
            new Veiculo { Id = 1, Placa = "ABC-1234", Modelo = "Scania R450" },
            new Veiculo { Id = 2, Placa = "XYZ-9876", Modelo = "Mercedes-Benz Actros" }
        );

        modelBuilder.Entity<Motorista>().HasData(
            new Motorista { Id = 1, Nome = "João Silva", Documento = "12345678901" },
            new Motorista { Id = 2, Nome = "Maria Souza", Documento = "98765432109" }
        );
    }
}