using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class CriterionService : ICriterionService
    {
        private IRepository repo;

        public CriterionService(IRepository repo)
        {
            this.repo = repo;
        }

        public async Task<CriterionDto> CreateCriterionAsync(CreateCriterionDto dto)
        {
            var criterion = new Criterion
            {
                DecisionId = dto.DecisionId,
                Name = dto.Name,
                CriterionType = dto.CriterionType,
                Weight = dto.Weight
            };

            await repo.AddAsync(criterion);
            repo.SaveChangesAsync();

            return new CriterionDto
            (
                criterion.Id,
                criterion.DecisionId,
                criterion.Name,
                criterion.CriterionType,
                criterion.Weight
            );
        }

        public async Task<bool> DeleteCriterionAsync(int id)
        {
            var criterion = await repo.GetByIdAsync<Criterion>(id);
            if(criterion == null)
                return false;

            await repo.DeleteAsync<Criterion>(id);
            await repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<CriterionDto>> GetAllCriteriaAsync()
        {
            return await repo
                .AllReadonly<Criterion>()
                .Select(c => new CriterionDto(
                    c.Id,
                    c.DecisionId,   
                    c.Name,
                    c.CriterionType,
                    c.Weight
                ))
                .ToListAsync();
        }

        public async Task<CriterionDto?> GetCriterionByIdAsync(int id)
        {
            return await repo.AllReadonly<Criterion>()
                .Where(c =>c.Id ==id)
                .Select(c => new CriterionDto(
                    c.Id,
                    c.DecisionId,
                    c.Name,
                    c.CriterionType,
                    c.Weight
                ))
                .FirstOrDefaultAsync();
        }

        public async Task PatchCriterionAsync(int id, PatchCriterionDto dto)
        {
            var criterion = await repo.GetByIdAsync<Criterion>(id) ?? throw new ArgumentException($"Criterion with ID {id} not found.");

            if(dto.DecisionId.HasValue)
                criterion.DecisionId = dto.DecisionId.Value;
            if(dto.Name != null)
                criterion.Name = dto.Name;
            if(dto.CriterionType.HasValue)
                criterion.CriterionType = dto.CriterionType.Value;
            if(dto.Weight != null)
                criterion.Weight = dto.Weight;

            await repo.SaveChangesAsync();
        }

        public async Task UpdateCriterionAsync(int id, UpdateCriterionDto dto)
        {
            var criterion = await repo.GetByIdAsync<Criterion>(id) ?? throw new ArgumentException($"Criterion with ID {id} not found.");

            criterion.DecisionId = dto.DecisionId;   
            criterion.Name = dto.Name;
            criterion.CriterionType = dto.CriterionType; 
            criterion.Weight = dto.Weight;

            await repo.SaveChangesAsync();
        }
    }
}
