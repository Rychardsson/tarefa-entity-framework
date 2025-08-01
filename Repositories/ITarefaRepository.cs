using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Repositories
{
    /// <summary>
    /// Interface para operações de repositório das tarefas
    /// </summary>
    public interface ITarefaRepository
    {
        /// <summary>
        /// Obtém todas as tarefas ativas
        /// </summary>
        Task<IEnumerable<Tarefa>> GetAllAsync();
        
        /// <summary>
        /// Obtém uma tarefa por ID
        /// </summary>
        Task<Tarefa?> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtém tarefas que contenham o título especificado
        /// </summary>
        Task<IEnumerable<Tarefa>> GetByTituloAsync(string titulo);
        
        /// <summary>
        /// Obtém tarefas por data
        /// </summary>
        Task<IEnumerable<Tarefa>> GetByDataAsync(DateTime data);
        
        /// <summary>
        /// Obtém tarefas por status
        /// </summary>
        Task<IEnumerable<Tarefa>> GetByStatusAsync(EnumStatusTarefa status);
        
        /// <summary>
        /// Obtém tarefas por prioridade
        /// </summary>
        Task<IEnumerable<Tarefa>> GetByPrioridadeAsync(int prioridade);
        
        /// <summary>
        /// Obtém tarefas atrasadas
        /// </summary>
        Task<IEnumerable<Tarefa>> GetTarefasAtrasadasAsync();
        
        /// <summary>
        /// Cria uma nova tarefa
        /// </summary>
        Task<Tarefa> CreateAsync(Tarefa tarefa);
        
        /// <summary>
        /// Atualiza uma tarefa existente
        /// </summary>
        Task<Tarefa> UpdateAsync(Tarefa tarefa);
        
        /// <summary>
        /// Remove uma tarefa (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Verifica se uma tarefa existe
        /// </summary>
        Task<bool> ExistsAsync(int id);
        
        /// <summary>
        /// Conta o total de tarefas ativas
        /// </summary>
        Task<int> CountAsync();
        
        /// <summary>
        /// Conta tarefas por status
        /// </summary>
        Task<int> CountByStatusAsync(EnumStatusTarefa status);
    }
}
