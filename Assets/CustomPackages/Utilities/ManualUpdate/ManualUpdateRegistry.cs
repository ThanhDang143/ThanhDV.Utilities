using System;
using System.Collections.Generic;

namespace ThanhDV.Utilities
{
    public static class ManualUpdateRegistry
    {
        private static readonly Dictionary<RuntimeTypeHandle, HashSet<IManualUpdate>> pendingAddByKey = new();
        private static readonly Dictionary<RuntimeTypeHandle, HashSet<IManualUpdate>> pendingRemoveByKey = new();
        private static readonly Dictionary<RuntimeTypeHandle, Action<IManualUpdate>> registers = new();
        private static readonly Dictionary<RuntimeTypeHandle, Action<IManualUpdate>> unregisters = new();

        public static void Bind<TMarker>(Action<IManualUpdate> _register, Action<IManualUpdate> _unregister) where TMarker : struct
        {
            if (_register == null) throw new ArgumentNullException(nameof(_register));
            if (_unregister == null) throw new ArgumentNullException(nameof(_unregister));

            var key = typeof(TMarker).TypeHandle;

            // Enforce 1 binder per key. If you need to recreate an updater, call Unbind first.
            if (!registers.TryAdd(key, _register))
            {
                throw new InvalidOperationException($"An updater is already bound for marker '{typeof(TMarker).FullName}'.");
            }

            if (!unregisters.TryAdd(key, _unregister))
            {
                registers.Remove(key);
                throw new InvalidOperationException($"An updater is already bound for marker '{typeof(TMarker).FullName}'.");
            }

            FlushPending(key);
        }

        public static bool Unbind<TMarker>(Action<IManualUpdate> _register, Action<IManualUpdate> _unregister) where TMarker : struct
        {
            if (_register == null || _unregister == null) return false;

            var key = typeof(TMarker).TypeHandle;

            if (!registers.TryGetValue(key, out var reg) || !unregisters.TryGetValue(key, out var unreg)) return false;
            if (!ReferenceEquals(reg, _register) || !ReferenceEquals(unreg, _unregister)) return false;

            var removed = registers.Remove(key);
            removed |= unregisters.Remove(key);
            return removed;
        }

        private static void FlushPending(RuntimeTypeHandle key)
        {
            if (!registers.TryGetValue(key, out var register) || register == null) return;

            if (pendingAddByKey.TryGetValue(key, out var pendingAdd) && pendingAdd.Count > 0)
            {
                foreach (IManualUpdate manualUpdater in pendingAdd)
                {
                    if (manualUpdater == null) continue;
                    register.Invoke(manualUpdater);
                }
            }

            if (unregisters.TryGetValue(key, out var unregister) && unregister != null)
            {
                if (pendingRemoveByKey.TryGetValue(key, out var pendingRemove) && pendingRemove.Count > 0)
                {
                    foreach (IManualUpdate manualUpdater in pendingRemove)
                    {
                        if (manualUpdater == null) continue;
                        unregister.Invoke(manualUpdater);
                    }
                }
            }

            pendingAddByKey.Remove(key);
            pendingRemoveByKey.Remove(key);
        }

        public static void Register<TMarker>(IManualUpdate manualUpdater)
            where TMarker : struct
        {
            if (manualUpdater == null) return;

            var key = typeof(TMarker).TypeHandle;

            if (registers.TryGetValue(key, out var register) && register != null)
            {
                register.Invoke(manualUpdater);
                return;
            }

            // Pending: last call wins
            GetOrCreate(pendingRemoveByKey, key).Remove(manualUpdater);
            GetOrCreate(pendingAddByKey, key).Add(manualUpdater);
        }

        public static void Unregister<TMarker>(IManualUpdate manualUpdater)
            where TMarker : struct
        {
            if (manualUpdater == null) return;

            var key = typeof(TMarker).TypeHandle;

            if (unregisters.TryGetValue(key, out var unregister) && unregister != null)
            {
                unregister.Invoke(manualUpdater);
                return;
            }

            // Pending: last call wins
            GetOrCreate(pendingAddByKey, key).Remove(manualUpdater);
            GetOrCreate(pendingRemoveByKey, key).Add(manualUpdater);
        }

        public static bool ClearPending<TMarker>()
            where TMarker : struct
        {
            var key = typeof(TMarker).TypeHandle;
            var removed = pendingAddByKey.Remove(key);
            removed |= pendingRemoveByKey.Remove(key);
            return removed;
        }

        private static HashSet<IManualUpdate> GetOrCreate(Dictionary<RuntimeTypeHandle, HashSet<IManualUpdate>> dict, RuntimeTypeHandle key)
        {
            if (!dict.TryGetValue(key, out var set))
            {
                set = new HashSet<IManualUpdate>();
                dict[key] = set;
            }

            return set;
        }
    }
}
