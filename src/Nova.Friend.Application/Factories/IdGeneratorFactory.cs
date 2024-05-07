namespace Nova.Friend.Application.Factories;

public interface IIdGeneratorFactory<out TId>
{
    TId GenerateId();
}

