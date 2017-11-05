using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;

namespace myCollections.Pages
{
   
    public partial class MessageBoxYesNo 
    {
        readonly Storyboard _hideStoryboard;

        public MessageBoxYesNo(string message, bool showNo, bool isError)
        {
            InitializeComponent();
            messageText.Text = message;
            if (showNo==false)
            {
                Cancel.Visibility = Visibility.Collapsed;
                Ok.SetValue(Grid.ColumnSpanProperty, 2);
            }

            if (isError)
                messageText.Foreground = new SolidColorBrush(Colors.Red);

            _hideStoryboard = ((Storyboard)Application.Current.Resources["hideWindow"]).Clone();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _hideStoryboard.Completed += hideStoryboard_Completed;
            _hideStoryboard.Begin(this);
        }

        void hideStoryboard_Completed(object sender, System.EventArgs e)
        {
            _hideStoryboard.Completed -= hideStoryboard_Completed;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            _hideStoryboard.Completed += hideStoryboard_Completed;
            _hideStoryboard.Begin(this);
        }

    }
}
