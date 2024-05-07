using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;

namespace Nova.Friend.Application.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userResult = request.ToCreateUserResult();
        if (userResult.IsFailure)
            return Result.Failure(userResult.Error);

        var existingUser = await _userRepository.FindUserById(userResult.Value.Id, cancellationToken);
        if (existingUser is null)
            return Result.Failure(UserError.AlreadyExists);
        
        await _userRepository.Add(userResult.Value, cancellationToken);

        return Result.Success();
    }
}