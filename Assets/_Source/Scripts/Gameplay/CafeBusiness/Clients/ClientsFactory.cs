using System.Collections.Generic;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using ITCafe.Solutions;
using R3;
using Object = UnityEngine.Object;

namespace ITCafe.CafeBusiness
{
    public class ClientsFactory : IFactory<ClientCharacter>
    {
        private readonly Dictionary<int, ItemInfoSO> _itemInfoMap;
        private readonly ClientCharacter _clientPrefab;
        private readonly OrderGenerator _orderGenerator;

        public ClientsFactory(Dictionary<int, ItemInfoSO> itemInfoMap, ClientCharacter clientPrefab,
            OrderGenerator orderGenerator)
        {
            _itemInfoMap = itemInfoMap;
            _clientPrefab = clientPrefab;
            _orderGenerator = orderGenerator;
        }

        public ClientCharacter Create()
        {
            var order = _orderGenerator.CreateOrder();
            var client = Object.Instantiate(_clientPrefab);
            client.Init(order);
            
            var orderUI = client.OrderUI;
            orderUI.Init();
            
            order.PropagateHashes(x => orderUI.AddImage(_itemInfoMap[x].Image, x));
            order.OnHashRemoved.Subscribe(x => orderUI.RemoveImage(x)); // отписка не требуется
            
            return client;
        }
    }
}