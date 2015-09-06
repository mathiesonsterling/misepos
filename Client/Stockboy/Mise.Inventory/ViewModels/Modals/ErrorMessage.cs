using System;
using System.Reflection;
using System.Dynamic;
using Mise.Inventory.ViewModels.Modals;

namespace Mise.Inventory.ViewModels.Modals
{
	/// <summary>
	/// Represents an error message to be sent to the user
	/// </summary>
	public class ErrorMessage
	{
		public string Title{get;set;}
		public string Message{get;set;}
		/// <summary>
		/// Caption for the cancel button
		/// </summary>
		/// <value><c>true</c> if this instance cancel; otherwise, <c>false</c>.</value>
		public string OK{get;set;}

		public ErrorMessage(string title, string message, string ok = "OK"){
			Title = title;
			Message = message;
			OK = ok;
		}
	}
}

