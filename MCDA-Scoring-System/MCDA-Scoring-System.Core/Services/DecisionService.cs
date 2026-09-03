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
        private readonly IRepository _repo;

        public DecisionService(IRepository repo)
        {
            this._repo = repo;
        }

        public async Task<DecisionDto> CreateDecisionAsync(CreateDecisionDto dto)
        {
            var decision = new Decision
            {
                Name = dto.Name,
                WeightingMethod = dto.WeightingMethod,
            };

            await _repo.AddAsync(decision);
            await _repo.SaveChangesAsync();

            return new DecisionDto(
                decision.Id,
                decision.Name,
                decision.WeightingMethod
            );
        }

        public async Task<bool> DeleteDecisionAsync(int id)
        {
            var decision = await _repo.GetByIdAsync<Decision>(id);

            if (decision is null)
                return false;

            await _repo.DeleteAsync<Decision>(id);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DecisionDto>> GetAllDecisionsAsync()
        {
            return await _repo.AllReadonly<Decision>()
                .Select(d => new DecisionDto(
                    d.Id,
                    d.Name,
                    d.WeightingMethod
                ))
                .ToListAsync();
        }

        public async Task<DecisionDto?> GetDecisionByIdAsync(int id)
        {
            return await _repo.AllReadonly<Decision>()
                .Where(d => d.Id == id)
                .Select(d => new DecisionDto(
                    d.Id,
                    d.Name,
                    d.WeightingMethod
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PartialUpdateDecisionAsync(int id, PatchDecisionDto dto)
        {
            var decision = await _repo.GetByIdAsync<Decision>(id) ?? throw new ArgumentException($"Decision with ID {id} not found.");

            if (dto.Name != null)
                decision.Name = dto.Name;
            if (dto.WeightingMethod != null)
                decision.WeightingMethod = (WeightingMethod)dto.WeightingMethod;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateDecisionAsync(int id, UpdateDecisionDto dto)
        {
            var decision = await _repo.GetByIdAsync<Decision>(id) ?? throw new ArgumentException($"Decision with ID {id} not found.");

            decision.Name = dto.Name;
            decision.WeightingMethod = dto.WeightingMethod;

            await _repo.SaveChangesAsync();
        }
    }
}
