using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;

namespace GamePatcher
{
    // Enum to represent the current state of the launcher
    enum PatcherStatus
    {
        ready,              // Launcher is ready to play the game
        failed,             // An error occurred
        downloadingGame,    // Game is being downloaded for the first time
        downloadingUpdate   // Game update is being downloaded
    }

    public partial class MainWindow : Window
    {
        // URLs for the version file and the game zip file
        // IMPORTANT: Needs direct download links
        private string versionFileURL = "YOUR_VERSION_FILE_URL";
        private string gameZipURL = "YOUR_GAME_ZIP_URL";

        // Paths used by the launcher
        private string rootPath;     // Root directory of the launcher
        private string versionFile;  // Path to the local version file
        private string gameZip;      // Path to the downloaded game zip
        private string gameExe;      // Path to the game's executable
        private string fileName;     // Name of the downloaded file (without extension)

        // Property to manage the current status of the launcher
        private PatcherStatus _status;
        internal PatcherStatus Status
        {
            get => _status;
            set
            {
                _status = value;

                // Update the button text based on the current status
                switch (_status)
                {
                    case PatcherStatus.ready:
                        PlayButton.Content = "Play";
                        break;
                    case PatcherStatus.failed:
                        PlayButton.Content = "Update Failed - Retry";
                        break;
                    case PatcherStatus.downloadingGame:
                        PlayButton.Content = "Downloading Game";
                        break;
                    case PatcherStatus.downloadingUpdate:
                        PlayButton.Content = "Downloading Update";
                        break;
                }
            }
        }

        // Constructor
        public MainWindow()
        {
            InitializeComponent();

            // Initialize paths
            rootPath = Directory.GetCurrentDirectory();
            versionFile = Path.Combine(rootPath, "Version.txt");

            // Dynamically extract the file name from the game zip URL
            fileName = Path.GetFileNameWithoutExtension(new Uri(gameZipURL).LocalPath);
            gameZip = Path.Combine(rootPath, $"{fileName}.zip");
        }

        // Method to check for updates by comparing local and online versions
        private void CheckForUpdates()
        {
            Version localVersion = new Version();

            if (File.Exists(versionFile))
            {
                // Read the local version from the version file
                localVersion = new Version(File.ReadAllText(versionFile));
                VersionText.Text = localVersion.ToString();
            }
            try
            {
                    WebClient webClient = new WebClient();

                    // Download the online version
                    Version onlineVersion = new Version(webClient.DownloadString(versionFileURL));

                    // Compare versions
                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        // Update if the versions differ
                        InstallGameFiles(true, onlineVersion);
                    }
                    else
                    {
                        // Game is up-to-date
                        Status = PatcherStatus.ready;
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors during the update check
                    Status = PatcherStatus.failed;
                    MessageBox.Show($"Error checking for game updates: {ex}");
                }
            
            
        }

        // Method to download and install the game files
        private void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();

                // Update the status based on whether it's an update or a fresh install
                Status = _isUpdate ? PatcherStatus.downloadingUpdate : PatcherStatus.downloadingGame;

                // Attach an event handler for when the download completes
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);

                // Start downloading the game zip
                webClient.DownloadFileAsync(new Uri(gameZipURL), gameZip, _onlineVersion);
            }
            catch (Exception ex)
            {
                // Handle errors during download
                Status = PatcherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }

        // Callback method invoked when the game download is complete
        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();

                // Extract the zip file to a folder named after the downloaded file
                string extractPath = Path.Combine(rootPath, fileName);
                ZipFile.ExtractToDirectory(gameZip, extractPath, true);

                // Delete the zip file after extraction
                File.Delete(gameZip);

                // Find the game's executable file in the extracted directory
                var exeFiles = Directory.GetFiles(extractPath, "*.exe", SearchOption.AllDirectories);
                if (exeFiles.Length > 0)
                {
                    gameExe = exeFiles[0]; // Use the first .exe file found
                }
                else
                {
                    throw new Exception("Executable file not found in the extracted archive.");
                }

                // Write the online version to the local version file
                File.WriteAllText(versionFile, onlineVersion);

                // Update the UI and status
                VersionText.Text = onlineVersion;
                Status = PatcherStatus.ready;
            }
            catch (Exception ex)
            {
                // Handle errors during installation
                Status = PatcherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }

        // Event triggered when the window finishes rendering
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        // Event handler for the Play button click
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(gameExe) && File.Exists(gameExe) && Status == PatcherStatus.ready)
            {
                // Launch the game executable
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe)
                {
                    WorkingDirectory = Path.GetDirectoryName(gameExe)
                };
                Process.Start(startInfo);

                // Close the launcher
                Close();
            }
            else if (Status == PatcherStatus.failed)
            {
                // Retry updates if the last attempt failed
                CheckForUpdates();
            }
        }
    }

    // Struct to handle versioning (major.minor.subminor)
    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;    // Major version number
        private short minor;    // Minor version number
        private short subMinor; // Sub-minor version number

        // Constructor to create a version from numbers
        internal Version(short _major, short _minor, short _subMinor)
        {
            major = _major;
            minor = _minor;
            subMinor = _subMinor;
        }

        // Constructor to create a version from a string
        internal Version(string _version)
        {
            string[] versionStrings = _version.Split('.');
            if (versionStrings.Length != 3)
            {
                major = 0;
                minor = 0;
                subMinor = 0;
                return;
            }

            major = short.Parse(versionStrings[0]);
            minor = short.Parse(versionStrings[1]);
            subMinor = short.Parse(versionStrings[2]);
        }

        // Compare this version with another version
        internal bool IsDifferentThan(Version _otherVersion)
        {
            return major != _otherVersion.major ||
                   minor != _otherVersion.minor ||
                   subMinor != _otherVersion.subMinor;
        }

        // Convert the version to a string
        public override string ToString()
        {
            return $"{major}.{minor}.{subMinor}";
        }
    }
}
