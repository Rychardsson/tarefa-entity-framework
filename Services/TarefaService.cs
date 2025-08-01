using AutoMapper;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Repositories;

namespace TrilhaApiDesafio.Services
{
    /// <summary>
    /// Serviço responsável pela lógica de negócio das tarefas
    /// </summary>
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<TarefaService> _logger;

        public TarefaService(ITarefaRepository repository, IMapper mapper, ILogger<TarefaService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Iniciando busca por todas as tarefas");
                var tarefas = await _repository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
                _logger.LogInformation("Busca concluída. {Count} tarefas encontradas", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as tarefas");
                throw;
            }
        }

        public async Task<TarefaResponseDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando tarefa com ID: {Id}", id);
                
                if (id <= 0)
                {
                    _logger.LogWarning("ID inválido fornecido: {Id}", id);
                    return null;
                }
                
                var tarefa = await _repository.GetByIdAsync(id);
                
                if (tarefa == null)
                {
                    _logger.LogWarning("Tarefa com ID {Id} não encontrada", id);
                    return null;
                }
                
                var result = _mapper.Map<TarefaResponseDto>(tarefa);
                _logger.LogInformation("Tarefa com ID {Id} encontrada com sucesso", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa com ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByTituloAsync(string titulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    _logger.LogWarning("Título inválido fornecido para busca");
                    return Enumerable.Empty<TarefaResponseDto>();
                }
                
                _logger.LogInformation("Buscando tarefas com título: {Titulo}", titulo);
                var tarefas = await _repository.GetByTituloAsync(titulo);
                var result = _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
                _logger.LogInformation("Busca por título concluída. {Count} tarefas encontradas", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por título: {Titulo}", titulo);
                throw;
            }
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByDataAsync(DateTime data)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas para a data: {Data:yyyy-MM-dd}", data.Date);
                var tarefas = await _repository.GetByDataAsync(data);
                var result = _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
                _logger.LogInformation("Busca por data concluída. {Count} tarefas encontradas", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por data: {Data:yyyy-MM-dd}", data);
                throw;
            }
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByStatusAsync(EnumStatusTarefa status)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas com status: {Status}", status);
                var tarefas = await _repository.GetByStatusAsync(status);
                var result = _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
                _logger.LogInformation("Busca por status concluída. {Count} tarefas encontradas", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por status: {Status}", status);
                throw;
            }
        }

        public async Task<TarefaResponseDto> CreateAsync(TarefaRequestDto tarefaDto)
        {
            try
            {
                if (tarefaDto == null)
                    throw new ArgumentNullException(nameof(tarefaDto));
                
                _logger.LogInformation("Criando nova tarefa: {Titulo}", tarefaDto.Titulo);
                
                var tarefa = _mapper.Map<Tarefa>(tarefaDto);
                tarefa.DataCriacao = DateTime.UtcNow;
                
                var tarefaCriada = await _repository.CreateAsync(tarefa);
                var result = _mapper.Map<TarefaResponseDto>(tarefaCriada);
                
                _logger.LogInformation("Tarefa criada com sucesso. ID: {Id}", tarefaCriada.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa: {Titulo}", tarefaDto?.Titulo);
                throw;
            }
        }

        public async Task<TarefaResponseDto?> UpdateAsync(int id, TarefaRequestDto tarefaDto)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID inválido fornecido para atualização: {Id}", id);
                    return null;
                }
                
                if (tarefaDto == null)
                    throw new ArgumentNullException(nameof(tarefaDto));
                
                _logger.LogInformation("Atualizando tarefa com ID: {Id}", id);
                
                var tarefaExistente = await _repository.GetByIdAsync(id);
                
                if (tarefaExistente == null)
                {
                    _logger.LogWarning("Tentativa de atualização de tarefa inexistente. ID: {Id}", id);
                    return null;
                }

                _mapper.Map(tarefaDto, tarefaExistente);
                tarefaExistente.MarcarComoAtualizada();
                
                var tarefaAtualizada = await _repository.UpdateAsync(tarefaExistente);
                var result = _mapper.Map<TarefaResponseDto>(tarefaAtualizada);
                
