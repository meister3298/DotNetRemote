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
using System.Threading;
using System.Runtime.Remoting.Activation;

namespace DotNetRemoteClient
{
    class Program
    {
        static void Example1()
        {
            XmlDictionary<int, Slot> slots = new XmlDictionary<int, Slot>();
            for (int i = 0; i < 4; i++)
            {
                slots.Add(i, new Slot(i, i.ToString()));
            }
            XmlDictionary<string, Cassette> cassettes = new XmlDictionary<string, Cassette>();
            for (int i = 0; i < 2; i++)
            {
                if (i % 2 == 0)
                    cassettes.Add(i.ToString(), new Cassette(i.ToString(), "TOP", slots));
                else
                    cassettes.Add(i.ToString(), new Cassette(i.ToString(), "BOTTOM", slots));
            }
            XmlDictionary<int, Primitive> primitives = new XmlDictionary<int, Primitive>();
            for (int i = 0; i < 4; i++)
            {
                primitives.Add(i, new Primitive(i, cassettes));
            }
            List<Rack> racks = new List<Rack>();
            for (int i = 0; i < 2; i++)
            {
                racks.Add(new Rack(i, primitives));
            }

            // Call a method on the object.
            Tester remoteObject = (Tester)Tester.GetObject(typeof(Tester), "tester_service", "localhost:9090");
            for (int i = 0; i < racks.Count; i++)
            {
                remoteObject.Racks.Add(i, racks[i]);
            }

            remoteObject.RunMessage();
            Console.WriteLine(string.Format("Tester message: {0}", remoteObject.Message));

            // Wait for method calls.
            Console.WriteLine("Subscribe...");
            remoteObject.RunMessage();
            remoteObject.Racks[0].Primitives[0].RunMessage();
            remoteObject.Racks[0].Primitives[2].RunMessage();
            Console.WriteLine(string.Format("Rack message: {0}", remoteObject.Racks[0].Message));
            Console.WriteLine(string.Format("Primitive0 message: {0}", remoteObject.Racks[0].Primitives[0].Message));
            Console.WriteLine(string.Format("Primitive2 message: {0}", remoteObject.Racks[0].Primitives[2].Message));

            /*Console.WriteLine("Update Message...");
            remoteObject.RunMessage();
            remoteObject.Racks[0].Primitives[0].RunMessage();
            remoteObject.Racks[0].Primitives[2].RunMessage();
            Console.WriteLine(string.Format("Rack message: {0}", remoteObject.Racks[0].Message));
            Console.WriteLine(string.Format("Primitive0 message: {0}", remoteObject.Racks[0].Primitives[0].Message));
            Console.WriteLine(string.Format("Primitive2 message: {0}", remoteObject.Racks[0].Primitives[2].Message));*/
        }
        
        static void Example2()
        {
            Console.WriteLine("Create client object!");
            Remotable client = (Remotable)Remotable.GetObject(typeof(Remotable), "remote_service", "localhost:9090");

            Console.WriteLine(string.Format("Previous message: {0}\n", client.Message));

            Console.WriteLine("Client: Set message...");
            Console.WriteLine(string.Format("Current State: {0}", client.State));
            client.SetMessage(string.Format("This is a message from Client_{0}!", client.State));
            Console.WriteLine(string.Format("Current Message: {0}", client.Message));
        }

        static void Example3()
        {
            Console.WriteLine("Create client object!");
            RemoteObject client = (RemoteObject)RemoteObject.GetObject(typeof(RemoteObject), "remote_service", "localhost:9090");
            client.CountInc();
            Console.WriteLine(string.Format("Get count from client: {0}", client.Count));
        }

        static void Example4()
        {
            Console.WriteLine("Create client object!");
            ServiceClass client = (ServiceClass)ServiceClass.GetObject(typeof(ServiceClass), "remote_service", "localhost:9090");

            Console.WriteLine(string.Format("Previous message: {0}\n", client.Message));

            Console.WriteLine("Client: Set message...");
            client.SetMessage(string.Format("This is a message from Client!"));
            Console.WriteLine(string.Format("Current Message: {0}", client.Message));
        }
        
        static void Main(string[] args)
        {
            Example2();

            Console.ReadLine();
        }
    }
}
