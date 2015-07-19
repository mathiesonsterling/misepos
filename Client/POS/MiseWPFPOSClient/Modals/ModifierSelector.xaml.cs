using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Menu;


namespace MiseWPFPOSClient.Modals
{
    /// <summary>
    /// Interaction logic for ModifierSelector.xaml
    /// </summary>
    public partial class ModifierSelector
    {
        public class ModifierSelectorResult
        {
            public ModifierSelectorResult()
            {
                Mods = new List<IMenuItemModifier>();
            }
            public enum ResType
            {
                /// <summary>
                /// We're adding a new modifier
                /// </summary>
                Added,
                /// <summary>
                /// We're removing the modifier clicked on
                /// </summary>
                Delete,
                /// <summary>
                /// We're multiplying
                /// </summary>
                Multiply,
                None
            }

            public ResType Result { get; set; }
            public IList<IMenuItemModifier> Mods { get; set; }
        }

        private readonly IEnumerable<IMenuItemModifier> _possibles;

        private readonly bool _locked;
   
        private readonly ModifierSelectorResult _res;
        private readonly ICanTakeModifier _subject;

        public ModifierSelectorResult Result { get { return _res; } }

        public ModifierSelector(ICanTakeModifier subject, IEnumerable<IMenuItemModifier> possibleModifiers)
        {
            _possibles = possibleModifiers;
            _locked = false;
   
            _subject = subject;
            _res = new ModifierSelectorResult();
            InitializeComponent();
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            _res.Result = ModifierSelectorResult.ResType.Delete;
            Close();
        }

        /// <summary>
        /// Fired when a user clicks on a category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifierClicked(object sender, RoutedEventArgs e)
        {
            //get our category that was selected
            var button = (Button)sender;

            //get the menu item or category that was selected
            var menuItemModifier = button.CommandParameter as IMenuItemModifier;
            if (menuItemModifier == null) return;

            _res.Result = ModifierSelectorResult.ResType.Added;
            _res.Mods.Add(menuItemModifier);
            if (!_locked)
            {
                Close();
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            txtTitle.Text = _subject.Name;

            btnCopy.Visibility = (_subject is IOrderItem)?Visibility.Visible : Visibility.Hidden;

            //load the items we were given
            LoadItems(_possibles);
        }


        private void LoadItems(IEnumerable<IMenuItemModifier> source)
        {
            icModifier.ItemsSource = source;
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void BtnCopyClick(object sender, RoutedEventArgs e)
        {
            _res.Result= ModifierSelectorResult.ResType.Multiply;
            Close();
        }
    }
}
