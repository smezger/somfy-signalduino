using smARTsoftware.SomfyRtsLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace smARTsoftware.SomfyRts
{
  class Program
  {
    private const string cControllerName = "/terrace";
    static HttpListener sHttp;
    static string sUrl = "http://*:8888/";
    static SomfyRtsController sController;
    static void Main(string[] args)
    {
      // Create a Http server and start listening for incoming connections
      sHttp = new HttpListener();
      sHttp.Prefixes.Add(sUrl);
      sHttp.Start();
      Console.WriteLine("Listening for connections on {0}", sUrl);

      // Handle requests
      Task listenTask = HandleRequests();
      listenTask.GetAwaiter().GetResult();

      // Close the listener
      sHttp.Close();

    }
    private static Dictionary<string, SomfyRtsButton> sButtonMapping = new Dictionary<string, SomfyRtsButton>()
    {
      { "open", SomfyRtsButton.Up },
      {"an", SomfyRtsButton.Up},
      {"on", SomfyRtsButton.Up},
      { "close", SomfyRtsButton.Down},
      { "aus",SomfyRtsButton.Down},
      { "off",SomfyRtsButton.Down},
      { "stop",SomfyRtsButton.My},
      { "favPos",SomfyRtsButton.My},
      { "fav",SomfyRtsButton.My},
      { "enable",SomfyRtsButton.DisableSensor},//not a bug in this code: it is implemented like this on Brustor devices
      { "disable", SomfyRtsButton.EnableSensor},//not a bug in this code: it is implemented like this on Brustor devices
      {"mydown" ,SomfyRtsButton.MyDown},
      { "myup",SomfyRtsButton.MyUp},
      {"prog" ,SomfyRtsButton.Prog},
      {"updown" ,SomfyRtsButton.UpDown},
      {"dimm" ,SomfyRtsButton.Down},
    };
    public static async Task HandleRequests()
    {
      bool runServer = true;
      sController = SomfyRtsController.CreateFromFile();
      string result = "OK";
      while (runServer)
      {
        // Will wait here until we hear from a connection
        HttpListenerContext ctx = await sHttp.GetContextAsync();

        // Peel out the requests and response objects
        HttpListenerRequest req = ctx.Request;
        HttpListenerResponse resp = ctx.Response;

        // Print out some info about the request
        Console.WriteLine(req.Url.ToString());
        Console.WriteLine(req.HttpMethod);
        Console.WriteLine(req.UserHostName);
        Console.WriteLine(req.UserAgent);
        Console.WriteLine();

        //Handle URLs
        if (req.HttpMethod == "GET")
        {
          if (req.Url.AbsolutePath == "/shutdown")
          {
            Console.WriteLine("Shutdown requested");
            runServer = false;
          }
          if (req.Url.AbsolutePath.StartsWith(cControllerName))
          {
            var path = req.Url.AbsolutePath.Replace(cControllerName, "");
            var token = path.Split(new string[]{ "/"},StringSplitOptions.RemoveEmptyEntries);
            if (token.Length > 0 && token[0] == "list")
            {
              StringBuilder sb = new StringBuilder();
              foreach (var dev in sController.Devices)
              {
                sb.AppendLine($"Device: {dev.Name} RC: {dev.RollingCode} EK: {dev.EncryptionKey} Address: {dev.Address}");
              }
              result = sb.ToString();
            }
            else if(token.Length > 0 && token[0] == "help")
            {
              StringBuilder sb = new StringBuilder();              
              foreach (var cmd in sButtonMapping)
              {
                sb.AppendLine($"Cmd: '{cmd.Key}' => {cmd.Value}");
              }
              result = sb.ToString();
            }
            else if (token.Length > 0 && token[0] == "version")
            {
              result = $"{Assembly.GetExecutingAssembly().GetName().Name} {Assembly.GetExecutingAssembly().GetName().Version}";
            }
            else if (token.Length > 0)
            {
              string dev = token[0];
              //asume device
              if (sController.DeviceAvailable(dev))
              {
                if(token.Length > 1)
                {
                  var cmd = token[1];
                  SomfyRtsButton? button = null;
                  if (sButtonMapping.ContainsKey(cmd))
                  {
                    button = sButtonMapping[cmd];
                    if(cmd == "dimm")
                    {
                      token = new string[]{token[0],token[1],"20" };
                    }
                  }
                    
                  if(null != button)
                  {
                    if(token.Length >2 && Int32.TryParse(token[2], out int rep))
                    {                      
                      sController.SendCommand(dev, button.Value, rep);
                    }
                    else
                    {
                      sController.SendCommand(dev, button.Value);
                    }                    
                    sController.Save();
                    result = "OK";
                  }else
                  {
                    result = $"Command '{cmd}' not found! Call help to list available cmds.";
                  }
                }
                else
                {
                  result = $"No command specified!";
                }
              }
              else
              {
                result = $"Device '{token[0]}' not found.";
              }
            }
          }
        }

        // Write the response info

        byte[] data = Encoding.UTF8.GetBytes(result);
        resp.ContentType = "text/plain";
        resp.ContentEncoding = Encoding.UTF8;
        resp.ContentLength64 = data.LongLength;

        // Write out to the response stream (asynchronously), then close it
        await resp.OutputStream.WriteAsync(data, 0, data.Length);
        resp.Close();
      }
    }
  
  }
}
