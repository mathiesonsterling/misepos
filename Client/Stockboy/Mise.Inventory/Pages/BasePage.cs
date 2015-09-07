using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.ViewModels.Modals;

namespace Mise.Inventory.Pages
{
	public abstract class BasePage : ContentPage
	{
		public abstract BaseViewModel ViewModel{ get;}
		public abstract string PageName{get;}

		protected BasePage(){
			//assign our event handlers
			ViewModel.AskUserQuestion = AskQuestion;
			ViewModel.DisplayMessage = DisplayMessage;

			BindingContext = ViewModel;
		}

		protected Task DisplayMessage(ErrorMessage message){
			return DisplayAlert (message.Title, message.Message, message.OK);
		}

		protected Task<bool> AskQuestion(UserQuestion question){
			return DisplayAlert (question.Title, question.Message, question.OK, question.NoOption);
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", PageName}});
			await ViewModel.OnAppearing ();
		}
	}
}