                _logger.LogInformation("Tarefa atualizada com sucesso. ID: {Id}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa com ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByPrioridadeAsync(int prioridade)
        {
            try
            {
                _logger.LogInformation("Buscando tarefas com prioridade: {Prioridade}", prioridade);
                var tarefas = await _repository.GetByPrioridadeAsync(prioridade);
                var result = _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
                _logger.LogInformation("Busca por prioridade concluída. {Count} tarefas encontradas", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por prioridade: {Prioridade}", prioridade);
                throw;
            }
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetTarefasAtrasadasAsync()
        {
            try
            {
                _logger.LogInformation("Buscando tarefas atrasadas");
                var tarefas = await _repository.GetTarefasAtrasadasAsync();
                var result = _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
                _logger.LogInformation("Busca por tarefas atrasadas concluída. {Count} tarefas encontradas", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas atrasadas");
                throw;
            }
        }

        public async Task<TarefaEstatisticasDto> GetEstatisticasAsync()
        {
            try
            {
                _logger.LogInformation("Calculando estatísticas das tarefas");
                
                var totalTarefas = await _repository.CountAsync();
                var tarefasPendentes = await _repository.CountByStatusAsync(EnumStatusTarefa.Pendente);
                var tarefasFinalizadas = await _repository.CountByStatusAsync(EnumStatusTarefa.Finalizado);
                var tarefasAtrasadas = await _repository.GetTarefasAtrasadasAsync();
                var tarefasHoje = await _repository.GetByDataAsync(DateTime.UtcNow);

                var estatisticas = new TarefaEstatisticasDto
                {
                    TotalTarefas = totalTarefas,
                    TarefasPendentes = tarefasPendentes,
                    TarefasFinalizadas = tarefasFinalizadas,
                    TarefasAtrasadas = tarefasAtrasadas.Count(),
                    TarefasHoje = tarefasHoje.Count(),
                    PercentualConclusao = totalTarefas > 0 ? (decimal)tarefasFinalizadas / totalTarefas * 100 : 0
                };

                // Calcular distribuição por prioridade
                for (int i = 1; i <= 4; i++)
                {
                    var count = (await _repository.GetByPrioridadeAsync(i)).Count();
                    estatisticas.DistribuicaoPorPrioridade[i] = count;
                }

                _logger.LogInformation("Estatísticas calculadas com sucesso");
                return estatisticas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular estatísticas das tarefas");
                throw;
            }
        }

        public async Task<TarefaResponseDto?> FinalizarTarefaAsync(int id)
        {
            try
            {
                _logger.LogInformation("Finalizando tarefa. ID: {Id}", id);
                
                var tarefa = await _repository.GetByIdAsync(id);
                if (tarefa == null)
                {
                    _logger.LogWarning("Tentativa de finalizar tarefa inexistente. ID: {Id}", id);
                    return null;
                }

                tarefa.Status = EnumStatusTarefa.Finalizado;
                tarefa.MarcarComoAtualizada();
                
                var tarefaAtualizada = await _repository.UpdateAsync(tarefa);
                var result = _mapper.Map<TarefaResponseDto>(tarefaAtualizada);
                
                _logger.LogInformation("Tarefa finalizada com sucesso. ID: {Id}", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar tarefa. ID: {Id}", id);
                throw;
            }
        }

        public Task<bool> ValidarTarefaAsync(TarefaRequestDto tarefaDto)
        {
            try
            {
                if (tarefaDto == null)
                    return Task.FromResult(false);

                // Validações de negócio
                if (string.IsNullOrWhiteSpace(tarefaDto.Titulo))
                    return Task.FromResult(false);

                if (tarefaDto.Data.Date < DateTime.UtcNow.Date)
                {
                    _logger.LogWarning("Tentativa de criar tarefa com data no passado: {Data}", tarefaDto.Data);
                    return Task.FromResult(false);
                }

                if (tarefaDto.Prioridade < 1 || tarefaDto.Prioridade > 4)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar tarefa");
                return Task.FromResult(false);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("ID inválido fornecido para deleção: {Id}", id);
                    return false;
                }
                
                _logger.LogInformation("Deletando tarefa com ID: {Id}", id);
                
                var resultado = await _repository.DeleteAsync(id);
                
                if (resultado)
                {
                    _logger.LogInformation("Tarefa deletada com sucesso. ID: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Tentativa de deleção de tarefa inexistente. ID: {Id}", id);
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar tarefa com ID: {Id}", id);
                throw;
            }
        }
    }
}
