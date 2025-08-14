using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace ThanhDV.Utilities
{
    public class EventDispatcher : Singleton<EventDispatcher>
    {
        // Cache key with event has no data
        private static readonly ConcurrentDictionary<Type, string> _noDataKeys = new ConcurrentDictionary<Type, string>();
        private readonly Dictionary<object, Delegate> _delegates = new Dictionary<object, Delegate>();

        #region Event With Data
        /// <summary>
        /// Registers a listener for an event of type T.
        /// </summary>
        /// <param name="delegator">The handler to be called when the event occurs.</param>
        /// <typeparam name="T">The type of the event parameter.</typeparam>
        public void Register<T>(Action<T> delegator)
        {
            var type = typeof(T);
            if (_delegates.TryGetValue(type, out var existingDelegate))
            {
                _delegates[type] = (Action<T>)existingDelegate + delegator;
            }
            else
            {
                _delegates[type] = delegator;
            }
        }

        /// <summary>
        /// Unregisters a listener from an event of type T.
        /// </summary>
        /// <param name="delegator">The handler that was previously registered.</param>
        /// <typeparam name="T">The type of the event parameter.</typeparam>
        public void Unregister<T>(Action<T> delegator)
        {
            var type = typeof(T);
            if (_delegates.TryGetValue(type, out var existingDelegate))
            {
                var newDelegate = (Action<T>)existingDelegate - delegator;
                if (newDelegate == null)
                {
                    _delegates.Remove(type);
                }
                else
                {
                    _delegates[type] = newDelegate;
                }
            }
        }

        /// <summary>
        /// Posts an event to all registered listeners.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <typeparam name="T">The type of the event parameter.</typeparam>
        public void Post<T>(T eventData)
        {
            var type = typeof(T);
            if (_delegates.TryGetValue(type, out var existingDelegate))
            {
                try
                {
                    (existingDelegate as Action<T>)?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        #endregion

        #region Event Without Data
        /// <summary>
        /// Registers a listener for an event without data.
        /// </summary>
        /// <param name="delegator">The handler to be called when the event occurs.</param>
        /// <typeparam name="T">The type used as event identifier.</typeparam>
        public void Register<T>(Action delegator)
        {
            var type = typeof(T);
            var key = _noDataKeys.GetOrAdd(type, t => $"{t.FullName}_NoData"); // Cache key

            if (_delegates.TryGetValue(key, out var existingDelegate))
            {
                _delegates[key] = (Action)existingDelegate + delegator;
            }
            else
            {
                _delegates[key] = delegator;
            }
        }

        /// <summary>
        /// Unregisters a listener from an event without data.
        /// </summary>
        /// <param name="delegator">The handler that was previously registered.</param>
        /// <typeparam name="T">The type used as event identifier.</typeparam>
        public void Unregister<T>(Action delegator)
        {
            var type = typeof(T);
            var key = _noDataKeys.GetOrAdd(type, t => $"{t.FullName}_NoData"); // Cache key

            if (_delegates.TryGetValue(key, out var existingDelegate))
            {
                var newDelegate = (Action)existingDelegate - delegator;
                if (newDelegate == null)
                {
                    _delegates.Remove(key);
                }
                else
                {
                    _delegates[key] = newDelegate;
                }
            }
        }

        /// <summary>
        /// Posts an event without data to all registered listeners.
        /// </summary>
        /// <typeparam name="T">The type used as event identifier.</typeparam>
        public void Post<T>()
        {
            var type = typeof(T);
            var key = _noDataKeys.GetOrAdd(type, t => $"{t.FullName}_NoData"); // Cache key

            if (_delegates.TryGetValue(key, out var existingDelegate))
            {
                try
                {
                    (existingDelegate as Action)?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        #endregion
    }
}
