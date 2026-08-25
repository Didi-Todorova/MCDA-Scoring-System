using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities

{
    public class Criterion
    {
        public int Id { get; set; }
        public int DecisionId { get; set; }
        public Decision Decision { get; set; } = null!;
        public string Name { get; set; }
        public CriterionType CriterionType { get; set; }
        public decimal Weight { get; set; }
        public ICollection<AlternativeValue> AlternativeValues { get; set; } = new List<AlternativeValue>();
        public ICollection<CriterionOption> CriterionOptions { get; set; } = new List<CriterionOption>();
        public ICollection<NumericalCriterionRule> NumericalCriterionRules { get; set; } = new List<NumericalCriterionRule>();
    }
}
