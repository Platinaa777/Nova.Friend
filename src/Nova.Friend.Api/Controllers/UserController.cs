using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Application.Queries.GetFriends;

namespace Nova.Friend.Api.Controllers;

[ApiController]
[Route("friend")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UserController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetUser([FromRoute] Guid id)
    {
        var result = await _mediator.Send(_mapper.Map<GetFriendsQuery>(id));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
}