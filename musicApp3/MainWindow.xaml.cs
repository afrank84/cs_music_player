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
using System.Data;
using System.Linq;

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
            mp3DataGrid.ItemsSource = MP3Files;
            DataContext = this;
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
        private void ReloadDataGrid()
        {
            // Assuming your DataGrid is named mp3DataGrid
            mp3DataGrid.ItemsSource = null; // Clear the current data source
            mp3DataGrid.ItemsSource = MP3Files; // Set the data source with the updated MP3Files collection
        }

        public ImageSource CoverArtImageSource
        {
            get { return (ImageSource)GetValue(CoverArtImageSourceProperty); }
            set { SetValue(CoverArtImageSourceProperty, value); }
        }

        public static readonly DependencyProperty CoverArtImageSourceProperty =
            DependencyProperty.Register(nameof(CoverArtImageSource), typeof(ImageSource), typeof(MainWindow), new PropertyMetadata(null));


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
            mp3DataGrid.Visibility = Visibility.Collapsed; // Hide the list view
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            mp3DataGrid.Visibility = Visibility.Visible;  // Show the list view
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
            if (mp3DataGrid.SelectedItem != null)
            {
                MP3FileInfo selectedFile = (MP3FileInfo)mp3DataGrid.SelectedItem;
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

        private void mp3DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var editedItem = e.Row.Item as MP3FileInfo;

                if (editedItem != null)
                {
                    // Check if the column being edited is not the "File Path" column
                    if (!e.Column.Header.Equals("File Path"))
                    {
                        // Get the MP3 file path from your data model
                        string mp3FilePath = editedItem.FilePath;

                        try
                        {
                            // Determine which column is being edited and update the corresponding metadata
                            if (e.Column.Header.Equals("Artist"))
                            {
                                // Update the artist metadata
                                using (var updatedMp3File = TagLib.File.Create(mp3FilePath))
                                {
                                    updatedMp3File.Tag.Performers = new[] { ((System.Windows.Controls.TextBox)e.EditingElement).Text };
                                    updatedMp3File.Save();
                                }
                            }
                            else if (e.Column.Header.Equals("Album"))
                            {
                                // Update the album metadata
                                using (var updatedMp3File = TagLib.File.Create(mp3FilePath))
                                {
                                    updatedMp3File.Tag.Album = ((System.Windows.Controls.TextBox)e.EditingElement).Text;
                                    updatedMp3File.Save();
                                }
                            }
                            else if (e.Column.Header.Equals("File Name"))
                            {
                                // Update the file name (rename the file)
                                string newFileName = ((System.Windows.Controls.TextBox)e.EditingElement).Text;

                                // Generate the new file path with the updated name
                                string newFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(mp3FilePath), newFileName);

                                // Try to rename the file and catch any IOException that may occur
                                try
                                {
                                    System.IO.File.Move(mp3FilePath, newFilePath);

                                    // Update the data model with the new file path
                                    editedItem.FilePath = newFilePath;

                                    // Load the MP3 file using TagLib# with the updated file path
                                    using (var updatedMp3File = TagLib.File.Create(newFilePath))
                                    {
                                        // Update any metadata as needed
                                        // For example, updating artist and album
                                        updatedMp3File.Tag.Performers = new[] { ((System.Windows.Controls.TextBox)e.EditingElement).Text };
                                        updatedMp3File.Tag.Album = ((System.Windows.Controls.TextBox)e.EditingElement).Text;

                                        // Save the changes back to the updated MP3 file
                                        updatedMp3File.Save();
                                    }

                                    // Find the index of the edited item in the MP3Files collection
                                    int index = MP3Files.IndexOf(editedItem);

                                    if (index >= 0)
                                    {
                                        // Remove and re-add the item to refresh the DataGrid
                                        MP3Files.RemoveAt(index);
                                        MP3Files.Insert(index, editedItem);
                                    }
                                }
                                catch (System.IO.IOException ex)
                                {
                                    // Handle the IOException (file in use) gracefully
                                    MessageBox.Show($"Error: {ex.Message}", "File In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        catch (System.IO.IOException ex)
                        {
                            // Handle the IOException (file in use) gracefully
                            MessageBox.Show($"Error: {ex.Message}", "File In Use", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }





        private ImageSource ExtractCoverArt(string mp3FilePath)
        {
            try
            {
                using (var mp3File = TagLib.File.Create(mp3FilePath))
                {
                    if (mp3File.Tag.Pictures.Length > 0)
                    {
                        var picture = mp3File.Tag.Pictures[0];
                        var imageStream = new MemoryStream(picture.Data.Data);
                        var bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = imageStream;
                        bitmapImage.EndInit();

                        return bitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if there is an issue reading the MP3 file's metadata or cover art
                Console.WriteLine($"Error extracting cover art: {ex.Message}");
            }

            return null; // Return null if there is no cover art or an error occurred
        }

        private void mp3DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mp3DataGrid.SelectedItem != null)
            {
                MP3FileInfo selectedFile = (MP3FileInfo)mp3DataGrid.SelectedItem;

                ImageSource coverArt = ExtractCoverArt(selectedFile.FilePath);

                if (coverArt != null)
                {
                    // Update the CoverArtImageSource property
                    CoverArtImageSource = coverArt;

                    selectedItemInfo.Text = $"Selected Item: {selectedFile.FileName}";
                    filePathTextBox.Text = selectedFile.FilePath;
                }
                else
                {
                    // Clear the CoverArtImageSource property if no cover art is available
                    CoverArtImageSource = null;

                    selectedItemInfo.Text = $"Selected Item: {selectedFile.FileName}";
                    filePathTextBox.Text = selectedFile.FilePath;
                }
            }
        }

        private void ReloadDataButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadDataGrid();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MP3Files != null) // Check if MP3Files is not null
            {
                string searchText = SearchTextBox.Text.ToLower();

                var filteredItems = MP3Files.Where(item =>
                    item.Artist != null && item.Artist.ToLower().Contains(searchText) ||
                    item.Album != null && item.Album.ToLower().Contains(searchText) ||
                    item.FileName != null && item.FileName.ToLower().Contains(searchText)
                ).ToList();

                mp3DataGrid.ItemsSource = filteredItems;
            }
        }



        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Search...")
            {
                SearchTextBox.Text = "";
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Search...";
            }
        }


    }
}
