using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities
{
    public class Decision
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}
