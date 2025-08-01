using AutoMapper;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Repositories;

namespace TrilhaApiDesafio.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<TarefaService> _logger;

        public TarefaService(ITarefaRepository repository, IMapper mapper, ILogger<TarefaService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todas as tarefas");
            var tarefas = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
        }

        public async Task<TarefaResponseDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando tarefa com ID: {Id}", id);
            var tarefa = await _repository.GetByIdAsync(id);
            return tarefa == null ? null : _mapper.Map<TarefaResponseDto>(tarefa);
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByTituloAsync(string titulo)
        {
            _logger.LogInformation("Buscando tarefas com título: {Titulo}", titulo);
            var tarefas = await _repository.GetByTituloAsync(titulo);
            return _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByDataAsync(DateTime data)
        {
            _logger.LogInformation("Buscando tarefas para a data: {Data}", data.Date);
            var tarefas = await _repository.GetByDataAsync(data);
            return _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
        }

        public async Task<IEnumerable<TarefaResponseDto>> GetByStatusAsync(EnumStatusTarefa status)
        {
            _logger.LogInformation("Buscando tarefas com status: {Status}", status);
            var tarefas = await _repository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<TarefaResponseDto>>(tarefas);
        }

        public async Task<TarefaResponseDto> CreateAsync(TarefaRequestDto tarefaDto)
        {
            _logger.LogInformation("Criando nova tarefa: {Titulo}", tarefaDto.Titulo);
            var tarefa = _mapper.Map<Tarefa>(tarefaDto);
            var tarefaCriada = await _repository.CreateAsync(tarefa);
            return _mapper.Map<TarefaResponseDto>(tarefaCriada);
        }

        public async Task<TarefaResponseDto?> UpdateAsync(int id, TarefaRequestDto tarefaDto)
        {
            _logger.LogInformation("Atualizando tarefa com ID: {Id}", id);
            var tarefaExistente = await _repository.GetByIdAsync(id);
            
            if (tarefaExistente == null)
                return null;

            _mapper.Map(tarefaDto, tarefaExistente);
            var tarefaAtualizada = await _repository.UpdateAsync(tarefaExistente);
            return _mapper.Map<TarefaResponseDto>(tarefaAtualizada);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deletando tarefa com ID: {Id}", id);
            return await _repository.DeleteAsync(id);
        }
    }
}
