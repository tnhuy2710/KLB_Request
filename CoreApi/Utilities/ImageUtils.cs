using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreApi.Utilities
{
    public static class ImageUtils
    {
        /// <summary>
        /// Auto download and convert it to base64 string.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static async Task<string> GetBase64ImageFromUriAsync(string uri, int maxSize = 300)
        {
            try
            {
                var client = new HttpClient();

                using (var imageStream = await client.GetStreamAsync(uri))
                {
                    if (imageStream != null)
                    {
                        using (var image = new Bitmap(imageStream))
                        {
                            int size = maxSize;       // Width Size to scale down
                            const int quality = 100;    // Image quality from 0 - 100

                            int width, height;

                            // Check Image are smaller than size scale down
                            if (image.Width <= size)
                            {
                                width = image.Width;
                                height = image.Height;
                            }
                            else
                            {
                                if (image.Width > image.Height)
                                {
                                    width = size;
                                    height = Convert.ToInt32(image.Height * size / (double)image.Width);
                                }
                                else
                                {
                                    width = Convert.ToInt32(image.Width * size / (double)image.Height);
                                    height = size;
                                }
                            }

                            var resized = new Bitmap(width, height);
                            using (var graphics = Graphics.FromImage(resized))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.CompositingMode = CompositingMode.SourceCopy;
                                graphics.DrawImage(image, 0, 0, width, height);

                                using (var output = new MemoryStream())
                                {
                                    resized.Save(output, ImageFormat.Jpeg);

                                    // Release memory
                                    resized.Dispose();
                                    imageStream.Dispose();
                                    image.Dispose();
                                    graphics.Dispose();

                                    return Convert.ToBase64String(output.GetBuffer());
                                }
                            }
                        }
                    }
                }



            }
            catch (Exception e)
            {
                var message = "Error at ImageUtils line 73 - Cant convert image to Base64: " + e.Message;
                Debug.WriteLine(message);
            }

            return null;
        }


        /// <summary>
        /// Convert image from Byte Array to Base64 Image.
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string ConvertByteArrayToBase64Image(byte[] imageBytes, int maxSize = 300)
        {
            try
            {
                using (var imageStream = new MemoryStream(imageBytes))
                {
                    using (var image = new Bitmap(imageStream))
                    {
                        int size = maxSize;       // Width Size to scale down
                        const int quality = 100;    // Image quality from 0 - 100

                        int width, height;

                        // Check Image are smaller than size scale down
                        if (image.Width <= size)
                        {
                            width = image.Width;
                            height = image.Height;
                        }
                        else
                        {
                            if (image.Width > image.Height)
                            {
                                width = size;
                                height = Convert.ToInt32(image.Height * size / (double)image.Width);
                            }
                            else
                            {
                                width = Convert.ToInt32(image.Width * size / (double)image.Height);
                                height = size;
                            }
                        }

                        using (var resized = new Bitmap(width, height))
                        {
                            using (var graphics = Graphics.FromImage(resized))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.CompositingMode = CompositingMode.SourceCopy;
                                graphics.DrawImage(image, 0, 0, width, height);

                                using (var output = new MemoryStream())
                                {
                                    resized.Save(output, ImageFormat.Jpeg);
                                    return Convert.ToBase64String(output.GetBuffer());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error cant ConvertByteArrayToBase64Image: " + e.Message);
            }

            return null;
        }
    }
}
