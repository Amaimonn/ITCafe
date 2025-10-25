using System.Collections.Generic;
using MiUI.MVVM;
using R3;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ObservableCollections;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class HUDViewModel : BaseViewModel
    {
        public Observable<string> TimerText => _timerText;
        public Observable<string> PointsText => _pointsText;
        public ObservableHashSet<IOrder> ActiveOrders { get; } = new();
        public IReadOnlyDictionary<int, ItemInfoSO> ItemInfoMap => _itemInfoMap;

        private readonly Dictionary<int, ItemInfoSO> _itemInfoMap;
        private readonly ReactiveProperty<string> _timerText = new("00:00");
        private readonly ReactiveProperty<string> _pointsText = new("0");

        public HUDViewModel(Dictionary<int, ItemInfoSO> itemInfoMap)
        {
            _itemInfoMap = itemInfoMap;
        }
    }
}