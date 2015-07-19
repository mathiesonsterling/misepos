using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Database.StorableEntities;
using Mise.Neo4J.Relationships;
using Neo4jClient;

namespace Mise.Neo4J.Neo4JDAL
{
    public partial class Neo4JEntityDAL
    {
        #region Employees

        public async Task<IEnumerable<IEmployee>> GetEmployeesAsync(Guid restaurantID)
        {
            var empNodes = await _graphClient.Cypher
                .Match("(e:Employee)")
                .Where((EmployeeGraphNode e) => e.RestaurantID == restaurantID)
                .Return(e => e.As<EmployeeGraphNode>())
                .ResultsAsync;
            //note we do NOT hydrate our emails here!

            var nodesAndEmails = empNodes.Select(en => new { Emails = GetEmailsAssociatedWithEntityAsync(en.ID).Result, EmpNode = en, Restaurants = GetRestaurantIDsEmployeeWorksAt(en.ID).Result });
            var emps = nodesAndEmails.Select(ene => ene.EmpNode.Rehydrate(ene.Emails, ene.Restaurants));
            return emps;
        }


        public async Task<IEnumerable<IEmployee>> GetInventoryAppUsingEmployeesAsync()
        {

            var empNodes = await _graphClient.Cypher
                .Match("(e:Employee)")
                // ReSharper disable once RedundantBoolCompare
                .Return(e => e.As<EmployeeGraphNode>())
                .ResultsAsync;

            var res = new List<IEmployee>();
            foreach (var node in empNodes)
            {
                var emails = await GetEmailsAssociatedWithEntity(node.ID);
                var restaurants = await GetRestaurantIDsEmployeeWorksAt(node.ID);

                res.Add(node.Rehydrate(emails, restaurants));
            }
            return res;
        }

        public async Task<IDictionary<Guid, IList<MiseAppTypes>>> GetRestaurantIDsEmployeeWorksAt(Guid employeeID)
        {
            var dbResults = await _graphClient
                .Cypher
                .Match("(e:Employee)-[rel:EMPLOYEE_OF]->(r:Restaurant)")
                .Where((EmployeeGraphNode e) => e.ID == employeeID)
                .Return((r, rel) => new { Restaurant = r.As<RestaurantGraphNode>().ID, RelAtts = rel.As<RelationshipInstance<EmployeeWorkPayload>>() })
                .ResultsAsync;


            var res = new Dictionary<Guid, IList<MiseAppTypes>>();
            foreach (var dbRes in dbResults)
            {
                if (dbRes.RelAtts != null)
                {
                    var allApps = dbRes.RelAtts.Data.AppsAllowed.Split(',');
                    var enumApps = allApps.Select(s => (MiseAppTypes)Enum.Parse(typeof(MiseAppTypes), s)).ToList();
                    res.Add(dbRes.Restaurant, enumApps);
                }
                else
                {
                    res.Add(dbRes.Restaurant, new List<MiseAppTypes>());
                }
            }

            return res;
        }

        public async Task<IEmployee> GetEmployeeByIDAsync(Guid empID)
        {
            var nodes = await _graphClient
                .Cypher
                .Match("(e:Employee)")
                .Where((EmployeeGraphNode e) => e.ID == empID)
                .Return(e => e.As<EmployeeGraphNode>())
                .ResultsAsync;


            var empNode = nodes.FirstOrDefault();
            if (empNode != null)
            {
                var emails = await GetEmailsAssociatedWithEntity(empNode.ID);
                var restaurants = await GetRestaurantIDsEmployeeWorksAt(empNode.ID);
                return empNode.Rehydrate(emails, restaurants);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="restaurantID"></param>
        /// <returns></returns>
        public Task AssociateEmployeeWithRestaurantAsync(IEmployee employee, Guid restaurantID)
        {
            //get the apps for this restaurant
            var apps = employee.GetAppsEmployeeCanUse(restaurantID);
            var appStrings = apps.Select(a => a.ToString());
            var finalString = string.Join(",", appStrings);


            //we assume our restaurant exists
            return _graphClient.Cypher
                .Match("(e:Employee), (r:Restaurant)")
                .Where((EmployeeGraphNode e) => e.ID == employee.ID)
                .AndWhere((RestaurantGraphNode r) => r.ID == restaurantID)
                .CreateUnique("(e)-[:EMPLOYEE_OF{since : {since}, AppsAllowed: {appString}}]->(r)")
                .WithParam("since", employee.HiredDate)
                .WithParam("appString", finalString)
                .ExecuteWithoutResultsAsync();
        }

        public async Task AddEmployeeAsync(IEmployee employee)
        {
            //check if the employee already exists!
            var existing = await _graphClient.Cypher
                .Match("(e:Employee)")
                .Where((EmployeeGraphNode e) => e.ID == employee.ID)
                .Return(e => e.As<EmployeeGraphNode>())
                .ResultsAsync;

            if (existing.Any() == false)
            {
                var node = new EmployeeGraphNode(employee);
                var addQuery = _graphClient.Cypher
                    .Create("(e:Employee {emp})")
                    .WithParam("emp", node);

                await addQuery.ExecuteWithoutResultsAsync().ConfigureAwait(false);

                if (employee.PrimaryEmail != null)
                {
                    await SetEmailAddressOnEntityAsync(employee.PrimaryEmail, employee.ID, "PRIMARY_EMAIL");
                }

                foreach (var email in employee.GetEmailAddresses())
                {
                    await SetEmailAddressOnEntityAsync(email, employee.ID);
                }
            }

            foreach (var restID in employee.GetRestaurantIDs())
            {
                await AssociateEmployeeWithRestaurantAsync(employee, restID);
            }
          

        }

        public async Task UpdateEmployeeAsync(IEmployee employee)
        {
            var updateNode = new EmployeeGraphNode(employee);
            await _graphClient
                .Cypher
                .Match("(e:Employee)")
                .Where((EmployeeGraphNode e) => e.ID == employee.ID)
                .Set("e = {empParam}")
                .WithParam("empParam", updateNode)
                .ExecuteWithoutResultsAsync().ConfigureAwait(false);

            var tasks = employee.GetRestaurantIDs().Select(restID => AssociateEmployeeWithRestaurantAsync(employee, restID)).ToList();

            var emailsToBe = employee.GetEmailAddresses().Select(em => em.Value).ToList();

            var emailsAlready = await GetEmailsAssociatedWithEntity(employee.ID).ConfigureAwait(false);
            var existingEmails = emailsAlready.Select(e => e.Value);

            //add those that aren't there
            tasks.AddRange(emailsToBe
                .Where(tb => existingEmails.Contains(tb) == false)
                .Select(tb => new EmailAddress { Value = tb })
                .Select(email => SetEmailAddressOnEntityAsync(email, employee.ID)));

            //remove relationship with any no longer with the employee
            tasks.AddRange(existingEmails
                .Where(ex => emailsToBe.Contains(ex) == false)
                .Select(ex => new EmailAddress { Value = ex }
                ).Select(email => RemoveEmailAddressFromEntityAsync(email, employee.ID)));

            foreach (var t in tasks)
            {
                await t.ConfigureAwait(false);
            }
        }
        #endregion
    }
}
