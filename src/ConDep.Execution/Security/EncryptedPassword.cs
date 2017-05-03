﻿namespace ConDep.Execution.Security
{
  public class EncryptedValue
  {
    public EncryptedValue(string iv, string value)
    {
      IV = iv;
      Value = value;
    }

    public string IV { get; }

    public string Value { get; }
  }
}