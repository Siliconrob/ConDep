using System.IO;

namespace ConDep.Execution.Config
{
  public class ArtifactManifestConfigParser
  {
    private readonly ISerializeConfig<ArtifactManifest> _configSerializer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configSerializer"></param>
    public ArtifactManifestConfigParser(ISerializeConfig<ArtifactManifest> configSerializer)
    {
      _configSerializer = configSerializer;
    }

    public ArtifactManifest GetTypedConfig(string filePath)
    {
      using (var fileStream = File.OpenRead(filePath))
      {
        return GetTypedConfig(fileStream);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public ArtifactManifest GetTypedConfig(Stream stream)
    {
      var manifest = _configSerializer.DeSerialize(stream);

      return manifest;
    }
  }
}