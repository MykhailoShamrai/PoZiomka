using backend.Interfaces;

namespace backend.Repositories;


public class RoomRepository : IRoomInterface
{
    public Task<ErrorCodes> AddStudentToRoom(string roomId, string studentId)
    {
        throw new NotImplementedException();
    }

    public Task<ErrorCodes> RemoveStudentFromRoom(string roomId, string studentId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Room>> GetAllRooms()
    {
        throw new NotImplementedException();
    }

    public Task<List<Room>> GetAllAvailableRooms()
    {
        throw new NotImplementedException();
    }
}