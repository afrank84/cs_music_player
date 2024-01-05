using System;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Diagnostics;
using TagLib;


namespace musicApp3
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MP3FileInfo> MP3Files { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MP3Files = new ObservableCollection<MP3FileInfo>();
            mp3ListView.ItemsSource = MP3Files;
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFileDialog
            {
                Title = "Select a Folder",
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = Environment.CurrentDirectory,
                Filter = "Folder|*.folder"
            };

            if (folderDialog.ShowDialog() == true)
            {
                string selectedFolderPath = System.IO.Path.GetDirectoryName(folderDialog.FileName);
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
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
