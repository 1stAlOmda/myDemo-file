using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Services.EmailMessageService
{

	public class EmailService : IEmailService //IEmailSender
	{
		private readonly IEmailConfiguration _emailConfiguration;

		public EmailService(IEmailConfiguration emailConfiguration)
		{
			_emailConfiguration = emailConfiguration;
		}

		//public List<EmailMessage> ReceiveEmail(int maxCount = 10)
		//{
		//	throw new NotImplementedException();
		//}

		public async Task Send(EmailMessage emailMessage)
		{
			try
			{

		   var message = new MimeMessage();
			//message.To.Add(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
			//message.From.Add(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

			message.From.Add(new MailboxAddress(emailMessage.FromAddresses.Name, emailMessage.FromAddresses.Address));
			message.To.Add(new MailboxAddress(emailMessage.ToAddresses.Name, emailMessage.ToAddresses.Address));

			message.Subject = emailMessage.Subject;

			//We will say we are sending HTML. But there are options for plaintext etc. 
			message.Body = new TextPart(TextFormat.Html)
			{
				Text = emailMessage.Content
			};
			//message.Body = new BodyBuilder
			//{
			//	HtmlBody = emailMessage.Content
			//};

			//Be careful that the SmtpClient class is the one from Mailkit not the framework!
			using (var emailClient = new SmtpClient())
			{
				//The last parameter here is to use SSL (Which you should!)
				emailClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);

				//Remove any OAuth functionality as we won't be using it. 
				emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

				emailClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);

				await emailClient.SendAsync(message);

				await emailClient.DisconnectAsync(true);
			}

		}
			  catch (Exception ex)
			{
				// TODO: handle exception
				throw new InvalidOperationException(ex.Message);
			}
		}



		//public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		//{
		//}

	}
}
