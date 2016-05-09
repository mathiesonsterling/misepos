using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    public class EmailAddressDb : EntityData, IDbValueItem<Core.ValueItems.EmailAddress>
    {
        public EmailAddressDb()
        {
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public EmailAddressDb(Core.ValueItems.EmailAddress source) : this(source.Value)
        {
        }

        public EmailAddressDb(string value) : this()
        {
            Id = value;
            Value = Value;
        }

        public string Value { get; set; }

        public Core.ValueItems.EmailAddress ToValueItem()
        {
            return new Core.ValueItems.EmailAddress(Value);
        }
    }
}
