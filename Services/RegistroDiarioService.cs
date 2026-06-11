using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Services;

public class RegistroDiarioService : IRegistroDiarioService
{
    private readonly IRegistroDiarioRepository _registroDiarioRepository;
    private readonly IMapper _mapper;

    public RegistroDiarioService(IRegistroDiarioRepository registroDiarioRepository, IMapper mapper)
    {
        _registroDiarioRepository = registroDiarioRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RegistroDiarioDto>> GetAllRegistrosDiariosByUserIdAsync(int userId)
    {
        var registros = await _registroDiarioRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RegistroDiarioDto>>(registros);
    }

    public async Task<RegistroDiarioDto?> GetRegistroDiarioByIdAsync(int id)
    {
        var registro = await _registroDiarioRepository.GetByIdAsync(id);
        return _mapper.Map<RegistroDiarioDto>(registro);
    }

    public async Task AddRegistroDiarioAsync(int userId, CreateRegistroDiarioDto registroDto)
    {
        ValidarDominioRegistroDiario(registroDto.Data, registroDto.Observacoes);

        var registroDiario = _mapper.Map<RegistroDiario>(registroDto);
        registroDiario.UserId = userId;
        await _registroDiarioRepository.AddAsync(registroDiario);
    }

    public async Task DeleteRegistroDiarioAsync(int id)
    {
        var registroDiario = await _registroDiarioRepository.GetByIdAsync(id);
        if (registroDiario != null)
        {
            await _registroDiarioRepository.DeleteAsync(registroDiario);
        }
    }

    public async Task<RegistroDiarioDto?> UpdateRegistroDiarioAsync(int id, int userId, UpdateRegistroDiarioDto registroDto)
    {
        var existingRegistroDiario = await _registroDiarioRepository.GetByIdAsync(id);

        if (existingRegistroDiario == null || existingRegistroDiario.UserId != userId)
        {
            return null;
        }

        ValidarDominioRegistroDiario(registroDto.Data, registroDto.Observacoes);

        _mapper.Map(registroDto, existingRegistroDiario);

        await _registroDiarioRepository.UpdateAsync(existingRegistroDiario);
        return _mapper.Map<RegistroDiarioDto>(existingRegistroDiario);
    }

    private static void ValidarDominioRegistroDiario(DateTime data, string? observacoes)
    {
        if (data == default)
            throw new ValidationException("A data do registro diário é obrigatória.");

        if (data > DateTime.UtcNow.AddMinutes(5))
            throw new ValidationException("A data do registro diário não pode ser futura.");

        if (!string.IsNullOrWhiteSpace(observacoes) && observacoes.Length > 1000)
            throw new ValidationException("As observações não podem exceder 1000 caracteres.");
    }
}