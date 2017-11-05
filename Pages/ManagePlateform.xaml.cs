using System.Collections.Generic;
using System.Windows;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;

namespace myCollections.Pages
{
    /// <summary>
    /// Interaction logic for ManagePlateform.xaml
    /// </summary>
    public partial class ManagePlateform
    {
        private readonly List<string> _added = new List<string>();

        public ManagePlateform()
        {
            InitializeComponent();
            DataContext = GameServices.GetPlatforms();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {

            SaveGamesPlateform();
            Close();

        }

        private void SaveGamesPlateform()
        {
            List<Platform> objSource = GameServices.GetPlatforms();
            List<Platform> objTarget = (List<Platform>)dtgData.DataContext;

            foreach (Platform item in objSource)
            {
                Platform type = item;
                if (!objTarget.Exists(objTemp => objTemp.Id == type.Id))
                    GameServices.DeletePlateform(item);
            }
            foreach (Platform item in objTarget)
            {
                Platform type = item;
                if (string.IsNullOrWhiteSpace(item.Name))
                    item.Name = item.DisplayName;

                if (!objSource.Exists(objTemp => objTemp.Id == type.Id) && !objSource.Exists(objTemp => objTemp.Name == type.Name))
                {

                    if (string.IsNullOrWhiteSpace(item.Name) == false && _added.Contains(item.Name) == false)
                    {
                        GameServices.GetPlatform(item);
                        _added.Add(item.Name);
                    }
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();

        }

    }
}
