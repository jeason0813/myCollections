using System.Windows;

namespace myCollections.Pages
{
    public partial class SelectFile
    {
        private readonly string[] _objBinding;
        public SelectFile(string[] strFiles)
        {
            InitializeComponent();
            _objBinding = strFiles;
            dtgData.DataContext = _objBinding;
        }
        public string SelectedValue { get; set; }

        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            //Fix since 2.6.0.0
            if (dtgData.SelectedCells.Count >= 2)
                SelectedValue = dtgData.SelectedCells[1].Item.ToString();
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}