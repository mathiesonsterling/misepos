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
using Mise.Inventory;

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

            _logger = new DataToolsLogger();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            DBChoices.LoadComboBox(cmbInvDBURL);
            DBChoices.LoadComboBox(CmbVendorDestDB);
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

                //var uri = new Uri(selItem.Value);
                var uri = new Uri("https://stockboymobileservice.azure-mobile.net/");
                var progress = new Progress<ProgressReport>(ReportProgress);
                var silentLogger = new DummyLogger();

                try
                {
                    var buildLevel = DBChoices.GetBuildLevelSelected(selItem.Key);
                    var populateInventoryCommand = new PopulateInventorySqlServerDBCommand(progress, silentLogger, uri, buildLevel);
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
             
            var val = report.TotalProgressAmount > 0 
                ?((double) report.CurrentProgressAmount)/((double) report.TotalProgressAmount) * 100
                :0;
            PgBar.Value = val;
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
                var address = new StreetAddress(TxtAddressNumber.Text, TxtStreetDir.Text, TxtStreetName.Text, 
                    TxtCity.Text, TxtState.Text, Country.UnitedStates.Name, TxtZip.Text);
                var email = new EmailAddress(TxtEmail.Text);

                var dbUri = new Uri(selItem.Value);

                var progress = new Progress<ProgressReport>(ReportProgress);

                var noMessageLogger = new DummyLogger();
                var level = DBChoices.GetBuildLevelSelected(selItem.Key);
                var cmd = new ImportVendorPriceListCommand(noMessageLogger, dbUri, TxtFileName.Text, TxtVendorName.Text, email,
                    address, level, progress);
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

        private async void BtnExportInventory_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var progress = new Progress<ProgressReport>(ReportProgress);

                var noMessageLogger = new DummyLogger();
                var cmd = new ExportInventoryCommand(progress, noMessageLogger);

                await cmd.Execute();

                MessageBox.Show("Exported Inventory");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private async void BtnExportLastPar_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var progress = new Progress<ProgressReport>(ReportProgress);

                var noMessageLogger = new DummyLogger();
                var cmd = new ExportParCommand(progress, noMessageLogger);

                await cmd.Execute();

                MessageBox.Show("Exported Par");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }
    }
}
