using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Criterion;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
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

        public async Task<CriterionDto> CreateCriterionAsync(CreateCriterionDto createCriterionDto)
        {
            var criterion = new Criterion
            {
                DecisionId = createCriterionDto.DecisionId,
                Name = createCriterionDto.Name,
                CriterionType = createCriterionDto.CriterionType,
                Weight = createCriterionDto.Weight
            };

            repo.AddAsync(criterion);
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

        public async Task DeleteCriterionAsync(int id)
        {
            var criterion = await repo.GetByIdAsync<Criterion>(id) ?? throw new ArgumentException($"Criterion with ID {id} not found.");
            await repo.DeleteAsync<Criterion>(id);
            await repo.SaveChangesAsync();
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

        public async Task UpdateCriterionAsync(int id, UpdateCriterionDto updateCriterionDto)
        {
            var criterion = await repo.GetByIdAsync<Criterion>(id) ?? throw new ArgumentException($"Criterion with ID {id} not found.");

            criterion.DecisionId = updateCriterionDto.DecisionId;   
            criterion.Name = updateCriterionDto.Name;
            criterion.CriterionType = updateCriterionDto.CriterionType; 
            criterion.Weight = updateCriterionDto.Weight;

            await repo.SaveChangesAsync();
        }
    }
}
