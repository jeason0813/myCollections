using System.Windows.Data;

namespace myCollections.Utils
{
    class SettingBindingExtension :Binding
    {
       
        public SettingBindingExtension(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            Source = Properties.Settings.Default;
            Mode = BindingMode.TwoWay;
        }
    }
}
