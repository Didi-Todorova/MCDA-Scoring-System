using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class NuimericalCriterionRuleService : INumericalCriterionRuleService
    {
        private readonly IRepository repo;

        public NuimericalCriterionRuleService(IRepository repo)
        {
            this.repo = repo;
        }

        public async Task<NumericalCriterionRuleDto> CreateNumericalCriterionRuleAsync(CreateNumericalCriterionRuleDto createNumericalCriterionRuleDto)
        {
            var numericalCriterionRule = new NumericalCriterionRule
            {
                CriterionId = createNumericalCriterionRuleDto.CriterionId,
                NumericType = createNumericalCriterionRuleDto.NumericType,
                TargetValue = createNumericalCriterionRuleDto.TargetValue,
                Direction = createNumericalCriterionRuleDto.Direction
            };

            await repo.AddAsync<NumericalCriterionRule>(numericalCriterionRule);
            await repo.SaveChangesAsync();

            return new NumericalCriterionRuleDto(
                numericalCriterionRule.Id,
                numericalCriterionRule.CriterionId,
                numericalCriterionRule.NumericType,
                numericalCriterionRule.TargetValue,
                numericalCriterionRule.Direction
            );
        }

        public async Task DeleteNumericalCriterionRuleAsync(int id)
        {
            var numericalCriterionRule = await repo.GetByIdAsync<NumericalCriterionRule>(id) ?? throw new ArgumentException($"NumericalCriterionRule with ID {id} not found.");

            await repo.DeleteAsync<NumericalCriterionRule>(numericalCriterionRule);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<NumericalCriterionRuleDto>> GetAllNumericalCriterionRulesAsync()
        {
            return await repo.All<NumericalCriterionRule>()
                .Select(ncr => new NumericalCriterionRuleDto(
                    ncr.Id,
                    ncr.CriterionId,
                    ncr.NumericType,
                    ncr.TargetValue,
                    ncr.Direction
                ))
                .ToListAsync();
        }

        public async Task<NumericalCriterionRuleDto?> GetNumericalCriterionRuleByIdAsync(int id)
        {
            return await repo.All<NumericalCriterionRule>()
                .Where(ncr => ncr.Id == id)
                .Select(ncr => new NumericalCriterionRuleDto(
                    ncr.Id,
                    ncr.CriterionId,
                    ncr.NumericType,
                    ncr.TargetValue,
                    ncr.Direction
                ))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateNumericalCriterionRuleAsync(int id, UpdateNumericalCriterionRuleDto updateNumericalCriterionRuleDto)
        {
            var numericalCriterionRule = await repo.GetByIdAsync<NumericalCriterionRule>(id) ?? throw new ArgumentException($"NumericalCriterionRule with ID {id} not found.");

            numericalCriterionRule.CriterionId = updateNumericalCriterionRuleDto.CriterionId;
            numericalCriterionRule.NumericType = updateNumericalCriterionRuleDto.NumericType;
            numericalCriterionRule.TargetValue = updateNumericalCriterionRuleDto.TargetValue;
            numericalCriterionRule.Direction = updateNumericalCriterionRuleDto.Direction;

            await repo.SaveChangesAsync();
        }
    }
}