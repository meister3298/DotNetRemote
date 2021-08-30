using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Lib1
{
    [Serializable]
    public class DataClass
    {
        private int _field1;
        private string _field2;
        public int Field1
        {
            get { return _field1; }
            set { _field1 = value; }
        }
        public string Field2
        {
            get { return _field2; }
            set { _field2 = value; }
        }
        public DataClass()
        {
            Console.WriteLine(string.Format("DataClass created in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public void Run()
        {
            Console.WriteLine(string.Format("DataClass running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
    }

    /*public class ServiceClassRR : RemoteResource
    {
        public ServiceClassRR()
        {
            Console.WriteLine(string.Format("ServiceClass created in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public void Proc1()
        {
            Console.WriteLine(string.Format("ServiceClass running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
    }*/
}
