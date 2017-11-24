using System;
using OpenSource.UPnP.AV.RENDERER.CP;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = new HttpUClient();
            //client.OnResponse += Client_OnResponse;
            //client.BroadcastDisvoeryRequst();

            var rendererDiscovery = new AVRendererDiscovery((new AVRendererDiscovery.DiscoveryHandler(RendererAddedSink)));
            Console.Read();
        }

        private static void RendererAddedSink(AVRendererDiscovery sender, AVRenderer renderer)
        {
           
        }
        
    }
}
