namespace TCTOS.Abstractions.Data;

public static class SocketMessageTypes
{
    public const string Launch = "launch";
    
    // Container lifetime
    public const string StartContainer = "startContainer";
    public const string StopContainer = "stopContainer";
    public const string ListContainers = "listContainers";
    public const string CreateContainer = "createContainer";
    public const string DeleteContainer = "deleteContainer";
    
    // Provisioning
    public const string ProvisionContainer = "provisionContainer";
    public const string SetProvisioningContent = "setProvision";
    public const string GetProvisioningContent = "getProvision";
    
    // Features
    public const string ListAllFeatures = "listFeatures";
    public const string ListContainerFeatures = "listContainerFeatures";
    public const string AddFeature = "addFeature";
    public const string RemoveFeature = "removeFeature";
    
    // Application export
    public const string ListExportedApplications = "listExportedApps";
    public const string ListExportableApplications = "listExportableApps";
    public const string ExportApplication = "exportApp";
    public const string UnexportApp = "unexportApp";
}