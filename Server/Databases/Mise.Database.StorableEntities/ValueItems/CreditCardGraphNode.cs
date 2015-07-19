using System;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.ValueItems
{
    public class CreditCardGraphNode
    {
        public CreditCardGraphNode()
        {
            ID = Guid.NewGuid();
        }

        public CreditCardGraphNode(CreditCard card) : this()
        {
            FirstName = card.Name.FirstName;
            MiddleName = card.Name.MiddleName;
            LastName = card.Name.LastName;
            ProcessorToken = card.ProcessorToken != null?card.ProcessorToken.Token:string.Empty;
            ProcessorName = card.ProcessorToken != null? card.ProcessorToken.Processor.ToString():string.Empty;
        }

        public CreditCard Rehydrate()
        {
            CreditCardProcessorToken token = null;
            if (string.IsNullOrEmpty(ProcessorToken) == false && string.IsNullOrEmpty(ProcessorName) == false)
            {
                token = new CreditCardProcessorToken
                {
                    Processor = (CreditCardProcessors) Enum.Parse(typeof (CreditCardProcessors), ProcessorName),
                    Token = ProcessorToken
                };
            }
            return new CreditCard
            {
                Name = new PersonName(FirstName, MiddleName, LastName),
                ProcessorToken = token
            };
        }

        /// <summary>
        /// Used internally only
        /// </summary>
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string OneWayHash { get; set; }

        public string ProcessorToken { get; set; }
        public string ProcessorName { get; set; }

    }
}
