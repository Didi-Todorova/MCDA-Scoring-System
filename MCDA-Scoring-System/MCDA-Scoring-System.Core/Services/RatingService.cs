using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services.RatingRuleService
{
    public class RatingService : IRatingService
    {
        private readonly IRepository _repo;

        public RatingService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<decimal> CalculateRatingAsync(
            int alternativeId,
            int criterionId)
        {
            var alternativeValue = await _repo
                .AllReadonly<AlternativeValue>()
                .Include(av => av.Alternative)
                .Include(av => av.Criterion)
                    .ThenInclude(c => c.CriterionNumericalRule)
                        .ThenInclude(ncr => ncr.IntervalRanges)
                .Include(av => av.Criterion)
                    .ThenInclude(c => c.CriterionOptions)
                .FirstOrDefaultAsync(
                    av =>
                        av.AlternativeId == alternativeId &&
                        av.CriterionId == criterionId);

            if (alternativeValue == null)
            {
                throw new KeyNotFoundException(
                    $"No value found for alternative {alternativeId} " +
                    $"and criterion {criterionId}.");
            }

            var alternative = alternativeValue.Alternative;
            var criterion = alternativeValue.Criterion;

            if (alternative == null)
            {
                throw new InvalidOperationException(
                    "The alternative value is not linked to an alternative.");
            }

            if (criterion == null)
            {
                throw new InvalidOperationException(
                    "The alternative value is not linked to a criterion.");
            }

            if (alternative.DecisionId !=
                criterion.DecisionId)
            {
                throw new InvalidOperationException(
                    "The alternative and criterion do not belong to the same decision.");
            }

            return criterion.CriterionType switch
            {
                CriterionType.Numerical =>
                    CalculateNumericalRating(
                        criterion,
                        alternativeValue),

                CriterionType.Categorical =>
                    CalculateCategoricalRating(
                        criterion,
                        alternativeValue),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(criterion.CriterionType),
                    criterion.CriterionType,
                    "Unsupported criterion type.")
            };
        }

        private decimal CalculateNumericalRating(
            Criterion criterion,
            AlternativeValue alternativeValue)
        {
            var rule = criterion.CriterionNumericalRule;

            if (rule == null)
            {
                throw new InvalidOperationException(
                    $"Numerical criterion '{criterion.Name}' has no numerical rule.");
            }

            ValidateNumericalRule(rule);

            if (!alternativeValue.NumericValue.HasValue)
            {
                throw new InvalidOperationException(
                    $"Numerical criterion '{criterion.Name}' has no numeric value.");
            }

            if (alternativeValue.CriterionOptionId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Numerical criterion '{criterion.Name}' cannot use a categorical option.");
            }

            var rawValue = alternativeValue.NumericValue.Value;

            if (rawValue < rule.MinValue ||
                rawValue > rule.MaxValue)
            {
                throw new ArgumentException(
                    $"Value {rawValue} for criterion '{criterion.Name}' " +
                    $"must be between {rule.MinValue} and {rule.MaxValue}.");
            }

            return rule.NumericType switch
            {
                NumericType.Scope =>
                    CalculateRatingScope(
                        rawValue,
                        rule.MinValue,
                        rule.MaxValue,
                        rule.Direction!.Value),

                NumericType.Interval =>
                    CalculateRatingInterval(
                        rawValue,
                        rule.IntervalRanges),

                NumericType.TargetValue =>
                    CalculateRatingTargetValue(
                        rawValue,
                        rule.TargetValue!.Value,
                        rule.MinValue,
                        rule.MaxValue),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(rule.NumericType),
                    rule.NumericType,
                    "Unsupported numeric type.")
            };
        }

        private decimal CalculateRatingScope(
            decimal rawValue,
            decimal minValue,
            decimal maxValue,
            Direction direction)
        {
            if (maxValue <= minValue)
            {
                throw new ArgumentException(
                    "Max value must be greater than min value.");
            }

            decimal rating;

            if (direction == Direction.Maximize)
            {
                rating =
                    1m +
                    (rawValue - minValue) * 4m /
                    (maxValue - minValue);
            }
            else if (direction == Direction.Minimize)
            {
                rating =
                    1m +
                    (maxValue - rawValue) * 4m /
                    (maxValue - minValue);
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    nameof(direction),
                    direction,
                    "Unsupported direction.");
            }

            return Math.Round(
                Math.Clamp(rating, 1m, 5m),
                2);
        }

        private decimal CalculateRatingInterval(
            decimal rawValue,
            ICollection<IntervalRange>? intervalRanges)
        {
            if (intervalRanges == null)
            {
                throw new InvalidOperationException(
                    "Interval rating has no configured ranges.");
            }

            var ranges = intervalRanges
                .OrderBy(r => r.Rank)
                .ToList();

            if (ranges.Count < 2)
            {
                throw new InvalidOperationException(
                    "At least two interval ranges are required.");
            }

            ValidateIntervalRanges(ranges);

            var selectedRange = ranges.FirstOrDefault(
                r =>
                    rawValue >= r.MinValue &&
                    rawValue <= r.MaxValue);

            if (selectedRange == null)
            {
                throw new ArgumentException(
                    $"Value {rawValue} does not belong to any interval range.");
            }

            var intervalPosition = ranges.IndexOf(selectedRange) + 1;

            decimal rating =
                5m -
                ((intervalPosition - 1) * 4m /
                (ranges.Count - 1));

            return Math.Round(
                Math.Clamp(rating, 1m, 5m),
                2);
        }

        private decimal CalculateRatingTargetValue(
            decimal rawValue,
            decimal targetValue,
            decimal lowerBound,
            decimal upperBound)
        {
            if (upperBound <= lowerBound)
            {
                throw new ArgumentException(
                    "Upper bound must be greater than lower bound.");
            }

            if (targetValue < lowerBound ||
                targetValue > upperBound)
            {
                throw new ArgumentException(
                    "Target value must be within the specified bounds.");
            }

            decimal rating;

            if (rawValue <= targetValue)
            {
                if (targetValue == lowerBound)
                {
                    rating = 5m;
                }
                else
                {
                    rating =
                        1m +
                        (rawValue - lowerBound) * 4m /
                        (targetValue - lowerBound);
                }
            }
            else
            {
                if (targetValue == upperBound)
                {
                    rating = 5m;
                }
                else
                {
                    rating =
                        1m +
                        (upperBound - rawValue) * 4m /
                        (upperBound - targetValue);
                }
            }

            return Math.Round(
                Math.Clamp(rating, 1m, 5m),
                2);
        }

        private decimal CalculateCategoricalRating(
            Criterion criterion,
            AlternativeValue alternativeValue)
        {
            if (alternativeValue.NumericValue.HasValue)
            {
                throw new InvalidOperationException(
                    $"Categorical criterion '{criterion.Name}' cannot use a numeric value.");
            }

            if (!alternativeValue.CriterionOptionId.HasValue)
            {
                throw new InvalidOperationException(
                    $"Categorical criterion '{criterion.Name}' has no selected option.");
            }

            var options = criterion.CriterionOptions
                .OrderBy(co => co.Rank)
                .ToList();

            if (options.Count < 2)
            {
                throw new InvalidOperationException(
                    $"Categorical criterion '{criterion.Name}' must have at least two options.");
            }

            ValidateCriterionOptions(options);

            var selectedOption = options.FirstOrDefault(
                co => co.Id == alternativeValue.CriterionOptionId.Value);

            if (selectedOption == null)
            {
                throw new InvalidOperationException(
                    $"Selected criterion option does not belong to criterion '{criterion.Name}'.");
            }

            decimal rating =
                5m -
                ((selectedOption.Rank - 1) * 4m /
                (options.Count - 1));

            return Math.Round(
                Math.Clamp(rating, 1m, 5m),
                2);
        }

        private static void ValidateNumericalRule(
            CriterionNumericalRule rule)
        {
            if (rule.MaxValue <= rule.MinValue)
            {
                throw new ArgumentException(
                    "Numerical rule max value must be greater than min value.");
            }

            switch (rule.NumericType)
            {
                case NumericType.Scope:

                    if (!rule.Direction.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Direction is required for Scope rating.");
                    }

                    if (rule.TargetValue.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Target value must not be set for Scope rating.");
                    }

                    break;

                case NumericType.Interval:

                    if (rule.Direction.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Direction must not be set for Interval rating.");
                    }

                    if (rule.TargetValue.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Target value must not be set for Interval rating.");
                    }

                    if (rule.IntervalRanges == null ||
                        rule.IntervalRanges.Count < 2)
                    {
                        throw new InvalidOperationException(
                            "At least two interval ranges are required.");
                    }

                    break;

                case NumericType.TargetValue:

                    if (!rule.TargetValue.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Target value is required for Target Value rating.");
                    }

                    if (rule.Direction.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Direction must not be set for Target Value rating.");
                    }

                    if (rule.TargetValue.Value < rule.MinValue ||
                        rule.TargetValue.Value > rule.MaxValue)
                    {
                        throw new ArgumentException(
                            "Target value must be within the specified bounds.");
                    }

                    break;

                default:

                    throw new ArgumentOutOfRangeException(
                        nameof(rule.NumericType),
                        rule.NumericType,
                        "Unsupported numeric type.");
            }
        }

        private static void ValidateIntervalRanges(
            List<IntervalRange> ranges)
        {
            var expectedRank = 1;

            foreach (var range in ranges)
            {
                if (range.Rank != expectedRank)
                {
                    throw new InvalidOperationException(
                        "Interval range ranks must be consecutive starting from 1.");
                }

                if (range.MinValue >= range.MaxValue)
                {
                    throw new ArgumentException(
                        "Each interval range must have a minimum value smaller than its maximum value.");
                }

                expectedRank++;
            }

            for (int i = 1; i < ranges.Count; i++)
            {
                if (ranges[i].MinValue <=
                    ranges[i - 1].MaxValue)
                {
                    throw new ArgumentException(
                        "Interval ranges must not overlap.");
                }
            }
        }

        private static void ValidateCriterionOptions(
            List<CriterionOption> options)
        {
            var expectedRank = 1;

            foreach (var option in options)
            {
                if (option.Rank != expectedRank)
                {
                    throw new InvalidOperationException(
                        "Criterion option ranks must be consecutive starting from 1.");
                }

                expectedRank++;
            }
        }
    }
}