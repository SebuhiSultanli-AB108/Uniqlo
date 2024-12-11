using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Uniqlo.Helpers;
using Uniqlo.Service.Abstract;

namespace Uniqlo.Service.Implement;

public class EmailService : IEmailService
{
    readonly SmtpClient _smtpClient;
    readonly MailAddress _from;
    readonly HttpContext _context;


    public EmailService(IOptions<SmtpOptions> option, IHttpContextAccessor acc)
    {
        if (option == null || option.Value == null)
            throw new ArgumentNullException(nameof(option), "SmtpOptions is not provided.");

        var opt = option.Value;
        _smtpClient = new(opt.Host, opt.Port);
        _smtpClient.Credentials = new NetworkCredential(opt.Sender, opt.Password);
        _smtpClient.EnableSsl = true;
        _from = new MailAddress(opt.Sender, "Uniqlo");
        _context = acc.HttpContext;
    }

    public void SendEmailConfirmation(string receiver, string name, string token)
    {
        MailAddress to = new(receiver);
        MailMessage message = new MailMessage(_from, to);
        message.Subject = "Confirm your email address";
        string url = _context.Request.Scheme
            + "://" + _context.Request.Host
            + "/Account/VerifyEmail?token=" + token
            + "&user=" + name;
        message.Body = EmailTemplates.VerifyEmail.Replace("__$name", name).Replace("__$link", url);
        message.IsBodyHtml = true;
        _smtpClient.Send(message);
    }
}
