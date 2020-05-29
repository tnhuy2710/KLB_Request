using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel
{
    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    [XmlRoot(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
    public class ExcelWorkbook
    {
        private byte activeSheetField;

        private string protectStructureField;

        private string protectWindowsField;

        private ushort windowHeightField;

        private byte windowTopXField;

        private byte windowTopYField;

        private ushort windowWidthField;

        /// <remarks />
        public ushort WindowHeight
        {
            get => windowHeightField;
            set => windowHeightField = value;
        }

        /// <remarks />
        public ushort WindowWidth
        {
            get => windowWidthField;
            set => windowWidthField = value;
        }

        /// <remarks />
        public byte WindowTopX
        {
            get => windowTopXField;
            set => windowTopXField = value;
        }

        /// <remarks />
        public byte WindowTopY
        {
            get => windowTopYField;
            set => windowTopYField = value;
        }

        /// <remarks />
        public byte ActiveSheet
        {
            get => activeSheetField;
            set => activeSheetField = value;
        }

        /// <remarks />
        public string ProtectStructure
        {
            get => protectStructureField;
            set => protectStructureField = value;
        }

        /// <remarks />
        public string ProtectWindows
        {
            get => protectWindowsField;
            set => protectWindowsField = value;
        }
    }
}