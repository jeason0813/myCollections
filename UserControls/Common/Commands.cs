
using System.Windows.Input;
namespace myCollections.UserControls.Common
{
    static class Commands
    {
        public static readonly RoutedCommand EditItem = new RoutedCommand("EditItem", typeof(UcFastBrowse));
        public static readonly RoutedCommand PlayItem = new RoutedCommand("PlayItem", typeof(UcFastBrowse));
        public static readonly RoutedCommand DeleteItem = new RoutedCommand("DeleteItem", typeof(UcFastBrowse));

    }
}
