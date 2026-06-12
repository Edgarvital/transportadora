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
		Transportadora.Api/
		Transportadora.Api/Controllers
		Transportadora.Services/
		Transportadora.Models/
		Transportadora.Data/
		Transportadora.DTOs/
		Transportadora.Shared/
	tests/
		Transportadora.UnitTests/
docs/
```

## Documentação

- O Modelo Entidade Relacionamento (MER), pode ser obtido através da pasta docs desse repositório.
- O Swagger pode ser obtido através do consumo do endpoint da API `/swagger/index.html`

## Diretriz De Arquitetura

- Api/Controllers: Recebem as requisições HTTP e devolvem as respostas.
- Services/: Onde as regras de negócio reais vivem. É aqui que você valida se o motorista foi vinculado antes de concluir a coleta ou se a solicitação foi cancelada.
- Models/: As suas classes de dados baseadas naquele diagrama que fizemos.
- Data/: O DbContext do Entity Framework e as migrations.
- DTOs/: Classes simples (usando record) para a entrada e saída de dados
- Shared/: Funcionalidades e Uteis em comum com as outras camadas.

## Critério De Pronto Do MVP Inicial

O MVP inicial será considerado pronto quando for possível criar uma solicitação, roteirizá-la, registrar uma ocorrência e impedir transições inválidas de status com rastreabilidade do usuário.

## Como Subir Localmente Com Docker

O backend e frontend foi preparado para subir com PostgreSQL e API via Docker Compose, usando a mesma base de banco da Infrastructure.

Comandos a partir da Raiz do projeto:

```bash
docker-compose up --build -d
```

Após subir, a API fica disponível em `http://localhost:8080`, o frontend fica disponível em `http://localhost:3000` e o PostgreSQL expõe a porta `5433` no host.

Endpoints úteis do backend:

- `GET /swagger`
