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

            ValidateNumericRule(
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

            if (dto.CriterionId.HasValue &&
                dto.CriterionId.Value != rule.CriterionId)
            {
                throw new ArgumentException(
                    "A numerical rule cannot be moved to another criterion.");
            }

            var numericType = dto.NumericType ?? rule.NumericType;
            var minValue = dto.MinValue ?? rule.MinValue;
            var maxValue = dto.MaxValue ?? rule.MaxValue;
            var targetValue = dto.TargetValue ?? rule.TargetValue;
            var direction = dto.Direction ?? rule.Direction;

            var criterion = await _repo.GetByIdAsync<Criterion>(
                rule.CriterionId);

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {rule.CriterionId} not found.");

            ValidateCriterionType(criterion);

            if (rule.NumericType == NumericType.Interval &&
                numericType != NumericType.Interval)
            {
                var hasRanges = await _repo
                    .AllReadonly<IntervalRange>()
                    .AnyAsync(r =>
                        r.CriterionNumericalRuleId == rule.Id);

                if (hasRanges)
                {
                    throw new ArgumentException(
                        "An interval numerical rule with existing interval ranges cannot be changed to another numeric type.");
                }
            }

            ValidateNumericRule(
                numericType,
                minValue,
                maxValue,
                targetValue,
                direction);

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

            if (rule.CriterionId != dto.CriterionId)
            {
                throw new ArgumentException(
                    "A numerical rule cannot be moved to another criterion.");
            }

            if (criterion == null)
                throw new KeyNotFoundException(
                    $"Criterion with ID {dto.CriterionId} not found.");

            ValidateCriterionType(criterion);

            if (rule.NumericType == NumericType.Interval && dto.NumericType != NumericType.Interval)
            {
                var hasRanges = await _repo
                    .AllReadonly<IntervalRange>()
                    .AnyAsync(r =>
                        r.CriterionNumericalRuleId == rule.Id);

                if (hasRanges)
                {
                    throw new ArgumentException(
                        "An interval numerical rule with existing interval ranges cannot be changed to another numeric type.");
                }
            }

            ValidateNumericRule(
                dto.NumericType,
                dto.MinValue,
                dto.MaxValue,
                dto.TargetValue,
                dto.Direction);

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

        private static void ValidateNumericRule(
    NumericType numericType,
    decimal minValue,
    decimal maxValue,
    decimal? targetValue,
    Direction? direction)
        {
            if (!Enum.IsDefined(numericType))
            {
                throw new ArgumentException(
                    "Invalid numeric type.");
            }

            if (minValue >= maxValue)
            {
                throw new ArgumentException(
                    "Minimum value must be smaller than maximum value.");
            }

            switch (numericType)
            {
                case NumericType.Scope:

                    if (!direction.HasValue)
                    {
                        throw new ArgumentException(
                            "Direction is required for a scope numerical rule.");
                    }

                    if (targetValue.HasValue)
                    {
                        throw new ArgumentException(
                            "Target value cannot be provided for a scope numerical rule.");
                    }

                    break;

                case NumericType.Interval:

                    if (direction.HasValue)
                    {
                        throw new ArgumentException(
                            "Direction cannot be provided for an interval numerical rule.");
                    }

                    if (targetValue.HasValue)
                    {
                        throw new ArgumentException(
                            "Target value cannot be provided for an interval numerical rule.");
                    }

                    break;

                case NumericType.TargetValue:

                    if (!targetValue.HasValue)
                    {
                        throw new ArgumentException(
                            "Target value is required for a target-value numerical rule.");
                    }

                    if (direction.HasValue)
                    {
                        throw new ArgumentException(
                            "Direction cannot be provided for a target-value numerical rule.");
                    }

                    if (targetValue.Value < minValue ||
                        targetValue.Value > maxValue)
                    {
                        throw new ArgumentException(
                            "Target value must be between the minimum and maximum values.");
                    }

                    break;

                default:

                    throw new ArgumentException(
                        "Invalid numeric type.");
            }
        }
    }
}
