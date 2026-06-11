using ControleGlicemia.API.DTOs.Medicamento;
using ControleGlicemia.API.Models;
using ControleGlicemia.API.Repositories;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Services;

public class MedicamentoService : IMedicamentoService
{
    private readonly IMedicamentoRepository _medicamentoRepository;
    private readonly IMapper _mapper;

    public MedicamentoService(IMedicamentoRepository medicamentoRepository, IMapper mapper)
    {
        _medicamentoRepository = medicamentoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MedicamentoDto>> GetAllMedicamentosByUserIdAsync(int userId)
    {
        var medicamentos = await _medicamentoRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<MedicamentoDto>>(medicamentos);
    }

    public async Task<MedicamentoDto?> GetMedicamentoByIdAsync(int id)
    {
        var medicamento = await _medicamentoRepository.GetByIdAsync(id);
        return _mapper.Map<MedicamentoDto>(medicamento);
    }

    public async Task AddMedicamentoAsync(int userId, CreateMedicamentoDto medicamentoDto)
    {
        ValidarDominioMedicamento(medicamentoDto.Nome, medicamentoDto.Dose, medicamentoDto.TomadoEm);

        var medicamento = _mapper.Map<Medicamento>(medicamentoDto);
        medicamento.UserId = userId;
        await _medicamentoRepository.AddAsync(medicamento);
    }

    public async Task DeleteMedicamentoAsync(int id)
    {
        var medicamento = await _medicamentoRepository.GetByIdAsync(id);
        if (medicamento != null)
        {
            await _medicamentoRepository.DeleteAsync(medicamento);
        }
    }

    public async Task<MedicamentoDto?> UpdateMedicamentoAsync(int id, int userId, UpdateMedicamentoDto medicamentoDto)
    {
        var existingMedicamento = await _medicamentoRepository.GetByIdAsync(id);

        if (existingMedicamento == null || existingMedicamento.UserId != userId)
        {
            return null;
        }

        ValidarDominioMedicamento(medicamentoDto.Nome, medicamentoDto.Dose, medicamentoDto.TomadoEm);

        _mapper.Map(medicamentoDto, existingMedicamento);

        await _medicamentoRepository.UpdateAsync(existingMedicamento);
        return _mapper.Map<MedicamentoDto>(existingMedicamento);
    }

    private static void ValidarDominioMedicamento(string nome, double dose, DateTime tomadoEm)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ValidationException("O nome do medicamento é obrigatório.");

        if (nome.Length > 100)
            throw new ValidationException("O nome do medicamento não pode exceder 100 caracteres.");

        if (dose < 0.1 || dose > 1000.0)
            throw new ValidationException("A dose deve ser entre 0.1 e 1000.");

        if (tomadoEm > DateTime.UtcNow.AddMinutes(5))
            throw new ValidationException("A data/hora de tomada não pode ser futura.");
    }
}