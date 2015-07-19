using System;
using Mise.Core.Common.Repositories;
using Mise.Core.Entities.Base;
using Mise.Core.Repositories;
using Mise.Core.Services.WebServices;


namespace MiseWPFPOSClient.Mapping
{
    class RepositoryMapper<T>
    {

        private static object GetRepositoryObject(IRestaurantTerminalService service)
        {
            if(typeof(T) == typeof(ICheckRepository))
            {
                return new CheckRepository(service);
            }
            if(typeof(T) == typeof(IEmployeeRepository))
            {
                return new EmployeeRepository(service);
            }

            throw new Exception("Invalid type of "+typeof(T) + " specified");
        }

        public static T GetRepository(IDataContext context, IRestaurantTerminalService service)
        {
            //get our repository
            var res =  (T)GetRepositoryObject(service);

            //add it to the context
            var bas = (IRepositoryBase) res;
            context.AddRepository(bas);

            //return the repository
            return res;
        }
    }

}
