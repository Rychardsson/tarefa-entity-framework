using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrilhaApiDesafio.Models
{
    /// <summary>
    /// Entidade representando uma tarefa no sistema
    /// </summary>
    public class Tarefa
    {
        /// <summary>
        /// Identificador único da tarefa
        /// </summary>
        public int Id { get; set; }
        
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
        /// Status atual da tarefa
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        public EnumStatusTarefa Status { get; set; }
        
        /// <summary>
        /// Data de criação da tarefa (gerada automaticamente)
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Data da última atualização da tarefa
        /// </summary>
        public DateTime? DataAtualizacao { get; set; }
        
        /// <summary>
        /// Indica se a tarefa foi excluída logicamente
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// ID do usuário proprietário da tarefa
        /// </summary>
        public int? UserId { get; set; }
        
        /// <summary>
        /// Prioridade da tarefa
        /// </summary>
        public int Prioridade { get; set; } = 1;
        
        /// <summary>
        /// Tags associadas à tarefa (JSON serializado)
        /// </summary>
        [StringLength(500)]
        public string? Tags { get; set; }
        
        /// <summary>
        /// Marca a tarefa como atualizada
        /// </summary>
        public void MarcarComoAtualizada()
        {
            DataAtualizacao = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Verifica se a tarefa está atrasada
        /// </summary>
        /// <returns>True se a tarefa está atrasada, false caso contrário</returns>
        public bool EstaAtrasada()
        {
            return DateTime.UtcNow.Date > Data.Date && Status != EnumStatusTarefa.Finalizado;
        }
        
        /// <summary>
        /// Verifica se a tarefa é para hoje
        /// </summary>
        /// <returns>True se a tarefa é para hoje, false caso contrário</returns>
        public bool EhParaHoje()
        {
            return Data.Date == DateTime.UtcNow.Date;
        }
    }
}