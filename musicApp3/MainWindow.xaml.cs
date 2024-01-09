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
using System.Windows;





namespace musicApp3
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MP3FileInfo> MP3Files { get; set; }
        public ObservableCollection<BitmapImage> CoverArtImages { get; } = new ObservableCollection<BitmapImage>();
        private MediaPlayer mediaPlayer = new MediaPlayer();

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
                // Get the selected item from the ListView
                MP3FileInfo selectedFile = (MP3FileInfo)mp3ListView.SelectedItem;

                // Update the cover art image based on the selected item
                if (selectedFile.CoverArtImage != null)
                {
                    // If cover art exists, set the source of the coverArtImage control
                    coverArtImage.Source = selectedFile.CoverArtImage;
                    coverArtImage.Visibility = Visibility.Visible; // Show the cover art image
                }
                else
                {
                    // If there is no cover art, display an error message or hide the cover art image
                    coverArtImage.Visibility = Visibility.Collapsed; // Hide the cover art image
                }

                // Update other elements in the GUI based on the selected item
                // For example, update a TextBlock named "selectedItemInfo"
                selectedItemInfo.Text = $"Selected Item: {selectedFile.FileName}";

                // Display the file path in the filePathTextBox
                filePathTextBox.Text = selectedFile.FilePath;
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
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            mp3ListView.Visibility = Visibility.Visible;  // Show the list view
        }

        private void LoadMP3Files(string folderPath)
        {
            MP3Files.Clear();

            string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");

            foreach (string mp3File in mp3Files)
            {
                var mp3Info = new MP3FileInfo
                {
                    FilePath = mp3File,
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
            if (mp3ListView.SelectedItem != null)
            {
                MP3FileInfo selectedFile = (MP3FileInfo)mp3ListView.SelectedItem;

                Debug.WriteLine("Selected File: " + selectedFile.FileName);

                if (selectedFile.CoverArtImage != null)
                {
                    Debug.WriteLine("Cover Art found.");

                    BitmapImage coverArtBitmap = new BitmapImage();
                    coverArtBitmap.BeginInit();
                    coverArtBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    coverArtBitmap.StreamSource = new MemoryStream(selectedFile.CoverArt);
                    coverArtBitmap.EndInit();

                    coverArtImage.Source = coverArtBitmap;
                    coverArtImage.Visibility = Visibility.Visible;

                    // Hide the coverArtMessage when cover art is found
                    coverArtMessage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    coverArtImage.Visibility = Visibility.Collapsed;
                    emptyCoverArt.Visibility = Visibility.Visible;
                    // Show the coverArtMessage when no cover art is found
                    coverArtMessage.Visibility = Visibility.Visible;
                }
            }
        }

        private void ImageClicked(object sender, RoutedEventArgs e)
        {
 
        }


        private void DisplayCoverArt(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);

                // Check if there is cover art.
                if (file.Tag.Pictures.Length > 0)
                {
                    var picture = file.Tag.Pictures[0];
                    var pictureData = picture.Data.Data;

                    // Display the cover art image in the Image control.
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(pictureData);
                    bitmapImage.EndInit();

                    coverArtImage.Source = bitmapImage;
                }
                else
                {
                    MessageBox.Show("No cover art found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void OpenMP3_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "MP3 Files|*.mp3";

            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private void UpdateMetadata_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtFilePath.Text;

            if (System.IO.File.Exists(filePath)) // Use System.IO.File.Exists for the file check
            {
                try
                {
                    // Load the MP3 file using TagLib.File
                    var mp3File = TagLib.File.Create(filePath);

                    // Update artist metadata
                    mp3File.Tag.Performers = new string[] { txtNewArtist.Text };

                    // Download the cover art from an online URL and set it
                    string coverArtUrl = txtNewCoverURL.Text;
                    using (System.Net.WebClient webClient = new System.Net.WebClient())
                    {
                        byte[] coverData = webClient.DownloadData(coverArtUrl);
                        var coverArt = new Picture
                        {
                            Type = PictureType.FrontCover,
                            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                            Data = new TagLib.ByteVector(coverData)
                        };
                        mp3File.Tag.Pictures = new IPicture[] { coverArt };
                    }

                    // Save changes
                    mp3File.Save();

                    MessageBox.Show("Metadata updated successfully.");

                    // Update TextBox controls with new values
                    txtNewArtist.Text = mp3File.Tag.Performers.Length > 0 ? mp3File.Tag.Performers[0] : ""; // Update artist TextBox
                    txtNewCoverURL.Text = coverArtUrl; // Update cover URL TextBox

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("File not found.");
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (mp3ListView.SelectedItem != null)
            {
                MP3FileInfo selectedFile = (MP3FileInfo)mp3ListView.SelectedItem;
                mediaPlayer.Open(new Uri(selectedFile.FilePath, UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
            }
            else
            {
                // Optionally, handle the case where no item is selected
                MessageBox.Show("Please select a file to play.");
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = volumeSlider.Value;
        }



    }
}
