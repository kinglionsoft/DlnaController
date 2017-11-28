using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SV.UPnPLite.Protocols.DLNA;
using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;
using SV.UPnPLite.Protocols.UPnP;

namespace DlnaController.Web
{
    public class UPnpTester
    {
        private static MediaServer _mini;

        private static MediaRenderer _renderer;
       public  static void Test2()
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
        }

        public static async Task Play()
        {
            try
            {
                var videos = await _mini.SearchAsync<VideoItem>();
                await _renderer.OpenAsync(videos.First());
                await _renderer.PlayAsync(); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}