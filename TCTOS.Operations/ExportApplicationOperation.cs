using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data.DesktopFiles;
using TCTOS.Common;

namespace TCTOS.Operations;

public static class ExportApplicationOperation
{
    private static string GetDesktopFileName(string containerName, string containerFilePath)
        => $"{containerName}-{Path.GetFileName(containerFilePath)}";

    public static Task<Result> ExportApplicationAsync(
        string containerName,
        string containerDesktopFilePath,
        IComputer computer,
        IIncusFileSystem incusFileSystem,
        IFileSystem fileSystem
    ) => RunCatchingAsync(async () =>
    {
        try
        {
            (await incusFileSystem.PrepareFileSystem(containerName)).ThrowIfFailed();

            var fileContents = (await incusFileSystem.GetFileTextAsync(containerDesktopFilePath)).GetOrThrow();
            var desktopFile = DesktopFileParser.ParseFromText(fileContents);

            var containerConfig = (await fileSystem.GetContainerConfigurationAsync(containerName)).GetOrThrow()!;

            List<IDesktopFileLine> lines = [];
            foreach (var line in desktopFile.Lines)
            {
                if (line is KeyValueDesktopFileLine { Key: "Exec" or "TryExec" } execDesktopFileLine)
                {
                    var command = $"tctos launch {containerName} {execDesktopFileLine.Value}";
                    lines.Add(execDesktopFileLine with { Value = command });
                }
                else if (line is KeyValueDesktopFileLine { Key: "Icon" } iconDesktopFileLine)
                {
                    var iconBytes = (await incusFileSystem.GetIconBytesAsync(iconDesktopFileLine.Value)).GetOrThrow();
                    if (iconBytes != null)
                    {
                        var iconName = Guid.NewGuid().ToString();
                        using var imageBytesStream = new MemoryStream(iconBytes);
                        using var image = await Image.LoadAsync<Rgba32>(imageBytesStream);
                        using var moduledImage = ColorModulator.ModulateImage(image, containerConfig.Color);
                        using var modulatedImageBytesStream = new MemoryStream();
                        await moduledImage.SaveAsync(modulatedImageBytesStream, image.Metadata.DecodedImageFormat!);
                        var iconPath =
                            (await computer.AddIconFileAsync(iconName, containerName,
                                modulatedImageBytesStream.ToArray())).GetOrThrow();
                        lines.Add(iconDesktopFileLine with { Value = iconPath });
                    }
                    else
                        lines.Add(line);
                }
                else if (line is KeyValueDesktopFileLine nameFileLine && nameFileLine.Key.Contains("Name"))
                    lines.Add(nameFileLine with { Value = $"({containerName})-{nameFileLine.Value}" });
                else
                    lines.Add(line);
            }

            desktopFile = new DesktopFile(lines.ToArray());

            (await computer.AddDesktopFileAsync(GetDesktopFileName(containerName, containerDesktopFilePath),
                    desktopFile))
                .ThrowIfFailed();
        }
        finally
        {
            (await incusFileSystem.DisposeFileSystem(containerName)).ThrowIfFailed();
        }
    });

    public static Task<Result> UnexportApplicationAsync(
        string containerName,
        string containerDesktopFilePath,
        IComputer computer
    ) => RunCatchingAsync(async () =>
    {
        (await computer.RemoveDesktopFileAsync(GetDesktopFileName(containerName, containerDesktopFilePath)))
            .ThrowIfFailed();
    });
}