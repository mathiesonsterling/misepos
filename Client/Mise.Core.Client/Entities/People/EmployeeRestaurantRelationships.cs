namespace Mise.Core.Client.Entities.People
{
    /// <summary>
    /// Shows that an employee works at a restaurant
    /// </summary>
    public class EmployeeRestaurantRelationships : EntityData
    {
        public EmployeeRestaurantRelationships()
        {
            IsCurrentEmployee = true;
        }

        public EmployeeRestaurantRelationships(Employee employee, Restaurant.Restaurant restaurant) : this()
        {
            Employee = employee;
            Restaurant = restaurant;
            IsCurrentEmployee = true;

            Id = employee.Id + ":" + restaurant.RestaurantID;
        }

        public Employee Employee { get; set; }

	    public string EmployeeId { get; set; }

        public Restaurant.Restaurant Restaurant { get; set; }

	    public string RestaurantId { get; set; }

        public bool IsCurrentEmployee { get; set; }
    }
}
