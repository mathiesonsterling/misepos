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

		public ErrorMessage(string title, string message){
			Title = title;
			Message = message;
		}
	}
}

