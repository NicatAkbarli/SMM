using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SMMPanel.Services
{
    public class SmtpSetting
    {
        public string Host { get; init; }
    public int Port { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
    public string FromName { get; init; }
    public string FromEmail { get; init; }
}

public class EmailService
{
    private readonly SmtpSetting _opts;
    public EmailService(IOptions<SmtpSetting> opts) => _opts = opts.Value;

    public async Task SendVerificationEmailAsync(string toEmail, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_opts.FromName, _opts.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your verification code";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"<p>Salam — Təsdiq kodunuz: <b>{code}</b></p><p>Kod 10 dəqiqə etibarlıdır.</p>",
            TextBody = $"Təsdiq kodunuz: {code}"
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_opts.Host, _opts.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
        if (!string.IsNullOrEmpty(_opts.Username))
            await client.AuthenticateAsync(_opts.Username, _opts.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    }
}