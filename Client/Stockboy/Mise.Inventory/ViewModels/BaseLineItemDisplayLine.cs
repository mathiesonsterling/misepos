using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;
using XLabs.Platform.Services;

namespace Mise.Inventory.ViewModels
{
	/// <summary>
	/// Simple class for anything that wants to use our custom cells
	/// </summary>
	public abstract class BaseDisplayLine<T> : ITextSearchable where T:ITextSearchable
	{
		public T Source{ get; private set;}

		protected BaseDisplayLine(T source){
			Source = source;
		}


		public bool ContainsSearchString(string searchString)
		{
			return Source.ContainsSearchString(searchString);
		}
			
		public abstract string DetailDisplay {get;}
		public abstract string DisplayName{get;}
	}

    /// <summary>
    /// Information needed to display a line item, including quantity
    /// </summary>
    public abstract class BaseLineItemDisplayLine<T> : BaseDisplayLine<T> where T:IBaseBeverageLineItem
    {
		protected BaseLineItemDisplayLine(T source) : base(source)
        {
        }

		public abstract Color TextColor { get; }
		public abstract string Quantity { get; }

        //TODO remove display name from other items!
        public override string DisplayName
        {
            get { return Source.DisplayName; }
        }


        public Guid ID { get { return Source.Id; } }

    }
}
