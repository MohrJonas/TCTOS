using TCTOS.Abstractions.Data;
using TCTOS.Daemon.Handlers;
using TCTOS.Daemon.Handlers.Applications;
using TCTOS.Daemon.Handlers.Container;
using TCTOS.Daemon.Handlers.Features;
using TCTOS.Daemon.Handlers.Provision;

namespace TCTOS.Daemon;

public static class HandlerFactory
{
    public static IMessageHandler? GetHandlerByMessageType(string messageType, DiContainer container) => messageType switch
    {
        SocketMessageTypes.Launch => new LaunchHandler(container),
        SocketMessageTypes.StopContainer => new StopContainerHandler(container),
        SocketMessageTypes.StartContainer => new StartContainerHandler(container),
        SocketMessageTypes.ListContainers => new ListContainersHandler(container),
        SocketMessageTypes.CreateContainer => new CreateContainerHandler(container),
        SocketMessageTypes.DeleteContainer => new DeleteContainersHandler(container),
        SocketMessageTypes.ProvisionContainer => new ProvisionContainerHandler(container),
        SocketMessageTypes.GetProvisioningContent => new GetProvisionContentHandler(container),
        SocketMessageTypes.SetProvisioningContent => new SetProvisionContentHandler(container),
        SocketMessageTypes.ListAllFeatures => new ListAllFeaturesHandler(container),
        SocketMessageTypes.ListContainerFeatures => new ListContainerFeaturesHandler(container),
        SocketMessageTypes.AddFeature => new AddFeatureHandler(container),
        SocketMessageTypes.RemoveFeature => new RemoveFeatureHandler(container),
        SocketMessageTypes.ListExportedApplications => new ListExportedApplicationsHandler(container),
        SocketMessageTypes.ExportApplication => new ExportApplicationHandler(container),
        SocketMessageTypes.ListExportableApplications => new ListExportableApplicationsHandler(container),
        SocketMessageTypes.UnexportApp => new UnExportApplicationHandler(container),
        _ => null
    };
}