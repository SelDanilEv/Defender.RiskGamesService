using System.Threading.Tasks;
using AutoMapper;
using Defender.Common.Attributes;
using Defender.Common.Consts;
using Defender.Common.DB.Pagination;
using Defender.RiskGamesService.Application.DTOs.Lottery;
using Defender.RiskGamesService.Application.Modules.Lottery.Queries;
using Defender.RiskGamesService.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1;

public class UserDataController(IMediator mediator, IMapper mapper)
    : BaseApiController(mediator, mapper)
{
    [HttpGet("tickets")]
    [Auth(Roles.User)]
    [ProducesResponseType(typeof(PagedResult<UserTicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<PagedResult<UserTicketDto>> GetUserLotteryTicketsAsync(
        [FromQuery] GetUserTicketsQuery query)
    {
        return await ProcessApiCallAsync<GetUserTicketsQuery, PagedResult<UserTicketDto>>(query);
    }
}
