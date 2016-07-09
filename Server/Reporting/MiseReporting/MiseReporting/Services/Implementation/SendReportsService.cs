using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.VendorManagement.Services;
using Mise.VendorManagement.Services.Implementation;
using Mono.Security.X509;

namespace MiseReporting.Services.Implementation
{
    public class SendReportsService : ISendReportsService
    {
        private readonly ManagementDAL _dal;
        private readonly ICSVExportService _icsvExportService;
        private readonly ISendEmailService _emailService;
        public SendReportsService()
        {
            var logger = new DummyLogger();
            _dal = new ManagementDAL();
            _icsvExportService = new CSVExportService(logger);
        }

        public async Task SendCSVReportsForNewItems()
        {
            var inventories = await GetInventoriesNeedingCSVSending();

            var sendTasks = inventories.Select(SendEmailForInventory);

            await Task.WhenAll(sendTasks);
            /*
            var itemsToEmail = await _dal.GetUnsentEmails();
            foreach (var itemToEmail in itemsToEmail)
            {
                byte[] csvFile = null;
                //get the CSV file
                switch (itemToEmail.MiseEntityType)
                {
                    case "Mise.Core.Common.Entities.Inventory.Inventory":
                        var inv = await _dal.GetInventoryById(itemToEmail.MiseEntityId);
                        csvFile = await _icsvExportService.ExportInventoryToCSVAggregated(inv);
                        break;
                    case "Mise.Core.Common.Entities.Inventory.Par":
                        var par = await _dal.GetParById(itemToEmail.MiseEntityId);
                        csvFile = await _icsvExportService.ExportParToCSV(par);
                        break;
                    case "Mise.Core.Common.Entities.Inventory.ReceivingOrder":
                        var ro = await _dal.GetReceivingOrderById(itemToEmail.MiseEntityId);
                        csvFile = await _icsvExportService.ExportReceivingOrderToCSV(ro);
                        break;
                    case "Mise.Core.Common.Entities.Inventory.PurchaseOrder":
                        var po = await _dal.GetPurchaseOrderById(itemToEmail.MiseEntityId);
                        var poReports = new List<byte[]>();
                        foreach (var pov in po.GetPurchaseOrderPerVendors())
                        {
                            var csvForVendor = await _icsvExportService.ExportPurchaseOrderToCSV(pov);
                            poReports.Add(csvForVendor);
                        }
                        break;
                }
            }*/
        }

        private async Task SendEmailForInventory(IInventory inventory)
        {
            if (inventory == null)
            {
                return;
            }
            var toEmails = (await _dal.GetEmailToSendReportToForRestaurant(inventory.Id)).ToList();
            if (toEmails.Any())
            {
                var csvFile = await _icsvExportService.ExportInventoryToCSVAggregated(inventory);

                await _emailService.SendEmail(toEmails, new EmailAddress("info@misepos.com"), "Inventory Completed",
                    "You have completed an inventory!", new [] { csvFile});

                //mark it in the DB as well
                var sentEmail = new SendEmailCSVFile
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    MiseEntityType = "Mise.Core.Common.Entities.Inventory.Inventory",
                    EntityId = inventory.Id,
                    Sent = true
                };
                await _dal.CreateEmailRecord(sentEmail);
            }
        }

        private async Task<IEnumerable<IInventory>> GetInventoriesNeedingCSVSending()
        {
            //find the cutoff point of emails sent
            var lastEmails = await _dal.GetLastSentEmails(10);
            var startDate = lastEmails
                .Where(e => e.CreatedAt.HasValue)
                .Select(e => e.CreatedAt.Value)
                .OrderByDescending(d => d)
                .FirstOrDefault();

            var checkInventoriesAfter = startDate.AddHours(4);
            var inventories = await _dal.GetInventoriesCompletedAfter(checkInventoriesAfter);

            //double check we haven't already sent them!
            var needed = new List<IInventory>();
            foreach (var inv in inventories)
            {
                var email = await _dal.GetEmailForEntity(inv.Id);
                if (email == null)
                {
                    needed.Add(inv);
                }
            }

            return needed;
        }
    }
}
