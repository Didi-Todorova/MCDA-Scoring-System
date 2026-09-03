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
        private readonly IRepository _repo;

        public AlternativeValueService(IRepository _repo)
        {
            this._repo = _repo;
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

            await _repo.AddAsync(alternativeValue);
            await _repo.SaveChangesAsync();

            return new AlternativeValueDto(
                alternativeValue.Id,
                alternativeValue.AlternativeId,
                alternativeValue.CriterionId,
                alternativeValue.NumericValue,
                alternativeValue.CriterionOptionId
            );
        }

        public async Task<bool> DeleteAlternativeValueAsync(int id)
        {
            var alternativeValue = await _repo.GetByIdAsync<AlternativeValue>(id);  

            if( alternativeValue == null ) 
                return false;

            await _repo.DeleteAsync<AlternativeValue>(alternativeValue);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AlternativeValueDto>> GetAllAlternativeValuesAsync()
        {
          return await _repo.AllReadonly<AlternativeValue>()
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
            return await _repo.AllReadonly<AlternativeValue>()
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

        public async Task PatchAlternativeValueAsync(int id, PatchAlternativeValueDto dto)
        {
            var alternativeValue = _repo.GetByIdAsync<AlternativeValue>(id).Result ?? throw new ArgumentException($"AlternativeValue with ID {id} not found.");

            if(dto.AlternativeId.HasValue)
                alternativeValue.AlternativeId = dto.AlternativeId.Value;
            if (dto.CriterionId.HasValue)
                alternativeValue.CriterionId = dto.CriterionId.Value;
            if(dto.NumericValue.HasValue)
                alternativeValue.NumericValue = dto.NumericValue.Value;
            if(dto.CriterionOptionId.HasValue)
                alternativeValue.CriterionOptionId = dto.CriterionOptionId.Value;

            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAlternativeValueAsync(int id, UpdateAlternativeValueDto dto)
        {
            var alternativeValue = await _repo.GetByIdAsync<AlternativeValue>(id) ?? throw new ArgumentException($"AlternativeValue with ID {id} not found.");

            alternativeValue.AlternativeId = dto.AlternativeId;
            alternativeValue.CriterionId = dto.CriterionId;
            alternativeValue.NumericValue = dto.NumericValue;
            alternativeValue.CriterionOptionId = dto.CriterionOptionId;

            await _repo.SaveChangesAsync();
        }
    }
}
