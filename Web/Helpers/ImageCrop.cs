using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace Web.Helpers
{
    static class ImageCrop
    {
        internal enum AnchorPosition { Top, Center, Bottom, Left, Right}

        public static Image Crop(HttpPostedFileBase image, int width, int height, AnchorPosition position)
        {
            var original = Image.FromStream(image.InputStream, true, true);

            var sourceWidht = original.Width;
            var sourceHeight = original.Height;
            var sourceX = 0;
            var sourceY = 0;
            var destX = 0;
            var destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width/(float)sourceWidht);
            nPercentH = ((float)height/(float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                switch (position)
                {
                    case AnchorPosition.Top:
                        destY = 0;
                        break;
                    case AnchorPosition.Bottom:
                        destY = (int)(height - (sourceHeight * nPercent));
                        break;
                    default:
                        destY = (int)((height - (sourceHeight + nPercent)) / 2);
                        break;
                }
            }
            else
            {
                nPercent = nPercentH;
                switch (position)
                {
                    case AnchorPosition.Left:
                        destX = 0; break;
                    case AnchorPosition.Right: destX = (int)(width - (sourceWidht * nPercent)); break;
                    default:
                        destX = (int)((width - (sourceWidht * nPercent)) / 2); break;
                }
            }
            int destWidth = (int)(sourceWidht * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            var bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            var grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(original, 
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidht, sourceHeight),GraphicsUnit.Pixel );

            grPhoto.Dispose();

            return bmPhoto;
        }
    }
}