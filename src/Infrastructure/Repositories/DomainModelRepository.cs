using Defender.Common.Configuration.Options;
using Defender.Common.DB.Repositories;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories;
using Defender.RiskGamesService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Defender.RiskGamesService.Infrastructure.Repositories.DomainModels;

public class DomainModelRepository : BaseMongoRepository<DomainModel>, IDomainModelRepository
{
    public DomainModelRepository(IOptions<MongoDbOptions> mongoOption) : base(mongoOption.Value)
    {
    }

    public async Task<DomainModel> GetDomainModelByIdAsync(Guid id)
    {
        return await GetItemAsync(id);
    }
}
