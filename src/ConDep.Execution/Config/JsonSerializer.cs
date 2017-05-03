using System.IO;
using Newtonsoft.Json;

namespace ConDep.Execution.Config
{
  public class JsonSerializer<T> : ISerializeConfig<T>
  {
    private readonly IHandleConfigCrypto _crypto;
    private JsonSerializerSettings _jsonSettings;

    public JsonSerializer(IHandleConfigCrypto crypto)
    {
      _crypto = crypto;
    }

    private JsonSerializerSettings JsonSettings => _jsonSettings ?? (_jsonSettings = new JsonSerializerSettings
    {
      NullValueHandling = NullValueHandling.Ignore,
      Formatting = Formatting.Indented
    });

    public string Serialize(T config)
    {
      var json = JsonConvert.SerializeObject(config, JsonSettings);
      var encryptedJson = _crypto.Encrypt(json);
      return encryptedJson;
    }

    public T DeSerialize(Stream stream)
    {
      T config;
      using (var memStream = GetMemoryStreamWithCorrectEncoding(stream))
      {
        using (var reader = new StreamReader(memStream))
        {
          var json = reader.ReadToEnd();
          config = DeSerialize(json);
        }
      }
      return config;
    }

    public T DeSerialize(string config)
    {
      if (_crypto.IsEncrypted(config))
        config = _crypto.Decrypt(config);
      return JsonConvert.DeserializeObject<T>(config, JsonSettings);
    }

    private static MemoryStream GetMemoryStreamWithCorrectEncoding(Stream stream)
    {
      using (var r = new StreamReader(stream, true))
      {
        var encoding = r.CurrentEncoding;
        return new MemoryStream(encoding.GetBytes(r.ReadToEnd()));
      }
    }
  }
}