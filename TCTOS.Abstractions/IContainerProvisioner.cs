using TCTOS.Common;

namespace TCTOS.Abstractions;

public interface IContainerProvisioner
{
    public Task<Result> ProvisionContainer(
        string containerName,
        string provisionFileContent,
        Dictionary<string, string> variables
    );

    public string GetDefaultProvisionFileTemplate();
}