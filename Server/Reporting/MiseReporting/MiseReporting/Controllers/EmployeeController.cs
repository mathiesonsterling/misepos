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
using Mise.Core.ValueItems;
using MiseReporting.Models;

namespace MiseReporting.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EntityDataTransportObjectFactory _dtoFactory;
        private readonly string _empType;
        private readonly string _restType;
        public EmployeeController()
        {
            _dtoFactory = new EntityDataTransportObjectFactory(new JsonNetSerializer());
            _empType = typeof(Employee).ToString();
            _restType = typeof(Restaurant).ToString();
        }

        // GET: Employee
        public async Task<ActionResult> Index()
        {
            var vms = new List<EmployeeViewModel>();
            using (var db = new AzureNonTypedEntities())
            {
                var emps = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && (ai.Deleted == false)).ToListAsync();
                //populate the vendors?

                foreach (var ai in emps)
                {
                    var vm = await HydrateEmployeeVM(ai, db);
                    vms.Add(vm);
                }
            }
            return View(vms.OrderBy(vm=> vm.LastName).ThenBy(vm => vm.FirstName).ThenBy(vm => vm.RestaurantsDisplay));
        }


        // GET: Employee/Details/5
        public async Task<ActionResult> Details(Guid empId)
        {
            using (var db = new AzureNonTypedEntities())
            {
                var emp =
                    await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && ai.EntityID == empId).FirstOrDefaultAsync();

                if (emp == null)
                {
                    throw new ArgumentException("No employee found for id " + empId);
                }
                var vm = await HydrateEmployeeVM(emp, db);
                return View(vm);
            }
        }

        // GET: Employee/Create
        public async Task<ActionResult> Create()
        {
            using (var db = new AzureNonTypedEntities())
            {
                var restVms = await GetPossibleRestaurantVms(db);
                var vm = new EmployeeViewModel {PossibleRestaurants = restVms};
                return View(vm);
            }
        }


        // POST: Employee/Create
        [HttpPost]
        public async Task<ActionResult> Create(Guid empId, FormCollection formCollection)
        {
            try
            {
                var emp = FormCollectionToVM(formCollection);
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
            using (var db = new AzureNonTypedEntities())
            {
                var emp = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && ai.EntityID == empId)
                            .FirstOrDefaultAsync();

                if (emp == null)
                {
                    throw new ArgumentException("No employee found for id " + empId);
                }
                var vm = await HydrateEmployeeVM(emp, db);
                //get possible restaurants
                vm.PossibleRestaurants = await GetPossibleRestaurantVms(db);
                return View(vm);
            }
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Guid empId, FormCollection collection)
        {
            EmployeeViewModel emp = null;
            try
            {
                emp = FormCollectionToVM(collection);


                using (var db = new AzureNonTypedEntities())
                {
                    //get the original to get the restaurants for now
                    var orign = db.AzureEntityStorages.FirstOrDefault(a => a.EntityID == emp.Id);
                    var originalEmp = _dtoFactory.FromDataStorageObject<Employee>(orign.ToRestaurantDTO());
                    db.AzureEntityStorages.Remove(orign);

                    var ai = ViewModelToAi(emp, originalEmp.Password);
                    db.AzureEntityStorages.Add(ai);
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            catch(Exception e)
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
            using (var db = new AzureNonTypedEntities())
            {
                var emp = await db.AzureEntityStorages.Where(ai => ai.MiseEntityType == _empType && ai.EntityID == empId)
                            .FirstOrDefaultAsync();

                if (emp == null)
                {
                    throw new ArgumentException("No employee found for id " + empId);
                }
                var vm = await HydrateEmployeeVM(emp, db);
                return View(vm);
            }
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(Guid empId, FormCollection formCollection)
        {
            try
            {
                // TODO: Add delete logic here
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
            var selectedGuids = collection["PostedRestaurantGuids"].Split(new[] { ',' });
            var guids = selectedGuids.Where(s => string.IsNullOrEmpty(s) == false).Select(Guid.Parse);
            //get the AI
            var emp = new EmployeeViewModel
            {
                Id = Guid.Parse(collection["Id"]),
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

        private async Task<EmployeeViewModel> HydrateEmployeeVM(AzureEntityStorage ai, AzureNonTypedEntities db)
        {
            var emp = _dtoFactory.FromDataStorageObject<Employee>(ai.ToRestaurantDTO());
            var empRestaurant = emp.GetRestaurantIDs().Where(id => id != Guid.Empty).ToList();

            var restAi = await db.AzureEntityStorages.Where(
                a => a.MiseEntityType == _restType && empRestaurant.Contains(a.EntityID)).ToListAsync();
            var rests = restAi.Select(r => _dtoFactory.FromDataStorageObject<Restaurant>(r.ToRestaurantDTO()));
            var restVm = rests.Select(r => new RestaurantViewModel(r));

            var vm = new EmployeeViewModel(emp, restVm);
            return vm;
        }

        private async Task<IEnumerable<RestaurantViewModel>> GetPossibleRestaurantVms(AzureNonTypedEntities db)
        {
            var restAIs = await db.AzureEntityStorages.Where(a => a.MiseEntityType == _restType).ToListAsync();
            var rests = restAIs.Select(ra => _dtoFactory.FromDataStorageObject<Restaurant>(ra.ToRestaurantDTO()));
            var restVms = rests.Select(r => new RestaurantViewModel(r));
            return restVms.OrderBy(rv => rv.Name);
        }

        private AzureEntityStorage ViewModelToAi(EmployeeViewModel emp, Password origPassword)
        {
            Password password;
            if (string.IsNullOrEmpty(emp.Password) == false)
            {
                password = new Password(emp.Password);
            }
            else
            {
                password = origPassword;
            }

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
