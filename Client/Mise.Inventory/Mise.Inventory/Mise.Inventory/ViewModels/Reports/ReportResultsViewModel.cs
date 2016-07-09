using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
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
        public class LineItemDisplayAdapter : BaseDisplayLine<ReportResultLineItem>
        {
			public LineItemDisplayAdapter(ReportResultLineItem source) : base(source)
            {
            }
            public override string DisplayName => Source.MainText;
            public Color TextColor => Source.IsErrored ? Color.Accent : Color.Default;
            public override string DetailDisplay => Source.DetailText;

            public string Quantity => Source.Quantity.HasValue ? Math.Round(Source.Quantity.Value, 2).ToString () : "None";
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
		public bool NoItems{ get { return GetValue<bool> (); } set { SetValue (value); } }

        public override async Task OnAppearing()
        {
            try
            {
				Processing = true;
                //load the items from our reports
                var result = await _reportsService.RunCurrentReport();

                LineItems = result.LineItems.Select(li => new LineItemDisplayAdapter(li));
				NoItems = LineItems.Any() == false;
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

