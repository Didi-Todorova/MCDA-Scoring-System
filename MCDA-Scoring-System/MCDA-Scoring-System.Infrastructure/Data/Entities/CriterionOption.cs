using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class CriterionOption
    {
        [Key]
        public int Id { get; set; }
        public ICollection<AlternativeValue> AlternativeValues { get; set; } = new List<AlternativeValue>();

        [Required]
        public int CriterionId { get; set; }
        public Criterion Criterion { get; set; } = null!;

        [Required]
        public string Value { get; set; }

        [Required]
        public int Rank { get; set; }     
    }
}
