# Detalhamento Técnico

## Visão geral

O projeto implementa uma solução composta por dois microsserviços ASP.NET Core e um front-end Angular:

- `Estoque.API`: responsável pelo cadastro de produtos e controle de saldo.
- `Faturamento.API`: responsável pela criação, manutenção e impressão de notas fiscais.
- `frontend`: aplicação Angular para operar produtos e notas fiscais.

Os microsserviços possuem bancos SQLite separados e se comunicam por HTTP. O `Faturamento.API` chama o `Estoque.API` durante a impressão da nota fiscal para reduzir o saldo dos produtos.

## Tecnologias utilizadas

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- Swagger / Swashbuckle
- Angular
- RxJS
- Docker
- Docker Compose
- Nginx para servir o front-end em container

## Estrutura da solução

```text
Korp/
  Estoque.API/
  Faturamento.API/
  frontend/
  docker-compose.yml
```

## Microsserviço de estoque

O `Estoque.API` expõe endpoints para cadastro, consulta e movimentação de saldo de produtos.

Endpoints principais:

```text
GET    /api/produtos
GET    /api/produtos/{id}
POST   /api/produtos
POST   /api/produtos/{id}/reduzir-saldo
POST   /api/produtos/{id}/aumentar-saldo
```

Regras principais:

- Cada produto possui `Id`, `Código`, `Descrição`, `Saldo`, `CriadoEm` e `ModificadoEm`.
- O código do produto deve ser único.
- O saldo não pode ficar negativo.
- A redução de saldo retorna erro quando a quantidade solicitada é maior que o saldo disponível.
- As consultas usam `AsNoTracking()` quando não há necessidade de rastreamento pelo EF Core.

## Microsserviço de faturamento

O `Faturamento.API` expõe endpoints para criar, consultar, alterar e imprimir notas fiscais.

Endpoints principais:

```text
GET     /api/notas-fiscais
GET     /api/notas-fiscais/{id}
POST    /api/notas-fiscais
POST    /api/notas-fiscais/{id}/itens
DELETE  /api/notas-fiscais/{id}/itens/{itemId}
POST    /api/notas-fiscais/{id}/imprimir
```

Regras principais:

- A nota fiscal inicia com status `Aberta`.
- Uma nota aberta pode receber e remover itens.
- Uma nota fechada não pode ser alterada.
- A impressão exige que a nota tenha pelo menos um item.
- Ao imprimir, o faturamento chama o estoque para reduzir o saldo dos produtos.
- Se o estoque estiver indisponível, a API retorna erro indicando falha de comunicação com o serviço de estoque.

## Comunicação entre microsserviços

O `Faturamento.API` usa `HttpClient` tipado por meio da interface `IEstoqueClient`.

Em execução local sem Docker, a URL do estoque vem de:

```json
"Services": {
  "EstoqueApi": "http://localhost:5099"
}
```

Em Docker Compose, essa configuração é sobrescrita por variável de ambiente:

```text
Services__EstoqueApi=http://estoque-api:8080
```

Isso permite que o faturamento encontre o estoque pelo nome do serviço dentro da rede Docker.

## Persistência

Cada microsserviço possui seu próprio banco SQLite.

Em execução local:

```text
Estoque.API      -> estoque.db
Faturamento.API  -> faturamento.db
```

Em Docker:

```text
Estoque.API      -> /app/data/estoque.db
Faturamento.API  -> /app/data/faturamento.db
```

O `docker-compose.yml` usa volumes nomeados para persistir os bancos:

```text
estoque-data
faturamento-data
```

As migrations são aplicadas na inicialização de cada API com `Database.Migrate()`.

## Front-end

O front-end Angular consome as APIs por caminhos relativos:

```text
/estoque-api
/faturamento-api
```

Em desenvolvimento, o Angular usa `proxy.conf.json` para redirecionar essas rotas para:

```text
Estoque.API      http://localhost:5099
Faturamento.API  http://localhost:5108
```

Em Docker, o front-end é buildado com Node é servido por Nginx. O Nginx também faz proxy para os microsserviços:

```text
/estoque-api      -> http://estoque-api:8080
/faturamento-api  -> http://faturamento-api:8080
```

## Docker

O projeto pode ser executado com Docker Compose a partir da raiz `Korp`:

```powershell
docker compose up --build
```

Serviços expostos:

```text
Frontend             http://localhost:4200
Estoque Swagger      http://localhost:5099/swagger
Faturamento Swagger  http://localhost:5108/swagger
```

Mapeamento de portas:

```text
Estoque.API      5099:8080
Faturamento.API  5108:8080
frontend         4200:80
```

Para parar os containers:

```powershell
docker compose down
```

Para remover também os volumes dos bancos:

```powershell
docker compose down -v
```

## Tratamento de falhas

O faturamento trata falhas de comunicação com o estoque durante a impressão da nota fiscal.

Cenários tratados:

- Estoque fora do ar.
- Timeout na chamada HTTP.
- Retorno não esperado da API de estoque.
- Saldo insuficiente no produto.

Quando o estoque volta a funcionar, uma nova tentativa de impressão pode ser feita normalmente, desde que a nota ainda esteja aberta e as regras de negócio sejam atendidas.

## Concorrência

O controle de saldo é realizado no microsserviço de estoque, centralizando a regra de negócio que impede saldo negativo.

Como a redução de saldo acontece dentro do `Estoque.API`, o faturamento não altera diretamente o banco de estoque. Essa separação evita acoplamento entre bancos e mantém a consistência da regra no serviço dono dos dados.

Para um ambiente de maior carga, melhorias possíveis seriam:

- token de concorrencia no produto;
- transação explícita na atualização de saldo;
- retry controlado em erros transitorios;
- fila ou mecanismo de compensacao para fluxos assíncronos.

Para o escopo do teste, a solução mantém a comunicação HTTP direta e regras centralizadas no microsserviço responsável.

## Decisões técnicas

- Foram usados dois bancos SQLite separados para manter a independência entre os microsserviços.
- O front-end usa paths relativos para evitar hardcode de host e porta no browser.
- O Nginx foi usado no container do front-end porque Angular gera arquivos estáticos em produção.
- O Docker Compose foi adotado para garantir ambiente reprodutível para avaliação.
- Swagger foi mantido nos dois microsserviços para facilitar testes manuais.
- Interfaces foram usadas em services e clients para reduzir acoplamento e facilitar manutenção.

## Como validar

1. Subir todos os serviços:

```powershell
docker compose up --build
```

2. Acessar o Swagger do estoque:

```text
http://localhost:5099/swagger
```

3. Cadastrar produtos.

4. Acessar o Swagger do faturamento:

```text
http://localhost:5108/swagger
```

5. Criar uma nota fiscal, adicionar itens e imprimir.

6. Acessar o front-end:

```text
http://localhost:4200
```

7. Verificar se a impressão da nota reduz o saldo do produto no estoque.

