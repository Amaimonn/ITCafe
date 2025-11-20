using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITCafe
{
    public class InputService : IDisposable
    {
        private readonly InputActionMap _inputActionMap;

        private readonly Dictionary<int, List<InputEntry>> _subscriptionsMap = new();
        private bool _isInputEnabled;

        public InputService(InputActionMap inputActionMap)
        {
            _inputActionMap = inputActionMap;
            _isInputEnabled = _inputActionMap.enabled;
        }

        public void SetInputEnabled(bool enabled)
        {
            if (_isInputEnabled == enabled)
                return;

            _isInputEnabled = enabled;
            if (_isInputEnabled)
                _inputActionMap.Enable();
            else
                _inputActionMap.Disable();
        }

        public IDisposable MakeOrderedSub(int inputId, InputEntry entry)
        {
            return MakeOrderedSub(inputId, entry.Sub, entry.Unsub, entry.Order);
        }

        public IDisposable MakeOrderedSub(int inputId, Action sub, Action unsub, int order = int.MaxValue)
        {
            var registeredEntry = new InputEntry(sub, unsub, order);
            if (!_subscriptionsMap.TryGetValue(inputId, out var subsList))
            {
                subsList = new List<InputEntry> { registeredEntry };
                _subscriptionsMap[inputId] = subsList;
                sub();
            }
            else
            {
                var listCount = subsList.Count;
                var subscribed = false;
                for (var i = 0; i < listCount; i++)
                {
                    var entry = subsList[i];
                    if (entry.Order > order)
                    {
                        Debug.Log($"{entry.Order} > {order}");
                        for (var j = i; j < listCount; j++)
                        {
                            subsList[j].Unsub();
                            Debug.Log($"{subsList[j].Order} unsub");
                        }

                        subsList.Insert(i, registeredEntry);
                        sub();

                        for (var j = i + 1; j < listCount + 1; j++)
                        {
                            subsList[j].Sub();
                            Debug.Log($"{subsList[j].Order} sub");
                        }

                        subscribed = true;
                        break;
                    }
                }

                if (!subscribed)
                {
                    subsList.Add(registeredEntry);
                    sub();
                }
            }

            return Disposable.Create(() =>
            {
                registeredEntry.Unsub();
                subsList.Remove(registeredEntry);
            });
        }

        public void Dispose()
        {
            foreach (var subsList in _subscriptionsMap.Values)
            {
                if (subsList == null)
                    continue;

                foreach (var entry in subsList)
                    entry.Unsub();
            }
        }
    }
}