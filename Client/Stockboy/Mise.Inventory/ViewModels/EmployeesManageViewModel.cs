using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Mise.Inventory.MVVM;
using Mise.Core.Entities.People;
using Mise.Inventory.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public class EmployeesManageViewModel : BaseViewModel
	{
		public IEnumerable<IEmployee> Employees { get; set; }

		readonly ILoginService _loginService;
		public EmployeesManageViewModel(IAppNavigation navi, ILoginService loginService, ILogger logger)
			:base(navi, logger)
		{
			_loginService = loginService;
			InviteEnabled = false;
			PropertyChanged += (sender, e) => {
				//are we a valid email?
				if(e.PropertyName == "InviteEmail")
				{
					InviteEnabled = EmailIsValid (InviteEmail);
				}
			};
		}

		public string InviteEmail{
			get{return GetValue<string> ();}
			set{ SetValue (value); }
		}

		public bool InviteEnabled{
			get{return GetValue<bool>();}
			set{ SetValue (value); }
		}
		#region Commands

		public ICommand InviteCommand {
			get { return new SimpleCommand(Invite, () => InviteEnabled); }
		}

		#endregion

		async void Invite()
		{
			try{
				//make the invite to them
				//check if we have a valid email
				if (EmailIsValid (InviteEmail)) {
					Processing = true;
					var email = new EmailAddress{ Value = InviteEmail };
					await _loginService.InviteEmployeeToUseApp (email);
					InviteEmail = string.Empty;
					Processing = false;
					await Navigation.ShowMainMenu ();
				}
			} catch(Exception e){
				HandleException (e);
			}
		}

		public static bool EmailIsValid(string email)
		{
			return EmailAddress.IsValid (email);
		}

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }
	}
}

