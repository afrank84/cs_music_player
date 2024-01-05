using System;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Win32;

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
                FileName = "Select Folder",
                Filter = "Folder|*.thisisfake"
            };

            if (folderDialog.ShowDialog() == true)
            {
                string selectedFolderPath = System.IO.Path.GetDirectoryName(folderDialog.FileName);
                string[] mp3Files = Directory.GetFiles(selectedFolderPath, "*.mp3");

                MP3Files.Clear();

                foreach (string mp3File in mp3Files)
                {
                    MP3Files.Add(new MP3FileInfo
                    {
                        FileName = Path.GetFileName(mp3File),
                        FilePath = mp3File
                    });
                }
            }
        }

        public class MP3FileInfo
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
