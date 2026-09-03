using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionOptionService : ICriterionOptionService
    {
        private readonly IRepository _repo;

        public CriterionOptionService(IRepository repo)
        {
            this._repo = repo;
        }

        public async Task<CriterionOptionDto> CreateCriterionOptionAsync(CreateCriterionOptionDto dto)
        {
            var criterionOption = new CriterionOption
            {
                CriterionId = dto.CriterionId,
                Value = dto.Value,
                Rank = dto.Rank
            };

            await _repo.AddAsync<CriterionOption>(criterionOption);
            await _repo.SaveChangesAsync();

            return new CriterionOptionDto(
                criterionOption.Id,
                criterionOption.CriterionId,
                criterionOption.Value,
                criterionOption.Rank
            );
        }

        public async Task<bool> DeleteCriterionOptionAsync(int id)
        {
            var criterionOption = await _repo.GetByIdAsync<CriterionOption>(id);

            if (criterionOption == null)
                return false;

            await _repo.DeleteAsync<CriterionOption>(criterionOption);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CriterionOptionDto>> GetAllCriterionOptionsAsync()
        {
            return await _repo.AllReadonly<CriterionOption>()
                .Select(co => new CriterionOptionDto(
                    co.Id,
                    co.CriterionId,
                    co.Value,
                    co.Rank
                ))
                .ToListAsync();
        }

        public async Task<CriterionOptionDto?> GetCriterionOptionByIdAsync(int id)
        {
            return await _repo.AllReadonly<CriterionOption>()
                .Where(co => co.Id == id)
                .Select(co => new CriterionOptionDto(
                    co.Id,
                    co.CriterionId,
                    co.Value,
                    co.Rank
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchCriterionOptionAsync(int id, PatchCriterionOptionDto dto)
        {
            var criterionOption = _repo.GetByIdAsync<CriterionOption>(id).Result ?? throw new ArgumentException($"Criterion Option with ID {id} not found.");

            if(dto.CriterionId.HasValue)
                criterionOption.CriterionId = dto.CriterionId.Value;
            if(dto.Value != null)
                criterionOption.Value = dto.Value;
            if(dto.Rank.HasValue && dto.Rank.Value > 0)
                criterionOption.Rank = dto.Rank.Value;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateCriterionOptionAsync(int id, UpdateCriterionOptionDto dto)
        {
            var criterionOption = await _repo.GetByIdAsync<CriterionOption>(id) ?? throw new ArgumentException($"Criterio nOption with ID {id} not found.");

            criterionOption.CriterionId = dto.CriterionId;
            criterionOption.Value = dto.Value;
            criterionOption.Rank = dto.Rank;

            await _repo.SaveChangesAsync();
        }
    }
}
