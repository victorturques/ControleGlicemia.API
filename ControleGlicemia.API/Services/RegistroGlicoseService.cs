using AutoMapper;
using ControleGlicemia.API.DTOs.RegistroGlicose;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;

namespace ControleGlicemia.API.Services;

public class RegistroGlicoseService : IRegistroGlicoseService
{
    private readonly IRegistroGlicoseRepository _registroGlicoseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RegistroGlicoseService> _logger;

    public RegistroGlicoseService(IRegistroGlicoseRepository registroGlicoseRepository, IMapper mapper, ILogger<RegistroGlicoseService> logger)
    {
        _registroGlicoseRepository = registroGlicoseRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RegistroGlicoseDto>> GetAllRegistrosGlicoseByUserIdAsync(int userId)
    {
        var registros = await _registroGlicoseRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RegistroGlicoseDto>>(registros);
    }

    public async Task<PagedResult<RegistroGlicoseDto>> GetRegistrosGlicosePagedAsync(int userId, int page, int pageSize)
    {
        var paged = await _registroGlicoseRepository.GetPagedByUserIdAsync(userId, page, pageSize);
        return new PagedResult<RegistroGlicoseDto>
        {
            Items = _mapper.Map<IEnumerable<RegistroGlicoseDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<RegistroGlicoseDto?> GetRegistroGlicoseByIdAsync(int id, int userId)
    {
        var registro = await _registroGlicoseRepository.GetByIdAsync(id);
        if (registro is null || registro.UserId != userId)
            return null;
        return _mapper.Map<RegistroGlicoseDto>(registro);
    }

    public async Task AddRegistroGlicoseAsync(int userId, CreateRegistroGlicoseDto registroDto)
    {
        var registro = _mapper.Map<RegistroGlicose>(registroDto);
        registro.UserId = userId;

        await _registroGlicoseRepository.AddAsync(registro);
    }

    public async Task<bool> DeleteRegistroGlicoseAsync(int id, int userId)
    {
        var registro = await _registroGlicoseRepository.GetByIdAsync(id);
        if (registro is null || registro.UserId != userId)
            return false;

        await _registroGlicoseRepository.DeleteAsync(registro);
        return true;
    }

    public async Task<RegistroGlicoseDto?> UpdateRegistroGlicoseAsync(int id, int userId, UpdateRegistroGlicoseDto registroDto)
    {
        var existingRegistro = await _registroGlicoseRepository.GetByIdAsync(id);

        if (existingRegistro is null || existingRegistro.UserId != userId)
            return null;

        _mapper.Map(registroDto, existingRegistro);

        await _registroGlicoseRepository.UpdateAsync(existingRegistro);
        return _mapper.Map<RegistroGlicoseDto>(existingRegistro);
    }
}