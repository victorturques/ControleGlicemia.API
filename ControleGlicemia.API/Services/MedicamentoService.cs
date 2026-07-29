using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using AutoMapper;

namespace ControleGlicemia.API.Services;

public class MedicamentoService : IMedicamentoService
{
    private readonly IMedicamentoRepository _medicamentoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicamentoService> _logger;

    public MedicamentoService(IMedicamentoRepository medicamentoRepository, IMapper mapper, ILogger<MedicamentoService> logger)
    {
        _medicamentoRepository = medicamentoRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<MedicamentoDto>> GetAllMedicamentosByUserIdAsync(int userId)
    {
        var medicamentos = await _medicamentoRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<MedicamentoDto>>(medicamentos);
    }

    public async Task<PagedResult<MedicamentoDto>> GetMedicamentosPagedAsync(int userId, int page, int pageSize)
    {
        var paged = await _medicamentoRepository.GetPagedByUserIdAsync(userId, page, pageSize);
        return new PagedResult<MedicamentoDto>
        {
            Items = _mapper.Map<IEnumerable<MedicamentoDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
    }

    public async Task<MedicamentoDto?> GetMedicamentoByIdAsync(int id, int userId)
    {
        var medicamento = await _medicamentoRepository.GetByIdAsync(id);
        if (medicamento is null || medicamento.UserId != userId)
            return null;
        return _mapper.Map<MedicamentoDto>(medicamento);
    }

    public async Task AddMedicamentoAsync(int userId, CreateMedicamentoDto medicamentoDto)
    {
        var medicamento = _mapper.Map<Medicamento>(medicamentoDto);
        medicamento.UserId = userId;
        await _medicamentoRepository.AddAsync(medicamento);
    }

    public async Task<bool> DeleteMedicamentoAsync(int id, int userId)
    {
        var medicamento = await _medicamentoRepository.GetByIdAsync(id);
        if (medicamento == null || medicamento.UserId != userId)
            return false;

        await _medicamentoRepository.DeleteAsync(medicamento);
        return true;
    }

    public async Task<MedicamentoDto?> UpdateMedicamentoAsync(int id, int userId, UpdateMedicamentoDto medicamentoDto)
    {
        var existingMedicamento = await _medicamentoRepository.GetByIdAsync(id);

        if (existingMedicamento == null || existingMedicamento.UserId != userId)
            return null;

        _mapper.Map(medicamentoDto, existingMedicamento);

        await _medicamentoRepository.UpdateAsync(existingMedicamento);
        return _mapper.Map<MedicamentoDto>(existingMedicamento);
    }
}