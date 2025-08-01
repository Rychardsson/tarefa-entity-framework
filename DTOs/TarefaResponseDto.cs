using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.DTOs
{
    /// <summary>
    /// DTO para resposta de tarefas
    /// </summary>
    public class TarefaResponseDto
    {
        /// <summary>
        /// Identificador único da tarefa
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Título da tarefa
        /// </summary>
        public string Titulo { get; set; } = string.Empty;
        
        /// <summary>
        /// Descrição da tarefa
        /// </summary>
        public string? Descricao { get; set; }
        
        /// <summary>
        /// Data prevista para conclusão
        /// </summary>
        public DateTime Data { get; set; }
        
        /// <summary>
        /// Status da tarefa
        /// </summary>
        public EnumStatusTarefa Status { get; set; }
        
        /// <summary>
        /// Descrição textual do status
        /// </summary>
        public string StatusDescricao => Status.ToString();
        
        /// <summary>
        /// Data de criação da tarefa
        /// </summary>
        public DateTime DataCriacao { get; set; }
        
        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime? DataAtualizacao { get; set; }
        
        /// <summary>
        /// Prioridade da tarefa
        /// </summary>
        public int Prioridade { get; set; }
        
        /// <summary>
        /// Descrição textual da prioridade
        /// </summary>
        public string PrioridadeDescricao => Prioridade switch
        {
            1 => "Baixa",
            2 => "Média",
            3 => "Alta",
            4 => "Crítica",
            _ => "Indefinida"
        };
        
        /// <summary>
        /// Tags da tarefa
        /// </summary>
        public string? Tags { get; set; }
        
        /// <summary>
        /// Lista de tags como array
        /// </summary>
        public string[] TagsList => string.IsNullOrWhiteSpace(Tags) 
            ? Array.Empty<string>() 
            : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim())
                  .ToArray();
        
        /// <summary>
        /// Indica se a tarefa está atrasada
        /// </summary>
        public bool EstaAtrasada => DateTime.UtcNow.Date > Data.Date && Status != EnumStatusTarefa.Finalizado;
        
        /// <summary>
        /// Indica se a tarefa é para hoje
        /// </summary>
        public bool EhParaHoje => Data.Date == DateTime.UtcNow.Date;
        
        /// <summary>
        /// Dias restantes para o vencimento
        /// </summary>
        public int DiasRestantes => (Data.Date - DateTime.UtcNow.Date).Days;
    }
}
