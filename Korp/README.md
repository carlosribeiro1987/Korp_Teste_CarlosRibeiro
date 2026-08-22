# Korp - Teste Técnico

Solução composta por dois microsserviços ASP.NET Core e um front-end Angular para cadastro de produtos, controle de estoque e emissão de notas fiscais.

## Como rodar com Docker

Pré-requisitos:

- Docker Desktop
- Docker Compose

Na raiz do projeto `Korp`, execute:

```powershell
docker compose up --build
```

Após a inicialização, acesse:

```text
Frontend:             http://localhost:4200
Estoque Swagger:      http://localhost:5099/swagger
Faturamento Swagger:  http://localhost:5108/swagger
```

Para parar os containers:

```powershell
docker compose down
```

Para parar e remover também os volumes dos bancos SQLite:

```powershell
docker compose down -v
```

## Como rodar sem Docker

Pré-requisitos:

- .NET 10 SDK
- Node.js
- npm

Abra três terminais.

Terminal 1 - Estoque:

```powershell
cd "Korp\Estoque.API"
dotnet run
```

Terminal 2 - Faturamento:

```powershell
cd "Korp\Faturamento.API"
dotnet run
```

Terminal 3 - Front-end:

```powershell
cd "Korp\frontend"
npm install
npm start
```

URLs da execução local:

```text
Frontend:             http://localhost:4200
Estoque Swagger:      http://localhost:5099/swagger
Faturamento Swagger:  http://localhost:5108/swagger
```

O front-end usa `proxy.conf.json` para encaminhar as chamadas:

```text
/estoque-api      -> http://localhost:5099
/faturamento-api  -> http://localhost:5108
```

## Visão geral da arquitetura

```text
frontend Angular
  -> Estoque.API
  -> Faturamento.API
       -> Estoque.API
```

- `Estoque.API`: cadastro de produtos e controle de saldo.
- `Faturamento.API`: criação, alteração e impressão de notas fiscais.
- `frontend`: interface Angular para operar produtos e notas fiscais.

Cada microsserviço possui seu próprio banco SQLite. O faturamento não acessa diretamente o banco do estoque; ele se comunica com o `Estoque.API` por HTTP.

## Portas

```text
Estoque.API      5099
Faturamento.API  5108
Frontend         4200
```

No Docker, as APIs escutam internamente na porta `8080` e são expostas nas portas acima pelo `docker-compose.yml`.

## Principais endpoints

Estoque:

```text
GET    /api/produtos
GET    /api/produtos/{id}
POST   /api/produtos
POST   /api/produtos/{id}/reduzir-saldo
POST   /api/produtos/{id}/aumentar-saldo
```

Faturamento:

```text
GET     /api/notas-fiscais
GET     /api/notas-fiscais/{id}
POST    /api/notas-fiscais
POST    /api/notas-fiscais/{id}/itens
DELETE  /api/notas-fiscais/{id}/itens/{itemId}
POST    /api/notas-fiscais/{id}/imprimir
```

## Fluxo principal

1. Cadastrar um produto no estoque.
2. Criar uma nota fiscal no faturamento.
3. Adicionar um item à nota fiscal usando o produto cadastrado.
4. Imprimir a nota fiscal.
5. Conferir que o saldo do produto foi reduzido no estoque.

## Persistência

Execução local:

```text
Estoque.API      estoque.db
Faturamento.API  faturamento.db
```

Execução com Docker:

```text
Estoque.API      /app/data/estoque.db
Faturamento.API  /app/data/faturamento.db
```

O Docker Compose usa volumes nomeados:

```text
estoque-data
faturamento-data
```

As migrations são aplicadas automaticamente na inicialização das APIs.

## Tratamento de falhas

Durante a impressão de uma nota fiscal, o `Faturamento.API` chama o `Estoque.API` para reduzir o saldo dos produtos.

Se o estoque estiver indisponível, a nota não é fechada indevidamente. O erro é retornado ao usuário e a operação pode ser tentada novamente quando o estoque voltar.

## Documentação técnica

Mais detalhes de arquitetura, decisões técnicas e validação estão em:

```text
DETALHAMENTO_TECNICO.md
```
