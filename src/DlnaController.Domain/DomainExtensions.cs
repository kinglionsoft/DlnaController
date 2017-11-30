using DlnaController.Domain;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DomainExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            // mapper
            AutoMapper.Mapper.Initialize(cfg => cfg.AddProfile<DtoMapperProfile>());

            return services;
        }
    }
}