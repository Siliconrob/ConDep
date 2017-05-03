﻿using System;
using System.Runtime.Serialization;

namespace ConDep.Execution.Config
{
  public class ConDepConfigurationTypeNotFoundException : Exception
  {
    public ConDepConfigurationTypeNotFoundException()
    {
    }

    public ConDepConfigurationTypeNotFoundException(string message) : base(message)
    {
    }

    public ConDepConfigurationTypeNotFoundException(string message, Exception innerException) : base(message,
      innerException)
    {
    }

    public ConDepConfigurationTypeNotFoundException(SerializationInfo info, StreamingContext context) : base(info,
      context)
    {
    }
  }
}