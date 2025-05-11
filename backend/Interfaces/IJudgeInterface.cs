
namespace backend.Interfaces;

public interface IJudgeInterface
{
    public Task<JudgeError> GenerateProposals();
}

public enum JudgeError
{
    StudentsAreNull,
    RoomsAreNull,
    Ok,
    DatabaseError
}