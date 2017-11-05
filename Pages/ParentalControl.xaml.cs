using System.Windows;
using myCollections.Data.SqlLite;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for ParentalControl.xaml
    /// </summary>
    public partial class ParentalControl
    {
        public ParentalControl()
        {
            InitializeComponent();
            txtPassword.Focus();
            ParentalOK = false;
        }
        public bool ParentalOK { get; set; }

        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                if (string.IsNullOrEmpty(txtPassword.Password) == true)
                    ParentalOK = false;
                else
                {
                    if (txtPassword.Password == MySettings.XXXPass)
                    {
                        ParentalOK = true;
                    }
                    else
                        ParentalOK = false;
                }
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}