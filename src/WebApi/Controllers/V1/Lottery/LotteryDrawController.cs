using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Defender.Common.Attributes;
using Defender.Common.Consts;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Application.DTOs.Lottery;
using Defender.RiskGamesService.Application.Modules.Lottery.Commands;
using Defender.RiskGamesService.Application.Modules.Lottery.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Defender.RiskGamesService.WebApi.Controllers.V1;

public class LotteryDrawController(IMediator mediator, IMapper mapper)
    : BaseApiController(mediator, mapper)
{
    [HttpGet("active")]
    [Auth(Roles.User)]
    [ProducesResponseType(typeof(PagedResult<LotteryDrawDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<PagedResult<LotteryDrawDto>> GetActiveLotteryDrawsAsync(
        [FromQuery] GetLotteryDrawsQuery query)
    {
        return await ProcessApiCallAsync<GetLotteryDrawsQuery, PagedResult<LotteryDrawDto>>(query);
    }

    [HttpPost("tickets/purchase")]
    [Auth(Roles.User)]
    [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<List<int>> PurchaseTicketsAsync([FromBody] PurchaseLotteryTicketCommand command)
    {
        return await ProcessApiCallAsync<PurchaseLotteryTicketCommand, List<int>>(command);
    }

    [HttpGet("tickets/search")]
    [Auth(Roles.User)]
    [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<List<int>> SearchAvailableTicketsAsync([FromQuery] SearchAvailableTicketsQuery query)
    {
        return await ProcessApiCallAsync<SearchAvailableTicketsQuery, List<int>>(query);
    }

}
