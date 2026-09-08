using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class AlternativeValueConfiguration : IEntityTypeConfiguration<AlternativeValue>
    {
        public void Configure(EntityTypeBuilder<AlternativeValue> builder)
        {
            builder.HasData(
                new AlternativeValue
                {
                    Id = 1,
                    AlternativeId = 1,
                    CriterionId = 1,
                    NumericValue = 56m,
                    CriterionOptionId = null
                },
                new AlternativeValue
                {
                    Id = 2,
                    AlternativeId = 1,
                    CriterionId = 2,
                    NumericValue = null,
                    CriterionOptionId = 1
                },
                new AlternativeValue
                {
                    Id = 3,
                    AlternativeId = 1,
                    CriterionId = 3,
                    NumericValue = null,
                    CriterionOptionId = 5
                },
                 new AlternativeValue
                 {
                     Id = 4,
                     AlternativeId = 1,
                     CriterionId = 4,
                     NumericValue = 1100m,
                     CriterionOptionId = null
                 },
                 new AlternativeValue
                 {
                     Id = 5,
                     AlternativeId = 1,
                     CriterionId = 5,
                     NumericValue = 880m,
                     CriterionOptionId = null
                 },
                 new AlternativeValue
                 {
                     Id = 6,
                     AlternativeId = 2,
                     CriterionId = 1,
                     NumericValue = 44,
                     CriterionOptionId = null
                 },
                 new AlternativeValue
                 {
                     Id = 7,
                     AlternativeId = 2,         
                     CriterionId = 2,
                     NumericValue = null,
                     CriterionOptionId = 3
                 },
                 new AlternativeValue
                 {
                     Id = 8,
                     AlternativeId = 2,
                     CriterionId = 3,
                     NumericValue = null,
                     CriterionOptionId = 5
                 },
                 new AlternativeValue
                 {
                     Id = 9,
                     AlternativeId = 2,
                     CriterionId = 4,
                     NumericValue = 1050m,
                     CriterionOptionId = null
                 },
                 new AlternativeValue
                 {
                     Id = 10,
                     AlternativeId = 2,
                     CriterionId = 5,
                     NumericValue = 15,
                     CriterionOptionId = null
                 }

            );
        }
    }
}
