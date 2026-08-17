using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Criterion> Criteria { get; set; }
        public DbSet<CriterionScope> CriterionScopes { get; set; }
        public DbSet<CriterionInterval> CriterionIntervals { get; set; }
        public DbSet<CriterionReferenceValue> CriterionReferenceValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Criterion>()
                .Property(c => c.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Criterion>()
                .Property(c => c.RatingMethod)
                .HasConversion<string>();
        }
    }
}