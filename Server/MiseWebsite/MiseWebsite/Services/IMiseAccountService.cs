using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace MiseWebsite.Services
{
    public interface IMiseAccountService
    {
        /// <summary>
        /// Take an email and password, and allow us to direct the app where to send the user (sales, restaurants, vendors, etc)
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Tuple<bool, Uri>> LoginForMiseCredentials(EmailAddress email, Password password);
    }
}
