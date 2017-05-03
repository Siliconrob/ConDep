using System;

namespace ConDep.Dsl.Remote.Node.Model
{
  public class SyncPath
  {
    public SyncPath()
    {
    }

    public SyncPath(string path, string rootPath, string attributes, bool isDirectory = false)
    {
      if (!rootPath.EndsWith("\\"))
        RootPath = rootPath + "\\";
      else
        RootPath = rootPath;

      Path = path;
      Attributes = attributes;
      IsDirectory = isDirectory;
    }

    public string RelativePath
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(Path)) return Path.Replace(RootPath, "");
        throw new ArgumentException("Neither SourcePath or DestPath exist.");
      }
    }

    public string Path { get; }

    public string RootPath { get; }

    public string Attributes { get; }

    public bool IsDirectory { get; set; }
  }
}