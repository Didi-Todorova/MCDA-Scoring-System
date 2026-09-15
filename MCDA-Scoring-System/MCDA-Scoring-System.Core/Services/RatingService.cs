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

            return alternativeValue.Criterion.CriterionType switch
            {
                CriterionType.Numerical =>
                    ChooseNumericalRatingMethod(
                        alternativeValue.Criterion
                            .CriterionNumericalRule?
                            .NumericType
                            ?? throw new InvalidOperationException(
                                "Numerical criterion has no numerical rule."),
                        alternativeValue),

                CriterionType.Categorical =>
                    CalculateRatingCategory(alternativeValue),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(alternativeValue.Criterion.CriterionType),
                    alternativeValue.Criterion.CriterionType,
                    "Unsupported criterion type.")
            };
        }

        private decimal ChooseNumericalRatingMethod(
            NumericType value,
            AlternativeValue alternativeValue)
        {
            var rule = alternativeValue.Criterion.CriterionNumericalRule;

            if (rule == null)
            {
                throw new InvalidOperationException(
                    "Numerical criterion has no numerical rule.");
            }

            if (alternativeValue.NumericValue == null)
            {
                throw new InvalidOperationException(
                    "Numerical criterion has no numeric value.");
            }

            var rawValue = alternativeValue.NumericValue.Value;

            switch (value)
            {
                case NumericType.Scope:

                    if (rule.Direction == null)
                    {
                        throw new InvalidOperationException(
                            "Direction is required for Scope rating.");
                    }

                    return CalculateRatingScope(
                        rawValue,
                        rule.MinValue,
                        rule.MaxValue,
                        rule.Direction.Value);

                case NumericType.Interval:

                    var intervalPosition = CheckIntervalAffiliation(
                        rawValue,
                        rule.IntervalRanges,
                        out int intervalNumber);

                    return CalculateRatingInterval(
                        intervalPosition,
                        intervalNumber);

                case NumericType.TargetValue:

                    if (rule.TargetValue == null)
                    {
                        throw new InvalidOperationException(
                            "Target value is required for TargetValue rating.");
                    }

                    return CalculateRatingTargetValue(
                        rawValue,
                        rule.TargetValue.Value,
                        rule.MinValue,
                        rule.MaxValue);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Unsupported numeric type.");
            }
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
            else
            {
                rating =
                    1m +
                    (maxValue - rawValue) * 4m /
                    (maxValue - minValue);
            }

            return Math.Round(rating, 2);
        }

        private decimal CalculateRatingInterval(
            int intervalPosition,
            int intervalNumber)
        {
            if (intervalPosition < 1 ||
                intervalPosition > intervalNumber)
            {
                throw new ArgumentException(
                    "The value does not belong to any interval range.");
            }

            if (intervalNumber < 2)
            {
                throw new InvalidOperationException(
                    "At least two interval ranges are required.");
            }

            decimal rating =
                5m -
                ((intervalPosition - 1) * 4m /
                (intervalNumber - 1));

            return Math.Round(rating, 2);
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

            if (rawValue < lowerBound || rawValue > upperBound)
            {
                rating = 1m;
            }
            else if (rawValue <= targetValue)
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

            return Math.Round(rating, 2);
        }

        private int CheckIntervalAffiliation(
            decimal rawValue,
            ICollection<IntervalRange> intervalRanges,
            out int intervalNumber)
        {
            var ranges = intervalRanges
                .OrderBy(r => r.Rank)
                .ToList();

            intervalNumber = ranges.Count;

            if (intervalNumber < 2)
            {
                throw new InvalidOperationException(
                    "At least two interval ranges are required.");
            }

            var selectedRange = ranges.FirstOrDefault(
                r =>
                    rawValue >= r.MinValue &&
                    rawValue <= r.MaxValue);

            if (selectedRange == null)
            {
                throw new ArgumentException(
                    $"Value {rawValue} does not belong to any interval range.");
            }

            return selectedRange.Rank;
        }

        private decimal CalculateRatingCategory(
            AlternativeValue alternativeValue)
        {
            var options = alternativeValue.Criterion.CriterionOptions;

            var selectedOption = options.FirstOrDefault(
                co => co.Id == alternativeValue.CriterionOptionId);

            if (selectedOption == null)
            {
                throw new InvalidOperationException(
                    "Selected criterion option was not found.");
            }

            int rank = selectedOption.Rank;
            int totalRanks = options.Count;

            if (totalRanks < 2)
            {
                throw new InvalidOperationException(
                    "At least two ranks are required.");
            }

            if (rank < 1 || rank > totalRanks)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rank),
                    "Rank must be between 1 and the total number of ranks.");
            }

            decimal rating =
                5m -
                ((rank - 1) * 4m /
                (totalRanks - 1));

            return Math.Round(rating, 2);
        }
    }
}