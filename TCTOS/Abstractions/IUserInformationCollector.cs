namespace TCTOS.Abstractions;

public interface IUserInformationCollector
{
    public uint GetUid();
    public uint GetGid();
}