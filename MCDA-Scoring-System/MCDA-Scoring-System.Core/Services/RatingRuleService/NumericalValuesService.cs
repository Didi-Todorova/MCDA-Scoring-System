using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services.RatingRuleService
{
    public class NumericalValuesService : INumericValuesService
    {
        private readonly IAlternativeValueService _alternativeValueService;
        private readonly DbContext _dbContext;
        private readonly IRepository _repo;

        public NumericalValuesService(IRepository repo, IAlternativeValueService alternativeValueService)
        {
            _repo = repo;
            _alternativeValueService = alternativeValueService;
        }

        public async Task<decimal> CalculateRatingAsync(int alternativeId, int criterionId)
        {
            decimal rating = 0; 

            var alternativeValue = await _repo.AllReadonly<AlternativeValue>()
                .Include(av => av.Alternative)
                .Include(av => av.Criterion)
                .ThenInclude(c => c.CriterionNumericalRule)
                .ThenInclude(ncr => ncr.IntervalRanges)
                .FirstOrDefaultAsync(
                av => av.AlternativeId == alternativeId && av.CriterionId == criterionId);

            if (alternativeValue == null)
            {
                throw new KeyNotFoundException(
                    $"No value found for alternative {alternativeId} " +
                    $"and criterion {criterionId}.");
            }

            if (alternativeValue.Criterion.CriterionType != CriterionType.Numerical)
            {
                throw new InvalidOperationException(
                    $"Criterion {criterionId} is not numerical.");
            }

            var rule = alternativeValue.Criterion.CriterionNumericalRule;

            if (rule == null)
            {
                throw new InvalidOperationException(
                    $"No numerical rule exists for criterion {criterionId}.");
            }

            switch (alternativeValue.Criterion.CriterionType)
            {
                case CriterionType.Numerical:
                    rating = ChooseNumericalRatingMethod(alternativeValue.Criterion.CriterionNumericalRule.NumericType, alternativeValue);
                    break;
                case CriterionType.Categorical:
                    // Handle categorical rating logic
                    break;
            }

            return rating;
        }

        private decimal ChooseNumericalRatingMethod(NumericType value, AlternativeValue alternativeValue)
        {
             switch (value)
            {
                case NumericType.Scope:
                    return CalculateRatingScope(alternativeValue.NumericValue!.Value, alternativeValue.Criterion.CriterionNumericalRule.MinValue, alternativeValue.Criterion.CriterionNumericalRule.MaxValue);
                   
                case NumericType.Interval:
                    return CalculateRatingInterval(CheckIntervalAffiliation(alternativeValue.NumericValue!.Value, alternativeValue.Criterion.CriterionNumericalRule.IntervalRanges, out int intervalPosition, out int intervalNumber), alternativeValue.Criterion.CriterionNumericalRule.IntervalRanges.Count);
                    
                case NumericType.TargetValue:
                   return CalculateRatingTargetValue(alternativeValue.NumericValue!.Value, alternativeValue.Criterion.CriterionNumericalRule.TargetValue!.Value, alternativeValue.Criterion.CriterionNumericalRule.MinValue, alternativeValue.Criterion.CriterionNumericalRule.MaxValue);
                   
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }



        private decimal CalculateRatingScope(decimal rawValue, decimal minValue, decimal maxValue)
        {
            decimal rating = 1 + (rawValue - minValue) * 4m / (maxValue - minValue);
            return Math.Round(rating, 2);
        }

        private decimal CalculateRatingInterval(int intervalPosition, int intervalNumber)
        {
            decimal rating = 1 + (decimal)(intervalPosition - 1) * 4m / (intervalNumber - 1);
            return Math.Round(rating, 2);
        }

        private decimal CalculateRatingTargetValue(decimal rawValue, decimal targetValue, decimal lowerBound, decimal upperBound)
        {
            decimal rating = 0;

            if (rawValue >= lowerBound && rawValue <= targetValue)
            {
                rating = 1 + (decimal)(rawValue - lowerBound) * 4m / (targetValue - lowerBound);
            }
            else if (rawValue > targetValue && rawValue <= upperBound)
            {
                rating = 1 + (decimal)(upperBound - rawValue) * 4m / (upperBound - targetValue);
            }

            return Math.Round(rating, 2);
        }

        private int CheckIntervalAffiliation(decimal rawValue, ICollection<IntervalRange> intervalRanges, out int intervalPosition, out int intervalNumber)
        {
            var ranges = intervalRanges.ToList();
            intervalPosition = 0;
            intervalNumber = intervalRanges.Count;
            for (int i = 0; i < ranges.Count; i++)
            {
                if (rawValue >= ranges[i].MinValue && rawValue <= ranges[i].MaxValue)
                {
                    intervalPosition = i + 1; // Position is 1-based
                    break;
                }
            }
            return intervalPosition;
        }
    }
}
