using System.ComponentModel;
namespace myCollections.Utils
{
    public class NotifyChanged:INotifyPropertyChanged
    {

        protected void FirePropertyChanged(string propertyName)
        {
            var p = PropertyChanged;
            if (p != null)
                p(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
