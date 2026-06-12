# Sistema de Gestão de Coletas

Inicio do projeto.

## Objetivo Do MVP

O MVP tem como foco centralizar solicitações de coleta criadas por clientes e atendentes, permitindo que a operação faça a roteirização com motorista e veículo, acompanhe o status de cada coleta e registre ocorrências com rastreabilidade.

## Problema Que O MVP Resolve

- Organizar solicitações de coleta em um fluxo único.
- Evitar mudanças de status inválidas durante a operação.
- Garantir auditoria de eventos importantes, como atraso, cancelamento e finalização.
- Dar visibilidade para operação, atendimento e gestão sobre o andamento das coletas.

## Regras Criticas Do Negocio

- Um pedido cancelado não pode voltar para `Em_Coleta` ou `Coletada`.
- Não é permitido marcar uma coleta como `Coletada` sem motorista e veículo vinculados.
- Toda ocorrência precisa registrar data, hora e usuário responsável.

## Escopo Inicial Do MVP

### Incluido

- Cadastro de solicitações de coleta.
- Consulta de solicitações com status.
- Roteirização da coleta com vínculo de motorista e veículo.
- Alteração de status da coleta com validação de regras.
- Registro e consulta de ocorrências.
- Base de autenticação para rastreio do usuário que executa as ações.

## Fases Do MVP

### Fase 1 - Fundacao Tecnica

- Criar a solution e o projeto backend.
- Definir a estrutura base do monorepo.
- Levantar arquitetura: Controllers, Services, Models, Data, DTOs
- Configurar Docker Compose para ambiente local.

### Fase 2 - Dominio E Banco de Dados

- Modelar as entidades principais.
- Criação das migrations e dos seeders
- Configurar EF Core e Fluent API.

### Fase 3 - API e Segurança

- Implementar autenticação via JWT
- Expor os primeiros endpoints da API.

### Fase 4 - Primeiro Fluxo Completo

## Fase 4a - Gestão de Recursos

- CRUD Motorista.
- CRUD carro.

## Fase 4b - Fluxo de Solicitação

- Criar solicitação.
- Roteirizar coleta.
- Registrar ocorrência.
- Validar transições de status.

## Fase 5 - Implementação do frontend

- Implementar Login e armazenamento do token.
- Implementar fluxo de solicitação.
- Implementar gerenciamento da solicitação.
- Implementar autorização de recurso com base na role.

## Estrutura Base

```text
backend/
	src/
		Transportadora.Controllers/
		Transportadora.Services/
		Transportadora.Models/
		Transportadora.Data/
		Transportadora.DTOs/
	tests/
		Transportadora.UnitTests/

frontend/
	src/
	public/

docs/
```

## Estrutura do Banco de Dados

// ==========================================
// ENUMS (Domínios Restritos)
// ==========================================
Enum RoleTipo {
  Cliente
  Atendente
  Admin
}

Enum PrioridadeNivel {
  Baixa
  Media
  Alta
}

Enum StatusColeta {
  Aberta
  Roteirizada
  Em_Coleta
  Coletada
  Cancelada
}

Enum OcorrenciaTipo {
  Atraso
  Cliente_Ausente
  Endereco_Incorreto
  Cancelamento
  Outro
}

Enum SituacaoUser {
  Ativo
  Inativo
}

// ==========================================
// TABELAS
// ==========================================

Table Role {
  ID int [pk, increment]
  Nome RoleTipo
}

Table User {
  ID int [pk, increment]
  Nome varchar
  Documento varchar
  Email varchar
  SenhaHash varchar
  Role_ID int
  Endereco_ID int
  Situacao SituacaoUser
}


Table Motorista {
  ID int [pk, increment]
  Nome varchar
  Documento varchar
}

Table Veiculo {
  ID int [pk, increment]
  Modelo varchar
  Placa varchar
}

Table Endereco {
  ID int [pk, increment]
  CEP varchar
  Logradouro varchar
  Numero varchar
  Complemento varchar
  Bairro varchar
  Cidade varchar
  UF varchar
}

Table Carga {
  ID int [pk, increment]
  Descricao_Nome varchar
  Tipo varchar
  Peso decimal
  Altura decimal
  Largura decimal
  Comprimento decimal
}

Table Solicitacao_Coleta {
  ID int [pk, increment]
  Remetente_ID int
  Destinatario_ID int
  Carga_ID int
  Criado_Por_User_ID int
  Data_Solicitacao datetime
  Data_Prevista_Retirada datetime
  Prioridade PrioridadeNivel
  Status StatusColeta
  Motorista_ID int [null]
  Veiculo_ID int [null]
  Observacoes_Gerais text
}

Table Ocorrencia {
  ID int [pk, increment]
  Solicitacao_ID int
  User_ID int
  Tipo OcorrenciaTipo
  Descricao text
  Data_Hora datetime
}

// ==========================================
// RELACIONAMENTOS (Foreign Keys)
// ==========================================

// Relacionamentos de Usuário e Pessoa com Endereço
Ref: User.Endereco_ID > Endereco.ID
Ref: User.Role_ID > Role.ID

// Relacionamentos da Solicitação de Coleta
Ref: Solicitacao_Coleta.Remetente_ID > User.ID
Ref: Solicitacao_Coleta.Destinatario_ID > User.ID
Ref: Solicitacao_Coleta.Criado_Por_User_ID > User.ID
Ref: Solicitacao_Coleta.Motorista_ID > Motorista.ID
Ref: Solicitacao_Coleta.Veiculo_ID > Veiculo.ID

// Relacionamento 1:1 entre Solicitação e Carga
Ref: Solicitacao_Coleta.Carga_ID - Carga.ID

// Relacionamentos da Ocorrência
Ref: Ocorrencia.Solicitacao_ID > Solicitacao_Coleta.ID
Ref: Ocorrencia.User_ID > User.ID

## Diretriz De Arquitetura

- Controllers/: Recebem as requisições HTTP e devolvem as respostas.
- Services/: Onde as regras de negócio reais vivem. É aqui que você valida se o motorista foi vinculado antes de concluir a coleta ou se a solicitação foi cancelada.
- Models/: As suas classes de dados baseadas naquele diagrama que fizemos.
- Data/: O DbContext do Entity Framework e as migrations.
- DTOs/: Classes simples (usando record) para a entrada e saída de dados

## Critério De Pronto Do MVP Inicial

O MVP inicial será considerado pronto quando for possível criar uma solicitação, roteirizá-la, registrar uma ocorrência e impedir transições inválidas de status com rastreabilidade do usuário.

## Como Subir Localmente Com Docker

O backend foi preparado para subir com PostgreSQL e API via Docker Compose, usando a mesma base de banco da Infrastructure.

Comandos a partir da pasta `backend/`:

```bash
docker compose up --build
```

Após subir, a API fica disponível em `http://localhost:8080` e o PostgreSQL expõe a porta `5433` no host.

Endpoints úteis:

- `GET /health`
- `GET /swagger`