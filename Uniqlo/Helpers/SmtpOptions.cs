namespace Uniqlo.Helpers;

public class SmtpOptions
{
    public const string Name = "SmtpOptions";
    public string Host { get; set; }
    public int Port { get; set; }
    public string Sender { get; set; }
    public string Password { get; set; }
}
