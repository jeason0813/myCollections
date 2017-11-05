using System.Windows;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Collections;

namespace myCollections.Pages
{
    public partial class ManageStudio
    {
        private readonly EntityType _entityType;
        public ManageStudio(EntityType entityType)
        {
            InitializeComponent();
            _entityType = entityType;
            DataContext = GetStudios();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            IList objSource = GetStudios();
            IList objTarget = (IList)dtgData.DataContext;

            foreach (Publisher item in objSource)
            {
                if (!objTarget.Contains(item))
                    DeleteStudio(item);
            }
            foreach (Publisher item in objTarget)
            {
                if (!objSource.Contains(item))
                    AddStudio(item);
            }
            Close();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private IList GetStudios()
        {
            switch (_entityType)
            {
                case EntityType.Apps:
                case EntityType.Books:
                case EntityType.Games:
                case EntityType.Nds:
                    return PublisherServices.GetPublishers("App_Editor");
                case EntityType.Movie:
                case  EntityType.Series:
                    return PublisherServices.GetPublishers("Movie_Studio");
                case EntityType.Music:
                    return PublisherServices.GetPublishers("Music_Studio");
                case EntityType.XXX:
                    return PublisherServices.GetPublishers("XXX_Studio");
                default:
                    return null;
            }
        }

        private void DeleteStudio(Publisher item)
        {
            switch (_entityType)
            {
                case EntityType.Apps:
                    PublisherServices.DeletePublisher(item.Id, "Apps", "Editor_Id", "App_Editor", item.Name);
                    break;
                case EntityType.Books:
                    PublisherServices.DeletePublisher(item.Id, "Books", "Editor_Id", "App_Editor", item.Name);
                    break;
                case EntityType.Games:
                    PublisherServices.DeletePublisher(item.Id, "Gamez", "Editor_Id", "App_Editor", item.Name);
                    break;
                case EntityType.Movie:
                    PublisherServices.DeletePublisher(item.Id, "Movie", "Studio_Id", "Movie_Studio",item.Name);
                    break;
                case EntityType.Music:
                    PublisherServices.DeletePublisher(item.Id, "Music", "Studio_Id", "Music_Studio", item.Name);
                    break;
                case EntityType.Nds:
                    PublisherServices.DeletePublisher(item.Id, "Nds", "Editor_Id", "App_Editor", item.Name);
                    break;
                case EntityType.Series:
                    PublisherServices.DeletePublisher(item.Id, "Series", "Studio_Id", "Movie_Studio", item.Name);
                    break;
                case EntityType.XXX:
                    PublisherServices.DeletePublisher(item.Id, "XXX", "Studio_Id", "XXX_Studio", item.Name);
                    break;
            }
        }

        private void AddStudio(Publisher publisher)
        {
            switch (_entityType)
            {
                case EntityType.Apps:
                case EntityType.Games:
                case EntityType.Nds:
                case EntityType.Books:
                    PublisherServices.CreatePublisher("App_Editor", publisher);
                    break;
                case EntityType.Series:
                case EntityType.Movie:
                    PublisherServices.CreatePublisher("Movie_Studio", publisher);
                    break;
                case EntityType.Music:
                    PublisherServices.CreatePublisher("Music_Studio", publisher);
                    break;
                case EntityType.XXX:
                    PublisherServices.CreatePublisher("XXX_Studio", publisher);
                    break;
            }
        }
    }
}