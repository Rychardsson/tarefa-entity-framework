using AutoMapper;
using TrilhaApiDesafio.DTOs;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Mappings
{
    public class TarefaProfile : Profile
    {
        public TarefaProfile()
        {
            CreateMap<TarefaRequestDto, Tarefa>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<Tarefa, TarefaResponseDto>();
        }
    }
}
