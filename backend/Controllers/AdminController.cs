public class AdminController
{
    private readonly ApplicationService _applicationService;
    private readonly CommunicationSender _communicationSender;
    private readonly RoomSelector _roomSelector;

    public AdminController(ApplicationService applicationService, CommunicationSender communicationSender, RoomSelector roomSelector)
    {
        _applicationService = applicationService;
        _communicationSender = communicationSender;
        _roomSelector = roomSelector;
    }

    public void MakeApplication(string type)
    {
        throw new NotImplementedException();
    }

    public void SendCommunication(Communication communication, List<User> users)
    {
        throw new NotImplementedException();
    }

    public void ResolveAnApplication(Application application)
    {
        throw new NotImplementedException();
    }
}
