using Microsoft.EntityFrameworkCore;
using Transportadora.Data.Context;
using Transportadora.DTOs.Users;
using Transportadora.Models.Entities;
using Transportadora.Models.Enums;
using Transportadora.Services.Authentication;
using Transportadora.Shared.Errors;
using Transportadora.Shared.Results;

namespace Transportadora.Services.Users;

public sealed class UserService(TransportadoraDbContext dbContext, IAuthService authService) : IUserService
{
    public async Task<Result<UserRegistrationResponseDTO>> RegisterAsync(UserRegistrationRequestDTO request, CancellationToken cancellationToken)
    {
        var roleCliente = await dbContext.Roles.FirstAsync(x => x.Nome == RoleTipo.Cliente, cancellationToken);

        var usuarioAtivo = !string.IsNullOrWhiteSpace(request.Email)
            && !string.IsNullOrWhiteSpace(request.Senha);

        if ((request.Email is null) != (request.Senha is null))
        {
            return Result<UserRegistrationResponseDTO>.Fail(
                UserErrorCodes.InvalidLoginPair,
                "Email e senha devem ser informados juntos ou omitidos juntos.");
        }

        if (usuarioAtivo)
        {
            var emailJaExiste = await dbContext.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
            if (emailJaExiste)
            {
                return Result<UserRegistrationResponseDTO>.Fail(
                    UserErrorCodes.EmailAlreadyExists,
                    "Ja existe um usuario cadastrado com este e-mail.");
            }
        }

        var endereco = new Endereco
        {
            Cep = request.Endereco.CEP,
            Logradouro = request.Endereco.Logradouro,
            Numero = request.Endereco.Numero,
            Complemento = request.Endereco.Complemento,
            Bairro = request.Endereco.Bairro,
            Cidade = request.Endereco.Cidade,
            Uf = request.Endereco.UF
        };

        var usuario = new User
        {
            Nome = request.Nome,
            Documento = request.Documento,
            Email = usuarioAtivo ? request.Email : null,
            SenhaHash = usuarioAtivo ? authService.HashPassword(request.Senha!) : null,
            RoleId = roleCliente.Id,
            Endereco = endereco,
            Situacao = usuarioAtivo ? SituacaoUser.Ativo : SituacaoUser.Inativo
        };

        dbContext.Users.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserRegistrationResponseDTO>.Ok(new UserRegistrationResponseDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Role = roleCliente.Nome.ToString(),
            Situacao = usuario.Situacao.ToString(),
            PossuiLogin = usuarioAtivo
        });
    }
}
