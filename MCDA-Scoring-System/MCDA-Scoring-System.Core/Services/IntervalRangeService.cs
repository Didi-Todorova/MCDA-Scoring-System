using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class IntervalRangeService : IIntervalRangeService
    {
        private readonly IRepository _repo;

        public IntervalRangeService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<IntervalRangeDto>>
            CreateIntervalRangesAsync(CreateIntervalRangesDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var rule = await _repo
                .GetByIdAsync<CriterionNumericalRule>(
                    dto.CriterionNumericalRuleId);

            if (rule == null)
                throw new KeyNotFoundException(
                    $"Criterion Numerical Rule with ID " +
                    $"{dto.CriterionNumericalRuleId} not found.");

            ValidateNumericType(rule);

            if (dto.Ranges == null || dto.Ranges.Count < 2)
                throw new ArgumentException(
                    "At least two interval ranges are required.");

            var existingRanges = await _repo
                .AllReadonly<IntervalRange>()
                .AnyAsync(r =>
                    r.CriterionNumericalRuleId ==
                    dto.CriterionNumericalRuleId);

            if (existingRanges)
                throw new ArgumentException(
                    "This numerical rule already has interval ranges.");

            ValidateRanges(
    dto.Ranges,
    rule.MinValue,
    rule.MaxValue);

            var ranges = dto.Ranges
                .Select((range, index) => new IntervalRange
                {
                    CriterionNumericalRuleId =
                        dto.CriterionNumericalRuleId,

                    MinValue = range.MinValue,
                    MaxValue = range.MaxValue,

                    Rank = index + 1
                })
                .ToList();

            await _repo.AddRangeAsync(ranges);
            await _repo.SaveChangesAsync();

            return ranges
                .OrderBy(r => r.Rank)
                .Select(r => new IntervalRangeDto(
                    r.Id,
                    r.CriterionNumericalRuleId,
                    r.MinValue,
                    r.MaxValue,
                    r.Rank
                ));
        }

        public async Task<IEnumerable<IntervalRangeDto>>
            GetAllIntervalRangesAsync()
        {
            return await _repo.AllReadonly<IntervalRange>()
                .OrderBy(r => r.CriterionNumericalRuleId)
                .ThenBy(r => r.Rank)
                .Select(r => new IntervalRangeDto(
                    r.Id,
                    r.CriterionNumericalRuleId,
                    r.MinValue,
                    r.MaxValue,
                    r.Rank
                ))
                .ToListAsync();
        }

        public async Task<IntervalRangeDto?>
            GetIntervalRangeByIdAsync(int id)
        {
            return await _repo.AllReadonly<IntervalRange>()
                .Where(r => r.Id == id)
                .Select(r => new IntervalRangeDto(
                    r.Id,
                    r.CriterionNumericalRuleId,
                    r.MinValue,
                    r.MaxValue,
                    r.Rank
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<IntervalRangeDto>>
            GetIntervalRangesByRuleIdAsync(
                int criterionNumericalRuleId)
        {
            return await _repo.AllReadonly<IntervalRange>()
                .Where(r =>
                    r.CriterionNumericalRuleId ==
                    criterionNumericalRuleId)
                .OrderBy(r => r.Rank)
                .Select(r => new IntervalRangeDto(
                    r.Id,
                    r.CriterionNumericalRuleId,
                    r.MinValue,
                    r.MaxValue,
                    r.Rank
                ))
                .ToListAsync();
        }

        public async Task UpdateIntervalRangesAsync(
            int criterionNumericalRuleId,
            UpdateIntervalRangesDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var rule = await _repo
                .GetByIdAsync<CriterionNumericalRule>(
                    criterionNumericalRuleId);

            if (rule == null)
                throw new KeyNotFoundException(
                    $"Criterion Numerical Rule with ID " +
                    $"{criterionNumericalRuleId} not found.");

            ValidateNumericType(rule);

            var ranges = await _repo.All<IntervalRange>()
                .Where(r =>
                    r.CriterionNumericalRuleId ==
                    criterionNumericalRuleId)
                .ToListAsync();

            if (ranges.Count == 0)
                throw new KeyNotFoundException(
                    $"No interval ranges found for " +
                    $"criterion numerical rule {criterionNumericalRuleId}.");

            if (dto.Ranges == null ||
                dto.Ranges.Count != ranges.Count)
            {
                throw new ArgumentException(
                    "All interval ranges must be provided.");
            }

            var existingIds = ranges
                .Select(r => r.Id)
                .ToHashSet();

            var submittedIds = dto.Ranges
                .Select(r => r.Id)
                .ToHashSet();

            if (!existingIds.SetEquals(submittedIds))
                throw new ArgumentException(
                    "The submitted interval ranges do not match " +
                    "the existing ranges.");

            ValidateRanges(
            dto.Ranges,
            rule.MinValue,
            rule.MaxValue);

            var rangesById = ranges.ToDictionary(r => r.Id);

            for (int i = 0; i < dto.Ranges.Count; i++)
            {
                var input = dto.Ranges[i];
                var range = rangesById[input.Id];

                range.MinValue = input.MinValue;
                range.MaxValue = input.MaxValue;
                range.Rank = i + 1;
            }

            await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteIntervalRangeAsync(int id)
        {
            var range = await _repo.GetByIdAsync<IntervalRange>(id);

            if (range == null)
                return false;

            var remainingRanges = await _repo.All<IntervalRange>()
                .Where(r =>
                    r.CriterionNumericalRuleId ==
                    range.CriterionNumericalRuleId &&
                    r.Id != id)
                .OrderBy(r => r.Rank)
                .ToListAsync();

            _repo.Delete(range);

            for (int i = 0; i < remainingRanges.Count; i++)
            {
                remainingRanges[i].Rank = i + 1;
            }

            await _repo.SaveChangesAsync();

            return true;
        }

        private static void ValidateNumericType(
            CriterionNumericalRule rule)
        {
            if (rule.NumericType != NumericType.Interval)
            {
                throw new ArgumentException(
                    "Interval ranges can only be created for an interval numerical rule.");
            }
        }

        private static void ValidateRanges(
    IEnumerable<IntervalRangeInputDto> ranges,
    decimal ruleMinValue,
    decimal ruleMaxValue)
        {
            var rangeList = ranges.ToList();

            ValidateRanges(
                rangeList.Select(r =>
                    (r.MinValue, r.MaxValue)),
                ruleMinValue,
                ruleMaxValue);
        }

        private static void ValidateRanges(
            IEnumerable<IntervalRangeUpdateDto> ranges,
            decimal ruleMinValue,
            decimal ruleMaxValue)
        {
            var rangeList = ranges.ToList();

            ValidateRanges(
                rangeList.Select(r =>
                    (r.MinValue, r.MaxValue)),
                ruleMinValue,
                ruleMaxValue);
        }

        private static void ValidateRanges(
            IEnumerable<(decimal MinValue, decimal MaxValue)> ranges,
            decimal ruleMinValue,
            decimal ruleMaxValue)
        {
            var orderedRanges = ranges
                .OrderBy(r => r.MinValue)
                .ToList();

            if (orderedRanges.Count < 2)
            {
                throw new ArgumentException(
                    "At least two interval ranges are required.");
            }

            for (int i = 0; i < orderedRanges.Count; i++)
            {
                var current = orderedRanges[i];

                if (current.MinValue >= current.MaxValue)
                {
                    throw new ArgumentException(
                        "Each interval range must have a minimum value smaller than its maximum value.");
                }

                if (current.MinValue < ruleMinValue ||
                    current.MaxValue > ruleMaxValue)
                {
                    throw new ArgumentException(
                        $"Interval ranges must stay within the numerical rule range of {ruleMinValue} to {ruleMaxValue}.");
                }

                if (i == 0)
                {
                    if (current.MinValue != ruleMinValue)
                    {
                        throw new ArgumentException(
                            $"Interval ranges must start at the numerical rule minimum of {ruleMinValue}.");
                    }

                    continue;
                }

                var previous = orderedRanges[i - 1];

                if (current.MinValue < previous.MaxValue)
                {
                    throw new ArgumentException(
                        "Interval ranges cannot overlap.");
                }

                if (current.MinValue > previous.MaxValue)
                {
                    throw new ArgumentException(
                        "Interval ranges must cover the complete numerical rule range without gaps.");
                }
            }

            var lastRange = orderedRanges[^1];

            if (lastRange.MaxValue != ruleMaxValue)
            {
                throw new ArgumentException(
                    $"Interval ranges must end at the numerical rule maximum of {ruleMaxValue}.");
            }
        }      
    }
}