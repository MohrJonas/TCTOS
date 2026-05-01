using System.CommandLine;

namespace TCTOS.Commands;

public static class SharedArguments
{
    public static readonly Argument<string> ContainerNameArgument = new("container_name")
    {
        Description = "The container to operate on"
    };
}