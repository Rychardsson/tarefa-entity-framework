# Sistema Gerenciador de Tarefas - Versão Melhorada

Este é um sistema gerenciador de tarefas desenvolvido como uma Web API usando **.NET 8**, Entity Framework Core e SQLite, implementando as melhores práticas de desenvolvimento.

## 🚀 Melhorias Implementadas

### ✅ **Atualização Tecnológica**

- **Migrado para .NET 8** (última versão LTS)
- **Nullable Reference Types** habilitado para maior segurança
- **Pacotes atualizados** para versões mais recentes

### ✅ **Arquitetura Melhorada**

- **Repository Pattern** - Separação da lógica de acesso aos dados
- **Service Layer** - Lógica de negócio centralizada
- **DTOs** - Data Transfer Objects para entrada/saída da API
- **AutoMapper** - Mapeamento automático entre entidades e DTOs

### ✅ **Validações Robustas**

- **FluentValidation** - Validações mais expressivas e flexíveis
- **Validações customizadas** - Regras de negócio específicas
- **Tratamento de erros** melhorado

### ✅ **Qualidade e Monitoramento**

- **Serilog** - Sistema de logging estruturado
- **Health Checks** - Endpoint para verificar saúde da aplicação
- **CORS** configurado para integração frontend
- **Response Caching** para melhor performance

### ✅ **Recursos Avançados**

- **Soft Delete** - Exclusão lógica dos registros
- **Auditoria** - Campos de data criação e atualização
- **Documentação Swagger** melhorada
- **Índices no banco** para otimização de consultas

## Funcionalidades

O sistema oferece um CRUD completo para gerenciamento de tarefas com as seguintes operações:

### Endpoints da API

- **GET /api/Tarefa/{id}** - Obtém uma tarefa específica por ID
- **GET /api/Tarefa** - Obtém todas as tarefas (não deletadas)
- **GET /api/Tarefa/titulo/{titulo}** - Obtém tarefas que contenham o título especificado
- **GET /api/Tarefa/data/{data}** - Obtém tarefas por data
- **GET /api/Tarefa/status/{status}** - Obtém tarefas por status (0=Pendente, 1=Finalizado)
- **POST /api/Tarefa** - Cria uma nova tarefa
- **PUT /api/Tarefa/{id}** - Atualiza uma tarefa existente
- **DELETE /api/Tarefa/{id}** - Remove uma tarefa (soft delete)

### Modelo de Dados

#### Request DTO (TarefaRequestDto)

```csharp
{
    "titulo": "string",      // Obrigatório, máximo 200 caracteres
    "descricao": "string",   // Opcional, máximo 1000 caracteres
    "data": "2025-07-31T10:00:00",  // Obrigatório
    "status": 0              // Obrigatório (0=Pendente, 1=Finalizado)
}
```

#### Response DTO (TarefaResponseDto)

```csharp
{
    "id": 1,
    "titulo": "string",
    "descricao": "string",
    "data": "2025-07-31T10:00:00",
    "status": 0,
    "statusDescricao": "Pendente",
    "dataCriacao": "2025-07-31T09:00:00"
}
```

### Status das Tarefas

```csharp
public enum EnumStatusTarefa
{
    Pendente = 0,
    Finalizado = 1
}
```

## Tecnologias Utilizadas

- **.NET 8** - Framework para desenvolvimento da Web API
- **Entity Framework Core 8** - ORM para acesso ao banco de dados
- **SQLite** - Banco de dados leve para desenvolvimento
- **AutoMapper** - Mapeamento entre objetos
- **FluentValidation** - Validações robustas
- **Serilog** - Sistema de logging estruturado
- **Swagger/OpenAPI** - Documentação automática da API
- **ASP.NET Core** - Framework web

## Como Executar

1. **Pré-requisitos:**

   - .NET 8 SDK instalado
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
   # As migrations já estão aplicadas, mas se necessário:
   dotnet ef database update
   ```

4. **Executar a aplicação:**

   ```bash
   dotnet run
   ```

5. **Acessar a documentação:**
   - Swagger UI: `https://localhost:7295/swagger`
   - API Base URL: `https://localhost:7295/api`
   - Health Check: `https://localhost:7295/health`

