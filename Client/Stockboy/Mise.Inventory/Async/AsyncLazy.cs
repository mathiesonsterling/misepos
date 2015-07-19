using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mise.Inventory.Async
{
	/// <remarks>
	/// http://blogs.msdn.com/b/pfxteam/archive/2011/01/15/10116210.aspx?utm_source=feedburner&utm_medium=twitter&utm_campaign=Feed%3A+SiteHome+(Microsoft+%7C+Blog+%7C+MSDN)
	/// </remarks>
	public class AsyncLazy<T> : Lazy<Task<T>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mise.Inventory.Async.AsyncLazy`1"/> class.
		/// </summary>
		/// <param name="valueFactory">Value factory.</param>
		public AsyncLazy(Func<T> valueFactory) :
			base(() => Task.Factory.StartNew(valueFactory))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Mise.Inventory.Async.AsyncLazy`1"/> class.
		/// </summary>
		/// <param name="taskFactory">Task factory.</param>
		public AsyncLazy(Func<Task<T>> taskFactory) :
			base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap())
		{
		}

		/// <summary>
		/// Gets the awaiter.
		/// </summary>
		/// <returns>The awaiter.</returns>
		public TaskAwaiter<T> GetAwaiter()
		{
			return base.Value.GetAwaiter();
		}

	}
}