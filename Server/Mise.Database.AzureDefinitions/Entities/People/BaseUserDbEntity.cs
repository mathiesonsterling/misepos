using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.People;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Database.AzureDefinitions.ValueItems;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Mise.Database.AzureDefinitions.Entities.People
{
    abstract class BaseUserDbEntity<TPersonEntity, TConcrete> : BaseDbEntity<TPersonEntity, TConcrete> 
        where TPersonEntity : IEntityBase, IPerson
        where TConcrete : User, TPersonEntity
    {
        public BaseUserDbEntity()
        {
            Password = new Password();
            Name = new PersonName();
        }

        protected abstract TConcrete CreateConcretePerson();

        protected override TConcrete CreateConcreteSubclass()
        {
            var concrete = CreateConcretePerson();
            concrete.Name = Name;
            concrete.DisplayName = DisplayName;
            concrete.Emails = Emails.Select(e => e.ToValueItem()).ToList();
            concrete.PrimaryEmail = PrimaryEmail.ToValueItem();
            concrete.Password = Password.ToValueItem();
            return concrete;
        }

        public PersonName Name { get; set; }

        public Password Password { get; set; }

        /// <summary>
        /// Override to allow custom display names
        /// </summary>
        public string DisplayName { get; set; }

        public List<EmailAddress> Emails { get; set; }

        public EmailAddress PrimaryEmail { get; set; }
    }
}
