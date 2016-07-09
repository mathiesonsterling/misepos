using System.Linq;
using Mise.Core.Common.Entities.People;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Entities.People
{
    public abstract class BaseUserDbEntity<TPersonEntity, TConcrete> : BaseDbEntity<TPersonEntity, TConcrete> 
        where TPersonEntity : IEntityBase, IUser
        where TConcrete : User, TPersonEntity
    {

	    protected BaseUserDbEntity(){}

	    protected BaseUserDbEntity(TPersonEntity source) : base(source)
	    {
		    FirstName = source.Name?.FirstName;
		    MiddleName = source.Name?.MiddleName;
		    LastName = source.Name?.LastName;

	        Emails = string.Join(",", source.GetEmailAddresses().Select(e => e.Value));
		    PasswordHash = source.Password?.HashValue;
		    PrimaryEmail = source.PrimaryEmail?.Value;
		    DisplayName = source.DisplayName;
	    }

        protected abstract TConcrete CreateConcretePerson();

        protected override TConcrete CreateConcreteSubclass()
        {
            var concrete = CreateConcretePerson();
            concrete.Name = GetName();
            concrete.DisplayName = DisplayName;
            concrete.PrimaryEmail = new EmailAddress(PrimaryEmail);
            concrete.Password = new Password {HashValue = PasswordHash};

            concrete.Emails = Emails.Split(',').Select(e => new EmailAddress(e)).ToList();
            return concrete;
        }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public PersonName GetName()
        {
            return new PersonName(FirstName, MiddleName, LastName);
        }

        public string PasswordHash { get; set; }

        /// <summary>
        /// Override to allow custom display names
        /// </summary>
        public string DisplayName { get; set; }

        public string Emails { get; set; }

        public string PrimaryEmail { get; set; }
    }
}
