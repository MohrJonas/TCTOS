using System.Text.Json.Serialization;

namespace TCTOS.Abstractions.Incus.Devices;

[JsonDerivedType(typeof(IncusDiskDevice))]
[JsonDerivedType(typeof(IncusNicDevice))]
public interface IIncusDevice
{
    public string Type { get; }
}