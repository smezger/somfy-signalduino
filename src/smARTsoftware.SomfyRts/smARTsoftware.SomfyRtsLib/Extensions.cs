using System;
using System.Collections.Generic;
using System.Text;

namespace smARTsoftware.SomfyRtsLib
{
  public static class Extensions
  {
    public static string ToHexString(this byte[] data)
    {
      StringBuilder sb = new StringBuilder();
      foreach (var b in data)
      {
        sb.Append(b.ToString("X"));
      }
      return sb.ToString();
    }
    public static string ToHexDisplayString(this byte[] data)
    {
      StringBuilder sb = new StringBuilder();
      foreach (var b in data)
      {
        sb.Append(b.ToString("X2")+" ");
      }
      return sb.ToString();
    }
  }
}
