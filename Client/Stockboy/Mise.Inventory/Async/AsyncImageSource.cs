using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Mise.Inventory.Async
{
	public static class AsyncImageSource
	{
		/// <summary>
		/// Froms the task.
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="task">Task.</param>
		/// <param name="defaultSource">Default source.</param>
		public static NotifyTaskCompletion<ImageSource> FromTask(Task<ImageSource> task, ImageSource defaultSource)
		{
			return new NotifyTaskCompletion<ImageSource>(task, defaultSource);
		}

		/// <summary>
		/// Froms the URI and resource.
		/// </summary>
		/// <returns>The URI and resource.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="resource">Resource.</param>
		public static NotifyTaskCompletion<ImageSource> FromUriAndResource(string uri, string resource)
		{
			var u = new Uri(uri);
			return FromUriAndResource(u, resource);
		}

		/// <summary>
		/// Froms the URI and resource.
		/// </summary>
		/// <returns>The URI and resource.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="resource">Resource.</param>
		public static NotifyTaskCompletion<ImageSource> FromUriAndResource(Uri uri, string resource)
		{
			var t = Task.Run(() => ImageSource.FromUri(uri));
			var defaultResouce = ImageSource.FromResource(resource);

			return FromTask(t, defaultResouce);
		}
	}
}