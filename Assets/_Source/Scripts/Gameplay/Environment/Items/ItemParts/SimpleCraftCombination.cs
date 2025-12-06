using System.Linq;
using DevKit.Utils;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    /// <summary>
    /// Common craft combination implementation.
    /// Reduces the number of unique assets required.
    /// </summary>
    public class SimpleCraftCombination : CraftCombination
    {
        public override ItemTag Tag => ItemTag.SimpleCombination;

        protected override void OnInit()
        {
#if UNITY_EDITOR
            var amount = _partsAmountMap.Sum(kp => kp.Value);
            FLogger.Log<SimpleCraftCombination>(amount.ToString());
#endif
        }
    }
}