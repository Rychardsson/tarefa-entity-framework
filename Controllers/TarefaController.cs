using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Services;

namespace TrilhaApiDesafio.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de tarefas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;
        private readonly ILogger<TarefaController> _logger;

        public TarefaController(ITarefaService tarefaService, ILogger<TarefaController> logger)
        {
            _tarefaService = tarefaService ?? throw new ArgumentNullException(nameof(tarefaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém uma tarefa específica por ID
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Tarefa encontrada</returns>
        /// <response code="200">Tarefa encontrada com sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TarefaResponseDto>> ObterPorIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando tarefa com ID: {TarefaId}", id);
                
                var tarefa = await _tarefaService.GetByIdAsync(id);
                
                if (tarefa == null)
                {
                    _logger.LogWarning("Tarefa com ID {TarefaId} não encontrada", id);
                    return NotFound($"Tarefa com ID {id} não encontrada");
                }
                
                _logger.LogInformation("Tarefa com ID {TarefaId} encontrada com sucesso", id);
                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa com ID: {TarefaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefa");
            }
        }

        /// <summary>
        /// Obtém todas as tarefas do usuário autenticado
        /// </summary>
        /// <returns>Lista de todas as tarefas</returns>
        /// <response code="200">Lista de tarefas retornada com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> ObterTodosAsync()
        {
            try
            {
                _logger.LogInformation("Buscando todas as tarefas");
                
                var tarefas = await _tarefaService.GetAllAsync();
                
                _logger.LogInformation("Retornadas {Count} tarefas", tarefas.Count());
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as tarefas");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefas");
            }
        }

        /// <summary>
        /// Obtém tarefas que contenham o título especificado
        /// </summary>
        /// <param name="titulo">Título a ser pesquisado</param>
        /// <returns>Lista de tarefas que contenham o título</returns>
        /// <response code="200">Lista de tarefas retornada com sucesso</response>
        /// <response code="400">Parâmetro inválido</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("titulo/{titulo}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> ObterPorTituloAsync(string titulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    _logger.LogWarning("Tentativa de busca com título vazio");
                    return BadRequest("O título não pode ser vazio");
                }
                
                _logger.LogInformation("Buscando tarefas com título: {Titulo}", titulo);
                
                var tarefas = await _tarefaService.GetByTituloAsync(titulo);
                
                _logger.LogInformation("Encontradas {Count} tarefas com título: {Titulo}", 
                    tarefas.Count(), titulo);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por título: {Titulo}", titulo);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefas por título");
            }
        }

        /// <summary>
        /// Obtém tarefas por data
        /// </summary>
        /// <param name="data">Data das tarefas (formato: yyyy-MM-dd)</param>
        /// <returns>Lista de tarefas da data especificada</returns>
        /// <response code="200">Lista de tarefas retornada com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("data/{data:datetime}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> ObterPorDataAsync(DateTime data)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas para data: {Data:yyyy-MM-dd}", data);
                
                var tarefas = await _tarefaService.GetByDataAsync(data);
                
                _logger.LogInformation("Encontradas {Count} tarefas para data: {Data:yyyy-MM-dd}", 
                    tarefas.Count(), data);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por data: {Data:yyyy-MM-dd}", data);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefas por data");
            }
        }

        /// <summary>
        /// Obtém tarefas por status
        /// </summary>
        /// <param name="status">Status das tarefas (0 = Pendente, 1 = Finalizado)</param>
        /// <returns>Lista de tarefas com o status especificado</returns>
        /// <response code="200">Lista de tarefas retornada com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("status/{status:int}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> ObterPorStatusAsync(EnumStatusTarefa status)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas com status: {Status}", status);
                
                var tarefas = await _tarefaService.GetByStatusAsync(status);
                
                _logger.LogInformation("Encontradas {Count} tarefas com status: {Status}", 
                    tarefas.Count(), status);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por status: {Status}", status);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefas por status");
            }
        }

        /// <summary>
        /// Obtém tarefas por prioridade
        /// </summary>
        /// <param name="prioridade">Prioridade das tarefas (1-4)</param>
        /// <returns>Lista de tarefas com a prioridade especificada</returns>
        /// <response code="200">Lista de tarefas retornada com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("prioridade/{prioridade:int}")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> ObterPorPrioridadeAsync(int prioridade)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas com prioridade: {Prioridade}", prioridade);
                
                var tarefas = await _tarefaService.GetByPrioridadeAsync(prioridade);
                
                _logger.LogInformation("Encontradas {Count} tarefas com prioridade: {Prioridade}", 
                    tarefas.Count(), prioridade);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por prioridade: {Prioridade}", prioridade);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefas por prioridade");
            }
        }

        /// <summary>
        /// Obtém tarefas atrasadas
        /// </summary>
        /// <returns>Lista de tarefas atrasadas</returns>
        /// <response code="200">Lista de tarefas retornada com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("atrasadas")]
        [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> ObterTarefasAtrasadasAsync()
        {
            try
            {
                _logger.LogInformation("Buscando tarefas atrasadas");
                
                var tarefas = await _tarefaService.GetTarefasAtrasadasAsync();
                
                _logger.LogInformation("Encontradas {Count} tarefas atrasadas", tarefas.Count());
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas atrasadas");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao buscar tarefas atrasadas");
            }
        }

        /// <summary>
        /// Obtém estatísticas das tarefas
        /// </summary>
        /// <returns>Estatísticas das tarefas</returns>
        /// <response code="200">Estatísticas retornadas com sucesso</response>
        /// <response code="401">Não autorizado</response>
        [HttpGet("estatisticas")]
        [ProducesResponseType(typeof(TarefaEstatisticasDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TarefaEstatisticasDto>> ObterEstatisticasAsync()
        {
            try
            {
                _logger.LogInformation("Calculando estatísticas das tarefas");
                
                var estatisticas = await _tarefaService.GetEstatisticasAsync();
                
                _logger.LogInformation("Estatísticas calculadas com sucesso");
                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular estatísticas das tarefas");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao calcular estatísticas");
            }
        }

        /// <summary>
        /// Finaliza uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa a ser finalizada</param>
        /// <returns>Tarefa finalizada</returns>
        /// <response code="200">Tarefa finalizada com sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        /// <response code="401">Não autorizado</response>
        [HttpPatch("{id:int}/finalizar")]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TarefaResponseDto>> FinalizarTarefaAsync(int id)
        {
            try
            {
                _logger.LogInformation("Finalizando tarefa. ID: {TarefaId}", id);
                
                var tarefa = await _tarefaService.FinalizarTarefaAsync(id);
                
                if (tarefa == null)
                {
                    _logger.LogWarning("Tentativa de finalizar tarefa inexistente. ID: {TarefaId}", id);
                    return NotFound($"Tarefa com ID {id} não encontrada");
                }
                
                _logger.LogInformation("Tarefa finalizada com sucesso. ID: {TarefaId}", id);
                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar tarefa. ID: {TarefaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao finalizar tarefa");
            }
        }

        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        /// <param name="tarefaDto">Dados da tarefa a ser criada</param>
        /// <returns>Tarefa criada</returns>
        /// <response code="201">Tarefa criada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="401">Não autorizado</response>
        [HttpPost]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TarefaResponseDto>> CriarAsync([FromBody] TarefaRequestDto tarefaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Tentativa de criação de tarefa com dados inválidos");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Criando nova tarefa: {Titulo}", tarefaDto.Titulo);
                
                var tarefaCriada = await _tarefaService.CreateAsync(tarefaDto);
                
                _logger.LogInformation("Tarefa criada com sucesso. ID: {TarefaId}", tarefaCriada.Id);
                return CreatedAtAction(nameof(ObterPorIdAsync), new { id = tarefaCriada.Id }, tarefaCriada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa: {Titulo}", tarefaDto.Titulo);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao criar tarefa");
            }
        }

        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        /// <param name="id">ID da tarefa a ser atualizada</param>
        /// <param name="tarefaDto">Novos dados da tarefa</param>
        /// <returns>Tarefa atualizada</returns>
        /// <response code="200">Tarefa atualizada com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="404">Tarefa não encontrada</response>
        /// <response code="401">Não autorizado</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TarefaResponseDto>> AtualizarAsync(int id, [FromBody] TarefaRequestDto tarefaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Tentativa de atualização de tarefa com dados inválidos. ID: {TarefaId}", id);
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Atualizando tarefa. ID: {TarefaId}", id);
                
                var tarefaAtualizada = await _tarefaService.UpdateAsync(id, tarefaDto);
                
                if (tarefaAtualizada == null)
                {
                    _logger.LogWarning("Tentativa de atualização de tarefa inexistente. ID: {TarefaId}", id);
                    return NotFound($"Tarefa com ID {id} não encontrada");
                }
                
                _logger.LogInformation("Tarefa atualizada com sucesso. ID: {TarefaId}", id);
                return Ok(tarefaAtualizada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa. ID: {TarefaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao atualizar tarefa");
            }
        }

        /// <summary>
        /// Remove uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa a ser removida</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="204">Tarefa removida com sucesso</response>
        /// <response code="404">Tarefa não encontrada</response>
        /// <response code="401">Não autorizado</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletarAsync(int id)
        {
            try
            {
                _logger.LogInformation("Tentando deletar tarefa. ID: {TarefaId}", id);
                
                var deleted = await _tarefaService.DeleteAsync(id);
                
                if (!deleted)
                {
                    _logger.LogWarning("Tentativa de deleção de tarefa inexistente. ID: {TarefaId}", id);
                    return NotFound($"Tarefa com ID {id} não encontrada");
                }
                
                _logger.LogInformation("Tarefa deletada com sucesso. ID: {TarefaId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar tarefa. ID: {TarefaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    "Erro interno do servidor ao deletar tarefa");
            }
        }
    }
}
