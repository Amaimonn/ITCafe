using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.Custom
{
    [UxmlElement]
    public partial class NodesElement : VisualElement
    {
        [UxmlAttribute]
        public float LineWidth { get; set; } = 2f;

        [UxmlAttribute]
        public Color LineColor { get; set; } = new(1f, 1f, 1f, 0.5f);

        [UxmlAttribute]
        public float CurveStrength { get; set; } = 0.3f;

        [UxmlAttribute]
        public CurveType LineCurveType { get; set; } = CurveType.Quadratic;

        public enum CurveType
        {
            Straight,
            Quadratic,
            Bezier,
            Arc
        }

        private readonly List<PathConnection> _connections = new();

        public NodesElement()
        {
            generateVisualContent += OnGenerateVisualContent;
        }

        public void AddConnection(VisualElement from, VisualElement to)
        {
            if (from == null || to == null)
                return;

            var connection = new PathConnection(from, to);
            _connections.Add(connection);

            MarkDirtyRepaint();
        }

        public void AddConnection(Vector2 fromWorldPos, Vector2 toWorldPos)
        {
            var connection = new PathConnection(fromWorldPos, toWorldPos);
            _connections.Add(connection);
            MarkDirtyRepaint();
        }

        public void ClearConnections()
        {
            _connections.Clear();
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            if (_connections.Count == 0)
                return;

            var painter = context.painter2D;

            painter.lineWidth = LineWidth;
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.strokeColor = LineColor;

            foreach (var connection in _connections)
                DrawConnection(painter, connection);
        }

        private void DrawConnection(Painter2D painter, PathConnection connection)
        {
            Vector2 startPoint;
            Vector2 endPoint;

            if (connection.IsElementBased)
            {
                startPoint = GetElementCenter(connection.FromElement);
                endPoint = GetElementCenter(connection.ToElement);
            }
            else
            {
                startPoint = connection.FromWorldPos;
                endPoint = connection.ToWorldPos;
            }

            startPoint = this.WorldToLocal(startPoint);
            endPoint = this.WorldToLocal(endPoint);

            painter.BeginPath();
            painter.MoveTo(startPoint);

            switch (LineCurveType)
            {
                case CurveType.Straight:
                    painter.LineTo(endPoint);
                    break;
                case CurveType.Quadratic:
                {
                    var controlPoint = CalculateQuadraticControlPoint(startPoint, endPoint, CurveStrength);
                    painter.QuadraticCurveTo(controlPoint, endPoint);
                    break;
                }
                case CurveType.Bezier:
                {
                    CalculateCubicControlPoints(startPoint, endPoint, CurveStrength, out var control1, 
                        out var control2);
                    painter.BezierCurveTo(control1, control2, endPoint);
                    break;
                }
                case CurveType.Arc:
                    DrawArc(painter, startPoint, endPoint, CurveStrength);
                    break;
            }

            painter.Stroke();
        }

        private Vector2 CalculateQuadraticControlPoint(Vector2 start, Vector2 end, float strength)
        {
            var middle = (start + end) * 0.5f;
            var direction = (end - start).normalized;
            var perpendicular = new Vector2(-direction.y, direction.x);

            var distance = Vector2.Distance(start, end);
            return middle + perpendicular * (distance * strength);
        }

        private void CalculateCubicControlPoints(Vector2 start, Vector2 end, float strength,
            out Vector2 control1, out Vector2 control2)
        {
            var distance = Vector2.Distance(start, end);
            var direction = (end - start).normalized;
            var perpendicular = new Vector2(-direction.y, direction.x);

            control1 = start + direction * (distance * 0.3f) + perpendicular * (distance * strength);
            control2 = end - direction * (distance * 0.3f) + perpendicular * (distance * strength);
        }

        private void DrawArc(Painter2D painter, Vector2 start, Vector2 end, float strength)
        {
            var middle = (start + end) * 0.5f;
            var direction = (end - start).normalized;

            var control = middle + new Vector2(-direction.y, direction.x) * Vector2.Distance(start, end) * strength;

            painter.QuadraticCurveTo(control, end);
        }

        private Vector2 GetElementCenter(VisualElement element)
        {
            return new Vector2(element.worldBound.center.x, element.worldBound.center.y);
        }

        private struct PathConnection
        {
            public readonly VisualElement FromElement;
            public readonly VisualElement ToElement;
            public readonly Vector2 FromWorldPos;
            public readonly Vector2 ToWorldPos;
            public readonly bool IsElementBased;

            public PathConnection(VisualElement from, VisualElement to)
            {
                FromElement = from;
                ToElement = to;
                FromWorldPos = Vector2.zero;
                ToWorldPos = Vector2.zero;
                IsElementBased = true;
            }

            public PathConnection(Vector2 from, Vector2 to)
            {
                FromElement = null;
                ToElement = null;
                FromWorldPos = from;
                ToWorldPos = to;
                IsElementBased = false;
            }
        }
    }
}