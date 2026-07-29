using ControleGlicemia.API.Models;
using ControleGlicemia.API.DTOs.Refeicao;
using ControleGlicemia.API.Repositories;
using AutoMapper;

namespace ControleGlicemia.API.Services
{
    public class RefeicaoService : IRefeicaoService
    {
        private readonly IRefeicaoRepository _refeicaoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RefeicaoService> _logger;

        public RefeicaoService(IRefeicaoRepository refeicaoRepository, IMapper mapper, ILogger<RefeicaoService> logger)
        {
            _refeicaoRepository = refeicaoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RefeicaoDto>> GetAllRefeicoesByUserIdAsync(int userId)
        {
            var refeicoes = await _refeicaoRepository.GetAllByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<RefeicaoDto>>(refeicoes);
        }

        public async Task<PagedResult<RefeicaoDto>> GetRefeicoesPagedAsync(int userId, int page, int pageSize)
        {
            var paged = await _refeicaoRepository.GetPagedByUserIdAsync(userId, page, pageSize);
            return new PagedResult<RefeicaoDto>
            {
                Items = _mapper.Map<IEnumerable<RefeicaoDto>>(paged.Items),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<RefeicaoDto?> GetRefeicaoByIdAsync(int id, int userId)
        {
            var refeicao = await _refeicaoRepository.GetByIdAsync(id);
            if (refeicao is null || refeicao.UserId != userId)
                return null;
            return _mapper.Map<RefeicaoDto>(refeicao);
        }

        public async Task AddRefeicaoAsync(int userId, CreateRefeicaoDto refeicaoDto)
        {
            var refeicao = _mapper.Map<Refeicao>(refeicaoDto);
            refeicao.UserId = userId;
            await _refeicaoRepository.AddAsync(refeicao);
        }

        public async Task<bool> DeleteRefeicaoAsync(int id, int userId)
        {
            var refeicao = await _refeicaoRepository.GetByIdAsync(id);
            if (refeicao == null || refeicao.UserId != userId)
                return false;

            await _refeicaoRepository.DeleteAsync(refeicao);
            return true;
        }

        public async Task<RefeicaoDto?> UpdateRefeicaoAsync(int id, int userId, UpdateRefeicaoDto refeicaoDto)
        {
            var existingRefeicao = await _refeicaoRepository.GetByIdAsync(id);

            if (existingRefeicao == null || existingRefeicao.UserId != userId)
                return null;

            _mapper.Map(refeicaoDto, existingRefeicao);

            await _refeicaoRepository.UpdateAsync(existingRefeicao);
            return _mapper.Map<RefeicaoDto>(existingRefeicao);
        }
    }
}