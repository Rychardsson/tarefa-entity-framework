using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Repositories
{
    /// <summary>
    /// Repositório para operações de dados das tarefas
    /// </summary>
    public class TarefaRepository : ITarefaRepository
    {
        private readonly OrganizadorContext _context;
        private readonly ILogger<TarefaRepository> _logger;

        public TarefaRepository(OrganizadorContext context, ILogger<TarefaRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Tarefa>> GetAllAsync()
        {
            try
            {
                _logger.LogDebug("Buscando todas as tarefas ativas no repositório");
                
                return await _context.Tarefas
                    .Where(t => !t.IsDeleted)
                    .OrderByDescending(t => t.Prioridade)
                    .ThenBy(t => t.Data)
                    .ThenByDescending(t => t.DataCriacao)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as tarefas no repositório");
                throw;
            }
        }

        public async Task<Tarefa?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Buscando tarefa com ID: {Id} no repositório", id);
                
                return await _context.Tarefas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefa com ID: {Id} no repositório", id);
                throw;
            }
        }

        public async Task<IEnumerable<Tarefa>> GetByTituloAsync(string titulo)
        {
            try
            {
                _logger.LogDebug("Buscando tarefas com título: {Titulo} no repositório", titulo);
                
                return await _context.Tarefas
                    .Where(t => EF.Functions.Like(t.Titulo.ToLower(), $"%{titulo.ToLower()}%") && !t.IsDeleted)
                    .OrderByDescending(t => t.Prioridade)
                    .ThenBy(t => t.Data)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por título: {Titulo} no repositório", titulo);
                throw;
            }
        }

        public async Task<IEnumerable<Tarefa>> GetByDataAsync(DateTime data)
        {
            try
            {
                _logger.LogDebug("Buscando tarefas para data: {Data:yyyy-MM-dd} no repositório", data);
                
                return await _context.Tarefas
                    .Where(t => t.Data.Date == data.Date && !t.IsDeleted)
                    .OrderByDescending(t => t.Prioridade)
                    .ThenByDescending(t => t.DataCriacao)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por data: {Data:yyyy-MM-dd} no repositório", data);
                throw;
            }
        }

        public async Task<IEnumerable<Tarefa>> GetByStatusAsync(EnumStatusTarefa status)
        {
            try
            {
                _logger.LogDebug("Buscando tarefas com status: {Status} no repositório", status);
                
                return await _context.Tarefas
                    .Where(t => t.Status == status && !t.IsDeleted)
                    .OrderByDescending(t => t.Prioridade)
                    .ThenBy(t => t.Data)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por status: {Status} no repositório", status);
                throw;
            }
        }

        public async Task<IEnumerable<Tarefa>> GetByPrioridadeAsync(int prioridade)
        {
            try
            {
                _logger.LogDebug("Buscando tarefas com prioridade: {Prioridade} no repositório", prioridade);
                
                return await _context.Tarefas
                    .Where(t => t.Prioridade == prioridade && !t.IsDeleted)
                    .OrderBy(t => t.Data)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas por prioridade: {Prioridade} no repositório", prioridade);
                throw;
            }
        }

        public async Task<IEnumerable<Tarefa>> GetTarefasAtrasadasAsync()
        {
            try
            {
                _logger.LogDebug("Buscando tarefas atrasadas no repositório");
                
                var hoje = DateTime.UtcNow.Date;
                return await _context.Tarefas
                    .Where(t => t.Data.Date < hoje && t.Status != EnumStatusTarefa.Finalizado && !t.IsDeleted)
                    .OrderByDescending(t => t.Prioridade)
                    .ThenBy(t => t.Data)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tarefas atrasadas no repositório");
                throw;
            }
        }

        public async Task<Tarefa> CreateAsync(Tarefa tarefa)
        {
            try
            {
                if (tarefa == null)
                    throw new ArgumentNullException(nameof(tarefa));
                
                _logger.LogDebug("Criando nova tarefa no repositório: {Titulo}", tarefa.Titulo);
                
                tarefa.DataCriacao = DateTime.UtcNow;
                tarefa.DataAtualizacao = null;
                
                await _context.Tarefas.AddAsync(tarefa);
                await _context.SaveChangesAsync();
                
                _logger.LogDebug("Tarefa criada com sucesso. ID: {Id}", tarefa.Id);
                return tarefa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tarefa no repositório: {Titulo}", tarefa?.Titulo);
                throw;
            }
        }

        public async Task<Tarefa> UpdateAsync(Tarefa tarefa)
        {
            try
            {
                if (tarefa == null)
                    throw new ArgumentNullException(nameof(tarefa));
                
                _logger.LogDebug("Atualizando tarefa no repositório. ID: {Id}", tarefa.Id);
                
                tarefa.DataAtualizacao = DateTime.UtcNow;
                
                _context.Entry(tarefa).State = EntityState.Modified;
                _context.Entry(tarefa).Property(x => x.DataCriacao).IsModified = false;
                
                await _context.SaveChangesAsync();
                
                _logger.LogDebug("Tarefa atualizada com sucesso. ID: {Id}", tarefa.Id);
                return tarefa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa no repositório. ID: {Id}", tarefa?.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogDebug("Deletando tarefa no repositório. ID: {Id}", id);
                
                var tarefa = await _context.Tarefas
                    .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
                
                if (tarefa == null)
                {
                    _logger.LogWarning("Tentativa de deletar tarefa inexistente. ID: {Id}", id);
                    return false;
                }

                // Soft delete para manter histórico
                tarefa.IsDeleted = true;
                tarefa.DataAtualizacao = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                
                _logger.LogDebug("Tarefa deletada com sucesso. ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar tarefa no repositório. ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Tarefas
                    .AsNoTracking()
                    .AnyAsync(t => t.Id == id && !t.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da tarefa. ID: {Id}", id);
                throw;
            }
        }

        public async Task<int> CountAsync()
        {
            try
            {
                return await _context.Tarefas
                    .AsNoTracking()
                    .CountAsync(t => !t.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar tarefas no repositório");
                throw;
            }
        }

        public async Task<int> CountByStatusAsync(EnumStatusTarefa status)
        {
            try
            {
                return await _context.Tarefas
                    .AsNoTracking()
                    .CountAsync(t => t.Status == status && !t.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar tarefas por status: {Status}", status);
                throw;
            }
        }
    }
}
