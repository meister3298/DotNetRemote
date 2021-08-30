using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Lib1
{
    public class MyServiceClass : RemoteResource
    {
        public int callCount = 0;
        public MyServiceClass()
        {
            callCount = 99;
            Console.WriteLine(string.Format("ServiceClass created in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public int GetCount()
        {
            Console.WriteLine(string.Format("ServiceClass running in {0}", AppDomain.CurrentDomain.FriendlyName));
            callCount++;
            return (callCount);
        }
        public void Run()
        {
            Console.WriteLine(string.Format("ServiceClass running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public void Proc1(int field1, string field2)
        {
            Console.WriteLine(string.Format("Proc1 called with {0},{1}!", field1, field2));
        }
        public void Proc2(DataClass item)
        {
            Console.WriteLine(string.Format("Proc2 called!"));
            item.Run();
        }
        public DataClass Proc3()
        {
            Console.WriteLine(string.Format("Proc3 called!"));
            DataClass item = new DataClass();
            item.Field1 = 100;
            item.Field2 = "ABC";
            return item;
        }
    }

    [Serializable]
    public class Remotable : RemoteResource
    {
        private int _state;
        private string _message;

        public Remotable()
        {
            this._state = 0;
            this._message = string.Empty;
            Console.WriteLine(string.Format("Remotable object is created in {0}!", AppDomain.CurrentDomain.FriendlyName));
        }
        public int State
        {
            get { return this._state; }
        }
        public string Message
        {
            get { return this._message; }
        }

        public void SetState(int state)
        {
            this._state = state;
            Console.WriteLine(string.Format("{0} SetState: {1}", AppDomain.CurrentDomain.FriendlyName, this._state));
        }
        public void SetMessage(string msg)
        {
            this._state++;
            this._message = msg;
            Console.WriteLine(string.Format("{0} SetMessage: {1}", AppDomain.CurrentDomain.FriendlyName, this._message));
        }
    }

    [Serializable]
    public class RemoteObject : RemoteResource
    {
        private int callCount = 0;

        public int Count
        {
            get { return callCount; }
        }

        public void CountInc()
        {
            callCount++;
        }
    }

    public class MyRemoteObject : RemoteResource
    {
        public MyRemoteObject(int val)
        {
            callCount = val;
        }
        private int callCount = 0;

        public int GetValue()
        {
            return callCount;
        }

        public void SetValue(int val)
        {
            callCount = val;
        }
    }

    public class ServiceClass : RemoteResource
    {
        private DateTime starttime;
        private string _message;

        public ServiceClass()
        {
            Console.WriteLine("A ServiceClass has been created.");
            starttime = DateTime.Now;
        }

        ~ServiceClass()
        {
            Console.WriteLine("ServiceClass being collected after " + (new TimeSpan(DateTime.Now.Ticks - starttime.Ticks)).ToString() + " seconds.");
        }
        public string Message
        {
            get { return this._message; }
        }

        public void SetMessage(string msg)
        {
            this._message = msg;
            Console.WriteLine(string.Format("{0} SetMessage: {1}", AppDomain.CurrentDomain.FriendlyName, this._message));
        }
        public DateTime GetServerTime()
        {
            Console.WriteLine("Time requested by client.");
            return DateTime.Now;
        }
    }
}
