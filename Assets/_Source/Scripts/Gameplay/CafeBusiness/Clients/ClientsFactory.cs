using System.Collections.Generic;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using ITCafe.Solutions;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ITCafe.CafeBusiness
{
    public class ClientsFactory : IFactory<ClientCharacter>
    {
        private readonly Dictionary<int, ItemInfoSO> _itemInfoMap;
        private readonly IList<ClientCharacter> _clientPrefabs;
        private readonly OrderGenerator _orderGenerator;
        private readonly int _clientPrefabsAmount;

        public ClientsFactory(Dictionary<int, ItemInfoSO> itemInfoMap, IList<ClientCharacter> clientPrefabs,
            OrderGenerator orderGenerator)
        {
            _itemInfoMap = itemInfoMap;
            _clientPrefabs = clientPrefabs;
            _clientPrefabsAmount = _clientPrefabs.Count;
            _orderGenerator = orderGenerator;
        }

        public ClientCharacter Create()
        {
            var order = _orderGenerator.Create();
            var randomClient = _clientPrefabs[Random.Range(0, _clientPrefabsAmount)];
            var client = Object.Instantiate(randomClient);
            client.Init(order);
            
            var orderUI = client.OrderUI;
            orderUI.Init();
            
            order.PropagateHashes(x => orderUI.AddImage(_itemInfoMap[x].Image, x));
            order.OnHashRemoved.Subscribe(x => orderUI.RemoveImage(x)); // отписка не требуется
            
            return client;
        }
    }
}