using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class ScoringService
    {
        private readonly IRepository _repo;

        public ScoringService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task CalculateScoreAsync()
    }
}
