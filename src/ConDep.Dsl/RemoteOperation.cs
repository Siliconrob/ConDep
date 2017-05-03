﻿using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl
{
  public abstract class RemoteOperation
  {
    public abstract string Name { get; }

    public abstract Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings,
      CancellationToken token);
  }
}