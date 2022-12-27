namespace Naheulbook.GenerateDevCertificate;

public static class FileSystemHelper
{
    public static string FindDirectoryContaining(string searchedFile)
    {
        var path = Directory.GetCurrentDirectory();
        while (path != null)
        {
            if (File.Exists(Path.Combine(path, searchedFile)))
                return path;
            var parent = Directory.GetParent(path)?.FullName;
            if (parent == path)
                break;
            path = parent;
        }

        throw new Exception($"Failed to find a directory containing '{searchedFile}' while searching from {Directory.GetCurrentDirectory()}");
    }
}