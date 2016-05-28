using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Mise.Database.AzureDefinitions.Entities.People;

namespace StockboyMobileAppServiceService.Controllers
{
    public class EmployeeController : BaseEntityController<Employee>
    {
        public async Task<SingleResult<Employee>> GetEntity(string email, string passwordHash)
        {
            var res =
                    Context.Employees.Where(
                        e => e.Emails.Contains(email) && e.PasswordHash == passwordHash);

            return new SingleResult<Employee>(res);
        } 
    }
}