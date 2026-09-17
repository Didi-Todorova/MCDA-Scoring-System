using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities

{
    public class Criterion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DecisionId { get; set; }
        public Decision Decision { get; set; } = null!;

        [Required]
        public string Name { get; set; }

        [Required]
        public CriterionType CriterionType { get; set; }
        public string? Unit { get; set; }
        public decimal? Weight { get; set; }
        public ICollection<AlternativeValue> AlternativeValues { get; set; } = new List<AlternativeValue>();
        public ICollection<CriterionOption> CriterionOptions { get; set; } = new List<CriterionOption>();
        public CriterionNumericalRule? CriterionNumericalRule { get; set; }
    }
}
