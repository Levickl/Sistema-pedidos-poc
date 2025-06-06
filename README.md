# Sistema de Gestão de Pedidos 📦

Este repositório contém a solução desenvolvida para a Prova de Conceito (PoC) de uma aplicação completa de gestão de pedidos, com backend em .NET, frontend em React, banco PostgreSQL via Docker e mensageria com Azure Service Bus.

---

## 🧰 Tecnologias Utilizadas

### 🔙 Backend (.NET 7)
- C#
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (via Docker)
- Azure Service Bus
- Swagger
- Docker

### 🎨 Frontend
- React (Vite)
- JavaScript
- TailwindCSS

---

## 📂 Estrutura do Projeto

```
/
├── backend/
│   ├── PedidoGestao.API/
│   ├── PedidoGestao.Infrastructure/
│   ├── PedidoGestao.Domain/
│   └── docker-compose.yml
│
├── frontend/
│   └── gestaopedidosApp/
│       ├── public/
│       ├── src/
│       ├── vite.config.js
│       └── tailwind.config.js
│
└── README.md
```

---

## 🚀 Como rodar o projeto

### 1. 🐘 Subir o banco de dados com Docker

> Certifique-se de ter o Docker instalado e rodando.

No diretório `backend/`, execute:

```bash
docker compose up -d
```

Esse comando irá subir um container PostgreSQL com as seguintes credenciais:

- Banco: `pedidos_db`
- Usuário: `postgres`
- Senha: `123456`
- Porta: `5432`

---

### 2. 🔧 Configurar a connection string (se necessário)

No `appsettings.json` da API (`PedidoGestao.API`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=pedidos_db;Username=postgres;Password=123456"
},
"ServiceBus": {
  "ConnectionString": "<sua_connection_string_azure_service_bus>",
  "QueueName": "orders"
}
```

---

### 3. 🧱 Criar a base de dados (caso precise)

No terminal da API:

```bash
dotnet ef database update --project ../PedidoGestao.Infrastructure --startup-project .
```

---

### 4. ▶️ Rodar a API

No diretório do projeto da API:

```bash
dotnet run
```

A API estará disponível em:

```
http://localhost:5109/swagger
```

---

### 5. 💼 Funcionalidades do Backend

- `POST /api/orders` — Cria um novo pedido e envia mensagem para o Azure Service Bus
- `GET /api/orders` — Retorna todos os pedidos
- `GET /api/orders/{id}` — Retorna pedido por ID
- Worker interno (BackgroundService) consome a fila `orders` do Azure e atualiza os pedidos:
  - `Pendente` → `Processando` → `Finalizado` (após 5 segundos)

---

### 6. 💻 Rodar o Frontend

No diretório `frontend/`, rode:

```bash
npm install
npm run dev
```

O frontend estará disponível em:

```
http://localhost:5173/
```

---

## 🧠 Observações

- Esta foi minha **primeira experiência prática criando uma API REST em C# com ASP.NET Core**.
- Também foi meu **primeiro uso real de Azure Service Bus** e **TailwindCSS no frontend**.
- Me dediquei ao máximo buscando, testando e integrando as tecnologias novas para completar o desafio.


🚀 Obrigado pela oportunidade e por considerar meu projeto!
