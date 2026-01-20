using System.Collections.Generic;

namespace ThanhDV.Utilities
{
    public class ManualUpdater<TMarker> : System.IDisposable where TMarker : struct
    {
        private readonly HashSet<IManualUpdate> manualUpdates = new();
        private readonly HashSet<IManualUpdate> manualUpdatesWaitToAdd = new();
        private readonly HashSet<IManualUpdate> manualUpdatesWaitToRemove = new();

        public System.Type MarkerType => typeof(TMarker);

        public ManualUpdater()
        {
            ManualUpdateRegistry.Bind<TMarker>(Register, Unregister);
        }

        public void Execute()
        {
            if (manualUpdatesWaitToAdd.Count > 0)
            {
                foreach (var mu in manualUpdatesWaitToAdd)
                {
                    manualUpdates.Add(mu);
                }
                manualUpdatesWaitToAdd.Clear();
            }

            if (manualUpdatesWaitToRemove.Count > 0)
            {
                foreach (var mu in manualUpdatesWaitToRemove)
                {
                    manualUpdates.Remove(mu);
                }
                manualUpdatesWaitToRemove.Clear();
            }

            manualUpdates.RemoveWhere(mu => mu == null || (mu is UnityEngine.Object unityObj && unityObj == null));

            foreach (IManualUpdate mu in manualUpdates)
            {
                mu.ExecuteUpdate();
            }
        }

        private void Register(IManualUpdate manualUpdate)
        {
            if (manualUpdate == null) return;

            manualUpdatesWaitToRemove.Remove(manualUpdate);
            manualUpdatesWaitToAdd.Add(manualUpdate);
        }

        private void Unregister(IManualUpdate manualUpdate)
        {
            if (manualUpdate == null) return;

            manualUpdatesWaitToAdd.Remove(manualUpdate);
            manualUpdatesWaitToRemove.Add(manualUpdate);
        }

        public void Dispose()
        {
            ManualUpdateRegistry.Unbind<TMarker>(Register, Unregister);
        }
    }
}
