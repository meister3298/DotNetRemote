using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using Lib1;
using Tech;

namespace DotNetRemote
{
    class Program
    {
        static void Main(string[] args)
        {
            /*// Set up a server channel.
            TcpServerChannel serverChannel = new TcpServerChannel(9090);
            ChannelServices.RegisterChannel(serverChannel, false);

            // Expose an object for remote calls.
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Remotable), "Remotable.rem", WellKnownObjectMode.Singleton
            );

            // Show the name and priority of the channel.
            Console.WriteLine("Channel Name: {0}", serverChannel.ChannelName);
            Console.WriteLine("Channel Priority: {0}", serverChannel.ChannelPriority);

            // Show the URIs associated with the channel.
            ChannelDataStore data = (ChannelDataStore)serverChannel.ChannelData;
            foreach (string uri in data.ChannelUris)
            {
                Console.WriteLine(uri);
            }*/

            //Remotable remote = new Remotable();
            ServiceClass remote = new ServiceClass();
            remote.SetupChannel("services");
            remote.Publish("localhost:9090");

            // Wait for method calls.
            Console.WriteLine("Listening...");
            Console.ReadLine();
        }
    }
}
