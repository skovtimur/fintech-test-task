using AutoMapper;
using FintechTestTask.Domain.Dtos;
using FintechTestTask.Domain.Entities;
using FintechTestTask.Domain.Models;

namespace FintechTestTask.WebAPI;

public class MainMapperProfile : Profile
{
    public MainMapperProfile()
    {
        CreateMap<JwtTokens, JwtTokensDto>()
            .ForMember(x => x.ExpiresAtUtc,
                g => g.MapFrom(m => m.ExpiresAtUtc.ToString()));
        CreateMap<MoveEntity, MoveDto>()
            .ForMember(x => x.Row, opt => opt.MapFrom(y => y.Cell.Row))
            .ForMember(x => x.Column, opt => opt.MapFrom(y => y.Cell.Column));

        CreateMap<GameEntity, GameDto>()
            .ForMember(x => x.FinishedAt, g => g.MapFrom(m => m.FinishedAt.ToString()));
        CreateMap<GameEntity, GamePartialDto>();
    }
}