## Exemplo de Uso

### Criar uma nova tarefa:

```json
POST /api/Tarefa
Content-Type: application/json

{
  "titulo": "Estudar .NET 8",
  "descricao": "Revisar novos recursos do .NET 8",
  "data": "2025-07-31T14:00:00",
  "status": 0
}
```

### Resposta:

```json
{
  "id": 1,
  "titulo": "Estudar .NET 8",
  "descricao": "Revisar novos recursos do .NET 8",
  "data": "2025-07-31T14:00:00",
  "status": 0,
  "statusDescricao": "Pendente",
  "dataCriacao": "2025-07-31T21:53:20"
}
```

### Atualizar uma tarefa:

```json
PUT /api/Tarefa/1
Content-Type: application/json

{
  "titulo": "Estudar .NET 8 - Concluído",
  "descricao": "Revisar novos recursos do .NET 8 - Finalizado",
  "data": "2025-07-31T14:00:00",
  "status": 1
}
```

## Estrutura do Projeto

```
├── Controllers/
│   └── TarefaController.cs      # Controller com endpoints da API
├── DTOs/
│   ├── TarefaRequestDto.cs      # DTO para entrada de dados
│   └── TarefaResponseDto.cs     # DTO para saída de dados
├── Models/
│   ├── Tarefa.cs               # Entidade do domínio
│   └── EnumStatusTarefa.cs     # Enum para status das tarefas
├── Context/
│   └── OrganizadorContext.cs   # Contexto do Entity Framework
├── Repositories/
│   ├── ITarefaRepository.cs    # Interface do repositório
│   └── TarefaRepository.cs     # Implementação do repositório
├── Services/
│   ├── ITarefaService.cs       # Interface do serviço
│   └── TarefaService.cs        # Implementação do serviço
├── Mappings/
│   └── TarefaProfile.cs        # Perfil do AutoMapper
├── Validators/
│   └── TarefaRequestValidator.cs # Validações com FluentValidation
├── Migrations/                 # Migrations do banco de dados
├── logs/                       # Arquivos de log da aplicação
├── Program.cs                  # Configuração da aplicação
└── appsettings.json           # Configurações (connection string, etc.)
```

## Validações Implementadas

### Título

- Obrigatório
- Máximo 200 caracteres

### Descrição

- Opcional
- Máximo 1000 caracteres quando informada

### Data

- Obrigatória
- Deve ser uma data válida
- Não pode ser mais de 1 ano no passado
- Não pode ser mais de 5 anos no futuro

### Status

- Obrigatório
- Deve ser 0 (Pendente) ou 1 (Finalizado)

## Recursos de Qualidade

### Logging

- Logs estruturados com Serilog
- Logs salvos em arquivos rotativos (pasta `logs/`)
- Logs no console para desenvolvimento

### Health Checks

- Endpoint `/health` para verificar status da aplicação
- Verificação automática do contexto do banco

### CORS

- Configurado para permitir chamadas de qualquer origem
- Ideal para integração com frontend

### Caching

- Response caching habilitado para melhor performance

## Banco de Dados

O sistema utiliza SQLite como banco de dados, criando automaticamente um arquivo `tarefas.db` na raiz do projeto.

### Características:

- **Soft Delete**: Registros não são removidos fisicamente
- **Auditoria**: Campos de data de criação e atualização
- **Índices**: Otimizações para consultas frequentes
- **Migrations**: Versionamento automático do schema

Para ambientes de produção, a string de conexão pode ser facilmente alterada para SQL Server ou outro provedor compatível com Entity Framework Core.

## Próximos Passos (Futuras Melhorias)

- [ ] **Autenticação JWT** - Sistema de login e autorização
- [ ] **Testes Unitários** - Cobertura de testes com xUnit
- [ ] **Paginação** - Para endpoints que retornam listas
- [ ] **Rate Limiting** - Controle de taxa de requisições
- [ ] **Docker** - Containerização da aplicação
- [ ] **CI/CD** - Pipeline de integração contínua
