using smARTsoftware.SomfyRtsLib;
using System;
using System.Net;
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
                  switch (cmd)
                  {
                    case "open":
                    case "an":
                      button = SomfyRtsButton.Up;
                      break;
                    case "close":
                    case "aus":
                      button = SomfyRtsButton.Down;
                      break;
                    case "stop":                    
                      button = SomfyRtsButton.My;
                      break;
                    case "favPos":
                      button = SomfyRtsButton.My;
                      break;
                    case "enable":
                      button = SomfyRtsButton.EnableSensor;
                      break;
                    case "disable":
                      button = SomfyRtsButton.DisableSensor;
                      break;
                    case "mydown":
                      button = SomfyRtsButton.MyDown;
                      break;
                    case "myup":
                      button = SomfyRtsButton.MyUp;
                      break;
                    case "prog":
                      button = SomfyRtsButton.Prog;
                      break;
                    case "updown":
                      button = SomfyRtsButton.UpDown;
                      break;
                  }
                  if(null != button)
                  {
                    sController.SendCommand(dev, button.Value);
                    sController.Save();
                  }else
                  {
                    result = $"Command '{cmd}' not found!";
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
        resp.ContentType = "text/json";
        resp.ContentEncoding = Encoding.UTF8;
        resp.ContentLength64 = data.LongLength;

        // Write out to the response stream (asynchronously), then close it
        await resp.OutputStream.WriteAsync(data, 0, data.Length);
        resp.Close();
      }
    }
  
  }
}
