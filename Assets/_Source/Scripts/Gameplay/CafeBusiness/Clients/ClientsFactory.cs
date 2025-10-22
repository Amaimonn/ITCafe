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
        private readonly IReadOnlyList<ClientCharacter> _clientPrefabs;
        private readonly OrderGenerator _orderGenerator;
        private readonly TableService _tableService;
        private readonly int _clientPrefabsAmount;

        public ClientsFactory(Dictionary<int, ItemInfoSO> itemInfoMap, 
            IReadOnlyList<ClientCharacter> clientPrefabs,
            OrderGenerator orderGenerator, 
            TableService tableService)
        {
            _itemInfoMap = itemInfoMap;
            _clientPrefabs = clientPrefabs;
            _clientPrefabsAmount = _clientPrefabs.Count;
            _orderGenerator = orderGenerator;
            _tableService = tableService;
        }

        public ClientCharacter Create()
        {
            var order = _orderGenerator.Create();
            var randomClient = _clientPrefabs[Random.Range(0, _clientPrefabsAmount)];
            var client = Object.Instantiate(randomClient);
            client.Init(order, _tableService);
            
            var orderUI = client.OrderUI;
            orderUI.Init();
            
            order.PropagateHashes(x => orderUI.AddImage(_itemInfoMap[x].Image, x));
            order.OnHashRemoved.Subscribe(x => orderUI.RemoveImage(x)); // dispose is redundant
            
            return client;
        }
    }
}