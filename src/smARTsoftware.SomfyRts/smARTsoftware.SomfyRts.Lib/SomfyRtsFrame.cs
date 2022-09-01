﻿using System;
using System.Collections.Generic;
using System.Text;

namespace smARTsoftware.SomfyRtsLib
{
  public class SomfyRtsFrame
  {
    public byte EncryptionKey { get; set; }

    public SomfyRtsButton Command { get; set; }

    public byte Checksum { get; private set; }
    public UInt16 RollingCode { get; set; }

    public UInt32 Address { get; set; }

    private byte[] mFrameData = new byte[7];

    private void UpdateFrameData()
    {
      mFrameData[0] = EncryptionKey;
      mFrameData[1] = (byte)((byte)Command << 4); //| Checksum);
      var bytes = BitConverter.GetBytes(RollingCode);
      mFrameData[2] = bytes[1];
      mFrameData[3] = bytes[0];
      bytes = BitConverter.GetBytes(Address);
      //Swap address
      mFrameData[4] = bytes[0];
      mFrameData[5] = bytes[1];
      mFrameData[6] = bytes[2];
    }

    private void CalculcateChecksum()
    {
      Checksum = 0;
      mFrameData[1] = (byte)(mFrameData[1] & 0xF0);
      for (int i = 0; i < 7; i++)
      {

        Checksum = (byte)(Checksum ^ mFrameData[i] ^ (mFrameData[i] >> 4));
      }
      Checksum = (byte)(Checksum & 0xf);
    }
    public byte[] GetFrame()
    {
      UpdateFrameData();
      CalculcateChecksum();
      mFrameData[1] = (byte)(mFrameData[1] | Checksum);
      return GetObfuscatedFrame();
    }
    private byte[] GetObfuscatedFrame()
    {
      //byte[] obFrame = new byte[7];
      //obFrame[0] = mFrameData[0];
      for (int i = 1; i < 7; i++)
      {
        mFrameData[i] = (byte)(mFrameData[i] ^ mFrameData[i - 1]);
      }
      return mFrameData;
    }
  }
}
