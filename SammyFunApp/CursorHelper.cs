using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SammyFunApp.Utils
{
    public static class CursorHelper
    {

        public class AppCursor
        {
            private AppCursor(string value) { Value = value; }

            public string Value { get; set; }

            public static AppCursor Pen { get { return new AppCursor("Pen"); } }
            public static AppCursor Marker { get { return new AppCursor("Marker"); } }
            public static AppCursor PaintBrush { get { return new AppCursor("PaintBrush"); } }
            public static AppCursor Truck { get { return new AppCursor("Truck"); } }
            public static AppCursor Car { get { return new AppCursor("Car"); } }
            public static AppCursor Boat { get { return new AppCursor("Warning"); } }
            public static AppCursor Plane { get { return new AppCursor("Plane"); } }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static bool DestroyIcon(IntPtr handle);

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public static Bitmap ChangeColor(Bitmap bitmap, Color newColor)
        {
            Color originalColor;
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    originalColor = bitmap.GetPixel(i, j);
                    if (originalColor.A > 150)
                    {
                        newBitmap.SetPixel(i, j, newColor);
                    }
                    else
                    {
                        newBitmap.SetPixel(i, j, originalColor);
                    }
                }
            }

            return newBitmap;
        }

        public static Cursor GetColourCursor(Color color, AppCursor cursor, int size = 3)
        {
            IntPtr Hicon = new IntPtr();

            BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/SammyFunApp;/Images/pen.ico"));

            Bitmap bmp;

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                bmp = new Bitmap(bitmap);
            }

            //using (var tempCurStream = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream($"SammyFunApp.Images.pen.ico"))
            //using (var tempCurStream = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream($"{cursor.Value}.ico"))
            //{
            //var bmp = new Bitmap(System.Drawing.Image.FromStream(tempCurStream), new Size(16 * size, 16 * size));

            var bmpNewColor = ChangeColor(bmp, color);

            // Get an Hicon for myBitmap.
            Hicon = bmpNewColor.GetHicon();

            IconInfo tmp = new IconInfo();
            GetIconInfo(Hicon, ref tmp);
            tmp.xHotspot = 0;
            tmp.yHotspot = bmp.Height;
            tmp.fIcon = false;
            Hicon = CreateIconIndirect(ref tmp);

            return new Cursor(Hicon);
            //}
        }

        public static int GetCursorSize(int penSize)
        {
            return (int)Math.Round(Math.Log(penSize * Math.Sqrt(5)) / Math.Log(1.618));
        }
    }
}
