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
            Vector2 startPoint, endPoint;

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
            painter.LineTo(endPoint);
            painter.Stroke();
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