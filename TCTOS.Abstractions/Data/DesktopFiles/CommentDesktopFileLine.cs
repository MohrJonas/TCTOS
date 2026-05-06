namespace TCTOS.Abstractions.Data.DesktopFiles;

public sealed record CommentDesktopFileLine(string Comment) : IDesktopFileLine
{
    public string SerializeToString()
        => Comment;
}