using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Xamarin.Forms;
using Mise.Inventory.Services;
using Mise.Inventory.ViewModels.Modals;

namespace Mise.Inventory.ViewModels
{
	public abstract class BaseViewModel : INotifyPropertyChanged
	{
        /// <summary>
        /// Fired when the view is shown
        /// </summary>
        /// <returns></returns>
	    public abstract Task OnAppearing();

		protected IAppNavigation Navigation{ get; private set;}
		protected ILogger Logger{get;private set;}
		protected BaseViewModel(IAppNavigation navigationService, ILogger logger){
			Navigation = navigationService;
			Logger = logger;
			NotProcessing = true;
		}

		protected void HandleException(Exception e){
			HandleException (e, e.Message);
		}

		public Func<ErrorMessage, Task> DisplayMessage{ private get; set;}
		public Func<UserQuestion, Task<bool>> AskUserQuestion{ private get; set;} 

		protected Task DisplayMessageModal(string title, string message){
			if(DisplayMessage == null){
				throw new InvalidOperationException ("Page does not have a DisplayMessage function!");
			}
			return DisplayMessage (new ErrorMessage (title, message));
		}

		protected Task<bool> AskUserQuestionModal(string title, string message, string yes, string no = "Cancel"){
			if(AskUserQuestion == null){
				throw new InvalidOperationException ("Page does not have a AskUserQuestion function!");
			}	
			return AskUserQuestion (new UserQuestion (title, message, yes, no));
		}

		protected void HandleException(Exception e, string message){
			Processing = false;
			DisplayMessageModal ("Error", e.Message);

			if(Logger != null){
				Logger.HandleException (e);
			}
		}

		/// <summary>
		/// The property values.
		/// </summary>
		readonly Dictionary<string, object> PropertyValues = new Dictionary<string, object>();

		/// <summary>
		/// Occurs when property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		/// <summary>
		/// Allows a view model to show if we're currently processing
		/// </summary>
		/// <value><c>true</c> if processing; otherwise, <c>false</c>.</value>
		public bool Processing{ get { 
				return GetValue<bool> (); 
			} 
			protected set {
				NotProcessing = !value;
				SetValue (value); 
			} 
		}
		public bool NotProcessing{ get { return GetValue<bool> (); } private set { SetValue (value); } }
		public Color ActivityColor{ get { return Color.Accent; } }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="propertyName">Property name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected T GetValue<T>([CallerMemberName] string propertyName = null)
		{
			return GetValue(propertyName, default(T));
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="propertyName">Property name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
		{
			if (string.IsNullOrEmpty(propertyName))
				return;

			var shouldNotify = !PropertyValues.ContainsKey(propertyName) || !object.Equals(value, PropertyValues[propertyName]);

			PropertyValues[propertyName] = value;

			if (shouldNotify)
				RaisePropertyChanged(propertyName);
		}

		/// <summary>
		/// Gets the value of the property specified by propertyName. If no
		/// value is present, defaultValue is returned.
		/// </summary>
		/// <param name="propertyName">The name of the property for which you're
		/// trying to get the value of.</param>
		/// <param name = "defaultValue"></param>
		/// <param name="propertyName">The name of the property (note this is case sensitive)
		/// for which you're trying to get the value of</param>
		T GetValue<T>(string propertyName, T defaultValue)
		{
			if (PropertyValues.ContainsKey(propertyName))
				return (T)PropertyValues[propertyName];

			return defaultValue;
		}

		void RaisePropertyChanged(string propName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}

