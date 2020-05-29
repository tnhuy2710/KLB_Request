using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
namespace CoreApi.MyFolder
{
    public class Excel
    {        
        Application excel = new Application();
        private readonly Worksheet ws;
        private readonly Workbook wb;
        private readonly string URL;
        private readonly string typeOFFILE;
        //public Excel()
        //{

        //}
        public Excel(string fILENAME, int Sheet)
        {                        
            typeOFFILE = ".xlsx";
            URL = @"D:\New folder\Klb_Request1\CoreApi\MyFolder\Excel_test\";
            wb = excel.Workbooks.Add(URL + fILENAME + typeOFFILE);
            ws = (Worksheet)wb.Worksheets[Sheet];
        }

        public async Task<bool> WriteToCell(int i, int j, string _content)
        //public bool WriteToCell(int i, int j, string _content)
        {
            //try
            //{
            //    i++;
            //    j++;
            //    ws.Cells[i, j] = _content;
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi insert Excel", e.InnerException);
            //}
            //return true;
            return await Task.Run(() =>
            {
                if (!String.IsNullOrEmpty(_content))
                {
                    i++;
                    j++;
                    ws.Cells[i, j] = _content;
                    return true;
                }
                return false;
            });
        }

        //public async Task<string> ReadCell(int i, int j)
        public string ReadCell(int i, int j)
        {
            //return await Task.Run(() =>
            //{
            //    if (ws.Cells[i, j].ToString() != null)
            //    {
            //        i++;
            //        j++;
            //        Range a = (Range)ws.Cells[i, j];
            //        if (!String.IsNullOrEmpty(a.Value2.ToString()))
            //        {
            //            return a.Value2.ToString();
            //        }
            //    }
            //    return null;
            //});
            i++;
            j++;
            Range a = (Range)ws.Cells[i, j];
            if (!String.IsNullOrEmpty(a.Value2.ToString()))
            {
                return a.Value2.ToString();
            }
            return null;
        }

        public void Save()
        {
            wb.Save();
        }

        public void SaveAs(string fILENAME)
        {
            wb.SaveAs(URL + fILENAME + typeOFFILE);
        }

        public void SaveAsAnotherFile(string URL)
        {
            wb.SaveAs(URL);
        }

        public void Close()
        {
            wb.Close(false);
            excel.Quit();
        }

        public int total_Column()
        {
            return ws.Columns.Count;
        }
    }
}
