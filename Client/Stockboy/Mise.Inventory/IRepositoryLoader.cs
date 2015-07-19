using System;
using System.Threading.Tasks;
using Autofac;

namespace Mise.Inventory
{
    public interface IRepositoryLoader
    {
        Task LoadRepositories(Guid? restaurantID);

        Task ClearAllRepositories();
    }
}