using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Services
{
    /// <summary>
    /// Interface para serviços de lógica de negócio das tarefas
    /// </summary>
    public interface ITarefaService
    {
        /// <summary>
        /// Obtém todas as tarefas
        /// </summary>
        Task<IEnumerable<TarefaResponseDto>> GetAllAsync();
        
        /// <summary>
        /// Obtém uma tarefa por ID
        /// </summary>
        Task<TarefaResponseDto?> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtém tarefas que contenham o título especificado
        /// </summary>
        Task<IEnumerable<TarefaResponseDto>> GetByTituloAsync(string titulo);
        
        /// <summary>
        /// Obtém tarefas por data
        /// </summary>
        Task<IEnumerable<TarefaResponseDto>> GetByDataAsync(DateTime data);
        
        /// <summary>
        /// Obtém tarefas por status
        /// </summary>
        Task<IEnumerable<TarefaResponseDto>> GetByStatusAsync(EnumStatusTarefa status);
        
        /// <summary>
        /// Obtém tarefas por prioridade
        /// </summary>
        Task<IEnumerable<TarefaResponseDto>> GetByPrioridadeAsync(int prioridade);
        
        /// <summary>
        /// Obtém tarefas atrasadas
        /// </summary>
        Task<IEnumerable<TarefaResponseDto>> GetTarefasAtrasadasAsync();
        
        /// <summary>
        /// Obtém estatísticas das tarefas
        /// </summary>
        Task<TarefaEstatisticasDto> GetEstatisticasAsync();
        
        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        Task<TarefaResponseDto> CreateAsync(TarefaRequestDto tarefaDto);
        
        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        Task<TarefaResponseDto?> UpdateAsync(int id, TarefaRequestDto tarefaDto);
        
        /// <summary>
        /// Remove uma tarefa
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Marca uma tarefa como finalizada
        /// </summary>
        Task<TarefaResponseDto?> FinalizarTarefaAsync(int id);
        
        /// <summary>
        /// Valida se uma tarefa pode ser criada/atualizada
        /// </summary>
        Task<bool> ValidarTarefaAsync(TarefaRequestDto tarefaDto);
    }
}
