using AutoMapper;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.DTOs.User;

namespace ControleGlicemia.API.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserProfileDto>();

        CreateMap<UpdateUserProfileDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrosGlicose, opt => opt.Ignore())
            .ForMember(dest => dest.Medicamentos, opt => opt.Ignore())
            .ForMember(dest => dest.Refeicoes, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrosDiarios, opt => opt.Ignore());

        CreateMap<CreateRegistroGlicoseDto, RegistroGlicose>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<UpdateRegistroGlicoseDto, RegistroGlicose>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<RegistroGlicose, RegistroGlicoseDto>();

        CreateMap<CreateRefeicaoDto, Refeicao>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<UpdateRefeicaoDto, Refeicao>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<Refeicao, RefeicaoDto>();

        CreateMap<CreateMedicamentoDto, Medicamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<UpdateMedicamentoDto, Medicamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<Medicamento, MedicamentoDto>();

        CreateMap<CreateRegistroDiarioDto, RegistroDiario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<UpdateRegistroDiarioDto, RegistroDiario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

        CreateMap<RegistroDiario, RegistroDiarioDto>();
    }
}