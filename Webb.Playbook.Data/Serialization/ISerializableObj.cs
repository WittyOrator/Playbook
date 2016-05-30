using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Data
{
    public interface ISerializableObj
    {
        void ReadXml(System.Xml.Linq.XElement element);

        void WriteXml(System.Xml.XmlWriter writer);
    }
}
