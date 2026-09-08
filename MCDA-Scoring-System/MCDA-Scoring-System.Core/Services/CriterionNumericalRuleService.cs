using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionNumericalRule;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionNumericalRuleService : ICriterionNumericalRuleService
    {
        private readonly IRepository _repo;

        public CriterionNumericalRuleService(IRepository repo)
        {
            this._repo = repo;
        }

        public async Task<CriterionNumericalRuleDto> CreateCriterionNumericalRuleAsync(CreateCriterionNumericalRuleDto dto)
        {
            var criterionNumericalRule = new CriterionNumericalRule
            {
                CriterionId = dto.CriterionId,
                NumericType = dto.NumericType,
                TargetValue = dto.TargetValue,
                Direction = dto.Direction
            };

            await _repo.AddAsync<CriterionNumericalRule>(criterionNumericalRule);
            await _repo.SaveChangesAsync();

            return new CriterionNumericalRuleDto(
                criterionNumericalRule.Id,
                criterionNumericalRule.CriterionId,
                criterionNumericalRule.NumericType,
                criterionNumericalRule.TargetValue,
                criterionNumericalRule.Direction
            );
        }

        public async Task<bool> DeleteCriterionNumericalRuleAsync(int id)
        {
            var criterionNumericalRule = await _repo.GetByIdAsync<CriterionNumericalRule>(id);

            if (criterionNumericalRule == null)
                return false;

            await _repo.DeleteAsync<CriterionNumericalRule>(criterionNumericalRule);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CriterionNumericalRuleDto>> GetAllCriterionNumericalRulesAsync()
        {
            return await _repo.All<CriterionNumericalRule>()
                .Select(cnr => new CriterionNumericalRuleDto(
                    cnr.Id,
                    cnr.CriterionId,
                    cnr.NumericType,
                    cnr.TargetValue,
                    cnr.Direction
                ))
                .ToListAsync();
        }

        public async Task<CriterionNumericalRuleDto?> GetCriterionNumericalRuleByIdAsync(int id)
        {
            return await _repo.All<CriterionNumericalRule>()
                .Where(ncr => ncr.Id == id)
                .Select(ncr => new CriterionNumericalRuleDto(
                    ncr.Id,
                    ncr.CriterionId,
                    ncr.NumericType,
                    ncr.TargetValue,
                    ncr.Direction
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchCriterionNumericalRuleAsync(int id, PatchCriterionNumericalDto dto)
        {
            var criterionNumericalRule = await _repo.GetByIdAsync<CriterionNumericalRule>(id) ?? throw new ArgumentException($"Criterion Numerical Rule with ID {id} not found.");

            if(dto.CriterionId.HasValue)            
                criterionNumericalRule.CriterionId = dto.CriterionId.Value;
            if(dto.NumericType.HasValue)
                criterionNumericalRule.NumericType = dto.NumericType.Value;
            if (dto.TargetValue.HasValue)
                criterionNumericalRule.TargetValue = dto.TargetValue.Value;
            if (dto.Direction.HasValue)
                criterionNumericalRule.Direction = dto.Direction.Value;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateCriterionNumericalRuleAsync(int id, UpdateCriterionNumericalRuleDto dto)
        {
            var criterionNumericalRule = await _repo.GetByIdAsync<CriterionNumericalRule>(id) ?? throw new ArgumentException($"Criterion Numerical Rule with ID {id} not found.");

            criterionNumericalRule.CriterionId = dto.CriterionId;
            criterionNumericalRule.NumericType = dto.NumericType;
            criterionNumericalRule.TargetValue = dto.TargetValue;
            criterionNumericalRule.Direction = dto.Direction;

            await _repo.SaveChangesAsync();
        }
    }
}