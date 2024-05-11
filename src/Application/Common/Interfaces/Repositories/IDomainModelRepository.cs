using Defender.RiskGamesService.Domain.Entities;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories;

public interface IDomainModelRepository
{
    Task<DomainModel> GetDomainModelByIdAsync(Guid id);
}
