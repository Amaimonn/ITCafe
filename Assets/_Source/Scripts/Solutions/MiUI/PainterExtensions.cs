using UnityEngine;
using UnityEngine.UIElements;

namespace MiUI
{
    public static class PainterExtensions
    {
        public static void DrawLine(this Painter2D painter, Vector2 moveTo, Vector2 lineTo)
        {
            painter.BeginPath();
            painter.MoveTo(moveTo);
            painter.LineTo(lineTo);
            painter.Stroke();
        }

        public static void DrawDot(this Painter2D painter, Vector2 moveTo, float radius)
        {
            painter.BeginPath();
            painter.Arc(moveTo, radius, 0, 360);
            painter.Fill();
        }
    }
}