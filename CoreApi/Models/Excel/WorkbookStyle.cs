using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class WorkbookStyle
    {
        private Alignment alignmentField;

        private Font fontField;

        private Interior interiorField;


        private string idField;

        private string nameField;

        private string parentField;

        /// <remarks />
        public Alignment Alignment
        {
            get => alignmentField;
            set => alignmentField = value;
        }

        /// <remarks />
        public Font Font
        {
            get => fontField;
            set => fontField = value;
        }

        /// <remarks />
        public Interior Interior
        {
            get => interiorField;
            set => interiorField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string ID
        {
            get => idField;
            set => idField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Name
        {
            get => nameField;
            set => nameField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Parent
        {
            get => parentField;
            set => parentField = value;
        }
    }


    // Internal Classes

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class Alignment
    {
        private string verticalField;
        private string horizontalField;

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Vertical
        {
            get => verticalField;
            set => verticalField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Horizontal
        {
            get => horizontalField;
            set => horizontalField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class Font
    {
        private string fontNameField;

        private string familyField;

        private float sizeField;

        private string colorField;

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string FontName
        {
            get => fontNameField;
            set => fontNameField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:office:excel")]
        public string Family
        {
            get => familyField;
            set => familyField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public float Size
        {
            get => sizeField;
            set => sizeField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Color
        {
            get => colorField;
            set => colorField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class Interior
    {
        private string colorField;

        private string patternField;

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Color
        {
            get => colorField;
            set => colorField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Pattern
        {
            get => patternField;
            set => patternField = value;
        }
    }
}