﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace MiseReporting
{
    public class MiseUserServices
    {
        private ManagementDAL _dal;

        public MiseUserServices()
        {
            _dal = new ManagementDAL();
        }

        public async Task<IEnumerable<Guid>> GetRestaurantIdsForEmail(EmailAddress email)
        {
            var res = new List<Guid>();
            var emp = await _dal.GetEmployeeWithEmail(email);
            if (emp != null)
            {
                var restIdsFromEmp = emp.GetRestaurantIDs(MiseAppTypes.ManagementWebsite);
                res.AddRange(restIdsFromEmp);
            }

            var accounts = await _dal.GetAccountsByEmail(email);
            foreach (var acct in accounts)
            {
                var rests = await _dal.GetRestaurantsUnderAccont(acct);
                var restIds = rests.Select(r => r.Id);
                res.AddRange(restIds);
            }

            return res;
        }
    }
}
