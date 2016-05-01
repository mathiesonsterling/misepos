using System.ComponentModel.DataAnnotations.Schema;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class PersonName : Core.ValueItems.PersonName, IDbValueItem<Core.ValueItems.PersonName>
    {
        public Core.ValueItems.PersonName ToValueItem()
        {
            return this;
        }
    }
}
