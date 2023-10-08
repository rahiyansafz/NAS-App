using System.Net;
using Polly;

const string nasIpAddress = "10.0.15.18";
const string nasUsername = "Shafin";
const string nasPassword = "Rahiyan@#123";
const string nasFolderPath = "Test"; // NAS folder
const string localFolderPath = @"C:\Users\Rahiyan\Downloads\"; // Local folder

const string imagePath = @"C:\Users\Rahiyan\Downloads\tap.png";

var credentials = new NetworkCredential(nasUsername, nasPassword);

var policy = Policy
    .Handle<Exception>()
    .Retry(3, (exception, retryCount) => { Console.WriteLine($"Retry {retryCount} due to {exception.Message}"); });

policy.Execute(() =>
{
    try
    {
        ListFilesInFolder($@"\\{nasIpAddress}\", nasFolderPath, credentials);
        Console.WriteLine("---------------");
        // UploadFileToFolder($@"\\{nasIpAddress}\", nasFolderPath, credentials, imagePath);
        Console.WriteLine("---------------");
        // CopyFileToLocalFolder($@"\\{nasIpAddress}\", nasFolderPath, credentials, "tap.png", localFolderPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
    }
});

static void ListFilesInFolder(string nasIpAddress, string folderPath, NetworkCredential credentials)
{
    try
    {
        var networkPath = $@"{nasIpAddress}{folderPath}";

        if (!Directory.Exists(networkPath))
        {
            Console.WriteLine($"Folder '{networkPath}' does not exist.");
            return;
        }

        ListFilesRecursively(networkPath, credentials);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void ListFilesRecursively(string folderPath, NetworkCredential credentials)
{
    var files = Directory.GetFiles(folderPath);
    var directories = Directory.GetDirectories(folderPath);

    foreach (var file in files)
        Console.WriteLine($"File: {file}");

    foreach (var directory in directories)
    {
        Console.WriteLine($"Directory: {directory}");
        ListFilesRecursively(directory, credentials);
    }
}

static void UploadFileToFolder(string nasIpAddress, string folderPath, NetworkCredential credentials,
    string sourceFilePath)
{
    try
    {
        var networkPath = $@"{nasIpAddress}{folderPath}\{Path.GetFileName(sourceFilePath)}";

        if (!File.Exists(sourceFilePath))
        {
            Console.WriteLine($"Source file '{sourceFilePath}' does not exist.");
            return;
        }

        File.Copy(sourceFilePath, networkPath);
        Console.WriteLine($"File '{Path.GetFileName(sourceFilePath)}' uploaded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error uploading file: {ex.Message}");
    }
}

static void CopyFileToLocalFolder(string nasIpAddress, string folderPath, NetworkCredential credentials,
    string fileName, string localFolderPath)
{
    try
    {
        var networkPath = $@"{nasIpAddress}{folderPath}\{fileName}";
        var localFilePath = Path.Combine(localFolderPath, fileName);

        if (!File.Exists(networkPath))
        {
            Console.WriteLine($"File '{networkPath}' does not exist on the NAS.");
            return;
        }

        File.Copy(networkPath, localFilePath);
        Console.WriteLine($"File '{fileName}' copied to local folder '{localFolderPath}' successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error copying file to local folder: {ex.Message}");
    }
}