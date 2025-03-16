public class ApplicationService
{
    private readonly SecurityService _securityService;

    public ApplicationService(SecurityService securityService)
    {
        _securityService = securityService;
    }

    public void MakeAnApplication(string type)
    {
        // Tworzenie wniosku
        throw new NotImplementedException();
    }

    public void ResolveAnApplication(Application application)
    {
        // Obsługa zatwierdzania lub odrzucania wniosku
        throw new NotImplementedException();
    }
}
