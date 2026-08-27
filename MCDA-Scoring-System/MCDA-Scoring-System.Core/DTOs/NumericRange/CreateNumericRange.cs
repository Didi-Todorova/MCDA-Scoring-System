namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange
{
    public record CreateNumericRange
    (
        int NumericalCriterionRuleId,
        decimal Minimum,
        decimal Maximum
    );
}
