using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Services;
using System.IO;
using System.Reflection;

namespace Tech
{
    public class TrackingHandler : ITrackingHandler
    {
        // Notifies a handler that an object has been marshaled.
        public void MarshaledObject(Object obj, ObjRef objref)
        {
            Console.WriteLine("Tracking: An instance of {0} was marshaled. The instance HashCode is: {1}", obj.ToString(), obj.GetHashCode().ToString());
            Console.WriteLine("ObjRef dump:");
            if (objref.ChannelInfo != null)
            {
                Console.WriteLine("  -- ChannelInfo: ");
                DumpChannelInfo(objref.ChannelInfo);
            }
            if (objref.EnvoyInfo != null)
                Console.WriteLine("  -- EnvoyInfo: " + objref.EnvoyInfo.ToString());
            if (objref.TypeInfo != null)
            {
                Console.WriteLine("  -- TypeInfo: " + objref.TypeInfo.ToString());
                Console.WriteLine("      -- " + objref.TypeInfo.TypeName);
            }
            if (objref.URI != null)
                Console.WriteLine("  -- URI: " + objref.URI.ToString());
        }

        private void DumpChannelInfo(IChannelInfo info)
        {

            foreach (object obj in info.ChannelData)
            {
                if (obj is ChannelDataStore)
                {
                    foreach (string uri in ((ChannelDataStore)obj).ChannelUris)
                        Console.WriteLine("      -- ChannelUris:" + uri);
                }
            }
        }

        // Notifies a handler that an object has been unmarshaled.
        public void UnmarshaledObject(Object obj, ObjRef or)
        {
            Console.WriteLine("Tracking: An instance of {0} was unmarshaled. The instance HashCode is: {1}", obj.ToString(), obj.GetHashCode().ToString());
        }

        // Notifies a handler that an object has been disconnected.
        public void DisconnectedObject(Object obj)
        {
            Console.WriteLine("Tracking: An instance of {0} was disconnected. The instance HashCode is: {1}",
                obj.ToString(), obj.GetHashCode().ToString());
        }
    }

    public interface IRemoteResource
    {
        void Publish(string instanceName);
        void SetupChannel(string channelName);
    }

    public abstract class RemoteResourceBase : MarshalByRefObject, IRemoteResource
    {
        public abstract void Publish(string instanceName);

        public abstract void SetupChannel(string channelName);
    }

    [Serializable]
    public class RemoteResourceTcp : RemoteResourceBase, IDisposable
    {
        public RemoteResourceTcp()
        {

        }
        ~RemoteResourceTcp()
        {

        }

        public override void Publish(string instanceName)
        {
            // Creating a custom formatter for a TcpChannel sink chain.
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            // Set up a server channel.
            string[] str = instanceName.Split(':');
            TcpServerChannel serverChannel = new TcpServerChannel(str[0], int.Parse(str[1]), provider);
            ChannelServices.RegisterChannel(serverChannel, false);

            // Show the name and priority of the channel.
            //Console.WriteLine("Channel Name: {0}", serverChannel.ChannelName);
            //Console.WriteLine("Channel Priority: {0}", serverChannel.ChannelPriority);

            // Expose an object for remote calls.
            RemotingConfiguration.RegisterWellKnownServiceType(this.GetType(), 
                string.Format("{0}", this._channelName), 
                WellKnownObjectMode.Singleton);
        }

        public override void SetupChannel(string channelName)
        {
            this._channelName = channelName;
        }
        public static object GetObject(Type type, string channelName, string instanceName, object state=null)
        {
            object service = null;
            if (type.BaseType == typeof(RemoteResourceTcp))
            {
                // Set up a client channel.
                TcpClientChannel clientChannel = new TcpClientChannel();
                ChannelServices.RegisterChannel(clientChannel, false);

                // Show the name and priority of the channel.
                //Console.WriteLine("Channel Name: {0}", clientChannel.ChannelName);
                //Console.WriteLine("Channel Priority: {0}", clientChannel.ChannelPriority);

                // Obtain a proxy for a remote object.
                //RemotingConfiguration.RegisterWellKnownClientType(type, string.Format("tcp://{0}/{1}", instanceName, channelName));
                service = state == null ? (object)Activator.GetObject(type, string.Format("tcp://{0}/{1}", instanceName, channelName)) : (object)Activator.GetObject(type, string.Format("tcp://{0}/{1}", instanceName, channelName), state);
            }

            return service;
        }

        public void Dispose()
        {
            
        }

