using AutoMapper;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Mappings
{
    /// <summary>
    /// Profile do AutoMapper para mapeamento entre DTOs e entidades de Tarefa
    /// </summary>
    public class TarefaProfile : Profile
    {
        public TarefaProfile()
        {
            // Mapeamento de TarefaRequestDto para Tarefa
            CreateMap<TarefaRequestDto, Tarefa>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    // Normalizar tags removendo espaços extras
                    if (!string.IsNullOrWhiteSpace(src.Tags))
                    {
                        var tags = src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(t => t.Trim())
                                          .Where(t => !string.IsNullOrEmpty(t));
                        dest.Tags = string.Join(", ", tags);
                    }
                });

            // Mapeamento de Tarefa para TarefaResponseDto
            CreateMap<Tarefa, TarefaResponseDto>()
                .AfterMap((src, dest) =>
                {
                    // Adicionar validações ou transformações específicas se necessário
                });
        }
    }
}
