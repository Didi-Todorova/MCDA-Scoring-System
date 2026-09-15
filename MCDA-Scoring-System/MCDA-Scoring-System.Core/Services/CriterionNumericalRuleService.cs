using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.CriterionNumericalRule;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionNumericalRuleService : ICriterionNumericalRuleService
    {
        private readonly IRepository _repo;

        public CriterionNumericalRuleService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<CriterionNumericalRuleDto> CreateCriterionNumericalRuleAsync(
            CreateCriterionNumericalRuleDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var criterion = await _repo.GetByIdAsync<Criterion>(dto.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {dto.CriterionId} not found.");

            ValidateCriterionType(criterion);

            var existingRule = await _repo.AllReadonly<CriterionNumericalRule>()
                .AnyAsync(r => r.CriterionId == dto.CriterionId);

            if (existingRule)
                throw new ArgumentException(
                    "This criterion already has a numerical rule.");

            ValidateRule(
                dto.NumericType,
                dto.MinValue,
                dto.MaxValue,
                dto.TargetValue,
                dto.Direction);

            var rule = new CriterionNumericalRule
            {
                CriterionId = dto.CriterionId,
                NumericType = dto.NumericType,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                TargetValue = dto.TargetValue,
                Direction = dto.Direction
            };

            await _repo.AddAsync(rule);
            await _repo.SaveChangesAsync();

            return new CriterionNumericalRuleDto(
                rule.Id,
                rule.CriterionId,
                rule.NumericType,
                rule.MinValue,
                rule.MaxValue,
                rule.TargetValue,
                rule.Direction
            );
        }

        public async Task<bool> DeleteCriterionNumericalRuleAsync(int id)
        {
            var rule = await _repo.GetByIdAsync<CriterionNumericalRule>(id);

            if (rule == null)
                return false;

            _repo.Delete(rule);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CriterionNumericalRuleDto>>
            GetAllCriterionNumericalRulesAsync()
        {
            return await _repo.AllReadonly<CriterionNumericalRule>()
                .Select(rule => new CriterionNumericalRuleDto(
                    rule.Id,
                    rule.CriterionId,
                    rule.NumericType,
                    rule.MinValue,
                    rule.MaxValue,
                    rule.TargetValue,
                    rule.Direction
                ))
                .ToListAsync();
        }

        public async Task<CriterionNumericalRuleDto?>
            GetCriterionNumericalRuleByIdAsync(int id)
        {
            return await _repo.AllReadonly<CriterionNumericalRule>()
                .Where(rule => rule.Id == id)
                .Select(rule => new CriterionNumericalRuleDto(
                    rule.Id,
                    rule.CriterionId,
                    rule.NumericType,
                    rule.MinValue,
                    rule.MaxValue,
                    rule.TargetValue,
                    rule.Direction
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchCriterionNumericalRuleAsync(
            int id,
            PatchCriterionNumericalDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var rule = await _repo.GetByIdAsync<CriterionNumericalRule>(id);

            if (rule == null)
                throw new KeyNotFoundException(
                    $"Criterion Numerical Rule with ID {id} not found.");

            var criterionId = dto.CriterionId ?? rule.CriterionId;
            var numericType = dto.NumericType ?? rule.NumericType;
            var minValue = dto.MinValue ?? rule.MinValue;
            var maxValue = dto.MaxValue ?? rule.MaxValue;
            var targetValue = dto.TargetValue ?? rule.TargetValue;
            var direction = dto.Direction ?? rule.Direction;

            var criterion = await _repo.GetByIdAsync<Criterion>(criterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {criterionId} not found.");

            ValidateCriterionType(criterion);

            if (criterionId != rule.CriterionId)
            {
                var existingRule = await _repo
                    .AllReadonly<CriterionNumericalRule>()
                    .AnyAsync(r =>
                        r.Id != id &&
                        r.CriterionId == criterionId);

                if (existingRule)
                    throw new ArgumentException(
                        "This criterion already has a numerical rule.");
            }

            ValidateRule(
                numericType,
                minValue,
                maxValue,
                targetValue,
                direction);

            rule.CriterionId = criterionId;
            rule.NumericType = numericType;
            rule.MinValue = minValue;
            rule.MaxValue = maxValue;
            rule.TargetValue = targetValue;
            rule.Direction = direction;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateCriterionNumericalRuleAsync(
            int id,
            UpdateCriterionNumericalRuleDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var rule = await _repo.GetByIdAsync<CriterionNumericalRule>(id);

            if (rule == null)
                throw new KeyNotFoundException(
                    $"Criterion Numerical Rule with ID {id} not found.");

            var criterion = await _repo.GetByIdAsync<Criterion>(dto.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {dto.CriterionId} not found.");

            ValidateCriterionType(criterion);

            var existingRule = await _repo
                .AllReadonly<CriterionNumericalRule>()
                .AnyAsync(r =>
                    r.Id != id &&
                    r.CriterionId == dto.CriterionId);

            if (existingRule)
                throw new ArgumentException(
                    "This criterion already has a numerical rule.");

            ValidateRule(
                dto.NumericType,
                dto.MinValue,
                dto.MaxValue,
                dto.TargetValue,
                dto.Direction);

            rule.CriterionId = dto.CriterionId;
            rule.NumericType = dto.NumericType;
            rule.MinValue = dto.MinValue;
            rule.MaxValue = dto.MaxValue;
            rule.TargetValue = dto.TargetValue;
            rule.Direction = dto.Direction;

            await _repo.SaveChangesAsync();
        }

        private static void ValidateCriterionType(Criterion criterion)
        {
            if (criterion.CriterionType != CriterionType.Numerical)
            {
                throw new ArgumentException(
                    "A numerical rule can only be created for a numerical criterion.");
            }
        }

        private static void ValidateRule(
            NumericType numericType,
            decimal minValue,
            decimal maxValue,
            decimal? targetValue,
            Direction? direction)
        {
            if (!Enum.IsDefined(typeof(NumericType), numericType))
                throw new ArgumentException("Invalid numeric type.");

            if (minValue >= maxValue)
                throw new ArgumentException(
                    "Minimum value must be smaller than maximum value.");

            if (direction.HasValue &&
                !Enum.IsDefined(typeof(Direction), direction.Value))
            {
                throw new ArgumentException("Invalid direction.");
            }

            switch (numericType)
            {
                case NumericType.Scope:

                    if (!direction.HasValue)
                        throw new ArgumentException(
                            "Direction is required for a scope criterion.");

                    if (targetValue.HasValue)
                        throw new ArgumentException(
                            "Target value cannot be provided for a scope criterion.");

                    break;

                case NumericType.Interval:

                    if (targetValue.HasValue)
                        throw new ArgumentException(
                            "Target value cannot be provided for an interval criterion.");

                    if (direction.HasValue)
                        throw new ArgumentException(
                            "Direction cannot be provided for an interval criterion.");

                    break;

                case NumericType.TargetValue:

                    if (!targetValue.HasValue)
                        throw new ArgumentException(
                            "Target value is required for a target-value criterion.");

                    if (targetValue.Value < minValue ||
                        targetValue.Value > maxValue)
                    {
                        throw new ArgumentException(
                            "Target value must be within the minimum and maximum values.");
                    }

                    if (direction.HasValue)
                        throw new ArgumentException(
                            "Direction cannot be provided for a target-value criterion.");

                    break;

                default:

                    throw new ArgumentException(
                        "Invalid numeric type.");
            }
        }
    }
}
