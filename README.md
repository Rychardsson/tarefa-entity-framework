# Sistema Gerenciador de Tarefas

Este é um sistema gerenciador de tarefas desenvolvido como uma Web API usando .NET 6, Entity Framework Core e SQLite.

## Funcionalidades

O sistema oferece um CRUD completo para gerenciamento de tarefas com as seguintes operações:

### Endpoints da API

- **GET /Tarefa/{id}** - Obtém uma tarefa específica por ID
- **GET /Tarefa/ObterTodos** - Obtém todas as tarefas
- **GET /Tarefa/ObterPorTitulo?titulo={titulo}** - Obtém tarefas que contenham o título especificado
- **GET /Tarefa/ObterPorData?data={data}** - Obtém tarefas por data
- **GET /Tarefa/ObterPorStatus?status={status}** - Obtém tarefas por status (Pendente ou Finalizado)
- **POST /Tarefa** - Cria uma nova tarefa
- **PUT /Tarefa/{id}** - Atualiza uma tarefa existente
- **DELETE /Tarefa/{id}** - Remove uma tarefa

### Modelo de Dados

A classe `Tarefa` possui as seguintes propriedades:

```csharp
public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; }          // Obrigatório, máximo 200 caracteres
    public string Descricao { get; set; }       // Opcional, máximo 1000 caracteres
    public DateTime Data { get; set; }          // Obrigatório
    public EnumStatusTarefa Status { get; set; } // Obrigatório (Pendente ou Finalizado)
}
```

### Status das Tarefas

```csharp
public enum EnumStatusTarefa
{
    Pendente,
    Finalizado
}
```

## Tecnologias Utilizadas

- **.NET 6** - Framework para desenvolvimento da Web API
- **Entity Framework Core 6** - ORM para acesso ao banco de dados
- **SQLite** - Banco de dados leve para desenvolvimento
- **Swagger/OpenAPI** - Documentação automática da API
- **ASP.NET Core** - Framework web

## Como Executar

1. **Pré-requisitos:**

   - .NET 6 SDK instalado
   - Visual Studio Code ou Visual Studio

2. **Configuração:**

   ```bash
   # Clone o repositório
   git clone <url-do-repositorio>

   # Navegue até a pasta do projeto
   cd tarefa-entity-framework

   # Restaure as dependências
   dotnet restore
   ```

3. **Banco de Dados:**

   ```bash
   # Aplicar as migrations (já configurado)
   dotnet ef database update
   ```

4. **Executar a aplicação:**

   ```bash
   dotnet run
   ```

5. **Acessar a documentação:**
   - Swagger UI: `https://localhost:7295/swagger`
   - API Base URL: `https://localhost:7295`

## Exemplo de Uso

### Criar uma nova tarefa:

```json
POST /Tarefa
{
  "titulo": "Estudar .NET",
  "descricao": "Revisar conceitos de Entity Framework",
  "data": "2025-07-24T10:00:00",
  "status": 0
}
```

### Atualizar uma tarefa:

```json
PUT /Tarefa/1
{
  "titulo": "Estudar .NET - Concluído",
  "descricao": "Revisar conceitos de Entity Framework - Finalizado",
  "data": "2025-07-24T10:00:00",
  "status": 1
}
```

## Estrutura do Projeto

```
├── Controllers/
│   └── TarefaController.cs      # Controller com endpoints da API
├── Models/
│   ├── Tarefa.cs               # Modelo da entidade Tarefa
│   └── EnumStatusTarefa.cs     # Enum para status das tarefas
├── Context/
│   └── OrganizadorContext.cs   # Contexto do Entity Framework
├── Migrations/                 # Migrations do banco de dados
├── Program.cs                  # Configuração da aplicação
└── appsettings.json           # Configurações (connection string, etc.)
```

## Validações Implementadas

- Título é obrigatório e deve ter no máximo 200 caracteres
- Descrição é opcional e deve ter no máximo 1000 caracteres
- Data é obrigatória e não pode ser vazia
- Status é obrigatório e deve ser Pendente (0) ou Finalizado (1)

## Banco de Dados

O sistema utiliza SQLite como banco de dados, criando automaticamente um arquivo `tarefas.db` na raiz do projeto. Para ambientes de produção, a string de conexão pode ser facilmente alterada para SQL Server ou outro provedor compatível com Entity Framework Core.
