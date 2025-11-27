using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    /// <summary>
    /// May be used in future with more than 2 item parts in burger.
    /// </summary>
    public class BurgerCombination : ItemCombination
    {
        [SerializeField] private GameObject _bun;
        [SerializeField] private GameObject _patty;

        protected override void Awake()
        {
            _bun.SetActive(false);
            _patty.SetActive(false);
        }
        
        protected override void OnInit()
        {
            if (_partsAmountMap.ContainsKey(ItemTag.BurgerBun))
                _bun.SetActive(true);
            if  (_partsAmountMap.ContainsKey(ItemTag.Patty))
                _patty.SetActive(true);
        }
    }
}