using System.ComponentModel;
using backend.Data;
using backend.Models.Communications;

public class CommunicationSender
{
    private AppDbContext _appDbContext;

    public CommunicationSender(
        AppDbContext appDbContext
    )
    {
        _appDbContext = appDbContext;
    }


    public void CreateCommunication(CreateCommunicationRequest req, List<int> usersIds)
    {
        foreach (var userId in usersIds)
            _appDbContext
                .Communications
                .Add(new Communication
                    {
                        Type = req.Type,
                        Description = req.Description,
                        UserId = userId
                    }
                );
    }
}