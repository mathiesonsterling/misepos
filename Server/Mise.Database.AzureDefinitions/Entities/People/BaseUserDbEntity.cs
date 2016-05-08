using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.People;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities.People
{
    public abstract class BaseUserDbEntity<TPersonEntity, TConcrete> : BaseDbEntity<TPersonEntity, TConcrete> 
        where TPersonEntity : IEntityBase, IUser
        where TConcrete : User, TPersonEntity
    {

	    protected BaseUserDbEntity(){}

	    protected BaseUserDbEntity(TPersonEntity source, List<EmailAddressDb> emails) : base(source)
	    {
		    FirstName = source.Name?.FirstName;
		    MiddleName = source.Name?.MiddleName;
		    LastName = source.Name?.LastName;

		    Emails = emails;
		    PasswordHash = source.Password?.HashValue;
		    PrimaryEmail = emails.FirstOrDefault(e => e.Value == source.PrimaryEmail?.Value);
		    DisplayName = source.DisplayName;
	    }

        protected abstract TConcrete CreateConcretePerson();

        protected override TConcrete CreateConcreteSubclass()
        {
            var concrete = CreateConcretePerson();
            concrete.Name = GetName();
            concrete.DisplayName = DisplayName;
            concrete.Emails = Emails.Select(e => e.ToValueItem()).ToList();
            concrete.PrimaryEmail = PrimaryEmail.ToValueItem();
            concrete.Password = new Password {HashValue = PasswordHash};
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

        public List<EmailAddressDb> Emails { get; set; }

        public EmailAddressDb PrimaryEmail { get; set; }
    }
}
