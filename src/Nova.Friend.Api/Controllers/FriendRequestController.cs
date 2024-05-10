using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Application.Commands.AcceptFriendRequest;
using Nova.Friend.Application.Commands.RejectFriendRequest;
using Nova.Friend.Application.Commands.SendFriendRequest;
using Nova.Friend.Application.Queries.GetFriendRequests;
using Nova.Friend.Application.Queries.GetFriends;
using Nova.Friend.HttpModels.Requests;

namespace Nova.Friend.Api.Controllers;


[ApiController]
[Route("friend-request")]
public class FriendRequestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public FriendRequestController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("/friend-requests/{userId:guid}")]
    public async Task<ActionResult> GetFriendRequests([FromRoute] Guid userId)
    {
        var result = await _mediator.Send(_mapper.Map<GetFriendRequestsQuery>(userId));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> SendFriendRequest([FromBody] SendRequestToFriend req)
    {
        var result = await _mediator.Send(_mapper.Map<SendFriendRequestCommand>(req));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpPut("accept")]
    public async Task<ActionResult> AcceptRequest([FromBody] AcceptFriend req)
    {
        var result = await _mediator.Send(_mapper.Map<AcceptFriendRequestCommand>(req));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
    
    [HttpPut("reject")]
    public async Task<ActionResult> AcceptRequest([FromBody] RejectFriend req)
    {
        var result = await _mediator.Send(_mapper.Map<RejectFriendCommand>(req));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
}