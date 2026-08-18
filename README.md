# 🌸 Distribuidora Flores

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-13-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10-6DB33F?style=for-the-badge&logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-planejado-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow?style=for-the-badge)

Marketplace B2B com ERP embutido, conectando uma distribuidora de flores a comerciantes (floriculturas). Permite que comerciantes façam pedidos direto do catálogo do distribuidor, enquanto o distribuidor gerencia estoque (com controle de validade), pedidos recebidos e clientes através do módulo de ERP.

Projeto pessoal desenvolvido com foco em boas práticas de arquitetura backend, pensado para portfólio.

---

## 🏗️ Arquitetura

O backend segue o padrão de **Monólito Modular** com **Clean Architecture** aplicada dentro de cada módulo — uma única API, organizada em módulos de domínio isolados, cada um com suas próprias camadas:

```
Modules/
└── <NomeDoModulo>/
    ├── Domain/           → Entidades e regras de negócio puras
    ├── Application/      → Casos de uso e interfaces de repositório
    ├── Infrastructure/   → Implementação de acesso a dados (EF Core)
    └── Api/               → Controllers e DTOs
        ├── Controllers/
        └── DTOs/
```

**Por que essa abordagem?**
- Evita o overhead prematuro de microserviços em um projeto de porte único (um distribuidor, escala controlada)
- Mantém fronteiras de domínio bem definidas — cada módulo pode, no futuro, ser extraído como serviço independente sem grande refatoração
- Aplica inversão de dependência de verdade: o `Domain` não depende de nada externo (nem EF Core, nem ASP.NET); é a `Infrastructure` que depende do `Domain` através de interfaces

## 📦 Módulos

| Módulo | Status | Descrição |
|---|---|---|
| **Catalogo** | ✅ Implementado | Produtos e lotes, com controle de validade e cálculo automático de disponibilidade |
| **Clientes** | 🔜 Planejado | Cadastro e aprovação de comerciantes |
| **Pedidos** | 🔜 Planejado | Pedidos, itens e acompanhamento de status |
| **Frota** | 🔜 Planejado (v2) | Veículos, motoristas e entregas — base para rastreio em tempo real e app do motorista |

## ✨ Funcionalidades implementadas

- Cadastro de produtos (nome, categoria, unidade de medida, preço)
- Cadastro de lotes por produto, com data de validade
- Cálculo automático de disponibilidade, considerando apenas lotes ainda válidos
- API REST documentada via Swagger

## 🛠️ Tecnologias

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core** (SQL Server)
- **Clean Architecture** por módulo
- **Swagger / Swashbuckle** para documentação da API
- **Angular** (planejado para o frontend)

## 🚀 Como rodar localmente

### Pré-requisitos
- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server (Express ou superior)

### Passos

```bash
# Clonar o repositório
git clone https://github.com/alexandrefgs/distribuidora-flores.git
cd distribuidora-flores/src/DistribuidoraFlores.Api

# Restaurar pacotes
dotnet restore

# Aplicar migrations
dotnet ef database update

# Rodar a API
dotnet run
```

A API sobe em `http://localhost:5014` (ou porta configurada) e redireciona automaticamente para o Swagger na raiz.

## 🗺️ Roadmap

- [ ] Módulo de Clientes (cadastro e aprovação de comerciantes)
- [ ] Módulo de Pedidos (fluxo completo: criação → aprovação → separação → entrega)
- [ ] Autenticação e autorização (distinção entre usuário admin/distribuidor e comerciante)
- [ ] Frontend em Angular
- [ ] v2: módulo de Frota com rastreio de veículos e app para motoristas

## 📄 Licença

Este projeto é de uso pessoal/portfólio.
