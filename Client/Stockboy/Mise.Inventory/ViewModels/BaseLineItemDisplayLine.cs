using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
    /// <summary>
    /// Information needed to display a line item, including quantity
    /// </summary>
    public abstract class BaseLineItemDisplayLine<T> : ITextSearchable where T:IBaseBeverageLineItem
    {
        private readonly T _source;

        protected BaseLineItemDisplayLine(T source)
        {
            _source = source;
        }

        public abstract Color TextColor { get; }
        public abstract decimal Quantity { get; }
		public abstract string DetailDisplay {get;}

        public T Source { get { return _source; } }

        //TODO remove display name from other items!
        public string DisplayName
        {
            get { return _source.DisplayName; }
        }


        public Guid ID { get { return _source.ID; } }


        public bool ContainsSearchString(string searchString)
        {
            return _source.ContainsSearchString(searchString);
        }
    }
}
