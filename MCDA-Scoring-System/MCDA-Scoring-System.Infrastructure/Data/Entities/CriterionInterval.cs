namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class CriterionInterval
    {
        public int Id { get; set; }
        public int CriterionId { get; set; }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public string Description { get; set; } = string.Empty;
        // Navigation property
        public Criterion Criterion { get; set; } = null!;
    }
}
