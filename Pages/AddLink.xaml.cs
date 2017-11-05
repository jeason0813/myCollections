using System.Threading.Tasks;
using System.Windows;
using myCollections.Utils;

namespace myCollections.Pages
{
    public partial class AddLink
    {
        private string _link = string.Empty;

        public string Link 
        {
            get { return _link; }
            set { _link = value; }
        }
        public AddLink()
        {
            InitializeComponent();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _link = string.Empty;
            Close();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            _link = txtLink.Text;
            Task.Factory.StartNew(() => Util.NotifyEvent("AddLink:" + " " + _link));
            Close();
        }

    }
}