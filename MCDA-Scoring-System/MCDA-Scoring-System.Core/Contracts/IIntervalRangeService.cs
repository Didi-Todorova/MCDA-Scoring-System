using MCDA_Scoring_System.MCDA_Scoring_System.Core.DTOs.IntervalRange;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts
{
    public interface IIntervalRangeService
    {
        Task<IEnumerable<IntervalRangeDto>> CreateIntervalRangesAsync(
            CreateIntervalRangesDto dto);

        Task<IEnumerable<IntervalRangeDto>> GetAllIntervalRangesAsync();

        Task<IntervalRangeDto?> GetIntervalRangeByIdAsync(int id);

        Task<IEnumerable<IntervalRangeDto>> GetIntervalRangesByRuleIdAsync(
            int criterionNumericalRuleId);

        Task UpdateIntervalRangesAsync(
            int criterionNumericalRuleId,
            UpdateIntervalRangesDto dto);

        //Task PatchIntervalRangeAsync(
        //    int id,
        //    PatchIntervalRangeDto dto);

        Task<bool> DeleteIntervalRangeAsync(int id);
    }
}
