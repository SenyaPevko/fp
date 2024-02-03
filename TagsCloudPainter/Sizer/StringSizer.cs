﻿using System.Drawing;

namespace TagsCloudPainter.Sizer;

public class StringSizer : IStringSizer
{
    public Result<Size> GetStringSize(string value, string fontName, float fontSize)
    {
        using var graphics = Graphics.FromHwnd(IntPtr.Zero);
        using var font = new Font(fontName, fontSize);
        {
            var size = Result.Of(() => graphics.MeasureString(value, font).ToSize());
            return size;
        }
    }
}