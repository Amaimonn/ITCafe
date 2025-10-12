using System.Collections.Generic;
using ITCafe.Data.Items;
using ITCafe.Solutions;
using Object = UnityEngine.Object;

namespace ITCafe.CafeBusiness
{
    public class ClientsFactory : IFactory<ClientCharacter>
    {
        private readonly IEnumerable<ItemInfoSO> _itemConfigs;
        private readonly ClientCharacter _clientPrefab;

        public ClientsFactory(IEnumerable<ItemInfoSO> itemConfigs, ClientCharacter clientPrefab)
        {
            _itemConfigs = itemConfigs;
            _clientPrefab = clientPrefab;
        }

        public ClientCharacter Create()
        {
            return Object.Instantiate(_clientPrefab);
        }
    }
}