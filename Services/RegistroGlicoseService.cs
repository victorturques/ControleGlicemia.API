using AutoMapper;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Services;

public class RegistroGlicoseService : IRegistroGlicoseService
{
    private readonly IRegistroGlicoseRepository _registroGlicoseRepository;
    private readonly IMapper _mapper;

    public RegistroGlicoseService(IRegistroGlicoseRepository registroGlicoseRepository, IMapper mapper)
    {
        _registroGlicoseRepository = registroGlicoseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RegistroGlicoseDto>> GetAllRegistrosGlicoseByUserIdAsync(int userId)
    {
        var registros = await _registroGlicoseRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RegistroGlicoseDto>>(registros);
    }

    public async Task<RegistroGlicoseDto?> GetRegistroGlicoseByIdAsync(int id)
    {
        var registro = await _registroGlicoseRepository.GetByIdAsync(id);
        return registro is null ? null : _mapper.Map<RegistroGlicoseDto>(registro);
    }

    public async Task AddRegistroGlicoseAsync(int userId, CreateRegistroGlicoseDto registroDto)
    {
        ValidarDominioRegistro(registroDto.Valor, registroDto.MedidoEm, registroDto.MomentoMedicao, registroDto.Observacoes);

        var registro = _mapper.Map<RegistroGlicose>(registroDto);
        registro.UserId = userId;

        await _registroGlicoseRepository.AddAsync(registro);
    }

    public async Task DeleteRegistroGlicoseAsync(int id)
    {
        var registro = await _registroGlicoseRepository.GetByIdAsync(id);
        if (registro is not null)
            await _registroGlicoseRepository.DeleteAsync(registro);
    }

    public async Task<RegistroGlicoseDto?> UpdateRegistroGlicoseAsync(int id, int userId, UpdateRegistroGlicoseDto registroDto)
    {
        var existingRegistro = await _registroGlicoseRepository.GetByIdAsync(id);

        if (existingRegistro is null || existingRegistro.UserId != userId)
            return null;

        ValidarDominioRegistro(registroDto.Valor, registroDto.MedidoEm, registroDto.MomentoMedicao, registroDto.Observacoes);

        _mapper.Map(registroDto, existingRegistro);

        await _registroGlicoseRepository.UpdateAsync(existingRegistro);
        return _mapper.Map<RegistroGlicoseDto>(existingRegistro);
    }

    private static void ValidarDominioRegistro(double valor, DateTime medidoEm, MomentoMedicao momento, string? observacoes)
    {
        if (valor < 1 || valor > 999)
            throw new ValidationException("O valor da glicose deve estar entre 1 e 999.");

        if (medidoEm == default)
            throw new ValidationException("A data da medição é obrigatória.");

        if (medidoEm > DateTime.UtcNow.AddMinutes(5))
            throw new ValidationException("A data da medição não pode ser futura.");

        if (!Enum.IsDefined(typeof(MomentoMedicao), momento))
            throw new ValidationException("Momento de medição inválido.");

        if (!string.IsNullOrWhiteSpace(observacoes) && observacoes.Length > 300)
            throw new ValidationException("Observações devem ter no máximo 300 caracteres.");
    }
}