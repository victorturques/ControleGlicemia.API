using ControleGlicemia.API.Models;
using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Repositories;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace ControleGlicemia.API.Services
{
    public class RefeicaoService : IRefeicaoService
    {
        private readonly IRefeicaoRepository _refeicaoRepository;
        private readonly IMapper _mapper;

        public RefeicaoService(IRefeicaoRepository refeicaoRepository, IMapper mapper)
        {
            _refeicaoRepository = refeicaoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RefeicaoDto>> GetAllRefeicoesByUserIdAsync(int userId)
        {
            var refeicoes = await _refeicaoRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<RefeicaoDto>>(refeicoes);
        }

        public async Task<RefeicaoDto?> GetRefeicaoByIdAsync(int id)
        {
            var refeicao = await _refeicaoRepository.GetByIdAsync(id);
            return _mapper.Map<RefeicaoDto>(refeicao);
        }

        public async Task AddRefeicaoAsync(int userId, CreateRefeicaoDto refeicaoDto)
        {
            ValidarDominioRefeicao(refeicaoDto.Nome, refeicaoDto.Descricao, refeicaoDto.DataHora, refeicaoDto.Observacoes);

            var refeicao = _mapper.Map<Refeicao>(refeicaoDto);
            refeicao.UserId = userId;
            await _refeicaoRepository.AddAsync(refeicao);
        }

        public async Task DeleteRefeicaoAsync(int id)
        {
            var refeicao = await _refeicaoRepository.GetByIdAsync(id);
            if (refeicao != null)
            {
                await _refeicaoRepository.DeleteAsync(refeicao);
            }
        }

        public async Task<RefeicaoDto?> UpdateRefeicaoAsync(int id, int userId, UpdateRefeicaoDto refeicaoDto)
        {
            var existingRefeicao = await _refeicaoRepository.GetByIdAsync(id);

            if (existingRefeicao == null || existingRefeicao.UserId != userId)
            {
                return null;
            }

            ValidarDominioRefeicao(refeicaoDto.Nome, refeicaoDto.Descricao, refeicaoDto.DataHora, refeicaoDto.Observacoes);

            _mapper.Map(refeicaoDto, existingRefeicao);

            await _refeicaoRepository.UpdateAsync(existingRefeicao);
            return _mapper.Map<RefeicaoDto>(existingRefeicao);
        }

        private static void ValidarDominioRefeicao(string nome, string? descricao, DateTime dataHora, string? observacoes)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ValidationException("O campo 'Nome' é obrigatório.");

            if (nome.Length > 100)
                throw new ValidationException("O nome da refeição não pode exceder 100 caracteres.");

            if (!string.IsNullOrWhiteSpace(descricao) && descricao.Length > 500)
                throw new ValidationException("A descrição da refeição não pode exceder 500 caracteres.");

            if (!string.IsNullOrWhiteSpace(observacoes) && observacoes.Length > 500)
                throw new ValidationException("As observações não podem exceder 500 caracteres.");

            if (dataHora > DateTime.UtcNow.AddMinutes(5))
                throw new ValidationException("A data/hora da refeição não pode ser futura.");
        }
    }
}