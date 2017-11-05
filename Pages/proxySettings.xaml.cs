using System;
using System.Windows;
using myCollections.Data.SqlLite;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for proxySettings.xaml
    /// </summary>
    public partial class proxySettings 
    {
        public proxySettings()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
                     MySettings.UseProxy = gboUseProxy.IsChecked.Value;
                     MySettings.ProxyIp = txtIP.Text;
                     MySettings.ProxyLogin = txtLogin.Text;
                     MySettings.ProxyPort = txtPort.Text;
                     MySettings.ProxyPwd = txtPwd.Text;                  
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void useProxy_Click(object sender, RoutedEventArgs e)
        {
            enableControl(gboUseProxy.IsChecked.Value);
        }

        private void BorderLessWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            gboUseProxy.IsChecked = Convert.ToBoolean(MySettings.UseProxy);
            enableControl((bool)gboUseProxy.IsChecked);

            txtIP.Text = MySettings.ProxyIp;
            txtLogin.Text = MySettings.ProxyLogin;
            txtPort.Text = MySettings.ProxyPort;
            txtPwd.Text = MySettings.ProxyPwd;
        }

        private void enableControl(bool isChecked)
        {
            txtIP.IsEnabled = isChecked;
            txtLogin.IsEnabled = isChecked;
            txtPort.IsEnabled = isChecked;
            txtPwd.IsEnabled = isChecked;
        }
    }
}
