namespace Nova.Friend.Application.Factories;

public class GuidFactory : IIdGeneratorFactory<Guid>
{
    public Guid GenerateId() => Guid.NewGuid();
}