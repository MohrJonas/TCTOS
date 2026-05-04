using TCTOS.Abstractions;

namespace TCTOS.Features.Abstractions;

public sealed class FeatureContext
{
    public required IIncusClient IncusClient { init; get; }
    public required IUserInformationCollector UserInformationCollector { init; get; }
    public required INonPersistentStorage NonPersistentStorage { init; get; }
    public required IFileSystem FileSystem { init; get; }
    public required IEnvironmentVariableProvider EnvironmentVariableProvider { init; get; }
    public required IBackgroundCommandRunner BackgroundCommandRunner { init; get; }
    public required ICommandRunner CommandRunner { init; get; }
    public required IDictionary<string, string> ContainerEnvironmentVariables { init; get; }
}