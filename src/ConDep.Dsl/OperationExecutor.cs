﻿using ConDep.Dsl.Builders;
using ConDep.Dsl.Logging;
using Newtonsoft.Json;

namespace ConDep.Dsl
{
  public class OperationExecutor
  {
    private static JsonSerializerSettings JsonSettings => new JsonSerializerSettings
    {
      NullValueHandling = NullValueHandling.Ignore,
      Formatting = Formatting.Indented
    };

    public static void Execute(LocalBuilder local, LocalOperation myOp)
    {
      Logger.WithLogSection(myOp.Name, () =>
      {
        var result = myOp.Execute(local.Settings, local.Token);
        local.Result = result;
        LogResultData(result.Data);
      });
    }

    public static void Execute(RemoteBuilder remote, RemoteOperation myOp)
    {
      Logger.WithLogSection(myOp.Name, () =>
      {
        var result = myOp.Execute(new RemoteOperationsBuilder(remote.Server, remote.Settings, remote.Token),
          remote.Server, remote.Settings, remote.Token);
        remote.Result = result;
        LogResultData(result.Data);
      });
    }

    public static void Execute(RemoteBuilder remote, RemoteCodeOperation myOp)
    {
      Logger.WithLogSection(myOp.Name, () =>
      {
        var result = myOp.Execute(remote.Server, remote.Settings, remote.Token);
        remote.Result = result;
        LogResultData(result.Data);
      });
    }

    private static void LogResultData(dynamic data)
    {
      Logger.Verbose(JsonConvert.SerializeObject(data, JsonSettings));
    }
  }
}