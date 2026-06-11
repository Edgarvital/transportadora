# 1. Imagem base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Imagem do SDK para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 3. Copiar os arquivos .csproj para restaurar as dependências
# Fazemos isso primeiro para aproveitar o cache do Docker (acelera os próximos builds)
COPY ["Transportadora.API/Transportadora.API.csproj", "Transportadora.API/"]
COPY ["Transportadora.Data/Transportadora.Data.csproj", "Transportadora.Data/"]
COPY ["Transportadora.DTOs/Transportadora.DTOs.csproj", "Transportadora.DTOs/"]
COPY ["Transportadora.Models/Transportadora.Models.csproj", "Transportadora.Models/"]
COPY ["Transportadora.Services/Transportadora.Services.csproj", "Transportadora.Services/"]

# Restaurar os pacotes a partir do projeto Host (ele puxa as referências automaticamente)
RUN dotnet restore "Transportadora.API/Transportadora.API.csproj"

# 4. Copiar o restante de todo o código fonte
COPY . .

# 5. Mudar para a pasta da API e compilar
WORKDIR "/src/Transportadora.API"
RUN dotnet build "Transportadora.API.csproj" -c Release -o /app/build

# 6. Publicar a aplicação otimizada
FROM build AS publish
RUN dotnet publish "Transportadora.API.csproj" -c Release -o /app/publish

# 7. Configurar a imagem final que vai rodar no container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Garante que a API vai escutar na porta 8080 conforme exigido no seu docker-compose
ENV ASPNETCORE_HTTP_PORTS=8080

ENTRYPOINT ["dotnet", "Transportadora.API.dll"]