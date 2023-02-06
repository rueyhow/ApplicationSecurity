using MailKit.Net.Smtp;
using MimeKit;

namespace FreshFarmMarket.Services
{
	public class EmailService
	{
		public async Task SendingEmail(string recieverEmail , string body , string subject)
		{
			var email = new MimeMessage();

			email.From.Add(MailboxAddress.Parse("freshfarmmarket12345@gmail.com"));

			email.To.Add(MailboxAddress.Parse(recieverEmail));

			email.Subject = subject;

			email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

			SmtpClient smtp = new SmtpClient();
			smtp.Connect("smtp.gmail.com" , 587 , MailKit.Security.SecureSocketOptions.StartTls);
			smtp.Authenticate("freshfarmmarket12345@gmail.com", "wkefgqnhlsfqpynr");
			await smtp.SendAsync(email);
			smtp.Disconnect(true);
		}
	}
}
