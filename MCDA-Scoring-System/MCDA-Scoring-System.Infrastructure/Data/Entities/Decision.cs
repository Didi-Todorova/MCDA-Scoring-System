using static System.Runtime.InteropServices.JavaScript.JSType;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class Decision
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //public int UserId { get; set; }
        //public User User { get; set; } = null!;

        [Required]
        public WeightingMethod WeightingMethod { get; set; }

        [Required]
        public string Name { get; set; }
        public ICollection<Alternative> Alternatives { get; set; } = new List<Alternative>();
        public ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();
        public DateTime CreatedAt { get; set; }
    }
}
