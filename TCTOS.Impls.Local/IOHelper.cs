namespace TCTOS.Impls.Local;

public static class IoHelper
{
    public static readonly UnixFileMode DefaultFileMode =
        UnixFileMode.UserRead |
        UnixFileMode.UserWrite |
        UnixFileMode.GroupRead |
        UnixFileMode.OtherRead;

    public static readonly UnixFileMode DefaultDirectoryMode =
        UnixFileMode.UserRead |
        UnixFileMode.UserWrite |
        UnixFileMode.UserExecute |
        UnixFileMode.GroupRead |
        UnixFileMode.GroupExecute |
        UnixFileMode.OtherRead |
        UnixFileMode.OtherExecute; 
     
    public static void CreateDirectory(string directoryPath, UnixFileMode fileMode)
    {
        Directory.CreateDirectory(directoryPath);
#pragma warning disable CA1416
        File.SetUnixFileMode(directoryPath, fileMode);
#pragma warning restore CA1416
    }

    public static async Task WriteTextFileAsync(string filePath, string fileContents, UnixFileMode fileMode)
    {
        await File.WriteAllTextAsync(filePath, fileContents);
#pragma warning disable CA1416
        File.SetUnixFileMode(filePath, fileMode);
#pragma warning restore CA1416
    }

    public static async Task WriteBinaryFileAsync(string filePath, byte[] fileContents, UnixFileMode fileMode)
    {
        await File.WriteAllBytesAsync(filePath, fileContents);
#pragma warning disable CA1416
        File.SetUnixFileMode(filePath, fileMode);
#pragma warning restore CA1416
    }
}