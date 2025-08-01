using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Repositories
{
    public interface ITarefaRepository
    {
        Task<IEnumerable<Tarefa>> GetAllAsync();
        Task<Tarefa?> GetByIdAsync(int id);
        Task<IEnumerable<Tarefa>> GetByTituloAsync(string titulo);
        Task<IEnumerable<Tarefa>> GetByDataAsync(DateTime data);
        Task<IEnumerable<Tarefa>> GetByStatusAsync(EnumStatusTarefa status);
        Task<Tarefa> CreateAsync(Tarefa tarefa);
        Task<Tarefa> UpdateAsync(Tarefa tarefa);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
