﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using SV.UPnPLite;
using SV.UPnPLite.Extensions;
using SV.UPnPLite.Protocols.DLNA;
using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
using SV.UPnPLite.Protocols.UPnP;

namespace TestDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            ReactiveTester.Run();
            Console.Read();
        }

        private static MediaServer _mini;

        private static MediaRenderer _renderer;

        static void Test2()
        {
            //var client = new HttpUClient();
            //client.OnResponse += Client_OnResponse;
            //client.BroadcastDisvoeryRequst();
            //var rendererDiscovery = new AVRendererDiscovery((new AVRendererDiscovery.DiscoveryHandler(RendererAddedSink)));

            //_containerDiscovery = ContainerDiscovery.GetInstance();

            //_containerDiscovery.AllRoots.OnContainerChanged += new CpRootContainer.Delegate_OnContainerChanged(ContainerChangedSink);


            var devicesDiscovery = new CommonUPnPDevicesDiscovery();

            // Receiving notifications about new devices added to a network
            devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                Console.WriteLine("{0} found", e.Device.FriendlyName);
            });

            // Receiving notifications about devices left the network
            devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            {
                Console.WriteLine("{0} gone", e.Device.FriendlyName);
            });

            //  Receiving notifications about new devices of specific type added to the network
            var newMediaServers = from activityInfo in devicesDiscovery.DevicesActivity
                where activityInfo.Activity == DeviceActivity.Available && activityInfo.Device.DeviceType == "urn:schemas-upnp-org:device:MediaServer"
                select activityInfo.Device;

            newMediaServers.Subscribe(s =>
            {
                Console.WriteLine("{0} found", s.FriendlyName);
            });


            var mediaServersDiscovery = new MediaServersDiscovery();
            var mediaRenderersDiscovery = new MediaRenderersDiscovery();

            // Enumerating currently available servers
            foreach (var server in mediaServersDiscovery.DiscoveredDevices)
            {
                Console.WriteLine("Server found: {0}", server.FriendlyName);
            }

            // Receiving notifications about new media servers added to a network
            mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(async e =>
            {
                Console.WriteLine("Server found: {0}", e.Device.FriendlyName);

                if (e.Device.FriendlyName == "raspberrypi: minidlna")
                {
                    //var rootObjects = await e.Device.BrowseAsync();
                    //var rootContainers = rootObjects.OfType<MediaContainer>();
                    //var rootMediaItems = rootObjects.OfType<MediaItem>();

                    //// Requesting media objects from child container
                    //var containerToBrowse = rootContainers.First(x=>x.Title=="Video");
                    //var childContainerObjects = await e.Device.BrowseAsync(containerToBrowse);

                   // var videos = await e.Device.SearchAsync<VideoItem>();

                    _mini = e.Device;
                }
            });

            // Receiving notifications about media renderers left the network
            mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                Console.WriteLine("Renderer found: {0}", e.Device.FriendlyName);
                if (e.Device.FriendlyName == "yangchao (CD-PC029 : Windows Media Player)")
                {
                    _renderer = e.Device;
                }
            });
            
            Console.Read();
            Play();
        }

        static void Play()
        {
            try
            {
                var videos = _mini.SearchAsync<VideoItem>().GetAwaiter().GetResult().First();
                _renderer.OpenAsync(videos).GetAwaiter().GetResult();
                _renderer.PlayAsync().GetAwaiter().GetResult(); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        static void Test()
        {
            var url = "http://10.0.201.61:8200/ctl/ContentDir";
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
  <s:Body>
    <u:Search xmlns:u=""urn:schemas-upnp-org:service:ContentDirectory:1"">
      <ContainerID>0</ContainerID>
      <SearchCriteria>upnp:class derivedfrom ""object.item.videoItem""</SearchCriteria>
      <Filter>*</Filter>
      <StartingIndex>0</StartingIndex>
      <RequestedCount>0</RequestedCount>
      <SortCriteria></SortCriteria>
    </u:Search>
  </s:Body>
</s:Envelope>
";

            var headers = new Dictionary<string, string>()
            {
                {"SOAPACTION", "urn:schemas-upnp-org:service:ContentDirectory:1#Search"}
            };
           var result =  HttpClientHelper.PostXmlAsync(url, xml, headers).Result;

        }

    }
}
