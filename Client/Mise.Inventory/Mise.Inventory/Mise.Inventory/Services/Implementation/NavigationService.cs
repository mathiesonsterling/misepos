using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mise.Inventory.Services.Implementation
{
	public class NavigationService : INavigationService
	{

		public INavigation Navi { get; internal set; }

		public Page CurrentPage { get; set; }

		/// <param name="page">To be added.</param>
		/// <summary>
		/// Removes the specified page from the navigation stack.
		/// </summary>
		/// <remarks>To be removed</remarks>
		public void RemovePage(Page page)
		{
			Navi.RemovePage(page);
		}

		/// <summary>
		/// Gets the stack of pages in the navigation.
		/// </summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		public IReadOnlyList<Page> NavigationStack {
			get {
				return Navi.NavigationStack;
			}
		}

		/// <summary>
		/// Gets the modal navigation stack.
		/// </summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		public IReadOnlyList<Page> ModalStack {
			get {
				return Navi.ModalStack;
			}
		}

		/// <param name="page">The page to add.</param>
		/// <summary>
		/// Inserts the page before.
		/// </summary>
		/// <param name="before">Before.</param>
		public void InsertPageBefore(Page page, Page before)
		{
			CurrentPage = page;
			Navi.InsertPageBefore(page, before);
		}

		/// <param name="page">To be added.</param>
		/// <param name="animated">To be added.</param>
		/// <summary>
		/// Pushs the async.
		/// </summary>
		/// <returns>The async.</returns>
		public Task PushAsync(Page page, bool animated)
		{
			CurrentPage = page;
			return Navi.PushAsync(page, animated);
		}

		/// <param name="animated">To be added.</param>
		/// <summary>
		/// Pops the async.
		/// </summary>
		/// <returns>The async.</returns>
		public Task<Page> PopAsync(bool animated)
		{
			return Navi.PopAsync(animated);
		}

		/// <param name="animated">To be added.</param>
		/// <summary>
		/// Pops to root async.
		/// </summary>
		/// <returns>The to root async.</returns>
		public Task PopToRootAsync(bool animated)
		{
			return Navi.PopToRootAsync(animated);
		}

		/// <param name="page">To be added.</param>
		/// <param name="animated">To be added.</param>
		/// <summary>
		/// Pushs the modal async.
		/// </summary>
		/// <returns>The modal async.</returns>
		public Task PushModalAsync(Page page, bool animated)
		{
			CurrentPage = page;
			return Navi.PushModalAsync(page, animated);
		}

		/// <param name="animated">To be added.</param>
		/// <summary>
		/// Pops the modal async.
		/// </summary>
		/// <returns>The modal async.</returns>
		public Task<Page> PopModalAsync(bool animated)
		{
			return Navi.PopModalAsync(animated);
		}

		/// <summary>
		/// Pops the async.
		/// </summary>
		/// <returns>The async.</returns>
		public Task<Page> PopAsync()
		{
			return Navi.PopAsync();
		}

		/// <summary>
		/// Pops the modal async.
		/// </summary>
		/// <returns>The modal async.</returns>
		public Task<Page> PopModalAsync()
		{
			return Navi.PopModalAsync();
		}

		/// <summary>
		/// Pops to root async.
		/// </summary>
		/// <returns>The to root async.</returns>
		public Task PopToRootAsync()
		{
			return Navi.PopToRootAsync();
		}

		/// <summary>
		/// Pushs the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="page">Page.</param>
		public Task PushAsync(Page page)
		{
			CurrentPage = page;
			return Navi.PushAsync(page);
		}

		/// <summary>
		/// Pushs the modal async.
		/// </summary>
		/// <returns>The modal async.</returns>
		/// <param name="page">Page.</param>
		public Task PushModalAsync(Page page)
		{
			CurrentPage = page;
			return Navi.PushModalAsync(page);
		}

		/// <summary>
		/// Displaies the alert.
		/// </summary>
		/// <returns>The alert.</returns>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="accept">Accept.</param>
		public Task DisplayAlert(string title, string message, string accept = "OK")
		{
			return CurrentPage.DisplayAlert(title, message, accept);
		}

		public Task<bool> AskUser(string title, string message, string accept="OK", string cancel="Cancel"){
			return CurrentPage.DisplayAlert (title, message, accept, cancel);
		}
	}
}