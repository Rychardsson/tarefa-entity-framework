using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly OrganizadorContext _context;

        public TarefaRepository(OrganizadorContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tarefa>> GetAllAsync()
        {
            return await _context.Tarefas
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<Tarefa?> GetByIdAsync(int id)
        {
            return await _context.Tarefas
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<IEnumerable<Tarefa>> GetByTituloAsync(string titulo)
        {
            return await _context.Tarefas
                .Where(t => t.Titulo.Contains(titulo) && !t.IsDeleted)
                .OrderBy(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> GetByDataAsync(DateTime data)
        {
            return await _context.Tarefas
                .Where(t => t.Data.Date == data.Date && !t.IsDeleted)
                .OrderBy(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> GetByStatusAsync(EnumStatusTarefa status)
        {
            return await _context.Tarefas
                .Where(t => t.Status == status && !t.IsDeleted)
                .OrderBy(t => t.DataCriacao)
                .ToListAsync();
        }

        public async Task<Tarefa> CreateAsync(Tarefa tarefa)
        {
            tarefa.DataCriacao = DateTime.UtcNow;
            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();
            return tarefa;
        }

        public async Task<Tarefa> UpdateAsync(Tarefa tarefa)
        {
            tarefa.DataAtualizacao = DateTime.UtcNow;
            _context.Tarefas.Update(tarefa);
            await _context.SaveChangesAsync();
            return tarefa;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tarefa = await GetByIdAsync(id);
            if (tarefa == null)
                return false;

            // Soft delete
            tarefa.IsDeleted = true;
            tarefa.DataAtualizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tarefas
                .AnyAsync(t => t.Id == id && !t.IsDeleted);
        }
    }
}
