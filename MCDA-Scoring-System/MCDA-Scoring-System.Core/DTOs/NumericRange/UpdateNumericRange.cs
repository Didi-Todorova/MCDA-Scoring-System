namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange
{
    public record UpdateNumericRange
    (
        int NumericalCriterionRuleId,
        decimal Minimum,
        decimal Maximum
    );
}
