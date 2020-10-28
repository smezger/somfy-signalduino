using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace smARTsoftware.SomfyRtsLib
{
  public class SomfyRtsController
  {
    public List<SomfyRtsDevice> Devices { get; set; } = new List<SomfyRtsDevice>();
    public string SignalDuinoAddress { get; set; } = "";
    Signalduino mSignalduino = new Signalduino();
    public void AddDevice(string name, uint address)
    {
      Devices.Add(new SomfyRtsDevice() { Name = name, Address = address });
    }
    public static SomfyRtsController CreateFromFile()
    {
      string path = Path.Combine(FileUtils.SettingsDir, $"{nameof(SomfyRtsController)}.artset");
      if (File.Exists(path))
      {
        XmlSerializer serializer = new XmlSerializer(typeof(SomfyRtsController));
        using (StreamReader r = new StreamReader(path))
        {
          var s = (SomfyRtsController)serializer.Deserialize(r);
          return s;
        }
      }
      else
      {
        return new SomfyRtsController();
      }
    }
    public void SendCommand(string device, SomfyRtsButton command, int repetition = 6)
    {
      if (!mSignalduino.IsOpen)
        mSignalduino.Open(SignalDuinoAddress);
      foreach (var dev in Devices)
      {
        if(dev.Name.Equals(device,StringComparison.OrdinalIgnoreCase))
        {
          var frame = dev.CreateFrame(command);
          Console.WriteLine($"Send command: {command} to device {dev.Name}");
          //mSignalduino.Open(SignalDuinoAddress);
          mSignalduino.SendSomfyFrame(frame, repetition);
          return;
        }
      }
    }
    public bool DeviceAvailable(string device)
    {
      foreach (var dev in Devices)
      {
        if (dev.Name.Equals(device, StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }
      return false;
    }
    public void Open()
    {
      Console.WriteLine("Open connection");
      mSignalduino.Open(SignalDuinoAddress);

    }
    public void Close()
    {

      Console.WriteLine("Close connection");
      mSignalduino.Close();
    }

    public void Save()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(SomfyRtsController));
      string path = Path.Combine(FileUtils.SettingsDir, $"{nameof(SomfyRtsController)}.artset");
      string dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }
      using (StreamWriter w = new StreamWriter(path))
      {
        serializer.Serialize(w, this);
      }
    }
  }
}
