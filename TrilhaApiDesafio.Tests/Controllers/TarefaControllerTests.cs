using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrilhaApiDesafio.Controllers;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Tests.Controllers
{
    public class TarefaControllerTests
    {
        private readonly Mock<ITarefaService> _serviceMock;
        private readonly Mock<ILogger<TarefaController>> _loggerMock;
        private readonly TarefaController _controller;

        public TarefaControllerTests()
        {
            _serviceMock = new Mock<ITarefaService>();
            _loggerMock = new Mock<ILogger<TarefaController>>();
            _controller = new TarefaController(_serviceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ObterPorId_ComIdValido_DeveRetornarOkComTarefa()
        {
            // Arrange
            var tarefaResponse = new TarefaResponseDto
            {
                Id = 1,
                Titulo = "Tarefa Teste",
                Status = EnumStatusTarefa.Pendente,
                DataCriacao = DateTime.Now
            };

            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(tarefaResponse);

            // Act
            var result = await _controller.ObterPorId(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tarefa = okResult.Value.Should().BeOfType<TarefaResponseDto>().Subject;
            tarefa.Id.Should().Be(1);
            tarefa.Titulo.Should().Be("Tarefa Teste");
        }

        [Fact]
        public async Task ObterPorId_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((TarefaResponseDto?)null);

            // Act
            var result = await _controller.ObterPorId(999);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Tarefa com ID 999 não encontrada");
        }

        [Fact]
        public async Task ObterTodos_DeveRetornarOkComListaDeTarefas()
        {
            // Arrange
            var tarefas = new List<TarefaResponseDto>
            {
                new TarefaResponseDto { Id = 1, Titulo = "Tarefa 1", Status = EnumStatusTarefa.Pendente, DataCriacao = DateTime.Now },
                new TarefaResponseDto { Id = 2, Titulo = "Tarefa 2", Status = EnumStatusTarefa.Finalizado, DataCriacao = DateTime.Now }
            };

            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(tarefas);

            // Act
            var result = await _controller.ObterTodos();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var listaTarefas = okResult.Value.Should().BeAssignableTo<IEnumerable<TarefaResponseDto>>().Subject;
            listaTarefas.Should().HaveCount(2);
        }

        [Fact]
        public async Task ObterPorTitulo_ComTituloValido_DeveRetornarOkComTarefas()
        {
            // Arrange
            var tarefas = new List<TarefaResponseDto>
            {
                new TarefaResponseDto { Id = 1, Titulo = "Estudar .NET", Status = EnumStatusTarefa.Pendente, DataCriacao = DateTime.Now }
            };

            _serviceMock.Setup(s => s.GetByTituloAsync("Estudar")).ReturnsAsync(tarefas);

            // Act
            var result = await _controller.ObterPorTitulo("Estudar");

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var listaTarefas = okResult.Value.Should().BeAssignableTo<IEnumerable<TarefaResponseDto>>().Subject;
            listaTarefas.Should().HaveCount(1);
        }

        [Fact]
        public async Task ObterPorTitulo_ComTituloVazio_DeveRetornarBadRequest()
        {
            // Act
            var result = await _controller.ObterPorTitulo("");

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("O título não pode ser vazio");
        }

        [Fact]
        public async Task Criar_ComDadosValidos_DeveRetornarCreated()
        {
            // Arrange
            var tarefaRequest = new TarefaRequestDto
            {
                Titulo = "Nova Tarefa",
                Descricao = "Descrição",
                Data = DateTime.Now.AddDays(1),
                Status = EnumStatusTarefa.Pendente
            };

            var tarefaResponse = new TarefaResponseDto
            {
                Id = 1,
                Titulo = tarefaRequest.Titulo,
                Descricao = tarefaRequest.Descricao,
                Data = tarefaRequest.Data,
                Status = tarefaRequest.Status,
                DataCriacao = DateTime.Now
            };

            _serviceMock.Setup(s => s.CreateAsync(tarefaRequest)).ReturnsAsync(tarefaResponse);

            // Act
            var result = await _controller.Criar(tarefaRequest);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.ObterPorId));
            createdResult.RouteValues!["id"].Should().Be(1);
            
            var tarefa = createdResult.Value.Should().BeOfType<TarefaResponseDto>().Subject;
            tarefa.Titulo.Should().Be("Nova Tarefa");
        }

        [Fact]
        public async Task Atualizar_ComIdValido_DeveRetornarOkComTarefaAtualizada()
        {
            // Arrange
            var tarefaRequest = new TarefaRequestDto
            {
                Titulo = "Tarefa Atualizada",
                Data = DateTime.Now,
                Status = EnumStatusTarefa.Finalizado
            };

            var tarefaResponse = new TarefaResponseDto
            {
                Id = 1,
                Titulo = tarefaRequest.Titulo,
                Data = tarefaRequest.Data,
                Status = tarefaRequest.Status,
                DataCriacao = DateTime.Now
            };

            _serviceMock.Setup(s => s.UpdateAsync(1, tarefaRequest)).ReturnsAsync(tarefaResponse);

            // Act
            var result = await _controller.Atualizar(1, tarefaRequest);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var tarefa = okResult.Value.Should().BeOfType<TarefaResponseDto>().Subject;
            tarefa.Titulo.Should().Be("Tarefa Atualizada");
            tarefa.Status.Should().Be(EnumStatusTarefa.Finalizado);
        }

        [Fact]
        public async Task Atualizar_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var tarefaRequest = new TarefaRequestDto
            {
                Titulo = "Tarefa Atualizada",
                Data = DateTime.Now,
                Status = EnumStatusTarefa.Finalizado
            };

            _serviceMock.Setup(s => s.UpdateAsync(999, tarefaRequest)).ReturnsAsync((TarefaResponseDto?)null);

            // Act
            var result = await _controller.Atualizar(999, tarefaRequest);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Tarefa com ID 999 não encontrada");
        }

        [Fact]
        public async Task Deletar_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Deletar(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Deletar_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            _serviceMock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.Deletar(999);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Tarefa com ID 999 não encontrada");
        }

        [Fact]
        public async Task ObterPorStatus_DeveRetornarOkComTarefasDoStatus()
        {
            // Arrange
            var tarefas = new List<TarefaResponseDto>
            {
                new TarefaResponseDto { Id = 1, Titulo = "Tarefa 1", Status = EnumStatusTarefa.Pendente, DataCriacao = DateTime.Now }
            };

            _serviceMock.Setup(s => s.GetByStatusAsync(EnumStatusTarefa.Pendente)).ReturnsAsync(tarefas);

            // Act
            var result = await _controller.ObterPorStatus(EnumStatusTarefa.Pendente);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var listaTarefas = okResult.Value.Should().BeAssignableTo<IEnumerable<TarefaResponseDto>>().Subject;
            listaTarefas.Should().HaveCount(1);
            listaTarefas.First().Status.Should().Be(EnumStatusTarefa.Pendente);
        }
    }
}
