using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace smARTsoftware.SomfyRtsLib
{
  public class SomfyRtsDevice
  {
    public UInt16 RollingCode { get; set; } = 0;
    public UInt32 Address { get; set; }
    public byte EncryptionKey { get; set; } = 0xA0;
    public string Name { get; set; }

    public SomfyRtsFrame CreateFrame(SomfyRtsButton button)
    {
      SomfyRtsFrame frame = new SomfyRtsFrame()
      {
        Address = Address,
        Command = button,
        EncryptionKey = EncryptionKey,
        RollingCode = RollingCode
      };
      //Update Keys
      RollingCode += 1;
      EncryptionKey += 1;
      if (EncryptionKey > 0xAF)
        EncryptionKey = 0xA0;
      Save();
      return frame;
    }

    public static SomfyRtsDevice CreateFromFile(string name)
    {
      string path = Path.Combine(FileUtils.SettingsDir, $"{name}.artset");
      if(File.Exists(path))
      {
        XmlSerializer serializer = new XmlSerializer(typeof(SomfyRtsDevice));
        using (StreamReader r = new StreamReader(path))
        {
          var s = (SomfyRtsDevice)serializer.Deserialize(r);
          return s;
        }
      }
      else
      {
        return new SomfyRtsDevice() { Name = name };
      }

    }
    public void Save()
    {
      XmlSerializer serializer = new XmlSerializer(typeof(SomfyRtsDevice));
      string path = Path.Combine(FileUtils.SettingsDir, $"{Name}.artset");
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
