using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeveloperTools.Commands;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Server.Services.Implementation;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Neo4J.Neo4JDAL;

namespace DeveloperTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ILogger _logger;
        public MainWindow()
        {
            InitializeComponent();

            txtDBURL.Text =
                "http://misetest:LeJwvOHjFnS7vgd7BLzX@misetest.sb02.stations.graphenedb.com:24789/db/data/";

            // txtDBURL.Text = "http://localhost:7474/db/data/";

            _logger = new DataToolsLogger();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DBChoices.LoadComboBox(cmbInvDBURL);
            DBChoices.LoadComboBox(CmbVendorDestDB);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnCreateDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var service = new FakeDomineesRestaurantServiceClient();

                Uri dbLoc;
                if (Uri.TryCreate(txtDBURL.Text, UriKind.RelativeOrAbsolute, out dbLoc) == false)
                {
                    return;
                }
                var menu = await service.GetMenusAsync();
                var restaurant = service.RegisterClientAsync("").Result.Item1;
                var account = new RestaurantAccount
                {
                    ID = restaurant.AccountID.HasValue ? restaurant.AccountID.Value : Guid.Empty,
                    CreatedDate = DateTime.UtcNow.AddMonths(-2),
                    Emails = new List<EmailAddress> { new EmailAddress { Value = "mathieson@misepos.com" } },
                    Status = MiseAccountStatus.Active,
                    LastUpdatedDate = DateTime.UtcNow,
                    PhoneNumber = new PhoneNumber { AreaCode = "718", Number = "7152945" },
                    ReferralCodeForAccountToGiveOut = new ReferralCode("mattyboi"),
                    Revision = new EventID { AppInstanceCode = MiseAppTypes.DummyData, OrderingID = 15 }
                };

                var config = new DevToolsConfigs
                {
                    Neo4JConnectionDBUri = dbLoc
                };
                var graphDAL = new Neo4JEntityDAL(config, new DummyLogger());

                graphDAL.ResetDatabase();

                await PopulateInventoryNeo4JDatabaseCommand.PopulateStates(graphDAL);

                await graphDAL.AddAccountAsync(account as IAccount);

                await graphDAL.AddRestaurantAsync(restaurant);
                foreach (var emp in service.GetEmployees())
                {
                    await graphDAL.AddEmployeeAsync(emp);
                }

                var fMenu = menu.FirstOrDefault();
                await graphDAL.AddMenuAsync(fMenu);


                _logger.Log("GraphDatabase Created", LogLevel.Info);
                await graphDAL.GetEmployeesAsync(restaurant.ID);

                //get the account
                await graphDAL.GetAccountsAsync();

                _logger.Log("Graph database verified!", LogLevel.Info);
            }
            catch (Exception ex)
            {
                _logger.HandleException(ex);
            }
        }


        private async void BtnGetRestaurant_Click(object sender, RoutedEventArgs e)
        {
            Uri dbLoc;
            if (Uri.TryCreate(txtDBURL.Text, UriKind.RelativeOrAbsolute, out dbLoc) == false)
            {
                return;
            }

            var config = new DevToolsConfigs
            {
                Neo4JConnectionDBUri = dbLoc
            };

            var graphDAL = new Neo4JEntityDAL(config, new DummyLogger());

            await graphDAL.GetRestaurantAsync(Guid.Parse("5567f4a5-d163-4f44-8d88-b4598339dcb7"));

        }

        private async void BtnCreateInvDB_Click(object sender, RoutedEventArgs e)
        {
            BtnCreateInvDB.IsEnabled = false;
            //get which DB we're going to populate
            if (cmbInvDBURL.SelectedItem != null)
            {
                KeyValuePair<string, string> selItem;
                try
                {
                    selItem = (KeyValuePair<string, string>)cmbInvDBURL.SelectedItem;
                }
                catch (Exception)
                {
                    BtnCreateInvDB.IsEnabled = true;
                    return;
                }

                var uri = new Uri(selItem.Value);

                var progress = new Progress<ProgressReport>(ReportProgress);
                var silentLogger = new DummyLogger();

                try
                {
                    var populateInventoryCommand = new PopulateInventoryNeo4JDatabaseCommand(silentLogger, uri, progress,
                        selItem.Key.ToUpper() == "DEV");
                    await populateInventoryCommand.Execute();
                    MessageBox.Show("GraphDB is now populated!");
                }
                catch (Exception ex)
                {
                    _logger.HandleException(ex);
                }


                BtnCreateInvDB.IsEnabled = true;
            }
        }

        public void ReportProgress(ProgressReport report)
        {
            PgBar.Value = ((double) report.CurrentProgressAmount)/((double) report.TotalProgressAmount) * 100;
            LblStatus.Content = report.CurrentProgressMessage;
        }


        private async void BtnImportVendor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtFileName.Text) || (string.IsNullOrEmpty(TxtVendorName.Text)))
            {
                return;
            }

            BtnImportVendor.IsEnabled = false;
            if (CmbVendorDestDB.SelectedItem != null)
            {
                KeyValuePair<string, string> selItem;
                try
                {
                    selItem = (KeyValuePair<string, string>)CmbVendorDestDB.SelectedItem;
                }
                catch (Exception)
                {
                    BtnImportVendor.IsEnabled = true;
                    return;
                }

                //make the command
                var address = new StreetAddress("519", "E", "7th St", "Austin", "Texas", Country.UnitedStates.Name,
                    "78701");
                var email = new EmailAddress("orders@twinliquors.com");

                var dbUri = new Uri(selItem.Value);

                var progress = new Progress<ProgressReport>(ReportProgress);

                var noMessageLogger = new DummyLogger();
                var cmd = new ImportVendorPriceListCommand(noMessageLogger, dbUri, TxtFileName.Text, TxtVendorName.Text, email,
                    address, progress);
                try
                {
                    await cmd.Execute();

                    MessageBox.Show("File was imported");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.Message);
                }
                BtnImportVendor.IsEnabled = true;
            }
        }

        private void BtnFindFile_OnClick(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog {Filter = "CSV Files (*.csv)|*.csv"};
            var res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
            {
                TxtFileName.Text = ofd.FileName;
            }
        }
    }
}
