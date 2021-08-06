using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib1;
using Tech;

namespace DotNetRemoteClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*// Set up a client channel.
            TcpClientChannel clientChannel = new TcpClientChannel();
            ChannelServices.RegisterChannel(clientChannel, false);

            // Show the name and priority of the channel.
            Console.WriteLine("Channel Name: {0}", clientChannel.ChannelName);
            Console.WriteLine("Channel Priority: {0}", clientChannel.ChannelPriority);

            // Obtain a proxy for a remote object.
            RemotingConfiguration.RegisterWellKnownClientType(
                typeof(Remotable), "tcp://localhost:9090/Remotable.rem"
            );*/

            // Call a method on the object.
            ServiceClass remoteObject = (ServiceClass)Remotable.GetObject(typeof(ServiceClass), "services", "localhost:9090");
            Console.WriteLine(remoteObject.callCount);
            Console.WriteLine(remoteObject.GetCount());
            Console.ReadLine();
        }
    }
}
