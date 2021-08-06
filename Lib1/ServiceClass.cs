using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tech;

namespace Lib1
{
    public class ServiceClass : RemoteResource
    {
        public int callCount = 0;
        public ServiceClass()
        {
            callCount = 99;
            Console.WriteLine(string.Format("ServiceClass created in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public int GetCount()
        {
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

    public class Remotable : RemoteResource
    {

        private int callCount = 0;

        public int GetCount()
        {
            callCount++;
            return (callCount);
        }
    }
}
