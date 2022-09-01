using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace smARTsoftware.SomfyRtsLib
{
  public static class FileUtils
  {
    public static String SettingsDir
    {
      get
      {
        return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      }
    }
  }
}
