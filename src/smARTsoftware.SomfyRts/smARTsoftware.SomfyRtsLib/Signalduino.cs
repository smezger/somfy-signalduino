using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace smARTsoftware.SomfyRtsLib
{
  public class Signalduino
  {
    const int cDatabits = 8;
    SerialPort mSerialPort = null;
    public void Open(string device)
    {
      mSerialPort = new SerialPort(device, 57600, Parity.None, cDatabits, StopBits.One);
      mSerialPort.Open();
    }
    public void Close()
    {
      mSerialPort?.Close();
      mSerialPort = null;
    }
    public void SendCommand(string command)
    {
      mSerialPort.WriteLine(command);
    }
    public string Read()
    {
      if(mSerialPort.BytesToRead>0)
      {
        byte[] buffer = new byte[mSerialPort.BytesToRead];
        var len = mSerialPort.Read(buffer, 0, buffer.Length);
        return new ASCIIEncoding().GetString(buffer);
      }
      return "";
    }
    public void SendSomfyFrame(SomfyRtsFrame frame, int repetition = 6)
    {
      var cmd = $"SC;R={repetition};SR;P0=-2560;P1=2560;P3=-640;D=10101010101010113;SM;C=645;D={frame.GetFrame().ToHexString()};F=10AB85550A;";
      Console.WriteLine($"SEND: {cmd}");
      SendCommand(cmd);
    }
    
  }
}
