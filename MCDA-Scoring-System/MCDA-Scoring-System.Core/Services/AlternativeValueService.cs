using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.AlternativeValue;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Services
{
    public class AlternativeValueService : IAlternativeValueService
    {
        private readonly IRepository repo;

        public AlternativeValueService(IRepository _repo)
        {
            repo = _repo;
        }

        public async Task<AlternativeValueDto> CreateAlternativeValueAsync(CreateAlternativeValueDto createAlternativeValueDto)
        {
            var alternativeValue = new AlternativeValue
            {
                AlternativeId = createAlternativeValueDto.AlternativeId,
                CriterionId = createAlternativeValueDto.CriterionId,
                NumericValue = createAlternativeValueDto.NumericValue,
                CriterionOptionId = createAlternativeValueDto.CriterionOptionId
            };

            await repo.AddAsync(alternativeValue);
            await repo.SaveChangesAsync();

            return new AlternativeValueDto(
                alternativeValue.Id,
                alternativeValue.AlternativeId,
                alternativeValue.CriterionId,
                alternativeValue.NumericValue,
                alternativeValue.CriterionOptionId
            );
        }

        public async Task DeleteAlternativeValueAsync(int id)
        {
            var alternativeValue = await repo.GetByIdAsync<AlternativeValue>(id) ?? throw new ArgumentException($"AlternativeValue with ID {id} not found.");  

            await repo.DeleteAsync<AlternativeValue>(alternativeValue);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<AlternativeValueDto>> GetAllAlternativeValuesAsync()
        {
          return await repo.AllReadonly<AlternativeValue>()
                .Select(av => new AlternativeValueDto(
                    av.Id,
                    av.AlternativeId,
                    av.CriterionId,
                    av.NumericValue,
                    av.CriterionOptionId
                ))
                .ToListAsync();
        }

        public async Task<AlternativeValueDto?> GetAlternativeValueByIdAsync(int id)
        {
            return await repo.AllReadonly<AlternativeValue>()
                .Where(av => av.Id == id)
                .Select(av => new AlternativeValueDto(
                    av.Id,
                    av.AlternativeId,
                    av.CriterionId,
                    av.NumericValue,
                    av.CriterionOptionId
                ))
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAlternativeValueAsync(int id, UpdateAlternativeValueDto updateAlternativeValueDto)
        {
            var alternativeValue = await repo.GetByIdAsync<AlternativeValue>(id) ?? throw new ArgumentException($"AlternativeValue with ID {id} not found.");

            alternativeValue.AlternativeId = updateAlternativeValueDto.AlternativeId;
            alternativeValue.CriterionId = updateAlternativeValueDto.CriterionId;
            alternativeValue.NumericValue = updateAlternativeValueDto.NumericValue;
            alternativeValue.CriterionOptionId = updateAlternativeValueDto.CriterionOptionId;

            await repo.SaveChangesAsync();
        }
    }
}
