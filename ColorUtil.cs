using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

class ColorUtil
{
    public static double ColorDistance(Color c1, Color c2)
    {
        int sum = 0;

        sum += (int)Math.Pow(c1.R - c2.R, 2);
        sum += (int)Math.Pow(c1.G - c2.B, 2);
        sum += (int)Math.Pow(c1.G - c2.B, 2);

        return Math.Sqrt(sum);
    }
}
