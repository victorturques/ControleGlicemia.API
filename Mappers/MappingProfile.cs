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
        // =========================
        // User
        // =========================
        // REMOVIDO: CreateMap<User, User>();
        // Esse mapa é perigoso/desnecessário.
        CreateMap<User, UserProfileDto>();

        CreateMap<UpdateUserProfileDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SenhaHash, opt => opt.Ignore())
            .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrosGlicose, opt => opt.Ignore())
            .ForMember(dest => dest.Medicamentos, opt => opt.Ignore())
            .ForMember(dest => dest.Refeicoes, opt => opt.Ignore())
            .ForMember(dest => dest.RegistrosDiarios, opt => opt.Ignore());

        // =========================
        // RegistroGlicose
        // =========================
        CreateMap<CreateRegistroGlicoseDto, RegistroGlicose>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateRegistroGlicoseDto, RegistroGlicose>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<RegistroGlicose, RegistroGlicoseDto>();

        // =========================
        // Refeicao
        // =========================
        CreateMap<CreateRefeicaoDto, Refeicao>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateRefeicaoDto, Refeicao>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<Refeicao, RefeicaoDto>();

        // =========================
        // Medicamento
        // =========================
        CreateMap<CreateMedicamentoDto, Medicamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateMedicamentoDto, Medicamento>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<Medicamento, MedicamentoDto>();

        // =========================
        // RegistroDiario
        // =========================
        CreateMap<CreateRegistroDiarioDto, RegistroDiario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateRegistroDiarioDto, RegistroDiario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<RegistroDiario, RegistroDiarioDto>();
    }
}