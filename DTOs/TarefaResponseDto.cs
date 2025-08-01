using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.DTOs
{
    public class TarefaResponseDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime Data { get; set; }
        public EnumStatusTarefa Status { get; set; }
        public string StatusDescricao => Status.ToString();
        public DateTime DataCriacao { get; set; }
    }
}
