namespace MCDA_Scoring_System.MCDA_Scoring_System.Infrastructure.DependencyInjection
{
    public static class MCDAServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register your application services here
            // Example: services.AddScoped<IMyService, MyService>();
            return services;
        }
    }
}
