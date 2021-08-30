using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lib1;
using Tech;

namespace DotNetRemote
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

            Tester tester = new Tester();
            for (int i = 0; i < racks.Count; i++)
            {
                tester.Racks.Add(i, racks[i]);
            }
            //Serialization<Tester> serialize = new Serialization<Tester>();
            //serialize.SerializeToXML(tester, @"C:\Users\1000271631\Desktop\Tester.xml");
            tester.SetupChannel("tester_service");
            tester.Publish("localhost:9090");

            /*Tester remoteObject = (Tester)Tester.GetObject(typeof(Tester), "tester_service", "localhost:9090");
            remoteObject.RunMessage();
            remoteObject.Racks[0].RunMessage();
            remoteObject.Racks[0].Primitives[0].RunMessage();
            remoteObject.Racks[0].Primitives[2].RunMessage();*/
        }
        
        static void Example2()
        {
            Remotable remote = new Remotable();
            Console.WriteLine(string.Format("Current State: {0}", remote.State));
            Console.WriteLine(string.Format("Current Message: {0}", remote.Message));

            Console.WriteLine("Server: Set message...");
            remote.SetMessage("This is a message from Server!");
            Console.WriteLine(string.Format("State: {0}", remote.State));

            Console.WriteLine("Publish server object!");
            remote.SetupChannel("remote_service");
            remote.Publish("localhost:9090");
        }

        static void Example3()
        {
            RemoteObject remote = new RemoteObject();
            remote.CountInc();
            Console.WriteLine(string.Format("Get count from server: {0}", remote.Count));

            Console.WriteLine("Publish server object!");
            remote.SetupChannel("remote_service");
            remote.Publish("localhost:9090");
        }

        static void Example4()
        {
            ServiceClass remote = new ServiceClass();
            Console.WriteLine("Server: Set message...");
            remote.SetMessage("This is a message from Server!");

            Console.WriteLine("Publish server object!");
            remote.SetupChannel("remote_service");
            remote.Publish("localhost:9090");
        }
        static void Main(string[] args)
        {
            Example2();

            // Wait for method calls.
            Console.WriteLine("Listening...");
            Console.ReadLine();
        }
    }
}
