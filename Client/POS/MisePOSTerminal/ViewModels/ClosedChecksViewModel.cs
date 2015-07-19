using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.Check;
using Mise.Core.Services;

namespace MisePOSTerminal.ViewModels
{
	public class ClosedChecksViewModel : BaseViewModel
	{
		public ICommand ReturnToViewTabs{ get; private set; }

		public ICommand Sort{ get; private set; }
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped (Mise.Core.ValueItems.CreditCard card)
		{
			throw new NotImplementedException ();
		}

		#endregion

		public ICommand ReturnToViewChecks{ get; private set; }
		public ICommand CheckClicked{ get; private set; }

		public event ViewModelEventHandler OnLoadClosedChecks;

		readonly IDictionary<SortingFields, Func<IEnumerable<ICheck>, IEnumerable<ICheck>>> _sortingFunctions;
		SortingFields _currentSort;

		public IEnumerable<ICheck> ClosedChecks{ get; private set; }


		enum SortingFields
		{
			DefaultSort,
			PaymentStatus,
			CustomerName,
			LastTime,
		}

		private bool _descendingSort = false;

		IDictionary<SortingFields, Func<IEnumerable<ICheck>, IEnumerable<ICheck>>> AddSortingFunctions()
		{
			var sortingFunctions = new Dictionary<SortingFields, Func<IEnumerable<ICheck>, IEnumerable<ICheck>>>();
			sortingFunctions.Add(SortingFields.DefaultSort, new Func<IEnumerable<ICheck>, IEnumerable<ICheck>>(collection => collection.OrderBy(c => c.PaymentStatus).ThenBy(c => c.Customer.LastName)));
			sortingFunctions.Add(SortingFields.PaymentStatus, new Func<IEnumerable<ICheck>, IEnumerable<ICheck>>(collection => _descendingSort ? collection.OrderByDescending(c => c.PaymentStatus) : collection.OrderBy(c => c.PaymentStatus)));
			sortingFunctions.Add(SortingFields.CustomerName, new Func<IEnumerable<ICheck>, IEnumerable<ICheck>>(coll => _descendingSort ? coll.OrderByDescending(c => c.Customer.LastName) : coll.OrderBy(c => c.Customer.LastName)));
			sortingFunctions.Add(SortingFields.LastTime, new Func<IEnumerable<ICheck>, IEnumerable<ICheck>>(coll => _descendingSort ? coll.OrderByDescending(c => c.LastUpdatedDate) : coll.OrderBy(c => c.LastUpdatedDate)));
		
			return sortingFunctions;
		}
			
		public ClosedChecksViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model){
			//possible sorting functions
			_currentSort = SortingFields.DefaultSort;
			_sortingFunctions = AddSortingFunctions();
	
			//commands
			ReturnToViewChecks = new Command(() => MoveToView (TerminalViewTypes.ViewChecks));

			Sort = new Command<string>(sortV => {
				var sort = (SortingFields)Enum.Parse(typeof(SortingFields), sortV);
				if (sort == _currentSort) {
					_descendingSort = !_descendingSort;
				}

				ClosedChecks = ApplySort(sort);
				_currentSort = sort;

				if(OnLoadClosedChecks != null){
					OnLoadClosedChecks(this);
				}
			});

			CheckClicked = new Command<ICheck> (check => {
				//let our view model know what we did!
				var dest = Model.CheckClicked(check);
				MoveToView(dest);
			}
			);

			//setup our checks
			ClosedChecks = Model.ClosedChecks;
			ApplySort (SortingFields.DefaultSort);
		}

		IEnumerable<ICheck> ApplySort(SortingFields sort)
		{
			var func = _sortingFunctions[sort];
			return func.Invoke(ClosedChecks);
		}
	}
}

