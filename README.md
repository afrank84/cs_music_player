## Overview
This C# application is a simple music player and manager that allows you to load and play MP3 files, view their metadata, and manipulate their cover art. It provides a user-friendly interface for managing and listening to your music collection. Below, you'll find an explanation of the key features and functionality of the code.

## Features

### 1. Loading MP3 Files
Click the "Open Folder" button to select a folder containing your MP3 files.
The application will search the selected folder for MP3 files and display them in a list.

### 2. Viewing MP3 Metadata
The application extracts and displays metadata from each MP3 file, including file name, artist, album, year, and duration.
You can switch between list view and gallery view to display the MP3 files differently.

### 3. Editing Metadata
You can edit the artist and album metadata directly within the application by double-clicking on a cell in the data grid.
The application also allows you to rename MP3 files by editing the file name in the data grid.

### 4. Displaying Cover Art
If an MP3 file has embedded cover art, the application displays it in the user interface.
You can click on an MP3 file in the list to view its cover art if available.

### 5. Playing Music
Select an MP3 file from the list and click the "Play" button to start playing it.
You can also pause, stop, and adjust the volume while playing music.
The application supports repeat mode for continuous playback of the same song.

### 6. Searching for MP3 Files
You can use the search bar to filter MP3 files by artist, album, or file name.
Enter your search query, and the list will update to display matching results.

### 7. Updating Metadata and Cover Art
You can update the artist metadata and cover art of an MP3 file by providing new values and clicking the "Update Metadata" button.
The application can download cover art from a specified URL and update the selected MP3 file's metadata.

### 8. Exit
Click the "Exit" button to close the application.


## How to Use
Open the application.
Click the "Open Folder" button and select a folder containing your MP3 files.
View and manage your MP3 files using the provided features.
Play music by selecting an MP3 file and clicking the "Play" button.
Edit metadata and cover art as needed.
Search for specific MP3 files using the search bar.
Exit the application when you're done.

###Dependencies
The application relies on the following libraries and frameworks:

System, 
System.IO, 
System.Windows, 
System.Collections.ObjectModel, 
Microsoft.Win32, 
System.Diagnostics, 
TagLib, 
Ookii.Dialogs.Wpf, 
System.Data, 
and other .NET libraries.

It uses Windows Presentation Foundation (WPF) for the user interface.
Disclaimer
This code is a simplified example of a music player and manager and may require additional error handling and refinement for production use. It's recommended to adapt and extend the code to meet your specific requirements and handle potential edge cases.

01/11/2024 (ScreenShot)
![image](https://github.com/afrank84/cs_music_art/assets/11393535/4f68ea75-8c3b-4c0d-a7bd-4ff31f909673)
