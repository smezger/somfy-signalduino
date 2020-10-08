using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace smARTsoftware.SomfyRtsLib
{
  public class Signalduino
  {
    const int cDatabits = 8;
    SerialPort mSerialPort = null;
    public void Open(string device)
    {

      mSerialPort = new SerialPort(device, 57600, Parity.None, cDatabits, StopBits.One);
      mSerialPort.Handshake = Handshake.XOnXOff;
      mSerialPort.DtrEnable = true;
      mSerialPort.ErrorReceived += MSerialPort_ErrorReceived;
      mSerialPort.PinChanged += MSerialPort_PinChanged;
      mSerialPort.Open();
      mSerialPort.Handshake = Handshake.XOnXOff;
      mSerialPort.DtrEnable = true;
      mSerialPort.ReadTimeout = 5000;
      mSerialPort.WriteTimeout = 500;
      Thread readThread = new Thread(ReadBack);
      //readThread.Start();
      //mSerialPort.DataReceived += SerialPort_DataReceived;
      Console.WriteLine($"Encoding is: {mSerialPort.Encoding}");
      Console.WriteLine($"Device is open: {mSerialPort.IsOpen}");
      Console.WriteLine($"DTR:{mSerialPort.DtrEnable} Handshake: {mSerialPort.Handshake}");
    }
    
    public void ReadBack()
    {
      while (true)
      {
        try
        {
          string message = mSerialPort.ReadLine();
          Console.WriteLine(message);
        }
        catch (TimeoutException) { }
      }
    }
    private void MSerialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
    {
      
    }

    private void MSerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {

    }

    public void RunTerminalMode()
    {
      do
      {
        var key = (byte)Console.Read();
        Console.WriteLine($"Key is {key.ToString("X")} {(char)key}");
        mSerialPort.Write(new byte[] { key, 10 }, 0, 1);
        Read();
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
      mSerialPort.Write(command+"\n");

    }
    public void WriteWithBytes(string text)
    {
      var buffer = ToBytes(text);
      Console.WriteLine($"Data is: {buffer.ToHexDisplayString()}");
      mSerialPort.Write(buffer, 0, buffer.Length);
    }
    public byte[] ToBytes(string text)
    {
      var buffer = new ASCIIEncoding().GetBytes(text);
      List<byte> list = new List<byte>();
      list.Add(0x02);
      list.AddRange(buffer);
      list.Add(0x0A);
      list.Add(0x03);
      return list.ToArray();
    }
    public string Read()
    {
      try
      {
        string message = mSerialPort.ReadLine();
        Console.WriteLine(message);
        return message;
      }
      catch (TimeoutException) { }
      
      //try
      //{
      //  Console.WriteLine($"Bytes to read: {mSerialPort.BytesToRead}");
      //  //Console.WriteLine(mSerialPort.ReadLine());
      //  byte tmpByte;
      //  string rxString = "";
      //  tmpByte = (byte)mSerialPort.ReadByte();
      //  while (tmpByte != 255)
      //  {
      //    rxString += ((char)tmpByte);
      //    tmpByte = (byte)mSerialPort.ReadByte();
      //  }
      //  return rxString;
      //}
      //catch (Exception ex)
      //{
      //  Console.WriteLine($"Nothing to read:{ex}");
      //}
      Console.WriteLine($"Bytes to read: {mSerialPort.BytesToRead}");
      if (mSerialPort.BytesToRead > 0)
      {
        byte[] buffer = new byte[mSerialPort.BytesToRead];
        var len = mSerialPort.Read(buffer, 0, buffer.Length);
        //Console.WriteLine($"Received text: {new ASCIIEncoding().GetString(buffer)}");
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
