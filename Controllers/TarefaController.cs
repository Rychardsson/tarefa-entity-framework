using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;
        private readonly ILogger<TarefaController> _logger;

        public TarefaController(ITarefaService tarefaService, ILogger<TarefaController> logger)
        {
            _tarefaService = tarefaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém uma tarefa específica por ID
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Tarefa encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var tarefa = await _tarefaService.GetByIdAsync(id);
            
            if (tarefa == null)
                return NotFound($"Tarefa com ID {id} não encontrada");
                
            return Ok(tarefa);
        }

        /// <summary>
        /// Obtém todas as tarefas
        /// </summary>
        /// <returns>Lista de todas as tarefas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodos()
        {
            var tarefas = await _tarefaService.GetAllAsync();
            return Ok(tarefas);
        }

        /// <summary>
        /// Obtém tarefas que contenham o título especificado
        /// </summary>
        /// <param name="titulo">Título a ser pesquisado</param>
        /// <returns>Lista de tarefas que contenham o título</returns>
        [HttpGet("titulo/{titulo}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterPorTitulo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return BadRequest("O título não pode ser vazio");
                
            var tarefas = await _tarefaService.GetByTituloAsync(titulo);
            return Ok(tarefas);
        }

        /// <summary>
        /// Obtém tarefas por data
        /// </summary>
        /// <param name="data">Data das tarefas</param>
        /// <returns>Lista de tarefas da data especificada</returns>
        [HttpGet("data/{data}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorData(DateTime data)
        {
            var tarefas = await _tarefaService.GetByDataAsync(data);
            return Ok(tarefas);
        }

        /// <summary>
        /// Obtém tarefas por status
        /// </summary>
        /// <param name="status">Status das tarefas (0 = Pendente, 1 = Finalizado)</param>
        /// <returns>Lista de tarefas com o status especificado</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefas = await _tarefaService.GetByStatusAsync(status);
            return Ok(tarefas);
        }

        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        /// <param name="tarefaDto">Dados da tarefa a ser criada</param>
        /// <returns>Tarefa criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Criar([FromBody] TarefaRequestDto tarefaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tarefaCriada = await _tarefaService.CreateAsync(tarefaDto);
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefaCriada.Id }, tarefaCriada);
        }

        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa a ser atualizada</param>
        /// <param name="tarefaDto">Novos dados da tarefa</param>
        /// <returns>Tarefa atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] TarefaRequestDto tarefaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tarefaAtualizada = await _tarefaService.UpdateAsync(id, tarefaDto);
            
            if (tarefaAtualizada == null)
                return NotFound($"Tarefa com ID {id} não encontrada");
            
            return Ok(tarefaAtualizada);
        }

        /// <summary>
        /// Remove uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa a ser removida</param>
        /// <returns>Confirmação da remoção</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar(int id)
        {
            var deleted = await _tarefaService.DeleteAsync(id);
            
            if (!deleted)
                return NotFound($"Tarefa com ID {id} não encontrada");
            
            return NoContent();
        }
    }
}
