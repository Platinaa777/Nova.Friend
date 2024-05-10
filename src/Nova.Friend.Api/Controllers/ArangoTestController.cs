using AutoMapper;
using Core.Arango;
using Core.Arango.Linq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nova.Friend.Application.Constants;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Api.Controllers;


[ApiController]
[Route("arango")]
public class ArangoTestController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IArangoContext _arango;

    public ArangoTestController(
        IMediator mediator,
        IMapper mapper,
        IArangoContext arango)
    {
        _mediator = mediator;
        _mapper = mapper;
        _arango = arango;
    }

    [HttpGet("get-user/{id:guid}")]
    public async Task<ActionResult> GetUser([FromRoute] Guid id)
    {
        var a = await _arango.Query<UserSnapshot>(DatabaseOptions.DatabaseName).Where(x => x.Key == id.ToString())
            .FirstOrDefaultAsync();
        
        return Ok(a);
    }
}