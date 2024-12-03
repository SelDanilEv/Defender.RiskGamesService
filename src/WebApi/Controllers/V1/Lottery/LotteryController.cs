using System;
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

namespace WebApi.Controllers.V1.Lottery;

public class LotteryController(IMediator mediator, IMapper mapper)
    : BaseApiController(mediator, mapper)
{
    [HttpGet("find")]
    [Auth(Roles.Admin)]
    [ProducesResponseType(typeof(PagedResult<LotteryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<PagedResult<LotteryDto>> GetLotteriesAsync([FromQuery] GetLotteriesQuery query)
    {
        return await ProcessApiCallAsync<GetLotteriesQuery, PagedResult<LotteryDto>>(query);
    }

    [HttpPost("create")]
    [Auth(Roles.Admin)]
    [ProducesResponseType(typeof(LotteryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<LotteryDto> CreateLotteryAsync([FromBody] CreateLotteryCommand command)
    {
        return await ProcessApiCallAsync<CreateLotteryCommand, LotteryDto>(command);
    }

    [HttpPut("update")]
    [Auth(Roles.Admin)]
    [ProducesResponseType(typeof(LotteryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<LotteryDto> UpdateLotteryAsync([FromBody] UpdateLotteryCommand command)
    {
        return await ProcessApiCallAsync<UpdateLotteryCommand, LotteryDto>(command);
    }

    [HttpPut("activate")]
    [Auth(Roles.Admin)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<Guid> ActivateLotteryAsync([FromBody] ActivateLotteryCommand command)
    {
        return await ProcessApiCallAsync<ActivateLotteryCommand, Guid>(command);
    }

    [HttpPut("deactivate")]
    [Auth(Roles.Admin)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<Guid> DeactivateLotteryAsync([FromBody] DeactivateLotteryCommand command)
    {
        return await ProcessApiCallAsync<DeactivateLotteryCommand, Guid>(command);
    }

    [HttpDelete("delete")]
    [Auth(Roles.SuperAdmin)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<Guid> DeleteLotteryAsync([FromBody] DeleteLotteryCommand command)
    {
        return await ProcessApiCallAsync<DeleteLotteryCommand, Guid>(command);
    }
}
