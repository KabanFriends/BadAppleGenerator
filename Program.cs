using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("BadAppleGenerator - v" + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("A tool to convert Bad Apple!! PV frames into text data");
            Console.WriteLine("Created by KabanFriends | https://github.com/KabanFriends/BadAppleGenerator");
            Console.WriteLine();
            Console.WriteLine(Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName) + " <width> <height> <color amount> <folder path> [<output file>]");
        }
        else
        {
            int width = 0;
            int height = 0;
            int colorAmount = 0;
            string folderPath = "";
            string outputFile = "output.txt";

            try
            {
                width = int.Parse(args[0]);
                height = int.Parse(args[1]);
                colorAmount = int.Parse(args[2]);

                if (args[3].EndsWith("/") || args[3].EndsWith("\\"))
                {
                    folderPath = args[3].Replace("/", "\\");
                }
                else
                {
                    folderPath = args[3] + "\\";
                }

                if (args.Length >= 5)
                {
                    outputFile = args[4];
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("BadAppleGenerator: Invalid argument");
            }

            if (colorAmount < 2)
            {
                Console.WriteLine("BadAppleGenerator: Needs more than one color");
            }
            if (width < 0 || height < 0)
            {
                Console.WriteLine("BadAppleGenerator: Invalid output size");
            }

            int colorDiff = 255 / (colorAmount - 1);
            int colorVal = 0;
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorAmount; i++)
            {
                Color color = Color.FromArgb(colorVal, colorVal, colorVal);
                colors.Add(color);
                colorVal += colorDiff;
            }

            StreamWriter writer = new StreamWriter(outputFile, false, Encoding.UTF8);
            string[] files = Directory.GetFiles(args[3], "*.png", SearchOption.TopDirectoryOnly);

            for (int i = 1; i < files.Length; i++)
            {
                Console.WriteLine("Processing frame " + i);
                Bitmap image = new Bitmap(folderPath + i + ".png");

                if (image.Width != width || image.Height != height) image = BitmapUtil.ResizeBitmap(image, width, height, InterpolationMode.Bilinear);

                for (int j = 0; j < image.Height; j++)
                {
                    for (int k = 0; k < image.Width; k++)
                    {
                        Color pixel = image.GetPixel(k, j);

                        double distance = 100000;
                        Color closestColor = Color.FromArgb(0, 0, 0);

                        foreach (Color c in colors)
                        {
                            double d = ColorUtil.ColorDistance(c, pixel);
                            if (d < distance)
                            {
                                distance = d;
                                closestColor = c;
                            }
                        }

                        int colorIndex = colors.IndexOf(closestColor);
                        writer.Write(colorIndex.ToString());
                    }
                    writer.WriteLine(",");
                }
                writer.WriteLine("#");

                image.Dispose();
            }
            writer.Close();

            Console.WriteLine("Conversion finished.");
        }
    }
}
