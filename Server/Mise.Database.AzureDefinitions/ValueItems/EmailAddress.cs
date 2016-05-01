using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Mobile.Server;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    public class EmailAddress : EntityData, IDbValueItem<Core.ValueItems.EmailAddress>
    {
        [Key]
        public string Value { get; set; }

        public Core.ValueItems.EmailAddress ToValueItem()
        {
            return new Core.ValueItems.EmailAddress(Value);
        }
    }
}
