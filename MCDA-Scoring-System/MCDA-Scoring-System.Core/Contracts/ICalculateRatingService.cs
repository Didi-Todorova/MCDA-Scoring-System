namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface ICalculateRatingService
    {
        public Task<decimal> CalculateRatingAsync(int alternativeId, int criterionId);
    }
}
