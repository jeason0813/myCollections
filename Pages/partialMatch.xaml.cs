using System.Collections.ObjectModel;
using System.Windows;
using myCollections.Data;
using myCollections.Utils;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for partialMatch.xaml
    /// </summary>
    public partial class partialMatch
    {
        public string SelectedTitle { get; private set; }
        public string SelectedLink { get; private set; }
        public string SelectedArtist { get; private set; }
        private readonly Provider? _provider;
        public partialMatch(Collection<PartialMatche> objResults, Provider? provider)
        {
            _provider = provider;
            Init(objResults);
        }
        public partialMatch(Collection<PartialMatche> objResults)
        {
            Init(objResults);
        }
        private void Init(Collection<PartialMatche> objResults)
        {
            InitializeComponent();
            MainStack.DataContext = objResults;

            switch (_provider)
            {
                    
                case Provider.GraceNote:
                case Provider.LastFM:
                case Provider.NokiaMusic:
                case Provider.Fnac:
                      MainStack.ItemTemplate = FindResource("MusicTemplate") as DataTemplate;
                    break;
                case Provider.Tvdb:
                    MainStack.ItemTemplate = FindResource("SerieTemplate") as DataTemplate;
                    break;
            }
              
        }

        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            if (MainStack.SelectedItem != null)
                if (MainStack.SelectedItem.GetType() == typeof(PartialMatche))
                {
                    SelectedTitle = ((PartialMatche)MainStack.SelectedItem).Title;
                    SelectedLink = ((PartialMatche)MainStack.SelectedItem).Link;
                    SelectedArtist = ((PartialMatche)MainStack.SelectedItem).Artist;
                }
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
