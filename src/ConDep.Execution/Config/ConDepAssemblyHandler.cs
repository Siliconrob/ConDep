﻿using System;
using System.IO;
using System.Reflection;

namespace ConDep.Execution.Config
{
  public class ConDepAssemblyHandler
  {
    private readonly string _assemblyName;

    public ConDepAssemblyHandler(string assemblyName)
    {
      _assemblyName = assemblyName;
    }

    public Assembly GetAssembly()
    {
      Assembly assembly;
      if (!TryGetAssembly(_assemblyName, out assembly))
        throw new ConDepAssemblyNotFoundException(
          string.Format(
            "Assembly [{0}] could not be resolved as a file path, in current directory or in executing directory.",
            _assemblyName));
      return assembly;
    }

    private static bool TryGetAssembly(string assemblyName, out Assembly assembly)
    {
      string path;

      if (TryGetAbsolutePath(assemblyName, out path))
      {
        assembly = Assembly.LoadFile(path);
        return true;
      }

      var currentPath = Directory.GetCurrentDirectory();
      var combinedPath = Path.Combine(currentPath, assemblyName);
      if (File.Exists(combinedPath))
      {
        assembly = Assembly.LoadFile(path);
        return true;
      }

      var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if (string.IsNullOrWhiteSpace(executingPath))
      {
        assembly = null;
        return false;
      }

      combinedPath = Path.Combine(executingPath, assemblyName);
      if (File.Exists(combinedPath))
      {
        assembly = Assembly.LoadFile(combinedPath);
        return true;
      }

      assembly = null;
      return false;
    }

    private static bool TryGetAbsolutePath(string assemblyName, out string absolutePath)
    {
      if (Path.IsPathRooted(assemblyName) && File.Exists(assemblyName))
      {
        absolutePath = assemblyName;
        return true;
      }

      var absPath = Path.GetFullPath(assemblyName);
      if (File.Exists(absPath))
      {
        absolutePath = absPath;
        return true;
      }

      var curPath = Path.Combine(Environment.CurrentDirectory, assemblyName);
      if (File.Exists(curPath))
      {
        absolutePath = curPath;
        return true;
      }

      absolutePath = string.Empty;
      return false;
    }
  }
}