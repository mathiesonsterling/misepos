using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akavache.Sqlite3.Internal;
using Mise.Core.Services;
using Mise.Core.ValueItems.Reports;
using Mise.Inventory.Services;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels.Reports
{
    public class ReportResultsViewModel : BaseViewModel
    {
        /// <summary>
        /// Matches the LineItemWithQuantity item fields for binding
        /// </summary>
        public class LineItemDisplayAdapter
        {
            private readonly ReportResultLineItem _source;
            public LineItemDisplayAdapter(ReportResultLineItem source)
            {
                _source = source;
            }
            public string DisplayName { get { return _source.MainText; } }
            public Color TextColor { get { return _source.IsErrored ? Color.Accent : Color.Default; } }
            public string DetailDisplay { get { return _source.DetailText; } }
            public string Quantity
            {
                get
                {
                    return _source.Quantity.HasValue ? _source.Quantity.Value.ToString() : "None";
                }
            }
        }

        private readonly IReportsService _reportsService;
        public ReportResultsViewModel(IAppNavigation navigationService, ILogger logger, IReportsService reportsService)
            : base(navigationService, logger)
        {
            _reportsService = reportsService;
        }

        public IEnumerable<LineItemDisplayAdapter> LineItems
        {
            get
            {
                return GetValue<IEnumerable<LineItemDisplayAdapter>>();
            }
            set
            {
                SetValue(value);
            }
        }

        public string Title { get { return GetValue<string>(); } set { SetValue(value); } }

        public override async Task OnAppearing()
        {
            try
            {
				Processing = true;
                //load the items from our reports
                var result = await _reportsService.RunCurrentReport();

                LineItems = result.LineItems.Select(li => new LineItemDisplayAdapter(li));
                Title = result.Title;
				Processing = false;
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }
    }
}

