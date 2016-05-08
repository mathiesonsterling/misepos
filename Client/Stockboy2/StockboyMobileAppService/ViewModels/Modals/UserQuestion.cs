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

		public UserQuestion(string title, string message, string yesOption, string noOption = "Cancel") 
			: base(title, message, yesOption)
		{
            if (yesOption.ToUpper() == "YES" || yesOption.ToUpper() == "OK")
            {
                throw new ArgumentException("Prohibed from using generic confirmations");
            }
			NoOption = noOption;
		}
	}
}

