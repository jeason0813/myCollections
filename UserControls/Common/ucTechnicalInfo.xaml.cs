using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using myCollections.BL.Services;
using myCollections.Data.SqlLite;
using myCollections.Utils;
using System.Linq;
using System.Windows.Media.Imaging;
using myCollections.Pages;

namespace myCollections.UserControls.Common
{
    public partial class UcTechnicalInfo
    {
        private IMyCollectionsData _objEntity;
        private bool _bLoaded;

        public UcTechnicalInfo()
        {
            InitializeComponent();
        }

        public IMyCollectionsData CurrentEntity
        {
            get { return _objEntity; }
            set { _objEntity = value; }
        }

        public void Refresh()
        {
            if (_bLoaded == false)
                AddCheckBox();
            Bind();
        }
        public void InitCombo()
        {
            try
            {
                cboFileFormat.ItemsSource = MovieServices.GetFormats();
                cboAspectRatio.ItemsSource = MovieServices.GetAspectRatios();
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                new MessageBoxYesNo(ex.Message, false, true).ShowDialog();
            }
        }
        private void Bind()
        {
            if (_objEntity != null)
            {
                DataContext = _objEntity;
                #region FileFormat
                if (_objEntity.FileFormat != null)
                {
                    List<FileFormat> items = cboFileFormat.ItemsSource as List<FileFormat>;
                    if (items != null && items.Any(item => item.Id == _objEntity.FileFormat.Id) == false)
                        items.Add(_objEntity.FileFormat);

                    cboFileFormat.Text = _objEntity.FileFormat.Name;
                }
                else
                    cboFileFormat.Text = string.Empty;
                #endregion
                #region AspectRatio
                if (_objEntity.AspectRatio != null)
                {
                    List<AspectRatio> items = cboAspectRatio.ItemsSource as List<AspectRatio>;
                    if (items != null && items.Any(item => item.Id == _objEntity.AspectRatio.Id) == false)
                        items.Add(_objEntity.AspectRatio);

                    cboAspectRatio.Text = _objEntity.AspectRatio.Name;
                }
                else
                    cboAspectRatio.Text = string.Empty;

                if (_objEntity.ObjectType == EntityType.XXX || _objEntity.ObjectType==EntityType.Series)
                { 
                    cboAspectRatio.Visibility = Visibility.Hidden;
                    lblAspectRatio.Visibility = Visibility.Hidden;
                }
                #endregion
                RefreshSubs();
                RefreshAudios();


            }
        }
        private void RefreshSubs()
        {
            try
            {
                CurrentSubs.Children.Clear();

                foreach (Language item in _objEntity.Subtitles)
                {
                    if (item != null)
                    {
                        Uri path = null;
                        switch (item.ShortName)
                        {
                            case "Français":
                            case "French":
                                path = new Uri("pack://application:,,,/myCollections;component/Images/Flags/French.png");
                                break;
                            case "Anglais":
                            case "English":
                                path = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Us.png");
                                break;
                            case "Dutch":
                                path = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Dutch.png");
                                break;
                        }

                        if (path != null)
                        {
                            BitmapImage objImage = new BitmapImage();
                            objImage.BeginInit();
                            objImage.UriSource = path;
                            objImage.DecodePixelHeight = 50;
                            objImage.EndInit();
                            objImage.Freeze();

                            Image image = new Image();
                            image.Height = 40;
                            image.Source = objImage;
                            image.Margin = new Thickness(5);

                            CurrentSubs.Children.Add(image);
                        }
                        else
                        {
                            TextBlock name = new TextBlock();
                            name.Margin = new Thickness(5);
                            name.Text = item.DisplayName;
                            CurrentSubs.Children.Add(name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        private void RefreshAudios()
        {
            try
            {
                CurrentAudios.Children.Clear();

                foreach (Audio item in _objEntity.Audios)
                {
                    if (item.Language != null)
                    {
                        Uri flag = null;
                        Uri logo = null;
                        switch (item.Language.ShortName)
                        {
                            case "Français":
                            case "French":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/French.png");
                                break;
                            case "Anglais":
                            case "English":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/US.png");
                                break;
                            case "Chinese":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Chinese.png");
                                break;
                            case "Japanese":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Japan.png");
                                break;
                            case "German":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Germany.png");
                                break;
                            case "Italian":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Italy.png");
                                break;
                            case "Thai":
                                flag =new Uri("pack://application:,,,/myCollections;component/Images/Flags/Thailand.png");
                                break;
                            case "Danish":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Danish.png");
                                break;
                            case "Spanish":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Spain.png");
                                break;
                            case "Portuguese":
                                flag = new Uri("pack://application:,,,/myCollections;component/Images/Flags/Portugal.png");
                                break;
                        }

                        switch (item.AudioType.Name)
                        {
                            case "DTS":
                                logo = new Uri("pack://application:,,,/myCollections;component/Images/Logos/DTS.png");
                                break;
                            case "DD2.03d":
                            case "DD2.0":
                                logo = new Uri("pack://application:,,,/myCollections;component/Images/Logos/dolby-surround.png");
                                break;
                            case "DD5.1":
                            case "AC-3":
                                logo = new Uri("pack://application:,,,/myCollections;component/Images/Logos/Ac3.png");
                                break;
                        }

                        StackPanel track = new StackPanel();
                        track.Orientation = Orientation.Horizontal;
                        if (flag != null)
                        {
                            BitmapImage objImage = new BitmapImage();
                            objImage.BeginInit();
                            objImage.UriSource = flag;
                            objImage.DecodePixelHeight = 40;
                            objImage.EndInit();
                            objImage.Freeze();

                            Image image = new Image();
                            image.Height = 40;
                            image.Source = objImage;
                            image.Margin = new Thickness(5);

                            track.Children.Add(image);
                        }
                        else
                        {
                            TextBlock name = new TextBlock();
                            name.Text = item.Language.DisplayName;
                            track.Children.Add(name);
                        }

                        if (logo != null)
                        {
                            BitmapImage objImage = new BitmapImage();
                            objImage.BeginInit();
                            objImage.UriSource = logo;
                            objImage.DecodePixelHeight = 40;
                            objImage.EndInit();
                            objImage.Freeze();

                            Image image = new Image();
                            image.VerticalAlignment = VerticalAlignment.Center;
                            image.Height = 30;
                            image.Source = objImage;
                            image.Margin = new Thickness(5);

                            track.Children.Add(image);
                        }
                        else
                        {
                            TextBlock name = new TextBlock();
                            name.Text = item.AudioType.Name;
                            track.Children.Add(name);
                        }

                        CurrentAudios.Children.Add(track);
                    }
                }
            }

            catch (Exception ex)
            {
                Util.LogException(ex);
            }
        }
        private void AddCheckBox()
        {
            _bLoaded = true;
            foreach (string item in MetaDataServices.Get(_objEntity.ObjectType))
            {
                if (string.IsNullOrWhiteSpace(item) == false)
                {
                    CheckBox objCheckBox = new CheckBox();
                    objCheckBox.Margin = new Thickness(5, 0, 0, 0);
                    objCheckBox.Content = item;
                    objCheckBox.IsChecked = false;

                    if (_objEntity != null)
                        if (_objEntity.MetaDatas != null)
                        {
                            foreach (MetaData itemData in _objEntity.MetaDatas)
                            {
                                if (itemData != null)
                                    if (itemData.Name == item)
                                    {
                                        objCheckBox.IsChecked = true;
                                        break;
                                    }
                            }
                        }
                    RootPanel.Children.Add(objCheckBox);
                }
            }
        }

    }
}