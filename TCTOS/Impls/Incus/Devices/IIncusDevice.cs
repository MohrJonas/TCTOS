using System.Text.Json.Serialization;

namespace TCTOS.Impls.Incus.Devices;

[JsonDerivedType(typeof(IncusDiskDevice))]
[JsonDerivedType(typeof(IncusNicDevice))]
public interface IIncusDevice
{
    public string Type { get; }
}