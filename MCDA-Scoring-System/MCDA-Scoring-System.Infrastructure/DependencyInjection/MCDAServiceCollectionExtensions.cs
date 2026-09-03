using MCDA_Scoring_System.MCDA_Scoring_System.Core.Contracts;
using MCDA_Scoring_System.MCDA_Scoring_System.Core.Services;
using MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.Repositories;

namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.DependencyInjection
{
    public static class MCDAServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IAlternativeService, AlternativeService>();
            services.AddScoped<IAlternativeValueService, AlternativeValueService>();
            services.AddScoped<ICriterionService, CriterionService>();
            services.AddScoped<ICriterionOptionService, CriterionOptionService>();
            services.AddScoped<IDecisionService, DecisionService>();
            services.AddScoped<INumericalCriterionRuleService, NumericalCriterionRuleService>();
            services.AddScoped<INumericRangeService, NumericRangeService>();

            return services;
        }
    }
}
