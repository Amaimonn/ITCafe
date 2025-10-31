using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DevKit.Solutions;
using ITCafe.Gameplay.UI.MVVM;
using R3;
using UnityEngine;
using VContainer;

namespace ITCafe.CafeBusiness
{
    public class ClientsRunner : IDisposable
    {
        private readonly IFactory<ClientCharacter> _clientsFactory;
        private readonly TableService _tableService;
        private readonly WorkProgressService _progressService;
        private readonly HUDViewModel _hudViewModel;
        private readonly Dictionary<Transform, bool> _orderAvailabilityMap;
        private CancellationTokenSource _cts;

        public ClientsRunner(
            IFactory<ClientCharacter> clientsFactory,
            TableService tableService,
            WorkProgressService progressService,
            [Key(Constants.CLIENT_ORDER_PLACES)] IEnumerable<Transform> clientOrderPoints,
            HUDViewModel hudViewModel)
        {
            _clientsFactory = clientsFactory;
            _tableService = tableService;
            _progressService = progressService;
            _hudViewModel = hudViewModel;
            _orderAvailabilityMap = clientOrderPoints.ToDictionary(x => x, _ => true);
        }

        public async UniTaskVoid RunClientsLifeCycleAsync(CancellationToken token)
        {
            _cts = new();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, token);
            Debug.Log($"[{nameof(ClientsRunner)}]: Running clients lifecycle");

            try
            {
                await UniTask.Delay(1000, cancellationToken: linkedTokenSource.Token);

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
                                _progressService.RegisterClient(client);

                                // TODO: Watch out for client subscription this time
                                client.OnOrdered.Subscribe(_ => _hudViewModel.AddOrderInfo(client.CurrentOrder));
                                Observable.Merge(client.OnLeft).Take(1).Subscribe(_ =>
                                    _hudViewModel.RemoveOrderInfo(client.CurrentOrder));
                                Observable.Merge(client.OnLeft, client.OnOrdered)
                                    .Take(1)
                                    .Subscribe(x => _orderAvailabilityMap[orderTransform] = true);

                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"[{nameof(ClientsRunner)}]: No free table available");
                    }

                    await UniTask.Delay(2500, cancellationToken: linkedTokenSource.Token);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Debug.Log($"[{nameof(ClientsRunner)}]: Operation cancelled");
            }
            finally
            {
                Debug.Log($"[{nameof(ClientsRunner)}]: Clients lifecycle stopped");
            }
        }

        public void Dispose()
        {
            Disposes.ClearCts(ref _cts);
        }
    }
}