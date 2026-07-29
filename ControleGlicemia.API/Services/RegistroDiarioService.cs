using ControleGlicemia.API.DTOs.RegistroDiario;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using AutoMapper;

namespace ControleGlicemia.API.Services;

public class RegistroDiarioService : IRegistroDiarioService
{
    private readonly IRegistroDiarioRepository _registroDiarioRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RegistroDiarioService> _logger;

    public RegistroDiarioService(IRegistroDiarioRepository registroDiarioRepository, IMapper mapper, ILogger<RegistroDiarioService> logger)
    {
        _registroDiarioRepository = registroDiarioRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RegistroDiarioDto>> GetAllRegistrosDiariosByUserIdAsync(int userId)
    {
        var registros = await _registroDiarioRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RegistroDiarioDto>>(registros);
    }

    public async Task<PagedResult<RegistroDiarioDto>> GetRegistrosDiariosPagedAsync(int userId, int page, int pageSize)
    {
        var paged = await _registroDiarioRepository.GetPagedByUserIdAsync(userId, page, pageSize);
        return new PagedResult<RegistroDiarioDto>
        {
            Items = _mapper.Map<IEnumerable<RegistroDiarioDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<RegistroDiarioDto?> GetRegistroDiarioByIdAsync(int id, int userId)
    {
        var registro = await _registroDiarioRepository.GetByIdAsync(id);
        if (registro is null || registro.UserId != userId)
            return null;
        return _mapper.Map<RegistroDiarioDto>(registro);
    }

    public async Task AddRegistroDiarioAsync(int userId, CreateRegistroDiarioDto registroDto)
    {
        var registroDiario = _mapper.Map<RegistroDiario>(registroDto);
        registroDiario.UserId = userId;
        await _registroDiarioRepository.AddAsync(registroDiario);
    }

    public async Task<bool> DeleteRegistroDiarioAsync(int id, int userId)
    {
        var registroDiario = await _registroDiarioRepository.GetByIdAsync(id);
        if (registroDiario == null || registroDiario.UserId != userId)
            return false;

        await _registroDiarioRepository.DeleteAsync(registroDiario);
        return true;
    }

    public async Task<RegistroDiarioDto?> UpdateRegistroDiarioAsync(int id, int userId, UpdateRegistroDiarioDto registroDto)
    {
        var existingRegistroDiario = await _registroDiarioRepository.GetByIdAsync(id);

        if (existingRegistroDiario == null || existingRegistroDiario.UserId != userId)
            return null;

        _mapper.Map(registroDto, existingRegistroDiario);

        await _registroDiarioRepository.UpdateAsync(existingRegistroDiario);
        return _mapper.Map<RegistroDiarioDto>(existingRegistroDiario);
    }
}