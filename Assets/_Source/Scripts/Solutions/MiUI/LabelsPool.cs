using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace MiUI
{
    public class LabelsPool
    {
        public ObjectPool<Label> Pool => _labelsPool;
        private ObjectPool<Label> _labelsPool;

        public LabelsPool()
        {
            InitPool();
        }

        private void InitPool()
        {
            _labelsPool = new ObjectPool<Label>(CreateLabel, OnGetLabel, OnReleaseLabel, OnDestroyLabel,
                true, 50, 200);
        }

        private Label CreateLabel()
        {
            var label = new Label();
            return label;
        }

        private void OnGetLabel(Label label)
        {
            label.style.display = DisplayStyle.Flex;
        }

        private void OnReleaseLabel(Label label)
        {
            label.text = string.Empty;
            label.style.display = DisplayStyle.None;
        }

        private void OnDestroyLabel(Label label)
        {
            label.RemoveFromHierarchy();
        }
    }
}