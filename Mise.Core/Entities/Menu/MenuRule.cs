using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Menu
{
    /// <summary>
    /// Menu rule to determin
    /// </summary>
    public class MenuRule : RestaurantEntityBase
    {
        public Guid MenuID { get; set; }

        public IEnumerable<Tuple<DayOfWeek, DateTime, DateTime>> DaysAndTimesAvailable;
        public bool CurrentlyApplies()
        {
            if (DaysAndTimesAvailable == null)
            {
                return false;
            }

            var timesForToday = DaysAndTimesAvailable.Where(d => d.Item1 == DateTime.Now.DayOfWeek).ToList();
            if (timesForToday.Any() == false)
            {
                return false;
            }

            var timeInRange =
                timesForToday.Where(
                    t => DateTime.Now.TimeOfDay > t.Item2.TimeOfDay && DateTime.Now.TimeOfDay < t.Item3.TimeOfDay);

            return timeInRange.Any();
        }
    }
}
