# TranspoLog — Gestão de Coletas (Front-end)

## Visão Geral

TranspoLog é a aplicação Front-end para gestão do ciclo de vida de coletas de uma transportadora. A interface consome uma API RESTful em C# (.NET 8) protegida por JWT e foi construída com foco em entregas rápidas, alta observabilidade e UX profissional para operadores e atendentes.

Este repositório contém a camada de apresentação (UI) com componentes reutilizáveis, integração clara com contratos DTO/Enums do backend e regras de negócio aplicadas no UI para evitar operações inválidas.

## Por que estas escolhas técnicas

- React 18 + Vite — Rápido feedback (HMR), builds leves e DX moderno; ideal para prototipagem ágil e produção.
- TypeScript — Tipagem estrita que espelha DTOs/Enums do backend (código mais seguro e autocompletado no IDE).
- Tailwind CSS v4 — Design system utilitário, velocidade para compor UIs consistentes (tons slate/blue adotados para paleta corporativa).
- Axios + jwt-decode — Interceptadores centralizados (autorização, tratamento de erros) e decodificação das claims do token emitido pelo .NET para hidratar um `User` confiável no Front-end.
- Lucide React — Ícones leves e consistentes com estilo moderno.

Essas decisões priorizam produtividade, mantenibilidade e alinhamento com o contrato do backend.

## O que implementamos (resumo de valor)

- Autenticação JWT com persistência segura e decodificação de claims quando o backend retorna apenas token.
- Layout consistente com Sidebar e proteção de rotas por roles (Admin, Atendente, Cliente).
- Dashboard com filtros que mapeiam exatamente os QueryParams da API e paginação baseada na resposta paginada do servidor.
- Busca por CPF para Remetente/Destinatário (lookup via API). Cadastro de usuário apenas no fluxo de criação de solicitação (NovaSolicitacao).
- Regras de UX aplicadas: botões de ação habilitados apenas conforme o estado da solicitação (ex.: Roteirizar apenas para status Aberta).
- Modal de roteirização com seleção de motorista/veículo (integração ao endpoint de roteirização pronta).
- Tela de detalhes com timeline de ocorrências, criação de ocorrências e fluxo de cancelamento que gera ocorrência e atualiza status.

## Estrutura do projeto (pontos de interesse)

- src/contexts/AuthContext.tsx — Gerencia autenticação, token, decodificação e headers do Axios.
- src/api/axiosConfig.ts — Instância Axios com interceptadores para Authorization e tratamento centralizado de erros.
- src/pages/Dashboard.tsx — Lista, filtros, ações de roteirização e avanço de status.
- src/pages/NovaSolicitacao.tsx — Formulário de criação de solicitações e fluxo para registrar remetente/destinatário quando necessário.
- src/pages/SolicitacaoDetalhes.tsx — Tela de detalhes com timeline de ocorrências e formulário para nova ocorrência (inclui cancelamento).
- src/types/solicitacao.ts — Tipagens e enums que espelham os contratos C# (StatusColeta, PrioridadeNivel, TipoOcorrencia, DTOs).
- src/routes.tsx — Rotas protegidas e mapeamento de permissões por role.

Explorar estes arquivos dá uma visão direta das decisões arquiteturais e dos pontos de integração com o backend.

## Regras de Negócio aplicadas no Front-end (importante para avaliadores)

- O botão Roteirizar só aparece para solicitações com StatusColeta.Aberta (1).
- Avançar Status está disponível para Roteirizada (2) e EmColeta (3) — o front dispara PATCH /solicitacoes/{id}/status com o próximo status lógico.
- Não é permitido adicionar novas ocorrências em solicitações já Concluídas ou Canceladas.
- Cancelar uma solicitação gera uma ocorrência e em seguida atualiza o status para Cancelada (fluxo atômico: POST ocorrência + PATCH status).
- O front interpreta Problem Details (RFC 7807) retornado pelo .NET e apresenta mensagens amigáveis ao usuário.

## Como rodar localmente

1. Instale dependências:

```bash
npm install
```

2. Crie um arquivo de ambiente (exemplo `.env`):

```env
VITE_API_URL=http://localhost:8080/api
```

3. Rodar em modo desenvolvimento:

```bash
npm run dev
```

4. Build para produção:

```bash
npm run build
npm run preview
```

Observações:
- Caso o backend esteja protegido por CORS ou autenticação, garanta que o token JWT seja obtido via /auth/login antes de navegar para áreas protegidas.

## Endpoints e contratos relevantes

- POST /auth/login — retorna token JWT (o front pode decodificar claims via jwt-decode).
- GET /motoristas, GET /carros — populam selects do modal de roteirização.
- GET /solicitacoes — aceita PageNumber, PageSize e filtros (Status, Prioridade, RemetenteId, DestinatarioId, DataSolicitacaoInicial/Final, DataPrevistaRetiradaInicial/Final).
- PUT /solicitacoes/{id}/roteirizar — atribui motorista/veículo.
- PATCH /solicitacoes/{id}/status — atualiza status (payload: { novoStatus }).
- POST /solicitacoes/{id}/ocorrencias — cria ocorrência (CreateOcorrenciaDto).

Esses contratos estão representados em types/solicitacao.ts para garantir tipagem end-to-end.

## Tratamento de erros e observabilidade

- Erros de rede e respostas 4xx/5xx são interceptados em src/api/axiosConfig.ts; mensagens derivadas do Problem Details são mapeadas e apresentadas perto dos campos (quando aplicável) ou como banners globais.
- Logs importantes são emitidos via console.debug para parâmetros de busca e operações (facilita troubleshooting em ambiente dev).

## UX e acessibilidade

- Paleta corporativa (tons `slate` e `blue`) para leve contraste e legibilidade em ambientes operacionais.
- Respeito a estados: botões desabilitados versus removidos (quando necessário) para evitar ações UI inválidas.
- Formulários com foco, labels claras e feedbacks inline para reduzir erros de operação no call-center.
- Componentes responsivos (grid que empilha no mobile) para uso em tablets.

## Testes manuais recomendados

- Login com token-only e com token+user (verificar hidratação do `AuthContext`).
- Fluxo: Nova Solicitação → CPF lookup (remetente/destinatário não encontrado → cadastro inline) → criação.
- Dashboard: filtros, paginação, roteirizar (modal), avançar status, ver detalhes.
- Tela de detalhes: criar ocorrência, cancelar solicitação e validar timeline + bolinha vermelha na ocorrência de cancelamento.

## Próximos passos e melhorias técnicas

- Adicionar toasts (sucesso/erro) centralizados e um serviço de notificações.
- Cobertura de testes automatizados: unit + integration (Jest + React Testing Library) para componentes críticos e hooks.
- Internacionalização (i18n) para labels e mensagens de validação.
- Centralizar manipulação de datas com date-fns ou dayjs e normalizar timezone.
- Mover strings de UI para um design tokens file e configurar temas escuro/claro.

## Como contribuir

1. Fork → branch com nome `feature/descricao`.
2. Faça commits claros e pequenos; escreva PR com objetivo e screenshots.
3. Adicione testes para novas features e atualize README se alterar contratos.

---

Se quiser, eu posso também gerar um `CHANGELOG.md` inicial com o que foi implementado até agora, ou criar scripts de QA para os fluxos críticos. Quer que eu adicione o arquivo `README.md` no repositório agora? (já o adicionei — abra para revisar e pedir ajustes rápidos).