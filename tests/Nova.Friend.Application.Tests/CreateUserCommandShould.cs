using DomainDrivenDesign.Abstractions;
using FluentAssertions;
using Moq;
using Nova.Friend.Application.Commands.CreateUser;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Application.Tests;

public class CreateUserCommandShould
{
    private CreateUserCommandHandler _sut = null!;

    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITransactionScope> _transactionScopeMock = new();
    private readonly UserId _id;
    private readonly CreateUserCommand _command;
    private User _user;
    
    public CreateUserCommandShould()
    {
         _id = UserId.Create("E099E1F0-7DEC-4677-B921-B76EF836CD38").Value; 
         _command = new()
        {
            UserId = _id.Value,
            UserName = "test",
            LastName = "test2"
        };
        _user = User.Create(_id.Value, _command.UserName, _command.LastName, new()).Value;
        _transactionScopeMock.Setup(x => x.AddReadScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
        _transactionScopeMock.Setup(x => x.AddWriteScope(It.IsAny<string>()))
            .Returns(_transactionScopeMock.Object);
    }
    

    [Fact]
    public async Task ReturnSuccess_WhenUserNotExist()
    {
        _userRepositoryMock.Setup(x => x.FindUserById(_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _sut = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);

        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ReturnAlreadyExistError_WhenUserIsExist()
    {
        _userRepositoryMock.Setup(x => x.FindUserById(_id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_user);

        _sut = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object,
            _transactionScopeMock.Object);
        
        var result = await _sut.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserError.AlreadyExists);
    }
}