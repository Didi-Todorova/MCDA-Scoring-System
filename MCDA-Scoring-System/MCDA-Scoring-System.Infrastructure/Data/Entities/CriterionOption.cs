namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class CriterionOption
    {
        public int Id { get; set; }
        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;
        public string Value { get; set; }
        public int Rank { get; set; }     
    }
}
