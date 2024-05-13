using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Tests;

public class UserTests
{
    [Fact]
    public void HandleAddFriend_WhenFriendNotExistInFriends_ShouldBeAdded()
    {
        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            "firstname", 
            "lastname", 
            new List<Id>()).Value;

        var friendUserId = UserId.Create("0766A6EE-2465-4B88-A0AA-4522407FA06F").Value;
        
        user.AddFriend(friendUserId);

        user.Friends.Should().Contain(x => x.Value == friendUserId.Value);
        user.Friends.Should().HaveCount(1);
    }

    [Fact]
    public void HandleAddFriend_WhenFriendInFriendList_ShouldBeNotAdded()
    {
        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            "firstname", 
            "lastname", 
            new List<Id>()).Value;

        var friendUserId = UserId.Create("0766A6EE-2465-4B88-A0AA-4522407FA06F").Value;
        
        user.AddFriend(friendUserId);
        user.AddFriend(friendUserId);
        
        user.Friends.Should().Contain(x => x.Value == friendUserId.Value);
        user.Friends.Should().HaveCount(1);
    }

    [Fact]
    public void HandleCreateUser_WhenIdInvalid_ShouldBeFailure()
    {
        var user = User.Create(
            "invalid", 
            "firstname", 
            "lastname", 
            new List<Id>());

        user.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void HandleCreateUser_WhenFirstNameTooLong_ShouldBeFailure()
    {
        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            new string('x',35), 
            "lastname", 
            new List<Id>());

        user.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public void HandleCreateUser_WhenFirstNameEmpty_ShouldBeFailure()
    {
        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            string.Empty, 
            "lastname", 
            new List<Id>());

        user.IsFailure.Should().BeTrue();
    }
    
    
    [Fact]
    public void HandleChangeFirstName_WhenFirstNameInvalid_ShouldBeThrowException()
    {
        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            "firstname", 
            "lastname", 
            new List<Id>());

        user.IsSuccess.Should().BeTrue();
        
        user.Invoking(u => u.Value.ChangeFirstName(new string('x', 31)))
            .Should().Throw<FirstNameException>();
    }
    
    [Fact]
    public void HandleChangeFirstName_WhenFirstNameValid_ShouldBeChanged()
    {
        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            "firstname", 
            "lastname", 
            new List<Id>());

        user.IsSuccess.Should().BeTrue();

        user.Value.ChangeFirstName("valid-name");

        user.Value.FirstName.Value.Should().BeEquivalentTo("valid-name");
    }
    
    [Fact]
    public void HandleDeleteFriend_WhenFriendExistInList_ShouldBeDeleted()
    {
        var friendUserId = UserId.Create("EF2B7335-7026-4B55-84D1-A07970CBE692").Value;

        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            "firstname", 
            "lastname", 
            new List<Id> { friendUserId }).Value;

        user.DeleteFromFriends(friendUserId);

        user.Friends.Should().BeEmpty();
    }
    
    [Fact]
    public void HandleDeleteFriend_WhenFriendNotExistInList_ShouldBeThrowException()
    {
        var friendUserId = UserId.Create("EF2B7335-7026-4B55-84D1-A07970CBE692").Value;

        var user = User.Create(
            "45929873-9CEE-4CAA-BCA9-ECBB09314A89", 
            "firstname", 
            "lastname", 
            new List<Id> { friendUserId }).Value;

        user.Invoking(u => 
            u.DeleteFromFriends(UserId.Create("68E3290D-5BFE-4213-B882-4F0E760C508C").Value))
            .Should().Throw<DeletingFriendException>();

        user.Friends.Should().ContainSingle(x => x.Value == friendUserId.Value);
    }
}