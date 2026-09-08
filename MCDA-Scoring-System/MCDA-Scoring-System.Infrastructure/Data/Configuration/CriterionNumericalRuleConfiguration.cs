using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class CriterionNumericalRuleConfiguration: IEntityTypeConfiguration<CriterionNumericalRule>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CriterionNumericalRule> builder)
        {
            builder.HasData(
                new CriterionNumericalRule
                {
                    Id = 1,
                    CriterionId = 1,
                    NumericType = NumericType.TargetValue,
                    MinValue = 16m,
                    MaxValue = 128m,
                    TargetValue = 64m,
                    Direction = null
                },
                new CriterionNumericalRule
                {
                    Id = 2,
                    CriterionId = 4,
                    NumericType = NumericType.Scope,
                    MinValue = 750m,
                    MaxValue = 1500m,
                    TargetValue = null,
                    Direction = Direction.Minimize
                },
                new CriterionNumericalRule
                {
                    Id = 3,
                    CriterionId = 5,
                    NumericType = NumericType.Interval,
                    MinValue = 32m,
                    MaxValue = 1024m,
                    TargetValue = null,
                    Direction = null
                }
            );
        }
    
    }
}
