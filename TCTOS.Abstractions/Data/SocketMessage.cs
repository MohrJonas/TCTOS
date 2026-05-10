using System.Text.Json.Serialization;
using TCTOS.Abstractions.Data.Messages;

namespace TCTOS.Abstractions.Data;

[JsonDerivedType(typeof(StartContainerSocketMessage), typeDiscriminator: SocketMessageTypes.StartContainer)]
[JsonDerivedType(typeof(StopContainerSocketMessage), typeDiscriminator: SocketMessageTypes.StopContainer)]
[JsonDerivedType(typeof(ListContainersSocketMessage), typeDiscriminator: SocketMessageTypes.ListContainers)]
[JsonDerivedType(typeof(CreateContainerSocketMessage), typeDiscriminator: SocketMessageTypes.CreateContainer)]
[JsonDerivedType(typeof(DeleteContainerSocketMessage), typeDiscriminator: SocketMessageTypes.DeleteContainer)]
[JsonDerivedType(typeof(ProvisionSocketMessage), typeDiscriminator: SocketMessageTypes.ProvisionContainer)]
[JsonDerivedType(typeof(SetProvisionContentSocketMessage), typeDiscriminator: SocketMessageTypes.SetProvisioningContent)]
[JsonDerivedType(typeof(GetProvisionContentSocketMessage), typeDiscriminator: SocketMessageTypes.GetProvisioningContent)]
[JsonDerivedType(typeof(ListAllFeaturesSocketMessage), typeDiscriminator: SocketMessageTypes.ListAllFeatures)]
[JsonDerivedType(typeof(ListContainerFeaturesSocketMessage), typeDiscriminator: SocketMessageTypes.ListContainerFeatures)]
[JsonDerivedType(typeof(AddFeatureSocketMessage), typeDiscriminator: SocketMessageTypes.AddFeature)]
[JsonDerivedType(typeof(RemoveFeatureSocketMessage), typeDiscriminator: SocketMessageTypes.RemoveFeature)]
[JsonDerivedType(typeof(ListExportedApplicationsSocketMessage), typeDiscriminator: SocketMessageTypes.ListExportedApplications)]
[JsonDerivedType(typeof(ListExportableApplicationsMessage), typeDiscriminator: SocketMessageTypes.ListExportableApplications)]
[JsonDerivedType(typeof(ExportApplicationSocketMessage), typeDiscriminator: SocketMessageTypes.ExportApplication)]
[JsonDerivedType(typeof(UnExportApplicationSocketMessage), typeDiscriminator: SocketMessageTypes.UnexportApp)]
public record SocketMessage([property:JsonPropertyName("type")] string MessageType);