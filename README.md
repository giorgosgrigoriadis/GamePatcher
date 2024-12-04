# Game Patcher

A simple game patcher/launcher built with C# to manage updates and installations for a game. This tool ensures that the game is always up-to-date by checking for version changes and downloading the latest game files when needed.

---

## Features

- **Automatic Update Check:** Compares local and online versions of the game to determine if an update is required.
- **Game Download & Installation:** Downloads the game files (ZIP format) and extracts them automatically.
- **Dynamic Version Handling:** Handles versioning with a simple `major.minor.subminor` format.
- **Play Button:** Launches the game executable once it is installed or updated.
- **Retry on Failure:** Provides an option to retry the update process if it fails.

---

## How It Works

### Check for Updates:
1. The launcher downloads a version file from a specified URL.
2. Compares the downloaded version with the local version stored on the user's machine.
3. If the versions differ, it triggers an update.

### Download and Install:
1. Downloads the latest game files (ZIP archive).
2. Extracts the ZIP file to a specified folder.
3. Locates the game executable (`.exe`) file dynamically.
4. Updates the local version file after a successful installation.

### Launch the Game:
- Allows the user to launch the game if the installation or update process is successful.

---

## Prerequisites

- **.NET Framework:** Ensure you have the required .NET version to run the application.
- **Game Files:**
  - A hosted `Version.txt` file containing the game version (e.g., `1.0.0`).
  - A hosted ZIP archive containing the game files.

---

## Setup

1. **Open the solution in Visual Studio.**
2. **Update the URLs in `MainWindow.xaml.cs` to match your hosted files:**

   ```csharp
   private string versionFileURL = "YOUR_VERSION_FILE_URL";
   private string gameZipURL = "YOUR_GAME_ZIP_URL";
3. Build and run the application.
