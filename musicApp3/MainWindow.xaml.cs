using System;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Diagnostics;
using TagLib;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using Ookii.Dialogs.Wpf;




namespace musicApp3
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MP3FileInfo> MP3Files { get; set; }
        public ObservableCollection<BitmapImage> CoverArtImages { get; } = new ObservableCollection<BitmapImage>();


        public MainWindow()
        {
            InitializeComponent();
            MP3Files = new ObservableCollection<MP3FileInfo>();
            mp3ListView.ItemsSource = MP3Files;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog
            {
                Description = "Select a Folder",
                UseDescriptionForTitle = true
            };

            if (folderDialog.ShowDialog(this) == true)
            {
                string selectedFolderPath = folderDialog.SelectedPath;
                string[] mp3Files = Directory.GetFiles(selectedFolderPath, "*.mp3");

                MP3Files.Clear();

                foreach (string mp3File in mp3Files)
                {
                    var mp3Info = new MP3FileInfo
                    {
                        FileName = Path.GetFileName(mp3File),
                        FilePath = mp3File
                    };

                    try
                    {
                        using (var tagFile = TagLib.File.Create(mp3File))
                        {
                            mp3Info.FileSizeBytes = new FileInfo(mp3File).Length; // Get file size in bytes
                            mp3Info.Duration = tagFile.Properties.Duration.ToString(@"mm\:ss");
                            mp3Info.Artist = tagFile.Tag.FirstPerformer;
                            mp3Info.Album = tagFile.Tag.Album;
                            mp3Info.Year = (int)tagFile.Tag.Year;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions if there is an issue reading the MP3 file's metadata
                        mp3Info.Duration = "N/A";
                        mp3Info.Artist = "N/A";
                        mp3Info.Album = "N/A";
                        mp3Info.Year = 0;
                    }

                    MP3Files.Add(mp3Info);
                }
            }
        }




        public class MP3FileInfo
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public long FileSizeBytes { get; set; }
            public string Duration { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
            public int Year { get; set; }
            public byte[] CoverArt { get; set; }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mp3ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mp3ListView.SelectedItem != null)
            {
                MP3FileInfo selectedFile = (MP3FileInfo)mp3ListView.SelectedItem;

                // Check if the selected file has cover art
                if (FileHasCoverArt(selectedFile.FilePath))
                {
                    // Load and display the cover art image
                    var coverArt = LoadCoverArt(selectedFile.FilePath);
                    coverArtImage.Source = coverArt;

                    // Show the coverArtImage and hide the redSquare
                    coverArtImage.Visibility = Visibility.Visible;
                    redSquare.Visibility = Visibility.Collapsed;

                    // Update the statusTextBlock to indicate that cover art is displayed
                    statusTextBlock.Text = "Cover art displayed";
                }
                else
                {
                    // If no cover art is found, show the red square and hide the coverArtImage
                    coverArtImage.Visibility = Visibility.Collapsed;
                    redSquare.Visibility = Visibility.Visible;

                    // Update the statusTextBlock to indicate that no cover art is available
                    statusTextBlock.Text = "No cover art available";
                }
            }
        }



        private bool FileHasCoverArt(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);
                return file.Tag.Pictures.Length > 0;
            }
            catch (Exception)
            {
                // Handle any exceptions here (e.g., invalid file format)
                return false;
            }
        }

        private BitmapImage LoadCoverArt(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);
                if (file.Tag.Pictures.Length > 0)
                {
                    var pictureData = file.Tag.Pictures[0].Data.Data;
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(pictureData);
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception)
            {
                // Handle any exceptions here (e.g., invalid file format)
            }

            return null; // Return null if no cover art is found or an error occurs
        }

        private void GalleryView_Click(object sender, RoutedEventArgs e)
        {
            mp3ListView.Visibility = Visibility.Collapsed; // Hide the list view
            galleryView.Visibility = Visibility.Visible; // Show the gallery view
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            mp3ListView.Visibility = Visibility.Visible;  // Show the list view
            galleryView.Visibility = Visibility.Collapsed; // Hide the gallery view
        }



    }
}
