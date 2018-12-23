using System;
using System.Collections.Generic;
using System.Reflection;

namespace ComLab
{

    public sealed class Messenger
    {
      
        private static Messenger _defaultMessenger;

        public static Messenger Default => _defaultMessenger ?? (_defaultMessenger = new Messenger());

        private readonly MessageToActionMap _messageToActionMap = new MessageToActionMap();


        private void AddListener(Messages message, Delegate callback, Type parameterType)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            _messageToActionMap.AddAction(message, callback.Target, callback.Method, parameterType);
        }

        public void AddListener<T>(Messages message, Action<T> callback)
        {
            AddListener(message, callback, typeof(T));
        }

        public void AddListener(Messages message, Action callback)
        {
            AddListener(message, callback, null);
        }

        public void Broadcast(Messages message, object parameter)
        {
                var actions = _messageToActionMap.GetActions(message);
                actions?.ForEach(a =>
                {
                    Core.Post(() =>
                        {
                    try
                    {
                        
                            a.DynamicInvoke(parameter);
                        
                    }
                    catch (Exception)
                    {
                        try
                        {
                                a.DynamicInvoke();
                        }
                        catch (Exception)
                        {
                            //
                        }
                    }
                    });
                });
        }

        public void Broadcast(Messages message)
        {
                var actions = _messageToActionMap.GetActions(message);
                actions?.ForEach(a =>
                {
                    try
                    {
                        Core.Post(() =>a.DynamicInvoke());
                    }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
                    catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                    {
                        try
                        {
                            Core.Post(()=>a.DynamicInvoke(null));
                        }
                        catch (Exception ex)
                        {
                            ex.Log("Messenger.Broadcast|83");
                        }
                    }

                });
        }

        private class MessageToActionMap
        {
            private readonly Dictionary<Messages, List<WeakAction>> _map = new Dictionary<Messages, List<WeakAction>>();

            internal void AddAction(Messages message, object target, MethodInfo method, Type actionType)
            {
                if (method == null)
                    throw new ArgumentNullException(nameof(method));
                lock (_map)
                {
                    if (!_map.ContainsKey(message))
                        _map.Add(message, new List<WeakAction>());
                    _map[message].Add(new WeakAction(target, method, actionType));
                }
            }

            internal List<Delegate> GetActions(Messages message)
            {
                List<Delegate> actions;
                lock (_map)
                {
                    if (!_map.ContainsKey(message)) return new List<Delegate>();
                    var weakActions = _map[message];
                    actions = new List<Delegate>(weakActions.Count);
                    for (var i = weakActions.Count - 1; i >= 0; i--)
                    {
                        var weakAction = weakActions[i];
                        if (weakAction == null)
                            continue;
                        var action = weakAction.CreateAction();
                        if (action != null)
                        {
                            actions.Add(action);
                        }
                        else
                        {
                            weakActions.Remove(weakAction);
                        }
                    }

                    if (weakActions.Count == 0)
                        _map.Remove(message);
                }

                actions.Reverse();
                return actions;
            }
        }

        private class WeakAction
        {
            private readonly Type _delegateType;
            private readonly MethodInfo _method;
            private readonly WeakReference _targetReference;

            internal WeakAction(object target, MethodInfo method, Type parameterType)
            {
                _targetReference = target == null ? null : new WeakReference(target);
                _method = method;
                _delegateType = parameterType == null
                    ? typeof(Action)
                    : typeof(Action<>).MakeGenericType(parameterType);
            }

            internal Delegate CreateAction()
            {
                if (_targetReference == null)
                {
                    return Delegate.CreateDelegate(_delegateType, _method);
                }
                try
                {
                    var target = _targetReference.Target;
                    if (target != null)
                        return Delegate.CreateDelegate(_delegateType, target, _method);
                }
                catch (Exception)
                {
                    //ignored
                }
                return null;
            }
        }
    }
}