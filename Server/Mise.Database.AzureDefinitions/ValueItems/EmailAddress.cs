using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    public class EmailAddress : EntityData, IDbValueItem<Core.ValueItems.EmailAddress>
    {
        public EmailAddress()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public string Value { get; set; }

        public Core.ValueItems.EmailAddress ToValueItem()
        {
            return new Core.ValueItems.EmailAddress(Value);
        }
    }
}
