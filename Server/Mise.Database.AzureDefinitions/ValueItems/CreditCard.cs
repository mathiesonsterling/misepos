using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class CreditCard : IDbValueItem<Core.ValueItems.CreditCard>
    {
        public CreditCard()
        {
            ProcessorToken = new CreditCardProcessorToken();
        }

        public CreditCard(Core.ValueItems.CreditCard source)
        {
            FirstName = source.Name?.FirstName;
            LastName = source.Name?.LastName;
            ProcessorToken = new CreditCardProcessorToken(source.ProcessorToken);
        }

        /// <summary>
        /// This has to be a string, can't have 2 person names in an entity
        /// </summary>
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /// <summary>
        /// The token we've gotten from our credit card processor.  Once we have this, we discard everything else sensitive
        /// </summary>
        public CreditCardProcessorToken ProcessorToken { get; set; }

        public Core.ValueItems.CreditCard ToValueItem()
        {
            return new Core.ValueItems.CreditCard
            {
                Name = new Core.ValueItems.PersonName(FirstName, LastName),
                ProcessorToken = ProcessorToken.ToValueItem()
            };
        }
    }

    [ComplexType]
    public class CreditCardProcessorToken : Core.ValueItems.CreditCardProcessorToken, IDbValueItem<Core.ValueItems.CreditCardProcessorToken>
    {
        public CreditCardProcessorToken()
        {
            
        }

        public CreditCardProcessorToken(Core.ValueItems.CreditCardProcessorToken source)
        {
            Processor = source.Processor;
            Token = source.Token;
        }

        public Core.ValueItems.CreditCardProcessorToken ToValueItem()
        {
            return this;
        }
    }
}
