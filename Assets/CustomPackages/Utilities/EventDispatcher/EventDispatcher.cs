using System;
using System.Collections.Concurrent;
using ThanhDV.Utilities.DebugExtensions;
using UnityEngine;

namespace ThanhDV.Utilities.EventDispatcher
{
    public class EventDispatcher
    {
        public delegate void Delegator(object data);

        private static readonly ConcurrentDictionary<string, Delegator> _maps = new ConcurrentDictionary<string, Delegator>();

        public EventDispatcher() { }

        /// <summary>
        /// Registers a delegate to a specific subject.
        /// </summary>
        public static void Register(string subject, Delegator delegator)
        {
            if (delegator == null) return;

            _maps.AddOrUpdate(subject, delegator, (key, existing) => existing + delegator);
        }

        /// <summary>
        /// Unregisters a delegate from a specific subject.
        /// </summary>
        public static void Unregister(string subject, Delegator delegator)
        {
            if (delegator == null || !_maps.ContainsKey(subject)) return;

            _maps.AddOrUpdate(subject, null, (key, existing) =>
            {
                existing -= delegator;
                return existing == null ? null : existing;
            });

            // Remove the subject if no delegates are left
            if (_maps.TryGetValue(subject, out var remaining) && remaining == null)
            {
                _maps.TryRemove(subject, out _);
            }
        }

        /// <summary>
        /// Posts an event to all delegates registered under the specified subject.
        /// </summary>
        public static void Post(string subject, object data = null)
        {
            if (_maps.TryGetValue(subject, out var map) && map != null)
            {
                try
                {
                    map.Invoke(data);
                }
                catch (Exception e)
                {
                    DebugExtension.Log(e.Message, Color.red);
                }
            }
        }
    }
}
