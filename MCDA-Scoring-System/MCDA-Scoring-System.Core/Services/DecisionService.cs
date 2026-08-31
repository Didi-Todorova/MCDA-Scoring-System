using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Decision;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class DecisionService : IDecisionService
    {
        private readonly IRepository repo;

        public DecisionService(IRepository repo)
        {
            this.repo = repo;
        }

        public async Task<DecisionDto> CreateDecisionAsync(CreateDecisionDto createDecisionDto)
        {
            var decision = new Decision
            {
                Name = createDecisionDto.Name,
                WeightingMethod = createDecisionDto.WeightingMethod,
            };

            repo.AddAsync(decision);
            repo.SaveChangesAsync();

            return new DecisionDto(
                decision.Id,
                decision.Name,
                decision.WeightingMethod
            );
        }

        public async Task DeleteDecisionAsync(int id)
        {
            var decision = await repo.GetByIdAsync<Decision>(id) ?? throw new ArgumentException($"Decision with ID {id} not found.");

            await repo.DeleteAsync<Decision>(id);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<DecisionDto>> GetAllDecisionsAsync()
        {
            return await repo.AllReadonly<Decision>()
                .Select(d => new DecisionDto(
                    d.Id,
                    d.Name,
                    d.WeightingMethod
                ))
                .ToListAsync();
        }

        public async Task<DecisionDto?> GetDecisionByIdAsync(int id)
        {
            return await repo.AllReadonly<Decision>()
                .Where(d => d.Id == id)
                .Select(d => new DecisionDto(
                    d.Id,
                    d.Name,
                    d.WeightingMethod
                ))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateDecisionAsync(int id, UpdateDecisionDto updateDecisionDto)
        {
            var decision = await repo.GetByIdAsync<Decision>(id) ?? throw new ArgumentException($"Decision with ID {id} not found.");

            decision.Name = updateDecisionDto.Name;
            decision.WeightingMethod = updateDecisionDto.WeightingMethod;

            await repo.SaveChangesAsync();
        }
    }
}
