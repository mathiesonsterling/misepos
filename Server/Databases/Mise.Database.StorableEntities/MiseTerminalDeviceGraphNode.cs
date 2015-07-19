using System;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public class MiseTerminalDeviceGraphNode : IStorableEntityGraphNode
    {
        public MiseTerminalDeviceGraphNode()
        {
            
        }

        public MiseTerminalDeviceGraphNode(IMiseTerminalDevice source)
        {
            CreatedDate = source.CreatedDate;
            CreditCardReaderType = source.CreditCardReaderType;
            DisplayName = source.DisplayName;
            FriendlyID = source.FriendlyID;
            HasCashDrawer = source.HasCashDrawer;
            ID = source.ID;
            IsActive = source.IsActive;
            LastUpdatedDate = source.LastUpdatedDate;
            MachineID = source.MachineID;
            PrintKitchenDupes = source.PrintKitchenDupes;
            RequireEmployeeSignIn = source.RequireEmployeeSignIn;
            RestaurantID = source.RestaurantID;
            Revision = source.Revision.ToDatabaseString();
            TableDropChecks = source.TableDropChecks;
            TopLevelCategoryID = source.TopLevelCategoryID;
            WaitForZToCloseCards = source.WaitForZToCloseCards;
        }

        public MiseTerminalDevice Rehydrate()
        {
            return new MiseTerminalDevice
            {
                CreatedDate = CreatedDate,
                CreditCardReaderType = CreditCardReaderType,
                DisplayName = DisplayName,
                FriendlyID = FriendlyID,
                HasCashDrawer = HasCashDrawer,
                ID = ID,
                IsActive = IsActive,
                LastUpdatedDate = LastUpdatedDate,
                MachineID = MachineID,
                PrintKitchenDupes = PrintKitchenDupes,
                RequireEmployeeSignIn = RequireEmployeeSignIn,
                RestaurantID = RestaurantID,
                Revision = new EventID(Revision),
                TableDropChecks = TableDropChecks,
                TopLevelCategoryID = TopLevelCategoryID,
                WaitForZToCloseCards = WaitForZToCloseCards,
            };
        }

        public bool WaitForZToCloseCards { get; set; }

        public Guid? TopLevelCategoryID { get; set; }

        public bool TableDropChecks { get; set; }

        public Guid RestaurantID { get; set; }

        public bool RequireEmployeeSignIn { get; set; }

        public bool PrintKitchenDupes { get; set; }

        public string MachineID { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public bool IsActive { get; set; }

        public bool HasCashDrawer { get; set; }

        public string FriendlyID { get; set; }

        public string DisplayName { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public CreditCardReaderType CreditCardReaderType { get; set; }

        public Guid ID { get; set; }
        public string Revision { get; set; }
    }
}
