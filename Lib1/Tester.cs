using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tech;

namespace Lib1
{
    [Serializable]
    public class XmlDictionary<T, V> : Dictionary<T, V>, IXmlSerializable
    {

        public XmlDictionary()
        {

        }
        //Needed for deserialization.
        protected XmlDictionary(SerializationInfo information, StreamingContext context)
                : base(information, context)
        {
        }

        [XmlType("Entry")]
        public struct Entry
        {
            public Entry(T key, V value) : this() { Key = key; Value = value; }
            [XmlElement("Key")]
            public T Key { get; set; }
            [XmlElement("Value")]
            public V Value { get; set; }
        }
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            this.Clear();
            var serializer = new XmlSerializer(typeof(List<Entry>));
            reader.Read();  // Why is this necessary?
            var list = (List<Entry>)serializer.Deserialize(reader);
            foreach (var entry in list) this[entry.Key] = entry.Value;
            reader.ReadEndElement();
        }
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            var list = new List<Entry>(this.Count);
            foreach (var entry in this) list.Add(new Entry(entry.Key, entry.Value));
            XmlSerializer serializer = new XmlSerializer(list.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(writer, list, ns);
        }
    }
    [Serializable]
    public class Tester : RemoteResource
    {
        private XmlDictionary<int, Rack> _racks;
        private string _msg;
        [XmlElement("Rack")]
        public XmlDictionary<int, Rack> Racks { get => _racks; set => _racks = value; }
        public string Message { get => _msg; set => _msg = value; }
        public int Count = 0;

        public Tester()
        {
            _racks = new XmlDictionary<int, Rack>();
            Console.WriteLine(string.Format("Tester running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public void UpdateMessage(string str)
        {
            _msg = str;
        }
        public void RunMessage()
        {
            _msg = string.Format("Tester: Hello {0}", Count);
            Count++;
        }
    }
    [Serializable]
    public class Rack : RemoteResource
    {
        private int _id;
        private string _msg;
        private XmlDictionary<int, Primitive> _primitives;
        /// <summary>
        /// The ID of the rack.
        /// </summary>
        [XmlAttribute]
        public int ID { get => _id; set => _id = value; }
        public string Message { get => _msg; set => _msg = value; }
        public int Count = 0;
        /// <summary>
        /// Collections of the primitives.
        /// </summary>
        [XmlElement("Primitive")]
        public XmlDictionary<int, Primitive> Primitives { get => _primitives; set => _primitives = value; }
       
        /// <summary>
        /// Default Constructor of the rack.
        /// </summary>
        public Rack()
        {
            _id = 0;
            _msg = "";
            _primitives = new XmlDictionary<int, Primitive>();
            Console.WriteLine(string.Format("Rack running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        /// <summary>
        /// Constructor of the rack.
        /// </summary>
        /// <param name="id">Specifies the id.</param>
        /// <param name="primitives">Specifies the primitives.</param>
        /// <param name="tester">Specifies the tester.</param>
        public Rack(int id, XmlDictionary<int, Primitive> primitives)
        {
            _id = id;
            _msg = "";
            _primitives = primitives;
            Console.WriteLine(string.Format("Rack running in {0}", AppDomain.CurrentDomain.FriendlyName));

            if (-1 == _id)
                return;
        }

        public void UpdateMessage(string str)
        {
            _msg = str;
        }
        public void RunMessage()
        {
            _msg = string.Format("Rack: Hello {0}", Count);
            Count++;
        }
        /// <summary>
        /// Returns the rack info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}]", _id); ;
        }
    }
    [Serializable]
    /// <summary>
    /// The Cassette class is to used by the automation module.
    /// </summary>
    public class Cassette : RemoteResource
    {
        private string _id;
        private string _msg;
        private string _location;
        private XmlDictionary<int, Slot> _slots;
        private Primitive _primitive;


        /// <summary>
        /// The ID of the cassette.
        /// </summary>
        [XmlAttribute]
        public string ID { get => _id; set => _id = value; }
        public string Message { get => _msg; set => _msg = value; }
        public int Count = 0;
        public string Location { get => _location; set => _location = value; }
        /// <summary>
        /// The collections of the slot.
        /// </summary>
        [XmlElement("Slot")]
        public XmlDictionary<int, Slot> Slots { get => _slots; set => _slots = value; }
        /// <summary>
        /// Access to the primitive parent class.
        /// </summary>
        public Primitive Primitive { get => _primitive; set => _primitive = value; }
        /// <summary>
        /// Default constructor of the cassette.
        /// </summary>
        public Cassette()
        {
            _id = "";
            _msg = "";
            _slots = new XmlDictionary<int, Slot>();
            Console.WriteLine(string.Format("Cassette running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public Cassette(string id, string location, XmlDictionary<int, Slot> slots)
        {
            _id = id;
            _msg = "";
            _slots = slots;
            _location = location;
            Console.WriteLine(string.Format("Cassette running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public void UpdateMessage(string str)
        {
            _msg = str;
        }
        public void RunMessage()
        {
            _msg = string.Format("Cassette: Hello {0}", Count);
            Count++;
        }
        /// <summary>
        /// Returns the cassette info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}/{1}/{2}]", _primitive.Rack.ID, _primitive.ID, _id);
        }
    }
    [Serializable]
    public class Primitive : RemoteResource
    {
        private int _id;
        private string _msg;
        private XmlDictionary<string, Cassette> _cassettes;
        private XmlDictionary<int, Slot> _slots;
        private Rack _rack;

        [XmlAttribute]
        public int ID { get => _id; set => _id = value; }
        public string Message { get => _msg; set => _msg = value; }
        public int Count = 0;
        [XmlElement("Cassette")]
        public XmlDictionary<string, Cassette> Cassettes { get => _cassettes; set => _cassettes = value; }
        /// <summary>
        /// The collections of the slot.
        /// </summary>
        [XmlElement("Slot")]
        public XmlDictionary<int, Slot> Slots { get => _slots; set => _slots = value; }
        /// <summary>
        /// Access to the rack parent class.
        /// </summary>
        public Rack Rack { get => _rack; set => _rack = value; }
        
        /// <summary>
        /// Default constructor of th primitive.
        /// </summary>
        public Primitive()
        {
            _id = 0;
            _msg = "";
            _cassettes = new XmlDictionary<string, Cassette>();
            _slots = new XmlDictionary<int, Slot>();
            Console.WriteLine(string.Format("Primitive running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        /// <summary>
        /// Constructor of the primitive.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cassettes"></param>
        /// <param name="rack"></param>
        public Primitive(int id, XmlDictionary<string, Cassette> cassettes, Rack rack = null)
        {
            _id = id;
            _msg = "";
            _cassettes = cassettes;
            _slots = new XmlDictionary<int, Slot>();
            _rack = rack;

            Console.WriteLine(string.Format("Primitive running in {0}", AppDomain.CurrentDomain.FriendlyName));

            if (_id < 0)
                return;
        }
        public void UpdateMessage(string str)
        {
            _msg = str;
        }
        public void RunMessage()
        {
            _msg = string.Format("Primitive: Hello {0}", Count);
            Count++;
        }
        /// <summary>
        /// Returns the primitive info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}/{1}]", _rack.ID, _id); ;
        }
        /// <summary>
        /// Tables to string.
        /// </summary>
        /// <param name="Dictionary">The table.</param>
        /// <returns>Table in string format</returns>
        public string DictionaryToString(Dictionary<string, string> Dictionary)
        {
            return string.Join("\n", Dictionary.Select(x => x.Key + "=" + x.Value).ToArray());
        }
        
        private bool IsNAorEmpty(string value)
        {
            return ((string.IsNullOrEmpty(value)) ||
                    (value == "NA"));
        }
    }
    [Serializable]
    /// <summary>Slot class. Tester -> Rack -> Primitive -> Slot </summary>
    public class Slot : RemoteResource
    {
        private int _id;
        private string _msg;
        private bool _moveIn;
        private string _serialNumber;
        private string _siteModule;

        private Primitive _primitive;

        /// <summary>
        /// The ID of the slot.
        /// </summary>
        [XmlAttribute]
        public int ID { get => _id; set => _id = value; }
        public string Message { get => _msg; set => _msg = value; }
        public int Count = 0;
        /// <summary>
        /// The flag of the slot.
        /// </summary>
        [XmlAttribute]
        public bool MovedIn { get => _moveIn; set => _moveIn = value; }

        /*/// <summary>
        /// OBSOLETE, Use SFCInfo.DriveInfo
        /// </summary>*/
        //[XmlAttribute]
        //public AdvDriveInfo SerialNoInfo
        //{
        //    get => _driveInfo;
        //    set
        //    {
        //        _driveInfo = value;
        //        //populateData();
        //    }
        //}

        /// <summary>
        /// The serial number of the DUT in slot.
        /// </summary>
        [XmlAttribute]
        public string SerialNumber { get => _serialNumber; set => _serialNumber = value; }
        /// <summary>
        /// The site module of the DUT in slot.
        /// </summary>
        [XmlAttribute]
        public string SiteModule { get => _siteModule; set { if (!string.IsNullOrEmpty(value)) _siteModule = value; } }
        /// <summary>
        /// Access to the primitive parrent class.
        /// </summary>
        /// 


        public Primitive Primitive { get => _primitive; set => _primitive = value; }
        /*/// <summary>
        /// SFC info.
        /// </summary>*/
        //public Dictionary<string, string> Sfc { get => _sfc; set => _sfc = value; }
        /// <summary>
        /// Default constructor of the slot.
        /// </summary>
        public Slot()
        {
            _id = 0;
            _msg = "";
            _serialNumber = "";
            Console.WriteLine(string.Format("Slot running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        /// <summary>
        /// Constructor of the slot.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serialNumber"></param>
        /// <param name="cassette"></param>
        public Slot(int id, string serialNumber)
        {
            _id = id;
            _msg = "";
            _serialNumber = serialNumber;
            Console.WriteLine(string.Format("Slot running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        /// <summary>
        /// Constructor of the slot.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serialNumber"></param>
        /// <param name="primitive"></param>
        /// <param name="cassette"></param>
        public Slot(int id, string serialNumber, Primitive primitive = null)
        {
            _id = id;
            _msg = "";
            _serialNumber = serialNumber;
            _primitive = primitive;
            Console.WriteLine(string.Format("Slot running in {0}", AppDomain.CurrentDomain.FriendlyName));
        }
        public void UpdateMessage(string str)
        {
            _msg = str;
        }
        public void RunMessage()
        {
            _msg = string.Format("Slot: Hello {0}", Count);
            Count++;
        }
        /// <summary>
        /// Reset the SFC, Status, AttrSfcMap, Flag and SiteModule.
        /// </summary>
        public void Reset()
        {
            _moveIn = false;
            _siteModule = "";
        }
        /// <summary>
        /// Returns the site module name.
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        public string GetSiteModuleName(int sm)
        {
            //if(_cassette != null)
            //    return String.Format("sm{0}{1}{2}", _cassette.Primitive.Rack.ID, _cassette.Primitive.ID, sm);
            return String.Format("sm{0}{1}{2}", _primitive.Rack.ID, _primitive.ID, sm);
        }
       
        /// <summary>
        /// Returns the slot info.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //if(_cassette != null)
            //    return String.Format("[{0}/{1}/{2}/{3}]", _cassette.Primitive.Rack.ID, _cassette.Primitive.ID, _cassette.ID, _id);
            return String.Format("[{0}/{1}/{2}]", Primitive.Rack.ID, Primitive.ID, _id);
        }

    }
}
