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
		/// Caption for the option to return true
		/// </summary>
		public string YesOption{get;set;}
		/// <summary>
		/// Caption for option to return false
		/// </summary>
		public string NoOption{get;set;}

		public UserQuestion(string title, string message, string yesOption, string noOption) : base(title, message)
		{
			YesOption = yesOption;
			NoOption = noOption;
		}
	}
}

