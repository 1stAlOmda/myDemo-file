using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Services.EmailMessageService
{
	public interface IEmailService
	{
		Task Send(EmailMessage emailMessage);
		//List<EmailMessage> ReceiveEmail(int maxCount = 10);
	}

}
