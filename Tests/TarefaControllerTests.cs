using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrilhaApiDesafio.Controllers;
using TrilhaApiDesafio.Services;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;

namespace Tests
{
    public class TarefaControllerTests
    {
        private readonly Mock<ITarefaService> _mockService;
        private readonly Mock<ILogger<TarefaController>> _mockLogger;
        private readonly TarefaController _controller;

        public TarefaControllerTests()
        {
            _mockService = new Mock<ITarefaService>();
            _mockLogger = new Mock<ILogger<TarefaController>>();
            _controller = new TarefaController(_mockService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarOk_QuandoTarefaExistir()
        {
            // Arrange
            var tarefaId = 1;
            var tarefaResponse = new TarefaResponseDto
            {
                Id = tarefaId,
                Titulo = "Teste",
                Descricao = "Descrição teste",
                Status = EnumStatusTarefa.Pendente,
                DataCriacao = "31/01/2025 22:00"
            };

            _mockService.Setup(s => s.BuscarPorIdAsync(tarefaId))
                       .ReturnsAsync(tarefaResponse);

            // Act
            var resultado = await _controller.ObterPorId(tarefaId);

            // Assert
            var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            var tarefa = okResult.Value.Should().BeOfType<TarefaResponseDto>().Subject;
            tarefa.Id.Should().Be(tarefaId);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNotFound_QuandoTarefaNaoExistir()
        {
            // Arrange
            var tarefaId = 999;
            _mockService.Setup(s => s.BuscarPorIdAsync(tarefaId))
                       .ReturnsAsync((TarefaResponseDto?)null);

            // Act
            var resultado = await _controller.ObterPorId(tarefaId);

            // Assert
            resultado.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Criar_DeveRetornarCreated_QuandoTarefaValidaForFornecida()
        {
            // Arrange
            var requestDto = new TarefaRequestDto
            {
                Titulo = "Nova Tarefa",
                Descricao = "Nova descrição",
                Status = EnumStatusTarefa.Pendente
            };

            var responseDto = new TarefaResponseDto
            {
                Id = 1,
                Titulo = requestDto.Titulo,
                Descricao = requestDto.Descricao,
                Status = requestDto.Status,
                DataCriacao = "31/01/2025 22:00"
            };

            _mockService.Setup(s => s.CriarAsync(requestDto))
                       .ReturnsAsync(responseDto);

            // Act
            var resultado = await _controller.Criar(requestDto);

            // Assert
            var createdResult = resultado.Should().BeOfType<CreatedAtActionResult>().Subject;
            var tarefa = createdResult.Value.Should().BeOfType<TarefaResponseDto>().Subject;
            tarefa.Titulo.Should().Be(requestDto.Titulo);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarOk_QuandoTarefaExistir()
        {
            // Arrange
            var tarefaId = 1;
            var requestDto = new TarefaRequestDto
            {
                Titulo = "Tarefa Atualizada",
                Descricao = "Descrição atualizada",
                Status = EnumStatusTarefa.Finalizada
            };

            var responseDto = new TarefaResponseDto
            {
                Id = tarefaId,
                Titulo = requestDto.Titulo,
                Descricao = requestDto.Descricao,
                Status = requestDto.Status,
                DataCriacao = "31/01/2025 22:00",
                DataAtualizacao = "31/01/2025 22:30"
            };

            _mockService.Setup(s => s.AtualizarAsync(tarefaId, requestDto))
                       .ReturnsAsync(responseDto);

            // Act
            var resultado = await _controller.Atualizar(tarefaId, requestDto);

            // Assert
            var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            var tarefa = okResult.Value.Should().BeOfType<TarefaResponseDto>().Subject;
            tarefa.Status.Should().Be(EnumStatusTarefa.Finalizada);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarNotFound_QuandoTarefaNaoExistir()
        {
            // Arrange
            var tarefaId = 999;
            var requestDto = new TarefaRequestDto
            {
                Titulo = "Tarefa Inexistente",
                Descricao = "Descrição",
                Status = EnumStatusTarefa.Pendente
            };

            _mockService.Setup(s => s.AtualizarAsync(tarefaId, requestDto))
                       .ReturnsAsync((TarefaResponseDto?)null);

            // Act
            var resultado = await _controller.Atualizar(tarefaId, requestDto);

            // Assert
            resultado.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Deletar_DeveRetornarNoContent_QuandoTarefaForRemovidaComSucesso()
        {
            // Arrange
            var tarefaId = 1;
            _mockService.Setup(s => s.RemoverAsync(tarefaId))
                       .ReturnsAsync(true);

            // Act
            var resultado = await _controller.Deletar(tarefaId);

            // Assert
            resultado.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Deletar_DeveRetornarNotFound_QuandoTarefaNaoExistir()
        {
            // Arrange
            var tarefaId = 999;
            _mockService.Setup(s => s.RemoverAsync(tarefaId))
                       .ReturnsAsync(false);

            // Act
            var resultado = await _controller.Deletar(tarefaId);

            // Assert
            resultado.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeTarefas()
        {
            // Arrange
            var tarefas = new List<TarefaResponseDto>
            {
                new() { Id = 1, Titulo = "Tarefa 1", Status = EnumStatusTarefa.Pendente, DataCriacao = "31/01/2025 22:00" },
                new() { Id = 2, Titulo = "Tarefa 2", Status = EnumStatusTarefa.Finalizada, DataCriacao = "31/01/2025 22:00" }
            };

            _mockService.Setup(s => s.BuscarTodosAsync())
                       .ReturnsAsync(tarefas);

            // Act
            var resultado = await _controller.ObterTodos();

            // Assert
            var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            var lista = okResult.Value.Should().BeAssignableTo<IEnumerable<TarefaResponseDto>>().Subject;
            lista.Should().HaveCount(2);
        }

        [Fact]
        public async Task ObterPorStatus_DeveRetornarOkComTarefasDoStatusCorreto()
        {
            // Arrange
            var status = EnumStatusTarefa.Pendente;
            var tarefas = new List<TarefaResponseDto>
            {
                new() { Id = 1, Titulo = "Tarefa Pendente 1", Status = EnumStatusTarefa.Pendente, DataCriacao = "31/01/2025 22:00" },
                new() { Id = 2, Titulo = "Tarefa Pendente 2", Status = EnumStatusTarefa.Pendente, DataCriacao = "31/01/2025 22:00" }
            };

            _mockService.Setup(s => s.BuscarPorStatusAsync(status))
                       .ReturnsAsync(tarefas);

            // Act
            var resultado = await _controller.ObterPorStatus(status);

            // Assert
            var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            var lista = okResult.Value.Should().BeAssignableTo<IEnumerable<TarefaResponseDto>>().Subject;
            lista.Should().OnlyContain(t => t.Status == EnumStatusTarefa.Pendente);
        }
    }
}
