using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Configuration
{
    public class AlternativeConfiguration: IEntityTypeConfiguration<Alternative>
    {
        public void Configure(EntityTypeBuilder<Alternative> builder)
        {
            builder.HasData(
                new Alternative
                {
                    Id = 1,
                    DecisionId = 1,
                    Name = "Laptop A"
                },
                new Alternative
                {
                    Id = 2,
                    DecisionId = 1,
                    Name = "Laptop B"
                }
            );
        }
    }
}
