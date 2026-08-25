using static System.Runtime.InteropServices.JavaScript.JSType;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class Decision
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public WeightingMethod WeightingMethod { get; set; }
        public string Name { get; set; }
        public ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();
        public ICollection<Alternative> Alternatives { get; set; } = new List<Alternative>();
        public DateTime CreatedAt { get; set; }
    }
}
