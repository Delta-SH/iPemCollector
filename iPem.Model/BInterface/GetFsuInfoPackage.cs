using System;
using System.Xml;

namespace iPem.Model {
    public partial class GetFsuInfoPackage {
        public string FsuId { get; set; }

        public virtual string ToXml() {
            var xmlDoc = new XmlDocument();
            var node = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "");
            xmlDoc.AppendChild(node);

            var root = xmlDoc.CreateElement("Request");
            xmlDoc.AppendChild(root);

            var PK_Type = xmlDoc.CreateElement("PK_Type");
            root.AppendChild(PK_Type);

            var Name = xmlDoc.CreateElement("Name");
            Name.InnerText = EnmBIPackType.GET_FSUINFO.ToString();
            PK_Type.AppendChild(Name);

            var Info = xmlDoc.CreateElement("Info");
            root.AppendChild(Info);

            var FSUID = xmlDoc.CreateElement("FSUID");
            FSUID.InnerText = this.FsuId ?? "";
            Info.AppendChild(FSUID);

            return xmlDoc.OuterXml;
        }
    }
}
