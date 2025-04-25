using backend.Repositories;

namespace backend.Interfaces;

public interface IRoomInterface
{
    public Task<ErrorCodes> AddStudentToRoom(string roomId, string studentId);
    public Task<ErrorCodes> RemoveStudentFromRoom(string roomId, string studentId);
    public Task<List<Room>> GetAllRooms();
    public Task<List<Room>> GetAllAvailableRooms();
    
}