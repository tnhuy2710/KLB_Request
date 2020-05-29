using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CoreApi.Models.Excel.Test
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>WorksheetTableRowCell
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas-microsoft-com:office:spreadsheet", IsNullable = false)]
    public partial class Workbook
    {

        private WorkbookFull documentPropertiesField;

        private OfficeDocumentSettings officeDocumentSettingsField;

        private ExcelWorkbook excelWorkbookField;

        private WorkbookStyles stylesField;

        private WorkbookWorksheet[] worksheetField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:schemas-microsoft-com:office:office")]
        public WorkbookFull DocumentProperties
        {
            get
            {
                return this.documentPropertiesField;
            }
            set
            {
                this.documentPropertiesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:schemas-microsoft-com:office:office")]
        public OfficeDocumentSettings OfficeDocumentSettings
        {
            get
            {
                return this.officeDocumentSettingsField;
            }
            set
            {
                this.officeDocumentSettingsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:schemas-microsoft-com:office:excel")]
        public ExcelWorkbook ExcelWorkbook
        {
            get
            {
                return this.excelWorkbookField;
            }
            set
            {
                this.excelWorkbookField = value;
            }
        }

        /// <remarks/>
        public WorkbookStyles Styles
        {
            get
            {
                return this.stylesField;
            }
            set
            {
                this.stylesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Worksheet")]
        public WorkbookWorksheet[] Worksheet
        {
            get
            {
                return this.worksheetField;
            }
            set
            {
                this.worksheetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:office")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
    public partial class WorkbookFull
    {

        private string authorField;

        private string lastAuthorField;

        private System.DateTime createdField;

        private System.DateTime lastSavedField;

        private decimal versionField;

        /// <remarks/>
        public string Author
        {
            get
            {
                return this.authorField;
            }
            set
            {
                this.authorField = value;
            }
        }

        /// <remarks/>
        public string LastAuthor
        {
            get
            {
                return this.lastAuthorField;
            }
            set
            {
                this.lastAuthorField = value;
            }
        }

        /// <remarks/>
        public System.DateTime Created
        {
            get
            {
                return this.createdField;
            }
            set
            {
                this.createdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime LastSaved
        {
            get
            {
                return this.lastSavedField;
            }
            set
            {
                this.lastSavedField = value;
            }
        }

        /// <remarks/>
        public decimal Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:office")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
    public partial class OfficeDocumentSettings
    {

        private object allowPNGField;

        /// <remarks/>
        public object AllowPNG
        {
            get
            {
                return this.allowPNGField;
            }
            set
            {
                this.allowPNGField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
    public partial class ExcelWorkbook
    {

        private ushort windowHeightField;

        private ushort windowWidthField;

        private byte windowTopXField;

        private byte windowTopYField;

        private string protectStructureField;

        private string protectWindowsField;

        /// <remarks/>
        public ushort WindowHeight
        {
            get
            {
                return this.windowHeightField;
            }
            set
            {
                this.windowHeightField = value;
            }
        }

        /// <remarks/>
        public ushort WindowWidth
        {
            get
            {
                return this.windowWidthField;
            }
            set
            {
                this.windowWidthField = value;
            }
        }

        /// <remarks/>
        public byte WindowTopX
        {
            get
            {
                return this.windowTopXField;
            }
            set
            {
                this.windowTopXField = value;
            }
        }

        /// <remarks/>
        public byte WindowTopY
        {
            get
            {
                return this.windowTopYField;
            }
            set
            {
                this.windowTopYField = value;
            }
        }

        /// <remarks/>
        public string ProtectStructure
        {
            get
            {
                return this.protectStructureField;
            }
            set
            {
                this.protectStructureField = value;
            }
        }

        /// <remarks/>
        public string ProtectWindows
        {
            get
            {
                return this.protectWindowsField;
            }
            set
            {
                this.protectWindowsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookStyles
    {

        private WorkbookStylesStyle styleField;

        /// <remarks/>
        public WorkbookStylesStyle Style
        {
            get
            {
                return this.styleField;
            }
            set
            {
                this.styleField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookStylesStyle
    {

        private WorkbookStylesStyleAlignment alignmentField;

        private object bordersField;

        private WorkbookStylesStyleFont fontField;

        private object interiorField;

        private object numberFormatField;

        private object protectionField;

        private string idField;

        private string nameField;

        /// <remarks/>
        public WorkbookStylesStyleAlignment Alignment
        {
            get
            {
                return this.alignmentField;
            }
            set
            {
                this.alignmentField = value;
            }
        }

        /// <remarks/>
        public object Borders
        {
            get
            {
                return this.bordersField;
            }
            set
            {
                this.bordersField = value;
            }
        }

        /// <remarks/>
        public WorkbookStylesStyleFont Font
        {
            get
            {
                return this.fontField;
            }
            set
            {
                this.fontField = value;
            }
        }

        /// <remarks/>
        public object Interior
        {
            get
            {
                return this.interiorField;
            }
            set
            {
                this.interiorField = value;
            }
        }

        /// <remarks/>
        public object NumberFormat
        {
            get
            {
                return this.numberFormatField;
            }
            set
            {
                this.numberFormatField = value;
            }
        }

        /// <remarks/>
        public object Protection
        {
            get
            {
                return this.protectionField;
            }
            set
            {
                this.protectionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookStylesStyleAlignment
    {

        private string verticalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string Vertical
        {
            get
            {
                return this.verticalField;
            }
            set
            {
                this.verticalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookStylesStyleFont
    {

        private string fontNameField;

        private string familyField;

        private byte sizeField;

        private string colorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string FontName
        {
            get
            {
                return this.fontNameField;
            }
            set
            {
                this.fontNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:office:excel")]
        public string Family
        {
            get
            {
                return this.familyField;
            }
            set
            {
                this.familyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte Size
        {
            get
            {
                return this.sizeField;
            }
            set
            {
                this.sizeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string Color
        {
            get
            {
                return this.colorField;
            }
            set
            {
                this.colorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookWorksheet
    {

        private WorkbookWorksheetTable tableField;

        private WorksheetOptions worksheetOptionsField;

        private string nameField;

        /// <remarks/>
        public WorkbookWorksheetTable Table
        {
            get
            {
                return this.tableField;
            }
            set
            {
                this.tableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:schemas-microsoft-com:office:excel")]
        public WorksheetOptions WorksheetOptions
        {
            get
            {
                return this.worksheetOptionsField;
            }
            set
            {
                this.worksheetOptionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookWorksheetTable
    {

        private WorkbookWorksheetTableColumn[] columnField;

        private WorkbookWorksheetTableRow rowField;

        private byte expandedColumnCountField;

        private byte expandedRowCountField;

        private byte fullColumnsField;

        private byte fullRowsField;

        private byte defaultRowHeightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Column")]
        public WorkbookWorksheetTableColumn[] Column
        {
            get
            {
                return this.columnField;
            }
            set
            {
                this.columnField = value;
            }
        }

        /// <remarks/>
        public WorkbookWorksheetTableRow Row
        {
            get
            {
                return this.rowField;
            }
            set
            {
                this.rowField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte ExpandedColumnCount
        {
            get
            {
                return this.expandedColumnCountField;
            }
            set
            {
                this.expandedColumnCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte ExpandedRowCount
        {
            get
            {
                return this.expandedRowCountField;
            }
            set
            {
                this.expandedRowCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:office:excel")]
        public byte FullColumns
        {
            get
            {
                return this.fullColumnsField;
            }
            set
            {
                this.fullColumnsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:office:excel")]
        public byte FullRows
        {
            get
            {
                return this.fullRowsField;
            }
            set
            {
                this.fullRowsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte DefaultRowHeight
        {
            get
            {
                return this.defaultRowHeightField;
            }
            set
            {
                this.defaultRowHeightField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookWorksheetTableColumn
    {

        private byte indexField;

        private bool indexFieldSpecified;

        private byte autoFitWidthField;

        private decimal widthField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte Index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IndexSpecified
        {
            get
            {
                return this.indexFieldSpecified;
            }
            set
            {
                this.indexFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte AutoFitWidth
        {
            get
            {
                return this.autoFitWidthField;
            }
            set
            {
                this.autoFitWidthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookWorksheetTableRow
    {

        private WorkbookWorksheetTableRowCell[] cellField;

        private byte autoFitHeightField;

        private bool autoFitHeightFieldSpecified;

        private decimal heightField;

        private bool heightFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Cell")]
        public WorkbookWorksheetTableRowCell[] Cell
        {
            get
            {
                return this.cellField;
            }
            set
            {
                this.cellField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte AutoFitHeight
        {
            get
            {
                return this.autoFitHeightField;
            }
            set
            {
                this.autoFitHeightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool AutoFitHeightSpecified
        {
            get
            {
                return this.autoFitHeightFieldSpecified;
            }
            set
            {
                this.autoFitHeightFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool HeightSpecified
        {
            get
            {
                return this.heightFieldSpecified;
            }
            set
            {
                this.heightFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookWorksheetTableRowCell
    {

        private WorkbookWorksheetTableRowCellData dataField;

        private byte indexField;

        private bool indexFieldSpecified;

        /// <remarks/>
        public WorkbookWorksheetTableRowCellData Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public byte Index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IndexSpecified
        {
            get
            {
                return this.indexFieldSpecified;
            }
            set
            {
                this.indexFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:spreadsheet")]
    public partial class WorkbookWorksheetTableRowCellData
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
    public partial class WorksheetOptions
    {

        private WorksheetOptionsPageSetup pageSetupField;

        private object selectedField;

        private string visibleField;

        private WorksheetOptionsPanes panesField;

        private string protectObjectsField;

        private string protectScenariosField;

        /// <remarks/>
        public WorksheetOptionsPageSetup PageSetup
        {
            get
            {
                return this.pageSetupField;
            }
            set
            {
                this.pageSetupField = value;
            }
        }

        /// <remarks/>
        public object Selected
        {
            get
            {
                return this.selectedField;
            }
            set
            {
                this.selectedField = value;
            }
        }

        /// <remarks/>
        public string Visible
        {
            get
            {
                return this.visibleField;
            }
            set
            {
                this.visibleField = value;
            }
        }

        /// <remarks/>
        public WorksheetOptionsPanes Panes
        {
            get
            {
                return this.panesField;
            }
            set
            {
                this.panesField = value;
            }
        }

        /// <remarks/>
        public string ProtectObjects
        {
            get
            {
                return this.protectObjectsField;
            }
            set
            {
                this.protectObjectsField = value;
            }
        }

        /// <remarks/>
        public string ProtectScenarios
        {
            get
            {
                return this.protectScenariosField;
            }
            set
            {
                this.protectScenariosField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    public partial class WorksheetOptionsPageSetup
    {

        private WorksheetOptionsPageSetupHeader headerField;

        private WorksheetOptionsPageSetupFooter footerField;

        private WorksheetOptionsPageSetupPageMargins pageMarginsField;

        /// <remarks/>
        public WorksheetOptionsPageSetupHeader Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public WorksheetOptionsPageSetupFooter Footer
        {
            get
            {
                return this.footerField;
            }
            set
            {
                this.footerField = value;
            }
        }

        /// <remarks/>
        public WorksheetOptionsPageSetupPageMargins PageMargins
        {
            get
            {
                return this.pageMarginsField;
            }
            set
            {
                this.pageMarginsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    public partial class WorksheetOptionsPageSetupHeader
    {

        private decimal marginField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Margin
        {
            get
            {
                return this.marginField;
            }
            set
            {
                this.marginField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    public partial class WorksheetOptionsPageSetupFooter
    {

        private decimal marginField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Margin
        {
            get
            {
                return this.marginField;
            }
            set
            {
                this.marginField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    public partial class WorksheetOptionsPageSetupPageMargins
    {

        private decimal bottomField;

        private decimal leftField;

        private decimal rightField;

        private decimal topField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Bottom
        {
            get
            {
                return this.bottomField;
            }
            set
            {
                this.bottomField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Left
        {
            get
            {
                return this.leftField;
            }
            set
            {
                this.leftField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Right
        {
            get
            {
                return this.rightField;
            }
            set
            {
                this.rightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public decimal Top
        {
            get
            {
                return this.topField;
            }
            set
            {
                this.topField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    public partial class WorksheetOptionsPanes
    {

        private WorksheetOptionsPanesPane paneField;

        /// <remarks/>
        public WorksheetOptionsPanesPane Pane
        {
            get
            {
                return this.paneField;
            }
            set
            {
                this.paneField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:schemas-microsoft-com:office:excel")]
    public partial class WorksheetOptionsPanesPane
    {

        private byte numberField;

        private byte activeRowField;

        private bool activeRowFieldSpecified;

        private byte activeColField;

        /// <remarks/>
        public byte Number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        public byte ActiveRow
        {
            get
            {
                return this.activeRowField;
            }
            set
            {
                this.activeRowField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ActiveRowSpecified
        {
            get
            {
                return this.activeRowFieldSpecified;
            }
            set
            {
                this.activeRowFieldSpecified = value;
            }
        }

        /// <remarks/>
        public byte ActiveCol
        {
            get
            {
                return this.activeColField;
            }
            set
            {
                this.activeColField = value;
            }
        }
    }


}