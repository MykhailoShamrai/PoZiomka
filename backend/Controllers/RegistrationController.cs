public class RegistrationController
{
    private readonly SecurityService _securityService;

    public RegistrationController(SecurityService securityService)
    {
        _securityService = securityService;
    }

    public void Register(User user)
    {
        throw new NotImplementedException();
    }

    public void Login(User user)
    {
        throw new NotImplementedException();
    }
}
