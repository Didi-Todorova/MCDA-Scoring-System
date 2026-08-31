using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionOption;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionOptionService : ICriterionOptionService
    {
        private readonly IRepository repo;

        public CriterionOptionService(IRepository repo)
        {
            this.repo = repo;
        }

        public async Task<CriterionOptionDto> CreateCriterionOptionAsync(CreateCriterionOptionDto createCriterionOptionDto)
        {
            var criterionOption = new CriterionOption
            {
                CriterionId = createCriterionOptionDto.CriterionId,
                Value = createCriterionOptionDto.Value,
                Rank = createCriterionOptionDto.Rank
            };

            await repo.AddAsync<CriterionOption>(criterionOption);
            await repo.SaveChangesAsync();

            return new CriterionOptionDto(
                criterionOption.Id,
                criterionOption.CriterionId,
                criterionOption.Value,
                criterionOption.Rank
            );
        }

        public async Task DeleteCriterionOptionAsync(int id)
        {
            var criterionOption = await repo.GetByIdAsync<CriterionOption>(id) ?? throw new ArgumentException($"CriterionOption with ID {id} not found.");
            await repo.DeleteAsync<CriterionOption>(criterionOption);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<CriterionOptionDto>> GetAllCriterionOptionsAsync()
        {
            return await repo.AllReadonly<CriterionOption>()
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
            return await repo.AllReadonly<CriterionOption>()
                .Where(co => co.Id == id)
                .Select(co => new CriterionOptionDto(
                    co.Id,
                    co.CriterionId,
                    co.Value,
                    co.Rank
                ))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateCriterionOptionAsync(int id, UpdateCriterionOptionDto updateCriterionOptionDto)
        {
            var criterionOption = await repo.GetByIdAsync<CriterionOption>(id) ?? throw new ArgumentException($"CriterionOption with ID {id} not found.");

            criterionOption.CriterionId = updateCriterionOptionDto.CriterionId;
            criterionOption.Value = updateCriterionOptionDto.Value;
            criterionOption.Rank = updateCriterionOptionDto.Rank;

            await repo.SaveChangesAsync();
        }
    }
}
