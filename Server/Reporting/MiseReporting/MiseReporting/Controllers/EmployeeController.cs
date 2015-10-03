using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ManagementDAL _dal;
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        public EmployeeController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _dal = new ManagementDAL();
        }

        // GET: Employee
        public async Task<ActionResult> Index()
        {
            var emps = await _dal.GetAllEmployees();
            var vms = new List<EmployeeViewModel>();
            foreach (var emp in emps)
            {
                var vm = await GetEmployeeVM(emp);
                vms.Add(vm);
            }

            return View(vms.OrderBy(vm=> vm.LastName).ThenBy(vm => vm.FirstName).ThenBy(vm => vm.RestaurantsDisplay));
        }


        // GET: Employee/Details/5
        public async Task<ActionResult> Details(Guid empId)
        {
            var emp = await _dal.GetEmployeeById(empId);
            var vm = await GetEmployeeVM(emp);
            return View(vm);
        }

        // GET: Employee/Create
        public async Task<ActionResult> Create()
        {
            var rests = await _dal.GetAllRestaurants();
            var restVms = rests.Select(r => new RestaurantViewModel(r));
            var vm = new EmployeeViewModel {PossibleRestaurants = restVms};
            return View(vm);
        }


        // POST: Employee/Create
        [HttpPost]
        public async Task<ActionResult> Create(FormCollection formCollection)
        {
            try
            {
                var emp = FormCollectionToVM(formCollection);
                if (emp.Id == Guid.Empty)
                {
                    emp.Id = Guid.NewGuid();
                }
                var ai = ViewModelToAi(emp, null);

                using (var db = new AzureNonTypedEntities())
                {
                    db.AzureEntityStorages.Add(ai);
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public async Task<ActionResult> Edit(Guid empId)
        {
            var emp = await _dal.GetEmployeeById(empId);
            var vm = await GetEmployeeVM(emp);
            return View(vm);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Guid empId, FormCollection collection)
        {
            EmployeeViewModel emp = null;
            try
            {
                emp = FormCollectionToVM(collection);
                var originalEmp = await _dal.GetEmployeeById(empId);

                using (var db = new AzureNonTypedEntities())
                {
                    //get the original to get the restaurants for now
                    var orign = db.AzureEntityStorages.FirstOrDefault(a => a.EntityID == emp.Id);
                    if (orign == null)
                    {
                        throw new ArgumentException("No employee found for id " + empId);
                    }
                    db.AzureEntityStorages.Remove(orign);

                    var ai = ViewModelToAi(emp, originalEmp.Password);
                    db.AzureEntityStorages.Add(ai);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                if (emp == null)
                {
                    throw;
                }
                return View(emp);
            }
        }


        // GET: Employee/Delete/5
        public async Task<ActionResult> Delete(Guid empId)
        {
            var emp = await _dal.GetEmployeeById(empId);
            var vm = await GetEmployeeVM(emp);
            return View(vm);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(Guid empId, FormCollection formCollection)
        {
            try
            {
                using (var db = new AzureNonTypedEntities())
                {
                    var existing = await db.AzureEntityStorages.Where(ai => ai.EntityID == empId).FirstOrDefaultAsync();
                    if (existing != null)
                    {
                        existing.Deleted = true;
                        db.Entry(existing).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private static EmployeeViewModel FormCollectionToVM(NameValueCollection collection)
        {
            var selectedGuids = collection["PostedRestaurantGuids"].Split(',');
            var guids = selectedGuids.Where(s => string.IsNullOrEmpty(s) == false).Select(Guid.Parse);

            Guid id;
            if (Guid.TryParse(collection["id"], out id) == false)
            {
                id = Guid.Empty;
            }
            //get the AI
            var emp = new EmployeeViewModel
            {
                Id = id,
                FirstName = collection["FirstName"],
                MiddleName = collection["MiddleName"],
                LastName = collection["LastName"],
                Email = collection["Email"],
                Password = collection["Password"],
                Restaurants = guids.Select(g => new RestaurantViewModel
                {
                    Id = g
                })
            };
            return emp;
        }

        private async Task<EmployeeViewModel> GetEmployeeVM(IEmployee emp)
        {
            var empRestaurants = emp.GetRestaurantIDs().Where(id => id != Guid.Empty).ToList();

            var rests = new List<RestaurantViewModel>();
            foreach (var restId in empRestaurants)
            {
                var rest = await _dal.GetRestaurantById(restId);
                if (rest != null)
                {
                    var vm = new RestaurantViewModel(rest);
                    rests.Add(vm);
                }
            }

            return new EmployeeViewModel(emp, rests);
        }


        private AzureEntityStorage ViewModelToAi(EmployeeViewModel emp, Password origPassword)
        {
            var password = string.IsNullOrEmpty(emp.Password) == false ? new Password(emp.Password) : origPassword;

            var name = new PersonName(emp.FirstName, emp.MiddleName, emp.LastName);
            var email = new EmailAddress(emp.Email);

            var restaurants = emp.Restaurants.ToDictionary<RestaurantViewModel, Guid, IList<MiseAppTypes>>(rest => rest.Id, rest => new List<MiseAppTypes> {MiseAppTypes.StockboyMobile});

            var empEntity = new Employee
            {
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                Revision = new EventID(MiseAppTypes.ManagementWebsite, 0),
                DisplayName = name.ToSingleString(),
                Name = name,
                Password = password,
                CanCompAmount = false,
                CompBudget = null,
                CurrentlyClockedInToPOS = false,
                PrimaryEmail = email,
                Emails = new List<EmailAddress> { email },
                RestaurantsAndAppsAllowed = restaurants
            };

            var dto = _dtoFactory.ToDataTransportObject(empEntity);
            var ai = new AzureEntityStorage(dto);
            return ai;
        }
    }
}
