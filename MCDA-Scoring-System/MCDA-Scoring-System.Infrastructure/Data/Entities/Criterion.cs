namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class Criterion
    {
        public int ID { get; set; }
        public int DecisionID { get; set; }
        public Decision Decision { get; set; } = null!;

        public string Name { get; set; }
        public CriterionType Type { get; set; }
        public string Description { get; set; }
        public RatingMethod? RatingMethod { get; set; }
        public decimal Weight { get; set; }
    }

    public enum CriterionType
    {
        Numerical,
        Categorical,
        Ordinal
    }

    public enum RatingMethod
    {
        Scope, 
        Interval,
        TargetValue
    }
}
