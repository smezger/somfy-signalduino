using smARTsoftware.SomfyRtsLib;
using System;
using System.Threading;

namespace smARTsoftware.SomfyRts
{
  class Program
  {
    private const string cLight = "Light";

    static void Main(string[] args)
    {
      string dev = "/dev/serial/by-id/usb-Unknown_radino_CC1101-if00";
      if (args.Length >0 && args[0].StartsWith("/"))
        dev = args[0];
      SomfyRtsController controller = SomfyRtsController.CreateFromFile();
      controller.SignalDuinoAddress = dev;
      do
      {
        var key = Console.ReadKey();
        switch(key.Key)
        {
          case ConsoleKey.A:
            controller.AddDevice(cLight, 180004);break;
          case ConsoleKey.P: 
            controller.SendCommand(cLight, SomfyRtsButton.Prog);break;
          case ConsoleKey.U:
            controller.SendCommand(cLight, SomfyRtsButton.Up);break;
          case ConsoleKey.D:
            controller.SendCommand(cLight, SomfyRtsButton.Down); break;
          case ConsoleKey.F:
            controller.SendCommand(cLight, SomfyRtsButton.My); break;
          case ConsoleKey.C:
            controller.Close(); break;
          case ConsoleKey.O:
            controller.Open(); break;
        }
        controller.Save();    
      } while (true);

    }
  }
}
