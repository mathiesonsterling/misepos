using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
namespace Mise.Core.Repositories
{
	/// <summary>
	/// Very basic repository functions, doesn't depend on what kind of entity we're storing
	/// </summary>
	public interface IRepository
	{
		/// <summary>
		/// Returns the latest EventID this repository knows about
		/// </summary>
		/// <returns>The last EventID in the repository</returns>
		EventID GetLastEventID();

        /// <summary>
        /// If true, the repository is currently loading and shouldn't be used
        /// </summary>
        bool Loading { get; }

		/// <summary>
		/// Signal the repository to load from the web service or DB
		/// </summary>
		Task Load(Guid? restaurantID);

        /// <summary>
        /// Clear all held data from the repository
        /// </summary>
        /// <returns></returns>
	    Task Clear();
	}
}

