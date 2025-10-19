using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ITCafe.CafeBusiness;
using ITCafe.Solutions;
using R3;
using UnityEngine;
using VContainer;

namespace ITCafe
{
    public class CafeRunner
    {
        private readonly IFactory<ClientCharacter> _clientsFactory;
        private readonly TableService _tableService;
        private readonly Dictionary<Transform, bool> _orderAvailabilityMap;

        public CafeRunner(
            IFactory<ClientCharacter> clientsFactory,
            TableService tableService,
            [Key(Constants.CLIENT_ORDER_PLACES)] IEnumerable<Transform> clientOrderPoints)
        {
            _clientsFactory = clientsFactory;
            _tableService = tableService;
            _orderAvailabilityMap = clientOrderPoints.ToDictionary(x => x, _ => true);
        }

        public async UniTaskVoid RunClientsLifeCycle(CancellationToken token)
        {
            Debug.Log("[CafeRunner] Running clients lifecycle");
            try
            {
                await UniTask.Delay(1000, cancellationToken: token);
            }
            catch
            {
            }

            while (!token.IsCancellationRequested)
            {
                if (_tableService.HasFreeTable)
                {
                    foreach (var (orderTransform, isAvailable) in _orderAvailabilityMap)
                    {
                        if (isAvailable)
                        {
                            var client = _clientsFactory.Create();
                            client.transform.SetPositionAndRotation(orderTransform.position,
                                orderTransform.rotation);
                            _orderAvailabilityMap[orderTransform] = false;
                            // TODO: Watch out for client subscription this time
                            Observable.Merge(client.OnLeft, client.OnOrdered)
                                .Take(1)
                                .Subscribe(x => _orderAvailabilityMap[orderTransform] = true);
                            Debug.Log("[CafeRunner] New Client");
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("[CafeRunner] No free table available");
                }

                try
                {
                    await UniTask.Delay(2500, cancellationToken: token);
                }
                catch
                {
                }
            }
        }
    }
}