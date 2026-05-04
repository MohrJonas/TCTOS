using System.CommandLine;
using Microsoft.Extensions.Logging;
using TCTOS.Abstractions;
using TCTOS.Abstractions.Data;
using TCTOS.Operations;
using KnownColor = System.Drawing.KnownColor;

namespace TCTOS.Console.Commands.Container;

public static class CreateContainerCommandOptions
{
    public static readonly Option<string> ContainerDescriptionOption = new("--description", "-d")
    {
        Description = "Description of the container",
        DefaultValueFactory = _ => string.Empty
    };

    public static readonly Option<string[]> ContainerFeaturesOption = new("--feature", "-f")
    {
        Description = "Enable the given feature for the container",
        DefaultValueFactory = _ => []
    };

    public static readonly Option<Color> ContainerColorOption = new("--color", "-c")
    {
        Description = "Specify the color of the container. Icons of exported applications will be this color",
        DefaultValueFactory = _ => new Color
        {
            Red = 0,
            Green = 255,
            Blue = 0
        },
        CustomParser = res =>
        {
            if (res.Tokens.Count != 1)
            {
                res.AddError("--color requires a string in the format: \"<red>, <green>, <blue>\" or a color constant");
                return null;
            }

            if (Enum.TryParse<KnownColor>(res.Tokens[0].Value, true, out var knownColor))
            {
                var drawingColor = System.Drawing.Color.FromKnownColor(knownColor);
                return new Color
                {
                    Red = drawingColor.R,
                    Green = drawingColor.G,
                    Blue = drawingColor.B
                };
            }

            var parts = res.Tokens[0].Value.Split(",", StringSplitOptions.TrimEntries);
            if (parts.Length != 3)
            {
                res.AddError($"Got {parts.Length} color components, expected 3");
                return null;
            }

            if (!byte.TryParse(parts[0], out var redValue))
                res.AddError("Red has to be a number between 0 and 255");
            if (!byte.TryParse(parts[1], out var greenValue))
                res.AddError("Green has to be a number between 0 and 255");
            if (!byte.TryParse(parts[2], out var blueValue))
                res.AddError("Blue has to be a number between 0 and 255");
            return new Color
            {
                Red = redValue,
                Green = greenValue,
                Blue = blueValue
            };
        }
    };
}

public static class CreateContainerCommandArguments
{
    public static readonly Argument<string> ImageNameArgument = new("image_name")
    {
        Description = "Name of the image to use"
    };
}

public sealed class CreateContainerCommand(DiContainer container)
    : CommandBase("create", "Create a new container", container, ["new"],
        arguments: [CreateContainerCommandArguments.ImageNameArgument, SharedArguments.ContainerNameArgument],
        options:
        [
            CreateContainerCommandOptions.ContainerDescriptionOption,
            CreateContainerCommandOptions.ContainerFeaturesOption,
            CreateContainerCommandOptions.ContainerColorOption
        ])
{
    protected override async Task RunAsync(ParseResult parseResult, DiContainer container, CancellationToken token)
    {
        var containerName = parseResult.GetRequiredValue(SharedArguments.ContainerNameArgument);
        var image = parseResult.GetRequiredValue(CreateContainerCommandArguments.ImageNameArgument);
        var color = parseResult.GetRequiredValue(CreateContainerCommandOptions.ContainerColorOption);
        var enabledFeatures = parseResult.GetRequiredValue(CreateContainerCommandOptions.ContainerFeaturesOption);
        var description = parseResult.GetRequiredValue(CreateContainerCommandOptions.ContainerDescriptionOption);

        var logger = container.Get<ILogger>();
        var incusClient = container.Get<IIncusClient>();
        var featureProvider = container.Get<IFeatureProvider>();
        var fileSystem = container.Get<IFileSystem>();
        var provisioner = container.Get<IContainerProvisioner>();

        (await CreateContainerOperation.CreateContainerAsync(
            containerName,
            color,
            enabledFeatures,
            description,
            image,
            logger,
            incusClient,
            provisioner,
            featureProvider,
            fileSystem
        )).ThrowIfFailed();
    }
}