# 🌸 Distribuidora Flores

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-13-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10-6DB33F?style=for-the-badge&logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-20-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Tailwind](https://img.shields.io/badge/Tailwind%20CSS-38BDF8?style=for-the-badge&logo=tailwindcss&logoColor=white)
![Tests](https://img.shields.io/badge/tests-56%20passing-brightgreen?style=for-the-badge&logo=xunit&logoColor=white)
![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow?style=for-the-badge)

Marketplace B2B com ERP embutido, conectando uma distribuidora de flores a comerciantes (floriculturas). Comerciantes navegam o catálogo, montam um carrinho e finalizam pedidos, enquanto o distribuidor gerencia produtos, estoque, clientes e frota através de um painel administrativo.

Projeto full-stack pessoal desenvolvido com foco em boas práticas de arquitetura, pensado para portfólio.

---

## 🏗️ Arquitetura

### Backend

Monólito Modular com Clean Architecture aplicada dentro de cada módulo — uma única API, organizada em módulos de domínio isolados, cada um com suas próprias camadas:

```
Modules/
└── <NomeDoModulo>/
    ├── Domain/           → Entidades e regras de negócio puras
    ├── Application/      → Casos de uso e interfaces de repositório
    ├── Infrastructure/   → Implementação de acesso a dados (EF Core)
    ├── Extension/         → Registro de DI do módulo, encadeado no Program.cs
    └── Api/               → Controllers e DTOs
        ├── Controllers/
        └── DTOs/
```

**Por que essa abordagem?**
- Evita o overhead prematuro de microserviços em um projeto de porte único (um distribuidor, escala controlada)
- Mantém fronteiras de domínio bem definidas — cada módulo pode, no futuro, ser extraído como serviço independente sem grande refatoração
- Aplica inversão de dependência de verdade: o `Domain` não depende de nada externo (nem EF Core, nem ASP.NET); é a `Infrastructure` que depende do `Domain` através de interfaces
- Módulos "de negócio" orquestram outros via interface (ex: `Pedidos` consulta `Clientes` e `Catalogo`; `Identidade` orquestra `Clientes` no registro combinado), sempre numa única direção de dependência

### Frontend

SPA em Angular (standalone components, sem NgModules) com Tailwind CSS:

```
src/app/
├── core/          → serviços, interceptors, guards e modelos globais
├── features/      → uma pasta por tela/funcionalidade (auth, catalogo, carrinho, admin)
└── shared/        → componentes e diretivas reutilizáveis entre features
```

## 📦 Módulos do Backend

| Módulo | Status | Descrição |
|---|---|---|
| **Catalogo** | ✅ Implementado | Produtos e lotes, com controle de validade, disponibilidade, upload de imagem, edição e exclusão (soft delete) |
| **Clientes** | ✅ Implementado | Cadastro com validação real de CPF/CNPJ (dígito verificador), ativação/desativação |
| **Pedidos** | ✅ Implementado | Criação orquestrando Clientes e Catalogo, máquina de estados, preço congelado |
| **Frota** | ✅ Implementado | Veículos, motoristas e entregas — base para rastreio em tempo real e app do motorista (v2) |
| **Identidade** | ✅ Implementado | Autenticação JWT com refresh token, autorização por role (Admin / Comerciante) |

## ✨ Funcionalidades implementadas

**Catalogo**
- Cadastro, edição e exclusão (soft delete) de produtos
- Cadastro de lotes por produto, com data de validade
- Cálculo automático de disponibilidade, considerando apenas lotes ainda válidos
- Upload de imagem por produto (armazenamento local, servido como arquivo estático)

**Clientes**
- Cadastro de comerciante (pessoa física ou jurídica) com validação real de CPF/CNPJ, incluindo dígito verificador
- Bloqueio de documentos duplicados (checagem na aplicação + constraint única no banco)
- Ativação/desativação de clientes

**Pedidos**
- Criação de pedido orquestrando os módulos Clientes e Catalogo
- Preço e nome do produto "congelados" no momento da compra (não mudam se o catálogo mudar depois)
- Sinalização de estoque insuficiente sem bloquear a criação do pedido
- Máquina de estados: Pendente → Aprovado → Separado → Em Rota → Entregue, com Cancelado em etapas iniciais

**Frota**
- Cadastro de veículos e motoristas, com placa e CNH únicos
- Criação de entrega vinculando pedido, veículo e motorista (só a partir de um pedido já separado)
- Máquina de estados da entrega: Aguardando Saída → Em Rota → Concluída

**Identidade (Autenticação e Autorização)**
- Registro de comerciante em uma única operação (cria Cliente + Usuário/login vinculados)
- Login com JWT: access token de curta duração (30 min) contendo role e clienteId
- Refresh token de longa duração (7 dias) com **rotação** — cada uso gera um novo par de tokens e revoga o anterior, protegendo contra reuso de token roubado
- Autorização por role em todos os módulos: Admin tem acesso administrativo completo; Comerciante só acessa e cria recursos vinculados ao próprio cadastro (identificado via claim do token, nunca por dado enviado no corpo da requisição)
- Seed automático de usuário Admin na primeira execução

**Frontend**
- Login e registro de comerciante, com visual moderno (glassmorphism, gradientes)
- Renovação de sessão transparente: um interceptor HTTP detecta token expirado, renova via refresh token e repete a requisição original automaticamente, sem o usuário perceber
- Rotas protegidas por autenticação e por role (guard de rota)
- Catálogo de produtos com imagens, consumindo a API
- Carrinho de compras (signals): adicionar, ajustar quantidade, remover itens e finalizar pedido
- Painel administrativo completo: cadastro, edição e exclusão de produtos, upload de imagem, adição de estoque com datepicker customizado
- Páginas de erro personalizadas (401, 403, 404, 500) com identidade visual própria

**Geral**
- API REST documentada via Swagger (com suporte a autenticação Bearer)
- CORS configurado para o frontend consumir a API
- Suíte de testes automatizados: 56 testes (unitários de domínio + integração via HTTP, incluindo fluxos autenticados)

## 🛠️ Tecnologias

**Backend**
- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core** (SQL Server)
- **Clean Architecture** por módulo
- **JWT Bearer + Refresh Token** para autenticação/autorização
- **BCrypt** para hash de senha
- **Swagger / Swashbuckle** para documentação da API

**Frontend**
- **Angular** (standalone components, signals)
- **Tailwind CSS**
- **Flatpickr** (seletor de data customizado)
- **RxJS**

## 🧪 Testes

O backend conta com uma suíte de 56 testes automatizados, dividida em dois níveis:

- **Testes unitários** — cobrem regras de negócio isoladas do Domain (validação de CPF/CNPJ com dígito verificador, cálculo de disponibilidade de estoque, máquinas de estado de Pedido e Entrega), sem depender de banco de dados.
- **Testes de integração** — sobem a API completa em memória (`WebApplicationFactory`) com um banco SQLite temporário, testando os fluxos principais via requisições HTTP reais — incluindo endpoints protegidos, usando um cliente HTTP autenticado gerado na própria suíte de testes.

Para rodar:

```bash
cd tests/DistribuidoraFlores.Tests
dotnet test
```

## 🚀 Como rodar localmente

### Backend

**Pré-requisitos:** [.NET SDK 10](https://dotnet.microsoft.com/download), SQL Server (Express ou superior)

```bash
cd src/DistribuidoraFlores.Api
dotnet restore
dotnet ef database update
dotnet run
```

A API sobe em `http://localhost:5014` e redireciona automaticamente para o Swagger na raiz. Um usuário **Admin** é criado automaticamente na primeira execução (`admin@distribuidoraflores.com` / `Admin@123`) — troque a senha assim que possível. Comerciantes se registram via `POST /api/auth/registrar-comerciante`.

### Frontend

**Pré-requisitos:** [Node.js](https://nodejs.org/) e Angular CLI (`npm install -g @angular/cli`)

```bash
cd web/distribuidora-flores-web
npm install
ng serve
```

A aplicação sobe em `http://localhost:4200` (é necessário que a API esteja rodando em paralelo).

## 🗺️ Roadmap

- [x] Módulo de Clientes (cadastro com validação de CPF/CNPJ)
- [x] Módulo de Pedidos (fluxo completo: criação → aprovação → separação → em rota → entrega)
- [x] Módulo de Frota (veículos, motoristas, entregas)
- [x] Suíte de testes automatizados (unitários + integração)
- [x] Autenticação JWT + refresh token, autorização por role (Admin / Comerciante)
- [x] Frontend em Angular: autenticação, catálogo, carrinho, painel admin completo (CRUD de produtos, estoque, imagens)
- [ ] Painel Admin de Pedidos (gerenciar status pela interface)
- [ ] Autocomplete de CEP no cadastro de endereço
- [ ] v2: rastreio de veículos em tempo real e app para motoristas

## 📄 Licença

Este projeto é de uso pessoal/portfólio.