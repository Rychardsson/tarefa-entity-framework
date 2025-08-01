namespace TrilhaApiDesafio.DTOs
{
    /// <summary>
    /// DTO para estatísticas das tarefas
    /// </summary>
    public class TarefaEstatisticasDto
    {
        /// <summary>
        /// Total de tarefas ativas
        /// </summary>
        public int TotalTarefas { get; set; }
        
        /// <summary>
        /// Total de tarefas pendentes
        /// </summary>
        public int TarefasPendentes { get; set; }
        
        /// <summary>
        /// Total de tarefas finalizadas
        /// </summary>
        public int TarefasFinalizadas { get; set; }
        
        /// <summary>
        /// Total de tarefas atrasadas
        /// </summary>
        public int TarefasAtrasadas { get; set; }
        
        /// <summary>
        /// Total de tarefas para hoje
        /// </summary>
        public int TarefasHoje { get; set; }
        
        /// <summary>
        /// Percentual de conclusão
        /// </summary>
        public decimal PercentualConclusao { get; set; }
        
        /// <summary>
        /// Distribuição por prioridade
        /// </summary>
        public Dictionary<int, int> DistribuicaoPorPrioridade { get; set; } = new();
        
        /// <summary>
        /// Data da última atualização das estatísticas
        /// </summary>
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
