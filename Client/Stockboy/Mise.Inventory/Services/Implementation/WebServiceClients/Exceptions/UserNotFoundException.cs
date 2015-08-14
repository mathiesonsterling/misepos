using System;
using Mise.Core.ValueItems;
using Mise.Inventory.Services.Implementation.WebServiceClients.Exceptions;

namespace Mise.Inventory.Services.Implementation.WebServiceClients.Exceptions
{
	public class UserNotFoundException : Exception
	{
		public EmailAddress Email{ get; private set;}
		public bool NoEmailFound{ get; private set;}
		public bool IncorrectPassword{ get; private set;}

		public UserNotFoundException (EmailAddress email, bool emailIncorrect, bool passwordIncorrect) 
			: base("Incorrect login")
		{
			Email = email;
			NoEmailFound = emailIncorrect;
			IncorrectPassword = passwordIncorrect;
		}

		public UserNotFoundException(EmailAddress email) : this(email, true, true){
			
		}
	}
}

