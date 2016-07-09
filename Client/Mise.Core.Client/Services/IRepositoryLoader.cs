using System;
using System.Threading.Tasks;

namespace Mise.Core.Client.Services
{
    public interface IRepositoryLoader
    {
        Task LoadRepositories(Guid? restaurantID);

        Task ClearAllRepositories();

		Task SaveOnSleep();
    }
}