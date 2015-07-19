using System.Collections.Generic;
using Mise.Core.Client.ViewModel;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Views;


namespace MiseWPFPOSClient
{
    public interface IWPFView : IView
    {
        /// <summary>
        /// Returns the enum of the view
        /// </summary>
        TerminalViewTypes View { get; }

        /// <summary>
        /// Allows a page to stop the master window from navigating away, if there's unsaved data
        /// </summary>
        /// <returns></returns>
        bool CanNavigateAway();

        /// <summary>
        /// Allows our view to specify which menu views should be enabled
        /// </summary>
        /// <returns></returns>
        IList<TerminalViewTypes> GetEnabledMenuViews();

        /// <summary>
        /// Allows our views to notify the application it needs to update menus
        /// </summary>
        void UpdateEnabledViews();

        /// <summary>
        /// Allows a page to specify information when a switch is called
        /// </summary>
        /// <param name="destView"></param>
        /// <returns></returns>
        IEntityBase GetSwitchingArgument(TerminalViewTypes destView);
    }


}
