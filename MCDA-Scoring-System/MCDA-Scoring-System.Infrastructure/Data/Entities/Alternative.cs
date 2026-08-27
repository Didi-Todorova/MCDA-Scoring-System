
using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class Alternative
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DecisionId { get; set; }
        public Decision Decision { get; set; } = null!;
        [Required]
        public string Name { get; set; }
        public ICollection<AlternativeValue> AlternativeValues { get; set; } = new List<AlternativeValue>();
    }
}
