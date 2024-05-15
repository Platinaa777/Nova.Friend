using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Application.Commands.CreateUser;
using Nova.Friend.HttpModels.Requests;

namespace Nova.Friend.Api.Controllers;

[ApiController]
[Route("user")]
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
    
    [HttpPost]
    public async Task<ActionResult> AddUser([FromBody] AddUserRequest req)
    {
        var result = await _mediator.Send(_mapper.Map<CreateUserCommand>(req));

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
}