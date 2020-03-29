using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Services.EmailMessageService
{
	public class EmailMessage
	{
		public EmailMessage()
		{
			//ToAddresses = new List<EmailAddress>();
			//FromAddresses = new List<EmailAddress>();
			ToAddresses = new EmailAddress();
			FromAddresses = new EmailAddress(); 
		}
		//public List<EmailAddress> ToAddresses { get; set; }
		//public List<EmailAddress> FromAddresses { get; set; }
		public EmailAddress ToAddresses { get; set; }
		public EmailAddress FromAddresses { get; set; }
		public string Subject { get; set; }
		public string Content { get; set; }
	}
}
