using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:office")]
    [XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
    public class OfficeDocumentSettings
    {
        private object _allowPngField;

        /// <remarks />
        public object AllowPNG
        {
            get => _allowPngField;
            set => _allowPngField = value;
        }
    }
}