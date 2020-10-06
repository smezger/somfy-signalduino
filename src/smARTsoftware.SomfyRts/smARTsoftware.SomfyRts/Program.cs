using smARTsoftware.SomfyRtsLib;
using System;

namespace smARTsoftware.SomfyRts
{
  class Program
  {
    private const string cLight = "Light";

    static void Main(string[] args)
    {
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
