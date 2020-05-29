using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Utilities
{
    public static class ColorUtils
    {
        public static Color TryConvertFromOleToArgb(this int item)
        {
            try
            {
                var intColor = Convert.ToInt32(item);
                return ColorTranslator.FromOle(intColor);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error cant convert Ole color to ARGB: " + e.Message);
            }

            return Color.Black;
        }
    }
}
