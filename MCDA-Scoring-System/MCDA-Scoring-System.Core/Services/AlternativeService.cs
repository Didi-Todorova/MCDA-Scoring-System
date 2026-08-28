using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.Alternative;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class AlternativeService : IAlternativeService
    {

        private readonly IRepository repo;

        public AlternativeService(IRepository repo)
        {
            this.repo = repo;
        }
        public async Task<AlternativeDto> CreateAlternativeAsync(CreateAlternativeDto createAlternativeDto)
        {
            var alternative = new Alternative
            {
                Name = createAlternativeDto.Name,
                DecisionId = createAlternativeDto.DecisionId
            };

            await repo.AddAsync(alternative);
            await repo.SaveChangesAsync();

            return new AlternativeDto(
                alternative.Id,
                alternative.Name,
                alternative.DecisionId
                );
        }

        public async Task DeleteAlternativeAsync(int id)
        {
            await repo.DeleteAsync<Alternative>(id);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlternativeDto>> GetAllAlternativesAsync()
        {
            return await repo
                .AllReadonly<Alternative>()
                .Select(a => new AlternativeDto(
                    a.Id,
                    a.Name,
                    a.DecisionId
                ))
                .ToListAsync();
        }

        public async Task<AlternativeDto?> GetAlternativeByIdAsync(int id)
        {
            return await repo
                .AllReadonly<Alternative>()
                .Where(a => a.Id == id)
                .Select(a => new AlternativeDto(
                    a.Id,
                    a.Name,
                    a.DecisionId
                ))
                .FirstOrDefaultAsync();
        }

        public Task UpdateAlternativeAsync(int id, UpdateAlternativeDto updateAlternativeDto)
        {
            throw new NotImplementedException();
        }
    }
}
