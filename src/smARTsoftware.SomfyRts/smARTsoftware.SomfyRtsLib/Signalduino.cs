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
      mSerialPort.ReadTimeout = 500;
      mSerialPort.DataReceived += SerialPort_DataReceived;
      Console.WriteLine($"Encoding is: {mSerialPort.Encoding}");
      Console.WriteLine($"Device is open: {mSerialPort.IsOpen}");
    }
    public void RunTerminalMode()
    {
      do {
        var key = Console.Read();
        mSerialPort.Write(new byte[] { (byte)key }, 0, 1);
          } while (true);
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      Console.WriteLine($"Data received through event: {(sender as SerialPort).ReadExisting()}");
    }

    public void Close()
    {
      mSerialPort?.Close();
      mSerialPort = null;
    }
    public void SendCommand(string command)
    {
      Console.WriteLine($"Send command: '{command}'");
      mSerialPort.Write(command+"\r\n\0");

    }
    public string Read()
    {
      Console.WriteLine($"Bytes to read: {mSerialPort.BytesToRead}");
      //Console.WriteLine(mSerialPort.ReadLine());
      byte tmpByte;
      string rxString = "";
      tmpByte = (byte)mSerialPort.ReadByte();
      while(tmpByte != 255)
      {
        rxString += ((char)tmpByte);
        tmpByte = (byte)mSerialPort.ReadByte();
      }
      return rxString;
      /*if(mSerialPort.BytesToRead>0)
      {
        byte[] buffer = new byte[mSerialPort.BytesToRead];
        var len = mSerialPort.Read(buffer, 0, buffer.Length);
        Console.WriteLine($"Received text: {new ASCIIEncoding().GetString(buffer)}");
        return new ASCIIEncoding().GetString(buffer);
      }*/
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
