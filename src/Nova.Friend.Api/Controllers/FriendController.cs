using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Application.Commands.DeleteFriend;
using Nova.Friend.Application.Queries.GetFriends;
using Nova.Friend.HttpModels.Requests;

namespace Nova.Friend.Api.Controllers;


[ApiController]
[Route("friends")]
public class FriendController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public FriendController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult> GetFriendsByUserId([FromRoute] Guid userId)
    {
        var result = await _mediator.Send(_mapper.Map<GetFriendsQuery>(userId));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpDelete]
    public async Task<ActionResult> DeleteUserFromFriendsList([FromBody] DeleteFriend req)
    {
        var result = await _mediator.Send(_mapper.Map<DeleteFriendCommand>(req));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
}