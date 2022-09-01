using System;
using System.Collections.Generic;
using System.Text;

namespace smARTsoftware.SomfyRtsLib
{
  public enum SomfyRtsButton
  {
    My = 0x1,//	My  Stop or move to favourite position
    Up = 0x2,// Move up
    MyUp = 0x3,//Set upper motor limit in initial programming mode
    Down = 0x4,//	Move down
    MyDown = 0x5, //Set lower motor limit in initial programming mode
    UpDown = 0x6,// Change motor limit and initial programming mode
    Prog = 0x8, //  Used for (de-)registering remotes, see below
    EnableSensor = 0x9,//Enable sun and wind detector (SUN and FLAG symbol on the Telis Soliris RC)
    DisableSensor = 0xA,//Disable sun detector (FLAG symbol on the Telis Soliris RC)
  }
}
