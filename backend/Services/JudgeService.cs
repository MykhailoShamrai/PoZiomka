using System.Net;
using System.Threading.Tasks;
using backend.Data;
using backend.Interfaces;
using backend.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class JudgeService : IJudgeInterface
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    private static readonly Random _random = new Random(DateTime.Now.Second);

    public JudgeService(AppDbContext appDbContext, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }
    public async Task<JudgeError> GenerateProposals()
    {
        var students = await _userManager.GetUsersInRoleAsync("Student");
        var activeStudents = students.Where(st => st.StudentStatus == StudentStatus.Active).ToList();


        var rooms = await _appDbContext.Rooms.Where(r => r.Status == RoomStatus.Available).ToListAsync();
        var indicesForRooms = new bool[rooms.Count];

        // It is good Idea to shuffle users here

        Stack<User> studentsStack = new Stack<User>(activeStudents);

        Room? chosenRoom = null;
        int capacityOfRoom = 0;
        List<int>? userIdTmp = null;

        List<Proposal> proposals = new List<Proposal>();
        while (studentsStack.Count > 0)
        {
            if (chosenRoom is null)
            {
            // Choosing the room
                int indexrng = _random.Next(0, rooms.Count);
                while (indicesForRooms[indexrng])
                    indexrng = _random.Next(0, rooms.Count);
                chosenRoom = rooms[indexrng];
                indicesForRooms[indexrng] = true;
                capacityOfRoom = chosenRoom.Capacity;
                userIdTmp = new List<int>(capacityOfRoom);
            }
            
            var user = studentsStack.Pop();
            userIdTmp!.Add(user.Id);
            if (userIdTmp.Count == capacityOfRoom || studentsStack.Count == 0)
            {
                proposals.Add(new Proposal{
                    Room = chosenRoom,
                    RoommatesIds = userIdTmp,
                    Statuses = userIdTmp.Select(ui => SingleStudentStatus.Pending).ToList(),
                });
                chosenRoom = null;
            }
        }
        await _appDbContext.AddRangeAsync(proposals);
        var res = await _appDbContext.SaveChangesAsync();
        if (res == proposals.Count)
            return JudgeError.Ok;
        return JudgeError.DatabaseError;
    }
}