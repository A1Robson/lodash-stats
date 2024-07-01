namespace LodashStats.Models;

public record RetrievedFileInfo(string FileName, string? Content)
{
    public bool HasValidContent => Content != null;
}