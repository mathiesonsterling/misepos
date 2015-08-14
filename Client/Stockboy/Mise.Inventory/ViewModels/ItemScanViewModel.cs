using System;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using System.Windows.Input;
using Xamarin.Forms;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public class ItemScanViewModel : BaseViewModel
	{
		readonly IAppNavigation _appNavigation;

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }

		public ItemScanViewModel(IAppNavigation appNavigation, ILogger logger) : base(appNavigation, logger)
		{
			_appNavigation = appNavigation;
			Processing = false;
			PropertyChanged += (sender, e) => {
				if(e.PropertyName == "Value"){
					CanConfirm = string.IsNullOrEmpty (Value) == false && Value.Length > 5;
				}
			};
		}

		/// <summary>
		/// Callback function to be fired once the scan is done.  Should include the navigation away
		/// </summary>
		/// <value>The return callback.</value>
		public Func<string, Task> ReturnCallback{ get; set; }

		public string Value{get{return GetValue<string> ();}set{ SetValue (value); }}
		public bool CanConfirm{ get { return GetValue<bool> (); } set { SetValue (value); } }

		#region Commands

		public ICommand ConfirmCommand {
			get { return new Command(Confirm, () => CanConfirm); }
		}

		public ICommand CancelCommand{
			get{return new Command (Cancel, () => NotProcessing);}
		}
		#endregion

		async void Confirm()
		{
			try{
				Processing = true;
				if (ReturnCallback != null) {
					await ReturnCallback.Invoke (Value);
				}
				Processing = false;
				await _appNavigation.CloseItemScan ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void Cancel(){
			await _appNavigation.CloseItemScan ();
		}
	}
}

