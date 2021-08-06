using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tech
{
    public interface IRemoteResource
    {
        void Publish(string instanceName);
        void SetupChannel(string channelName);
    }

    public class RemoteResource : MarshalByRefObject, IDisposable, IRemoteResource
    {
        public RemoteResource()
        {

        }
        ~RemoteResource()
        {

        }

        public void Publish(string instanceName)
        {   // Set up a server channel.
            string[] str = instanceName.Split(':');
            TcpServerChannel serverChannel = new TcpServerChannel(str[0], int.Parse(str[1]));
            ChannelServices.RegisterChannel(serverChannel, false);

            // Show the name and priority of the channel.
            //Console.WriteLine("Channel Name: {0}", serverChannel.ChannelName);
            //Console.WriteLine("Channel Priority: {0}", serverChannel.ChannelPriority);

            // Expose an object for remote calls.
            RemotingConfiguration.RegisterWellKnownServiceType(this.GetType(), 
                string.Format("{0}", this._channelName), 
                WellKnownObjectMode.Singleton);
        }

        public void SetupChannel(string channelName)
        {
            this._channelName = channelName;
        }
        public static object GetObject(Type type, string channelName, string instanceName)
        {
            object service = null;
            if (type.BaseType == typeof(RemoteResource))
            {
                // Set up a client channel.
                TcpClientChannel clientChannel = new TcpClientChannel();
                ChannelServices.RegisterChannel(clientChannel, false);

                // Show the name and priority of the channel.
                //Console.WriteLine("Channel Name: {0}", clientChannel.ChannelName);
                //Console.WriteLine("Channel Priority: {0}", clientChannel.ChannelPriority);

                // Obtain a proxy for a remote object.
                RemotingConfiguration.RegisterWellKnownClientType(type, string.Format("tcp://{0}/{1}", instanceName, channelName));
                service = (object)Activator.CreateInstance(type);
            }

            return service;
        }

        public void Dispose()
        {
            
        }

        private string _channelName;
    }
}
