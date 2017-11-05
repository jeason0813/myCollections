
using System;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer 
    {
        public VideoPlayer(string url)
        {
            InitializeComponent();
            Player.Source = new Uri(url);
        }
    }
}
