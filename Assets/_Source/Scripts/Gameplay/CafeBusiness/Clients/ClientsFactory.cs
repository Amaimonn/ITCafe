using System.Collections.Generic;
using DevKit.Solutions;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using R3;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace ITCafe.CafeBusiness
{
    public class ClientsFactory : IFactory<ClientCharacter>
    {
        private readonly IReadOnlyDictionary<int, ItemInfoSO> _menuItemsHashMap;
        private readonly IReadOnlyList<ClientCharacter> _clientPrefabs;
        private readonly OrderGenerator _orderGenerator;
        private readonly TableService _tableService;
        private readonly int _clientPrefabsAmount;

        public ClientsFactory([Key(Constants.MENU_ITEMS_HASH_MAP)] IReadOnlyDictionary<int, ItemInfoSO> menuItemsHashMap, 
            IReadOnlyList<ClientCharacter> clientPrefabs,
            OrderGenerator orderGenerator, 
            TableService tableService)
        {
            _menuItemsHashMap = menuItemsHashMap;
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

            client.WaitingTimeNormalized.Subscribe(orderUI.SetRemainingTime);
            order.RemainingTimeNormalized.Skip(1).Subscribe(orderUI.SetRemainingTime);
            
            order.PropagateHashes(x => orderUI.AddImage(_menuItemsHashMap[x].Image, x));
            order.OnHashRemoved.Subscribe(x => orderUI.RemoveImage(x)); // dispose is redundant
            
            return client;
        }
    }
}