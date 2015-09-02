using System;
using System.Dynamic;

namespace Mise.Inventory.ViewModels.Modals
{
	/// <summary>
	/// A boolean question for a user
	/// </summary>
	public class UserQuestion : ErrorMessage
	{
		/// <summary>
		/// Caption for option to return false
		/// </summary>
		public string NoOption{get;set;}

		public UserQuestion(string title, string message, string yesOption = "OK", string noOption = "Cancel") 
			: base(title, message, yesOption)
		{
			NoOption = noOption;
		}
	}
}

