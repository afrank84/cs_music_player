using System;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Diagnostics;
using TagLib;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;
using System.Windows.Input;
using System.Windows.Media;




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
            var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                Description = "Select a Folder"
            };

            if (folderDialog.ShowDialog(this) == true)
            {
                LoadMP3Files(folderDialog.SelectedPath);
            }
        }




        public class MP3FileInfo
        {
            public MP3FileInfo()
            {
                FileName = ""; // Initialize with an empty string or another appropriate default value
                FilePath = "";
                FileSizeBytes = 0;
                Duration = "";
                Artist = "";
                Album = "";
                Year = 0;
                CoverArt = null; // Initialize as null for now
            }

            public string FileName { get; set; }
            public string FilePath { get; set; }
            public long FileSizeBytes { get; set; }
            public string Duration { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
            public int Year { get; set; }
            public byte[]? CoverArt { get; set; } // Nullable byte[] for cover art

            // Computed property to convert CoverArt data to ImageSource
            public ImageSource? CoverArtImage
            {
                get
                {
                    if (CoverArt != null)
                    {
                        using (var ms = new MemoryStream(CoverArt))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            return image;
                        }
                    }
                    return null; // Return null if CoverArt is null or empty
                }
            }
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

                if (selectedFile.CoverArt != null)
                {
                    // Create a new BitmapImage and set its source using a MemoryStream
                    BitmapImage coverArtBitmap = new BitmapImage();
                    coverArtBitmap.BeginInit();
                    coverArtBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    coverArtBitmap.StreamSource = new MemoryStream(selectedFile.CoverArt);
                    coverArtBitmap.EndInit();

                    // Set the source of the coverArtImage control
                    coverArtImage.Source = coverArtBitmap;
                    statusTextBlock.Text = "Cover art displayed";
                    coverArtImage.Visibility = Visibility.Visible; // Show the cover art image
                    redSquare.Visibility = Visibility.Collapsed; // Hide the red square
                }
                else
                {
                    // If there is no cover art, display an error message
                    statusTextBlock.Text = "No cover art available AHAHAHAAH";
                    coverArtImage.Visibility = Visibility.Collapsed; // Hide the cover art image
                    redSquare.Visibility = Visibility.Visible; // Show the red square
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

        private void LoadMP3Files(string folderPath)
        {
            MP3Files.Clear();

            string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");

            foreach (string mp3File in mp3Files)
            {
                var mp3Info = new MP3FileInfo
                {
                    FileName = Path.GetFileName(mp3File)
                };

                try
                {
                    using (var tagFile = TagLib.File.Create(mp3File))
                    {
                        mp3Info.Artist = tagFile.Tag.FirstPerformer;
                        mp3Info.Album = tagFile.Tag.Album;

                        if (tagFile.Tag.Pictures.Length > 0)
                        {
                            var picture = tagFile.Tag.Pictures[0];
                            mp3Info.CoverArt = picture.Data.Data; // Assign the cover art data directly
                        }
                    }
                }
                catch (Exception)
                {
                    // Handle exceptions if there is an issue reading the MP3 file's metadata
                }

                MP3Files.Add(mp3Info);
            }
        }

        private BitmapImage ToImage(byte[] array)
        {
            using var ms = new System.IO.MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }


        private void mp3ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Handle the double-click event here
            if (mp3ListView.SelectedItem != null)
            {
                MP3FileInfo selectedFile = (MP3FileInfo)mp3ListView.SelectedItem;

                // Check if the selected file has cover art
                if (selectedFile.CoverArtImage != null)
                {
                    // If cover art exists, set the source of the coverArtImage control
                    coverArtImage.Source = selectedFile.CoverArtImage;
                    statusTextBlock.Text = "Cover art displayed";
                    coverArtImage.Visibility = Visibility.Visible; // Show the cover art image
                    redSquare.Visibility = Visibility.Collapsed; // Hide the red square
                }
                else
                {
                    // If there is no cover art, display an error message
                    statusTextBlock.Text = "No cover art available, this sucks";
                    coverArtImage.Visibility = Visibility.Collapsed; // Hide the cover art image
                    redSquare.Visibility = Visibility.Visible; // Show the red square
                }
            }
        }




    }
}
