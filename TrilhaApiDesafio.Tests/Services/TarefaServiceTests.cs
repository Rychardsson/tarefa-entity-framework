using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Mappings;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Repositories;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Tests.Services
{
    public class TarefaServiceTests
    {
        private readonly Mock<ITarefaRepository> _repositoryMock;
        private readonly Mock<ILogger<TarefaService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly TarefaService _service;

        public TarefaServiceTests()
        {
            _repositoryMock = new Mock<ITarefaRepository>();
            _loggerMock = new Mock<ILogger<TarefaService>>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TarefaProfile>();
            });
            _mapper = configuration.CreateMapper();

            _service = new TarefaService(_repositoryMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodasAsTarefas()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new Tarefa { Id = 1, Titulo = "Tarefa 1", Status = EnumStatusTarefa.Pendente, Data = DateTime.Now, DataCriacao = DateTime.Now },
                new Tarefa { Id = 2, Titulo = "Tarefa 2", Status = EnumStatusTarefa.Finalizado, Data = DateTime.Now, DataCriacao = DateTime.Now }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tarefas);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllBeOfType<TarefaResponseDto>();
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ComIdValido_DeveRetornarTarefa()
        {
            // Arrange
            var tarefa = new Tarefa 
            { 
                Id = 1, 
                Titulo = "Tarefa Teste", 
                Descricao = "Descrição teste",
                Status = EnumStatusTarefa.Pendente, 
                Data = DateTime.Now, 
                DataCriacao = DateTime.Now 
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tarefa);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Titulo.Should().Be("Tarefa Teste");
            result.StatusDescricao.Should().Be("Pendente");
        }

        [Fact]
        public async Task GetByIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Tarefa?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarTarefa()
        {
            // Arrange
            var tarefaRequest = new TarefaRequestDto
            {
                Titulo = "Nova Tarefa",
                Descricao = "Nova descrição",
                Data = DateTime.Now.AddDays(1),
                Status = EnumStatusTarefa.Pendente
            };

            var tarefaCriada = new Tarefa
            {
                Id = 1,
                Titulo = tarefaRequest.Titulo,
                Descricao = tarefaRequest.Descricao,
                Data = tarefaRequest.Data,
                Status = tarefaRequest.Status,
                DataCriacao = DateTime.Now
            };

            _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Tarefa>())).ReturnsAsync(tarefaCriada);

            // Act
            var result = await _service.CreateAsync(tarefaRequest);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Titulo.Should().Be(tarefaRequest.Titulo);
            result.Status.Should().Be(EnumStatusTarefa.Pendente);
            _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Tarefa>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComIdValido_DeveAtualizarTarefa()
        {
            // Arrange
            var tarefaExistente = new Tarefa
            {
                Id = 1,
                Titulo = "Tarefa Original",
                Status = EnumStatusTarefa.Pendente,
                Data = DateTime.Now,
                DataCriacao = DateTime.Now
            };

            var tarefaRequest = new TarefaRequestDto
            {
                Titulo = "Tarefa Atualizada",
                Descricao = "Descrição atualizada",
                Data = DateTime.Now.AddDays(1),
                Status = EnumStatusTarefa.Finalizado
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tarefaExistente);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Tarefa>())).ReturnsAsync(tarefaExistente);

            // Act
            var result = await _service.UpdateAsync(1, tarefaRequest);

            // Assert
            result.Should().NotBeNull();
            result!.Titulo.Should().Be("Tarefa Atualizada");
            result.Status.Should().Be(EnumStatusTarefa.Finalizado);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tarefa>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            var tarefaRequest = new TarefaRequestDto
            {
                Titulo = "Tarefa Atualizada",
                Data = DateTime.Now,
                Status = EnumStatusTarefa.Finalizado
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Tarefa?)null);

            // Act
            var result = await _service.UpdateAsync(999, tarefaRequest);

            // Assert
            result.Should().BeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Tarefa>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ComIdValido_DeveRetornarTrue()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ComIdInvalido_DeveRetornarFalse()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetByTituloAsync_ComTituloValido_DeveRetornarTarefas()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new Tarefa { Id = 1, Titulo = "Estudar .NET", Status = EnumStatusTarefa.Pendente, Data = DateTime.Now, DataCriacao = DateTime.Now },
                new Tarefa { Id = 2, Titulo = "Estudar Entity Framework", Status = EnumStatusTarefa.Pendente, Data = DateTime.Now, DataCriacao = DateTime.Now }
            };

            _repositoryMock.Setup(r => r.GetByTituloAsync("Estudar")).ReturnsAsync(tarefas);

            // Act
            var result = await _service.GetByTituloAsync("Estudar");

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(t => t.Titulo.Should().Contain("Estudar"));
        }

        [Fact]
        public async Task GetByStatusAsync_ComStatus_DeveRetornarTarefasComStatus()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new Tarefa { Id = 1, Titulo = "Tarefa 1", Status = EnumStatusTarefa.Pendente, Data = DateTime.Now, DataCriacao = DateTime.Now },
                new Tarefa { Id = 2, Titulo = "Tarefa 2", Status = EnumStatusTarefa.Pendente, Data = DateTime.Now, DataCriacao = DateTime.Now }
            };

            _repositoryMock.Setup(r => r.GetByStatusAsync(EnumStatusTarefa.Pendente)).ReturnsAsync(tarefas);

            // Act
            var result = await _service.GetByStatusAsync(EnumStatusTarefa.Pendente);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(t => t.Status.Should().Be(EnumStatusTarefa.Pendente));
        }
    }
}
