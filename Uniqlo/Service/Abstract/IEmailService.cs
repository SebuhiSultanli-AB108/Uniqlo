namespace Uniqlo.Service.Abstract;

public interface IEmailService
{
    void SendEmailConfirmation(string receiver, string name, string token);
}
