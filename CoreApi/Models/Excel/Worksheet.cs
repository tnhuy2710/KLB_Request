using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel
{
    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class Worksheet
    {
        private string _nameField;
        private WorksheetTable _tableField;

        private WorksheetOptions _worksheetOptionsField;

        /// <remarks />
        public WorksheetTable Table
        {
            get => _tableField;
            set => _tableField = value;
        }

        /// <remarks />
        [XmlElement(Namespace = "urn:schemas-microsoft-com:office:excel")]
        public WorksheetOptions WorksheetOptions
        {
            get => _worksheetOptionsField;
            set => _worksheetOptionsField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Name
        {
            get => _nameField;
            set => _nameField = value;
        }
    }


    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class WorksheetTable
    {
        private WorksheetTableColumn[] _columnField;

        private float _defaultRowHeightField;

        private int _expandedColumnCountField;

        private int _expandedRowCountField;

        private int _fullColumnsField;

        private int _fullRowsField;

        private WorksheetTableRow[] _rowField;

        /// <remarks />
        [XmlElement("Column")]
        public WorksheetTableColumn[] Columns
        {
            get => _columnField;
            set => _columnField = value;
        }

        /// <remarks />
        [XmlElement("Row")]
        public WorksheetTableRow[] Rows
        {
            get => _rowField;
            set => _rowField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public int ExpandedColumnCount
        {
            get => _expandedColumnCountField;
            set => _expandedColumnCountField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public int ExpandedRowCount
        {
            get => _expandedRowCountField;
            set => _expandedRowCountField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:office:excel")]
        public int FullColumns
        {
            get => _fullColumnsField;
            set => _fullColumnsField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:office:excel")]
        public int FullRows
        {
            get => _fullRowsField;
            set => _fullRowsField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public float DefaultRowHeight
        {
            get => _defaultRowHeightField;
            set => _defaultRowHeightField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class WorksheetTableColumn
    {
        private byte autoFitWidthField;
        private byte indexField;

        private decimal widthField;

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public byte Index
        {
            get => indexField;
            set => indexField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public byte AutoFitWidth
        {
            get => autoFitWidthField;
            set => autoFitWidthField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public decimal Width
        {
            get => widthField;
            set => widthField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class WorksheetTableRow
    {
        private byte _autoFitHeightField;
        private WorksheetTableRowCell[] _cellField;
        private int _indexField;

        private decimal _heightField;
        private byte _hiddenField;

        /// <remarks />
        [XmlElement("Cell")]
        public WorksheetTableRowCell[] Cells
        {
            get => _cellField;
            set => _cellField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public byte AutoFitHeight
        {
            get => _autoFitHeightField;
            set => _autoFitHeightField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public decimal Height
        {
            get => _heightField;
            set => _heightField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public byte Hidden
        {
            get => _hiddenField;
            set => _hiddenField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public int Index
        {
            get => _indexField;
            set => _indexField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class WorksheetTableRowCell
    {
        private WorksheetTableRowCellData _dataField;

        private int _indexField;

        private bool _indexFieldSpecified;

        private string _styleIdField;

        private string _formulaField;

        private int _mergeAcrossField;

        private int _mergeDownField;

        /// <remarks />
        public WorksheetTableRowCellData Data
        {
            get => _dataField;
            set => _dataField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string StyleID
        {
            get => _styleIdField;
            set => _styleIdField = value;
        }

        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Formula
        {
            get => _formulaField;
            set => _formulaField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public int Index
        {
            get => _indexField;
            set => _indexField = value;
        }

        /// <remarks />
        [XmlIgnore]
        public bool IndexSpecified
        {
            get => _indexFieldSpecified;
            set => _indexFieldSpecified = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public int MergeAcross
        {
            get => _mergeAcrossField;
            set => _mergeAcrossField = value;
        }

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public int MergeDown
        {
            get => _mergeDownField;
            set => _mergeDownField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public class WorksheetTableRowCellData
    {
        private string _typeField;

        private string _valueField;

        /// <remarks />
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string Type
        {
            get => _typeField;
            set => _typeField = value;
        }

        /// <remarks />
        [XmlText]
        public string Value
        {
            get => _valueField;
            set => _valueField = value;
        }
    }

    /// <remarks />
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    [XmlRoot(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
    public class WorksheetOptions
    {
        private string _visibleField;
        
        /// <remarks />
        public string Visible
        {
            get => _visibleField;
            set => _visibleField = value;
        }
    }

}