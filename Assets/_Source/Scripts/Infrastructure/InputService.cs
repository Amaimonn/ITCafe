using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITCafe
{
    public class InputService
    {
        // private readonly Dictionary<InputAction, bool> _isPropagatingMap = new();

        // private readonly Dictionary<InputAction, double> _lastReadMap = new();
        private readonly Dictionary<int, List<InputEntry>> _subscribtionsMap = new();

        // private readonly
        //     Dictionary<Action<InputAction.CallbackContext>, List<(Action<InputAction.CallbackContext>, int)>>
        //     _actionsMap = new();

        // public void StopPropagating(InputAction actionRef)
        // {
        //     _isPropagatingMap[actionRef] = false;
        // }
        public IDisposable MakeOrderedSub(int inputId, InputEntry entry)
        {
            return MakeOrderedSub(inputId, entry.Sub, entry.Unsub, entry.Order);
        }
        
        public IDisposable MakeOrderedSub(int inputId, Action sub, Action unsub, int order = int.MaxValue)
        {
            var registeredEntry = new InputEntry(sub, unsub, order);
            if (!_subscribtionsMap.TryGetValue(inputId, out var subsList))
            {
                subsList = new List<InputEntry> { registeredEntry };
                _subscribtionsMap[inputId] = subsList;
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

        // public IDisposable SubscribeOrdered(Action<InputAction.CallbackContext> systemAction,
        //     Action<InputAction.CallbackContext> callback,
        //     int order = int.MaxValue) // Action<InputAction.CallbackContext> systemAction
        // {
        //     if (!_actionsMap.TryGetValue(systemAction, out var list))
        //     {
        //         list = new List<(Action<InputAction.CallbackContext>, int)>();
        //         _actionsMap.Add(systemAction, list);

        //         systemAction += ExecuteCallbacks;
        //     }

        //     var entry = (callback, order);
        //     list.Add((callback, order));
        //     list.Sort((a, b) => -a.Item2.CompareTo(b.Item2));

        //     return Disposable.Create(() =>
        //     {
        //         list.Remove(entry);
        //         if (list.Count == 0)
        //             systemAction -= ExecuteCallbacks;
        //     });

        //     void ExecuteCallbacks(InputAction.CallbackContext context)
        //     {
        //         var actions = _actionsMap[systemAction];
        //         foreach (var (action, _) in actions)
        //         {
        //             action(context);
        //         }
        //     }
        // }


        // public Action<InputAction.CallbackContext> MediateAction(InputAction inputAction,
        //     Action<InputAction.CallbackContext> callback)
        // {
        //     _isPropagatingMap[inputAction] = true;
        //     _lastReadMap[inputAction] = 0d;

        //     return (x) =>
        //     {
        //         if (x.time != _lastReadMap[inputAction])
        //         {
        //             _isPropagatingMap[inputAction] = true;
        //             _lastReadMap[inputAction] = x.time;
        //         }

        //         if (_isPropagatingMap[inputAction])
        //             callback(x);
        //     };
        // }
    }
}