        private string _channelName;
    }

    [Serializable]
    public class RemoteResourceHttp : RemoteResourceBase, IDisposable
    {
        public RemoteResourceHttp()
        {

        }
        ~RemoteResourceHttp()
        {

        }

        public override void Publish(string instanceName)
        {
            string[] str = instanceName.Split(':');
            HttpServerChannel http = new HttpServerChannel(int.Parse(str[1]));
            ChannelServices.RegisterChannel(http, false);
            RemotingConfiguration.RegisterWellKnownServiceType(this.GetType(), this._channelName, WellKnownObjectMode.Singleton);
        }

        public override void SetupChannel(string channelName)
        {
            this._channelName = channelName;
        }
        public static object GetObject(Type type, string channelName, string instanceName, object state = null)
        {
            object service = null;
            if (type.BaseType == typeof(RemoteResourceHttp))
            {
                string[] str = instanceName.Split(':');
                HttpClientChannel http = new HttpClientChannel();
                ChannelServices.RegisterChannel(http, false);
                service = Activator.GetObject(type, string.Format("http://{0}/{1}", instanceName, channelName));
                
                /*RemotingConfiguration.RegisterWellKnownClientType(type, string.Format("http://{0}/{1}", instanceName, channelName));
                //object[] url = { new UrlAttribute(string.Format("tcp://{0}/{1}", instanceName, channelName)) };
                service = Activator.CreateInstance(type);*/
            }

            return service;
        }

        public void Dispose()
        {

        }

        private string _channelName;
    }

    [Serializable]
    public class RemoteResourceClientActivate : RemoteResourceBase
    {
        public RemoteResourceClientActivate()
        {

        }
        ~RemoteResourceClientActivate()
        {

        }

        public override void Publish(string instanceName)
        {
            // Create and register channel
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);

            // Register client activated object
            RemotingConfiguration.RegisterActivatedClientType(this.GetType(), string.Format("tcp://{0}/My{1}", instanceName, this._channelName));

        }

        public override void SetupChannel(string channelName)
        {
            this._channelName = channelName;
        }
        public static object GetObject(Type type, string channelName, string instanceName, object state = null)
        {
            object service = null;
            if (type.BaseType == typeof(RemoteResourceClientActivate))
            {
                // Create and register channel
                TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, false);

                // Register client activated object
                RemotingConfiguration.RegisterActivatedClientType(type, string.Format("tcp://{0}/My{1}", instanceName, channelName));

                object[] url = { new UrlAttribute(string.Format("tcp://{0}/{1}", instanceName, channelName)) };
                service = Activator.CreateInstance(type, null, url);
            }

            return service;
        }

        public void Dispose()
        {

        }

        private string _channelName;
    }

    [Serializable]
    public class RemoteResource : RemoteResourceBase, IDisposable
    {
        public RemoteResource()
        {

        }
        ~RemoteResource()
        {

        }

        public override void Publish(string instanceName)
        {
            string[] str = instanceName.Split(':');
            TcpChannel channel = new TcpChannel(int.Parse(str[1]));
            ChannelServices.RegisterChannel(channel, false);

            TrackingServices.RegisterTrackingHandler(new TrackingHandler());
            
            var service = (RemoteResourceBase)this;

            ObjRef obj = RemotingServices.Marshal(service, this._channelName);
        }

        public override void SetupChannel(string channelName)
        {
            this._channelName = channelName;
        }
        public static object GetObject(Type type, string channelName, string instanceName, object state = null)
        {
            object service = null;
            if (type.BaseType == typeof(RemoteResource))
            {
                ChannelServices.RegisterChannel(new TcpChannel(), false);

                WellKnownClientTypeEntry remotetype = new WellKnownClientTypeEntry(
                    type, string.Format("tcp://{0}/{1}", instanceName, channelName));
                RemotingConfiguration.RegisterWellKnownClientType(remotetype);

                object[] url = { new UrlAttribute(string.Format("tcp://{0}/{1}", instanceName, channelName)) };
                service = Activator.CreateInstance(type, null, url);
            }

            return service;
        }

        public void Dispose()
        {
            //Console.WriteLine("\r\nPress Enter to unmarshal the object.");
            //Console.ReadLine();

            //RemotingServices.Unmarshal(obj);

            //Console.WriteLine("Press Enter to disconnect the object.");
            //Console.ReadLine();

            //RemotingServices.Disconnect(service);
        }

        private string _channelName;
    }
}
