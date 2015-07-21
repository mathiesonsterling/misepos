using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Inventory.Services;

namespace Mise.Inventory.ViewModels
{
	public class UpdateParLineItemViewModel : BaseNextViewModel<IPARBeverageLineItem>
	{
		readonly IPARService _parService;
		public UpdateParLineItemViewModel(IAppNavigation appNav, ILogger logger, IPARService parService) : base(appNav, logger){
			_parService = parService;
		}

		#region Fields
		public string ItemName{ get { return GetValue<string> (); } set { SetValue(value);} }

		#endregion

		#region implemented abstract members of BaseNextViewModel

		protected override Task<IList<IPARBeverageLineItem>> LoadItems ()
		{
			throw new NotImplementedException ();
		}

		protected override Task BeforeMoveNext (IPARBeverageLineItem currentItem)
		{
			throw new NotImplementedException ();
		}

		protected override Task AfterMoveNext (IPARBeverageLineItem newItem)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

