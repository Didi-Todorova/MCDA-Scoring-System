
namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class Alternative
    {
        public int Id { get; set; }  
        public int DecisionId { get; set; }
        public Decision Decision { get; set; } = null!;
        public string Name { get; set; }
        public ICollection<AlternativeValue> AlternativeValues { get; set; } = new List<AlternativeValue>();
    }
}
