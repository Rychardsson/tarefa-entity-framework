using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Mappings;
using TrilhaApiDesafio.Repositories;
using TrilhaApiDesafio.Services;
using TrilhaApiDesafio.Validators;
using System.Reflection;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddDbContext<OrganizadorContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("ConexaoPadrao")));

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(TarefaProfile));

    // FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<TarefaRequestValidator>();

    // Repository Pattern e Services
    builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
    builder.Services.AddScoped<ITarefaService, TarefaService>();

    // Controllers com configurações JSON
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Health Checks
    builder.Services.AddHealthChecks();

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Sistema Gerenciador de Tarefas API",
            Version = "v1",
            Description = "API para gerenciamento de tarefas com CRUD completo",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Desenvolvedor",
                Email = "dev@example.com"
            }
        });

        // Incluir comentários XML
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // Response Caching
    builder.Services.AddResponseCaching();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tarefas API V1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseCors("AllowAll");

    app.UseResponseCaching();

    app.UseAuthorization();

    // Health Check endpoint
    app.MapHealthChecks("/health");

    app.MapControllers();

    Log.Information("Iniciando aplicação...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
