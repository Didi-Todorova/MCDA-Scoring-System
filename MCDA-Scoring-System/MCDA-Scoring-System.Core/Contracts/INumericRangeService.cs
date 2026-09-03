using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.NumericRange;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface INumericRangeService
    {
        public Task<NumericRangeDto> CreateNumericRangeAsync(CreateNumericRangeDto dto);
        public Task<NumericRangeDto?> GetNumericRangeByIdAsync(int id);
        public Task<IEnumerable<NumericRangeDto>> GetAllNumericRangesAsync();
        public Task UpdateNumericRangeAsync(int id, UpdateNumericRangeDto dto);
        public Task PatchNumericRangeAsync(int id, PatchNumericRangeDto dto);
        public Task<bool> DeleteNumericRangeAsync(int id);
    }
}
