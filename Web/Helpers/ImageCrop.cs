using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;

namespace Web.Helpers
{
    static class ImageCrop
    {
        internal enum AnchorPosition { Top, Center, Bottom, Left, Right}

        public static Image Crop(HttpPostedFileBase image, int width, int height, AnchorPosition position)
        {

            var original = Image.FromStream(image.InputStream, true, true);
            if (original.Width<=width && original.Height<=height)
            {
                return original;
            }
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
                        destY = (int)((height - (sourceHeight * nPercent)) / 2);
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
                new Rectangle(destX-1, destY-1, destWidth+2, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidht, sourceHeight),GraphicsUnit.Pixel );

            grPhoto.Dispose();

            return bmPhoto;
        }

        public static Image ImageScale(HttpPostedFileBase image, int? persent, int? pixelWidth, int? pixelHeight)
        {
            var img = Image.FromStream(image.InputStream, false, false);
            int sourceWidth = img.Width;
            int sourceHeight = img.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destWidth = 0;
            int destHeight = 0;
            int destX = 0;
            int destY = 0;

            if (persent != null)
            {
                if (persent == 100)
                {
                    return img;
                }
                double nPersent = ((double) persent/100);
                destWidth = (int) (sourceWidth*nPersent);
                destHeight = (int) (sourceHeight*nPersent);
            }
            else if (pixelWidth != null)
            {
                if (sourceWidth == (int) pixelWidth)
                {
                    return img;
                }
                destWidth = (int) (pixelWidth);
                destHeight = (int) ((destWidth*sourceHeight)/sourceWidth);
                
            }
            else
            {
                if (sourceHeight==(int) (pixelHeight))
                {
                    return img;
                }
                destHeight = (int) pixelHeight;
                destWidth = (int)((destHeight*sourceWidth)/sourceHeight);
            }

            var bmImg = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb);
            bmImg.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            var graphics = Graphics.FromImage(bmImg);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.DrawImage(img, new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),GraphicsUnit.Pixel );

            graphics.Dispose();

            return bmImg;
        }
    }
}