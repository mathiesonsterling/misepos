using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Restaurant;

namespace MiseReporting.Repository
{
    public interface IRestaurantRepository
    {
        Task<IEnumerable<IRestaurant>> GetAll();

        Task<IRestaurant> GetById(Guid Id);
    }
}
