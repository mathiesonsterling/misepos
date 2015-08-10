using System;
using Mise.Core.Common.Services.WebServices.Exceptions;

namespace Mise.Core.Common.Services.WebServices.Exceptions
{
	public class DataNotSavedOnServerException : Exception
	{
		public DataNotSavedOnServerException () : this(null)
		{
		}

		public DataNotSavedOnServerException(Exception inner) : base("Unable to save the data on the server", inner){
			
		}
	}
}

