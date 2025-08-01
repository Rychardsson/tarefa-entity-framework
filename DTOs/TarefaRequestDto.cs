using System.ComponentModel.DataAnnotations;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.DTOs
{
    public class TarefaRequestDto
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Data { get; set; }
        
        [Required(ErrorMessage = "O status é obrigatório")]
        public EnumStatusTarefa Status { get; set; }
    }
}
