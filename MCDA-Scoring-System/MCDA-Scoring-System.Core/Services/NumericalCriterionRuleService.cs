using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericalCriterionRule;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class NumericalCriterionRuleService : INumericalCriterionRuleService
    {
        private readonly IRepository _repo;

        public NumericalCriterionRuleService(IRepository repo)
        {
            this._repo = repo;
        }

        public async Task<NumericalCriterionRuleDto> CreateNumericalCriterionRuleAsync(CreateNumericalCriterionRuleDto dto)
        {
            var numericalCriterionRule = new NumericalCriterionRule
            {
                CriterionId = dto.CriterionId,
                NumericType = dto.NumericType,
                TargetValue = dto.TargetValue,
                Direction = dto.Direction
            };

            await _repo.AddAsync<NumericalCriterionRule>(numericalCriterionRule);
            await _repo.SaveChangesAsync();

            return new NumericalCriterionRuleDto(
                numericalCriterionRule.Id,
                numericalCriterionRule.CriterionId,
                numericalCriterionRule.NumericType,
                numericalCriterionRule.TargetValue,
                numericalCriterionRule.Direction
            );
        }

        public async Task<bool> DeleteNumericalCriterionRuleAsync(int id)
        {
            var numericalCriterionRule = await _repo.GetByIdAsync<NumericalCriterionRule>(id);

            if (numericalCriterionRule == null)
                return false;

            await _repo.DeleteAsync<NumericalCriterionRule>(numericalCriterionRule);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NumericalCriterionRuleDto>> GetAllNumericalCriterionRulesAsync()
        {
            return await _repo.All<NumericalCriterionRule>()
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
            return await _repo.All<NumericalCriterionRule>()
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

        public async Task PatchNumericalCriterionRuleAsync(int id, PatchNumericalCriterionRuleDto dto)
        {
            var numericalCriterionRule = await _repo.GetByIdAsync<NumericalCriterionRule>(id) ?? throw new ArgumentException($"Numerical Criterion Rule with ID {id} not found.");

            if(dto.CriterionId.HasValue)            
                numericalCriterionRule.CriterionId = dto.CriterionId.Value;
            if(dto.NumericType.HasValue)
                numericalCriterionRule.NumericType = dto.NumericType.Value;
            if (dto.TargetValue.HasValue)
                numericalCriterionRule.TargetValue = dto.TargetValue.Value;
            if (dto.Direction.HasValue)
                numericalCriterionRule.Direction = dto.Direction.Value;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateNumericalCriterionRuleAsync(int id, UpdateNumericalCriterionRuleDto dto)
        {
            var numericalCriterionRule = await _repo.GetByIdAsync<NumericalCriterionRule>(id) ?? throw new ArgumentException($"Numerical Criterion Rule with ID {id} not found.");

            numericalCriterionRule.CriterionId = dto.CriterionId;
            numericalCriterionRule.NumericType = dto.NumericType;
            numericalCriterionRule.TargetValue = dto.TargetValue;
            numericalCriterionRule.Direction = dto.Direction;

            await _repo.SaveChangesAsync();
        }
    }
}