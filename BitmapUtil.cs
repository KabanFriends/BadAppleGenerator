using System.Drawing;

class BitmapUtil
{
    /// <summary>
    /// Bitmap画像データのリサイズ
    /// </summary>
    /// <param name="original">元のBitmapクラスオブジェクト</param>
    /// <param name="width">リサイズ後の幅</param>
    /// <param name="height">リサイズ後の高さ</param>
    /// <param name="interpolationMode">補間モード</param>
    /// <returns>リサイズされたBitmap</returns>
    public static Bitmap ResizeBitmap(Bitmap original, int width, int height, System.Drawing.Drawing2D.InterpolationMode interpolationMode)
    {
        Bitmap bmpResize;
        Bitmap bmpResizeColor;
        Graphics graphics = null;

        try
        {
            System.Drawing.Imaging.PixelFormat pf = original.PixelFormat;

            if (original.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                // モノクロの時は仮に24bitとする
                pf = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            }

            bmpResizeColor = new Bitmap(width, height, pf);
            var dstRect = new RectangleF(0, 0, width, height);
            var srcRect = new RectangleF(-0.5f, -0.5f, original.Width, original.Height);
            graphics = Graphics.FromImage(bmpResizeColor);
            graphics.Clear(Color.Transparent);
            graphics.InterpolationMode = interpolationMode;
            graphics.DrawImage(original, dstRect, srcRect, GraphicsUnit.Pixel);

        }
        finally
        {
            if (graphics != null)
            {
                graphics.Dispose();
            }
        }

        if (original.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
        {
            // モノクロ画像のとき、24bit→8bitへ変換

            // モノクロBitmapを確保
            bmpResize = new Bitmap(
                bmpResizeColor.Width,
                bmpResizeColor.Height,
                System.Drawing.Imaging.PixelFormat.Format8bppIndexed
                );

            var pal = bmpResize.Palette;
            for (int i = 0; i < bmpResize.Palette.Entries.Length; i++)
            {
                pal.Entries[i] = original.Palette.Entries[i];
            }
            bmpResize.Palette = pal;

            // カラー画像のポインタへアクセス
            var bmpDataColor = bmpResizeColor.LockBits(
                    new Rectangle(0, 0, bmpResizeColor.Width, bmpResizeColor.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmpResizeColor.PixelFormat
                    );

            // モノクロ画像のポインタへアクセス
            var bmpDataMono = bmpResize.LockBits(
                    new Rectangle(0, 0, bmpResize.Width, bmpResize.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmpResize.PixelFormat
                    );

            int colorStride = bmpDataColor.Stride;
            int monoStride = bmpDataMono.Stride;

            unsafe
            {
                var pColor = (byte*)bmpDataColor.Scan0;
                var pMono = (byte*)bmpDataMono.Scan0;
                for (int y = 0; y < bmpDataColor.Height; y++)
                {
                    for (int x = 0; x < bmpDataColor.Width; x++)
                    {
                        // R,G,B同じ値のため、Bの値を代表してモノクロデータへ代入
                        pMono[x + y * monoStride] = pColor[x * 3 + y * colorStride];
                    }
                }
            }

            bmpResize.UnlockBits(bmpDataMono);
            bmpResizeColor.UnlockBits(bmpDataColor);

            //　解放
            bmpResizeColor.Dispose();
        }
        else
        {
            // カラー画像のとき
            bmpResize = bmpResizeColor;
        }

        return bmpResize;
    }
}