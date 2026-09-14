using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion.Weights;

public interface IPercentageAllocationService
{
    Task SetWeightsAsync(
        int decisionId,
        List<CriterionPercentageAllocationDto> weights);
}