using System.ComponentModel.DataAnnotations;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.DTOs
{
    /// <summary>
    /// DTO para criação e atualização de tarefas
    /// </summary>
    public class TarefaRequestDto
    {
        /// <summary>
        /// Título da tarefa
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O título deve ter entre 3 e 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;
        
        /// <summary>
        /// Descrição detalhada da tarefa
        /// </summary>
        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; set; }
        
        /// <summary>
        /// Data prevista para conclusão da tarefa
        /// </summary>
        [Required(ErrorMessage = "A data é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }
        
        /// <summary>
        /// Status da tarefa
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        public EnumStatusTarefa Status { get; set; }
        
        /// <summary>
        /// Prioridade da tarefa (1 = Baixa, 2 = Média, 3 = Alta, 4 = Crítica)
        /// </summary>
        [Range(1, 4, ErrorMessage = "A prioridade deve estar entre 1 e 4")]
        public int Prioridade { get; set; } = 1;
        
        /// <summary>
        /// Tags da tarefa separadas por vírgula
        /// </summary>
        [StringLength(500, ErrorMessage = "As tags devem ter no máximo 500 caracteres")]
        public string? Tags { get; set; }
    }
}
