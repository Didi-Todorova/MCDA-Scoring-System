namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class AlternativeValue
    {
        public int ID { get; set; }
        public int AlternativeId { get; set; }
        public Alternative Alternative { get; set; } = null!;
        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;   
        public decimal? NumericValue { get; set; }
        public int? CriterionOptionId { get; set; }
        public CriterionOption CriterionOption { get; set; } = null!;
    }
}
