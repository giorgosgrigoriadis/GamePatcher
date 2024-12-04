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

### 1. **.NET Framework**
Ensure you have the required .NET version installed to run the application. The version should match the target framework specified in the project's `csproj` file (e.g., `.NET 6.0`).

### 2. **Game Files**
To use the patcher, you need to host two files:
1. A `Version.txt` file in the format `major.minor.subminor` (e.g., `1.0.0`).
2. A ZIP archive containing the game files.

---

### Hosted File Links

Both the `Version.txt` and the ZIP file must be hosted online with direct download links. Here is an example if you are using Dropbox:

#### How to Create a Direct Download Link with Dropbox

1. **Upload Files to Dropbox**
   - Upload `Version.txt` and your game ZIP file to your Dropbox account.

2. **Copy the Sharing Link**
   - **On the web app:**  
     Hover over the file you want to share and click the **Copy link** button.  
     Alternatively, right-click the file and select **Copy link**.
   - **In the Dropbox Finder/File Explorer app:**  
     Right-click the file and select **Copy Dropbox link**.

3. **Modify the Sharing Link**
   - Paste the copied link into a text editor or browser tab.  
     For example:  
     ```
     https://www.dropbox.com/scl/fi/svmal51kp1qbf0rhnhhq6/Version.txt?rlkey=ubr8jiwyu90w1gzcfowxvx8a3&dl=0
     ```
   - Replace the `dl=0` at the end of the link with `dl=1` to create a direct download link:
     ```
     https://www.dropbox.com/scl/fi/svmal51kp1qbf0rhnhhq6/Version.txt?rlkey=ubr8jiwyu90w1gzcfowxvx8a3&dl=1
     ```

4. **Use the Modified Link in Your Code**
   ```csharp
   private string versionFileURL = "https://www.dropbox.com/scl/fi/svmal51kp1qbf0rhnhhq6/Version.txt?rlkey=ubr8jiwyu90w1gzcfowxvx8a3&dl=1";
   private string gameZipURL = "https://www.dropbox.com/scl/fi/svmal51kp1qbf0rhnhhq6/GameFiles.zip?rlkey=ubr8jiwyu90w1gzcfowxvx8a3&dl=1";

## Setup

1. Open the solution in Visual Studio.  
2. Update the URLs in `MainWindow.xaml.cs` to match your hosted files:  
   ```csharp
   private string versionFileURL = "YOUR_VERSION_FILE_URL";
   private string gameZipURL = "YOUR_GAME_ZIP_URL";
3. Build and run the application.  
4. After building the solution, the compiled `.exe` file will be located in the following directory:  
   ```bash
   GamePatcher/bin/Debug/net6.0-windows/GamePatcher.exe

