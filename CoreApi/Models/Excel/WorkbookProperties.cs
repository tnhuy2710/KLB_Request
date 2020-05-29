using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    [XmlRoot(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
    public class WorkbookProperties
    {
        private ushort _windowHeightField;

        private ushort _windowWidthField;

        private byte _windowTopXField;

        private byte _windowTopYField;

        private byte _activeSheetField;

        private string _protectStructureField;

        private string _protectWindowsField;

        /// <remarks />
        public ushort WindowHeight
        {
            get => _windowHeightField;
            set => _windowHeightField = value;
        }

        /// <remarks />
        public ushort WindowWidth
        {
            get => _windowWidthField;
            set => _windowWidthField = value;
        }

        /// <remarks />
        public byte WindowTopX
        {
            get => _windowTopXField;
            set => _windowTopXField = value;
        }

        /// <remarks />
        public byte WindowTopY
        {
            get => _windowTopYField;
            set => _windowTopYField = value;
        }

        /// <remarks />
        public byte ActiveSheet
        {
            get => _activeSheetField;
            set => _activeSheetField = value;
        }

        /// <remarks />
        public string ProtectStructure
        {
            get => _protectStructureField;
            set => _protectStructureField = value;
        }

        /// <remarks />
        public string ProtectWindows
        {
            get => _protectWindowsField;
            set => _protectWindowsField = value;
        }
    }
}