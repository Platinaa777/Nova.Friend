using DomainDrivenDesign.Abstractions;
using MediatR;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.UserAggregate.Repositories;

namespace Nova.Friend.Application.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ITransactionScope scope)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        scope.AddReadScope(DatabaseOptions.UserCollection).AddWriteScope(DatabaseOptions.UserCollection);
    }
    
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.StartTransaction(cancellationToken);
        
        var userResult = request.ToCreateUserResult();
        if (userResult.IsFailure)
            return Result.Failure(userResult.Error);

        var existingUser = await _userRepository.FindUserById(userResult.Value.Id, cancellationToken);
        if (existingUser is not null)
            return Result.Failure(UserError.AlreadyExists);
        
        await _userRepository.Add(userResult.Value, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        return Result.Success();
    }
}