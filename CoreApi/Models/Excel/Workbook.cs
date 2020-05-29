using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    [XmlRoot(Namespace = "urn:schemas-microsoft-com:office:spreadsheet", IsNullable = false)]
    public class Workbook
    {
        private WorkbookStyle[] _stylesField;
        private Worksheet[] _worksheetField;


        /// <remarks />
        [XmlArrayItem("Style", IsNullable = false)]
        public WorkbookStyle[] Styles
        {
            get => _stylesField;
            set => _stylesField = value;
        }

        /// <remarks />
        [XmlElement("Worksheet", IsNullable = false)]
        public Worksheet[] Worksheets
        {
            get => _worksheetField;
            set => _worksheetField = value;
        }
    }

    
}