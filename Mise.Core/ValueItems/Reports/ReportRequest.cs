using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems.Inventory;
namespace Mise.Core.ValueItems.Reports
{
    public class ReportRequest : IEquatable<ReportRequest>
    {
        public ReportRequest(ReportTypes type, DateTimeOffset? startDate, DateTimeOffset? endDate, Guid? entityID,
            int? maxResults, LiquidAmountUnits unit = LiquidAmountUnits.Milliliters)
        {
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            EntityID = entityID;
            MaxResults = maxResults;
			LiquidUnit = unit;
        }

        public ReportTypes Type { get; private set; }

        public DateTimeOffset? StartDate { get; private set; }
        public DateTimeOffset? EndDate { get; private set; }
        /// <summary>
        /// If set, the specific entity this is tied to
        /// </summary>
        public Guid? EntityID { get; private set; }

		public LiquidAmountUnits LiquidUnit{get;private set;}
        /// <summary>
        /// If set, return only this amount of results
        /// </summary>
        public int? MaxResults { get; private set; }
        public bool Equals(ReportRequest other)
        {
            if (other == null)
            {
                return false;
            }

            return StartDate == other.StartDate
                   && EndDate == other.EndDate
                   && EntityID == other.EntityID
                   && MaxResults == other.MaxResults;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ReportRequest;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int)Type;
                hashCode = (hashCode * 397) ^ StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EntityID.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxResults.GetHashCode();
                return hashCode;
            }
        }
    }
}
