using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Services
{
    public interface ITarefaService
    {
        Task<IEnumerable<TarefaResponseDto>> GetAllAsync();
        Task<TarefaResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<TarefaResponseDto>> GetByTituloAsync(string titulo);
        Task<IEnumerable<TarefaResponseDto>> GetByDataAsync(DateTime data);
        Task<IEnumerable<TarefaResponseDto>> GetByStatusAsync(EnumStatusTarefa status);
        Task<TarefaResponseDto> CreateAsync(TarefaRequestDto tarefaDto);
        Task<TarefaResponseDto?> UpdateAsync(int id, TarefaRequestDto tarefaDto);
        Task<bool> DeleteAsync(int id);
    }
}
