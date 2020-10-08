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
      if (args.Length > 0)
      {
        Signalduino sig = new Signalduino();
        string dev = "/dev/serial/by-id/usb-Unknown_radino_CC1101-if00";
        if (args[0].StartsWith("/"))
          dev = args[0];
        sig.Open(dev);
        do
        {
          //Console.WriteLine("new");
          string cmd = Console.ReadLine();
          sig.SendCommand(cmd);//WriteWithBytes(cmd);//
          Thread.Sleep(100);
          Console.WriteLine(sig.Read());
        } while (true);
        return;
      }

      SomfyRtsController controller = SomfyRtsController.CreateFromFile();
      controller.SignalDuinoAddress = "/dev/serial/by-id/usb-Unknown_radino_CC1101-if00";

      if (controller.Devices.Count == 0)
      {
        controller.AddDevice(cLight, 180004);
        controller.SendCommand(cLight, SomfyRtsButton.Prog);
        Console.WriteLine("Sending PROG to device light");
        Console.ReadLine();
        controller.Save();
      }
      controller.SendCommand(cLight, SomfyRtsButton.Up);
      Console.WriteLine("Sending Up to device light");
      Console.ReadLine();
      controller.SendCommand(cLight, SomfyRtsButton.Down);
      Console.WriteLine("Sending Up to device light");
      Console.ReadLine();

    }
  }
}
