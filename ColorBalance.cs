using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Image_Inspector
{
    static class ColorBalance
    {
        public static Bitmap Balance(this Bitmap sourceBitmap, byte redLevel, byte greenLevel, byte blueLevel)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            /* One pixel data is stored in 4 bytes (BGRA)
             * [0] = blue
             * [1] = green
             * [2] = red
             * [3] = alpha
             */
            for (int i = 0; i < pixelBuffer.Length; i += 4)
            {
                float blue = 255f / blueLevel * pixelBuffer[i];
                float green = 255f / greenLevel * pixelBuffer[i + 1];
                float red = 255f / redLevel * pixelBuffer[i + 2];

                blue = Clamp(blue, 0, 255);
                green = Clamp(green, 0, 255);
                red = Clamp(red, 0, 255);

                pixelBuffer[i] = (byte)blue;
                pixelBuffer[i + 1] = (byte)green;
                pixelBuffer[i + 2] = (byte)red;
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0,0,resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        static float Clamp(float value, float min, float max)
        {
            if (value > max) return max;
            else if (value < min) return min;
            else return value;
        }
    }
}
