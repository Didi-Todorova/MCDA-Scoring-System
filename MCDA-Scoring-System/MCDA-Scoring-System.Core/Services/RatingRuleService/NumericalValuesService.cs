using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services.RatingRuleService
{
    public class NumericalValuesService
    {
        private readonly IAlternativeValueService _alternativeValueService;
        private readonly DbContext _dbContext;
        private readonly IRepository _repo;

        public NumericalValuesService(IAlternativeValueService alternativeValueService)
        {
            _alternativeValueService = alternativeValueService;
        }

        public async Task<decimal> CalculateRatingAsync(int alternativeId, int criterionId)
        {
            var alternativeValue = await _repo.AllReadonly<AlternativeValue>()
                .Include(av => av.Alternative)
                .Include(av => av.Criterion)
                .ThenInclude(c => c.CriterionNumericalRule)
                .ThenInclude(ncr => ncr.IntervalRanges)
                .FirstOrDefaultAsync(
                av => av.AlternativeId == alternativeId && av.CriterionId == criterionId);


            switch (alternativeValue.Criterion.CriterionType)
            {
                case CriterionType.Numerical:
                    ChooseNumericalRatingMethod(alternativeValue.Criterion.CriterionNumericalRule.NumericType, alternativeValue);
                    break;
                case CriterionType.Categorical:
                    // Handle categorical rating logic
                    break;
            }
        }

        private void ChooseNumericalRatingMethod(NumericType value, AlternativeValue alternativeValue)
        {
            switch (value)
            {
                case NumericType.Scope:
                    CalculateRatingScope(alternativeValue.NumericValue, alternativeValue.Criterion.CriterionNumericalRule.NumericRange.MinValue, alternativeValue.Criterion.CriterionNumericalRule.MaxValue);
                    break;
                case NumericType.Interval:
                    CalculateRatingInterval(alternativeValue.NumericValue, alternativeValue.Criterion.CriterionNumericalRule.IntervalNumber);
                    break;
                case NumericType.TargetValue:
                    CalculateRatingTargetValue(alternativeValue.NumericValue alternativeValue.Criterion.CriterionNumericalRule.TargetValue, alternativeValue.Criterion.CriterionNumericalRule.MinValue, alternativeValue.Criterion.CriterionNumericalRule.MaxValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        private decimal CalculateRatingTargetValue(int rawValue, int targetValue, int lowerBound, int upperBound)
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

        private decimal CalculateRatingInterval(int intervalPosition, int intervalNumber)
        {
            decimal rating = 1 + (decimal)(intervalPosition - 1) * 4m / (intervalNumber - 1);
            return Math.Round(rating, 2);
        }

        private decimal CalculateRatingScope(int rawValue, int minValue, int maxValue)
        {
            decimal rating = 1 + (rawValue - minValue) * 4m / (maxValue - minValue);
            return Math.Round(rating, 2);
        }


    }
}
