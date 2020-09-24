using smARTsoftware.SomfyRtsLib;
using System;

namespace smARTsoftware.SomfyRts
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      var cmd = new SomfyRtsFrame();
      cmd.Address = 131815;
      cmd.EncryptionKey = 0xA2;
      cmd.RollingCode = 0102;
      cmd.Command = SomfyRtsButton.Up;
      var data = cmd.GetFrame();

    }
  }
}
//A2 84 85 87 92 8A 99