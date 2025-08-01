using Xunit;
using Moq;
using FluentAssertions;
using TrilhaApiDesafio.Services;
using TrilhaApiDesafio.Repositories;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TrilhaApiDesafio.Mappings;

namespace Tests
{
    public class TarefaServiceTests
    {
        private readonly Mock<ITarefaRepository> _mockRepository;
        private readonly Mock<ILogger<TarefaService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly TarefaService _service;

        public TarefaServiceTests()
        {
            _mockRepository = new Mock<ITarefaRepository>();
            _mockLogger = new Mock<ILogger<TarefaService>>();
            
            var config = new MapperConfiguration(cfg => cfg.AddProfile<TarefaProfile>());
            _mapper = config.CreateMapper();
            
            _service = new TarefaService(_mockRepository.Object, _mapper, _mockLogger.Object);
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarTarefa_QuandoExistir()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa 
            { 
                Id = tarefaId, 
                Titulo = "Teste", 
                Descricao = "Descrição teste",
                Status = EnumStatusTarefa.Pendente,
                DataCriacao = DateTime.Now
            };
            
            _mockRepository.Setup(r => r.BuscarPorIdAsync(tarefaId))
                          .ReturnsAsync(tarefa);

            // Act
            var resultado = await _service.BuscarPorIdAsync(tarefaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(tarefaId);
            resultado.Titulo.Should().Be("Teste");
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            // Arrange
            var tarefaId = 999;
            _mockRepository.Setup(r => r.BuscarPorIdAsync(tarefaId))
                          .ReturnsAsync((Tarefa?)null);

            // Act
            var resultado = await _service.BuscarPorIdAsync(tarefaId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task CriarAsync_DeveRetornarTarefaResponseDto_QuandoSucesso()
        {
            // Arrange
            var requestDto = new TarefaRequestDto
            {
                Titulo = "Nova Tarefa",
                Descricao = "Nova descrição",
                Status = EnumStatusTarefa.Pendente
            };

            var tarefaCriada = new Tarefa
            {
                Id = 1,
                Titulo = requestDto.Titulo,
                Descricao = requestDto.Descricao,
                Status = requestDto.Status,
                DataCriacao = DateTime.Now
            };

            _mockRepository.Setup(r => r.CriarAsync(It.IsAny<Tarefa>()))
                          .ReturnsAsync(tarefaCriada);

            // Act
            var resultado = await _service.CriarAsync(requestDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Titulo.Should().Be(requestDto.Titulo);
            resultado.Descricao.Should().Be(requestDto.Descricao);
            resultado.Status.Should().Be(requestDto.Status);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarTarefaAtualizada_QuandoExistir()
        {
            // Arrange
            var tarefaId = 1;
            var requestDto = new TarefaRequestDto
            {
                Titulo = "Tarefa Atualizada",
                Descricao = "Descrição atualizada",
                Status = EnumStatusTarefa.Finalizada
            };

            var tarefaExistente = new Tarefa
            {
                Id = tarefaId,
                Titulo = "Tarefa Original",
                Descricao = "Descrição original",
                Status = EnumStatusTarefa.Pendente,
                DataCriacao = DateTime.Now.AddDays(-1)
            };

            _mockRepository.Setup(r => r.BuscarPorIdAsync(tarefaId))
                          .ReturnsAsync(tarefaExistente);
            
            _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Tarefa>()))
                          .ReturnsAsync((Tarefa t) => t);

            // Act
            var resultado = await _service.AtualizarAsync(tarefaId, requestDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Titulo.Should().Be(requestDto.Titulo);
            resultado.Descricao.Should().Be(requestDto.Descricao);
            resultado.Status.Should().Be(requestDto.Status);
        }

        [Fact]
        public async Task RemoverAsync_DeveRetornarTrue_QuandoExistir()
        {
            // Arrange
            var tarefaId = 1;
            var tarefa = new Tarefa
            {
                Id = tarefaId,
                Titulo = "Tarefa para remover",
                Status = EnumStatusTarefa.Pendente
            };

            _mockRepository.Setup(r => r.BuscarPorIdAsync(tarefaId))
                          .ReturnsAsync(tarefa);
            
            _mockRepository.Setup(r => r.RemoverAsync(tarefaId))
                          .ReturnsAsync(true);

            // Act
            var resultado = await _service.RemoverAsync(tarefaId);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task RemoverAsync_DeveRetornarFalse_QuandoNaoExistir()
        {
            // Arrange
            var tarefaId = 999;
            _mockRepository.Setup(r => r.BuscarPorIdAsync(tarefaId))
                          .ReturnsAsync((Tarefa?)null);

            // Act
            var resultado = await _service.RemoverAsync(tarefaId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task BuscarTodosAsync_DeveRetornarListaDeTarefas()
        {
            // Arrange
            var tarefas = new List<Tarefa>
            {
                new() { Id = 1, Titulo = "Tarefa 1", Status = EnumStatusTarefa.Pendente },
                new() { Id = 2, Titulo = "Tarefa 2", Status = EnumStatusTarefa.Finalizada }
            };

            _mockRepository.Setup(r => r.BuscarTodosAsync())
                          .ReturnsAsync(tarefas);

            // Act
            var resultado = await _service.BuscarTodosAsync();

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().Contain(t => t.Titulo == "Tarefa 1");
            resultado.Should().Contain(t => t.Titulo == "Tarefa 2");
        }

        [Fact]
        public async Task BuscarPorStatusAsync_DeveRetornarTarefasComStatusCorreto()
        {
            // Arrange
            var status = EnumStatusTarefa.Pendente;
            var tarefas = new List<Tarefa>
            {
                new() { Id = 1, Titulo = "Tarefa Pendente 1", Status = EnumStatusTarefa.Pendente },
                new() { Id = 2, Titulo = "Tarefa Pendente 2", Status = EnumStatusTarefa.Pendente }
            };

            _mockRepository.Setup(r => r.BuscarPorStatusAsync(status))
                          .ReturnsAsync(tarefas);

            // Act
            var resultado = await _service.BuscarPorStatusAsync(status);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().OnlyContain(t => t.Status == EnumStatusTarefa.Pendente);
        }
    }
}
