using System.Collections.Generic;

namespace ThanhDV.Utilities
{
    public class ManualUpdater
    {
        private HashSet<IManualUpdate> manualUpdates = new();
        private HashSet<IManualUpdate> manualUpdatesWaitToAdd = new();
        private HashSet<IManualUpdate> manualUpdatesWaitToRemove = new();

        public void ManualUpdate()
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

        public void Register(IManualUpdate manualUpdate)
        {
            if (manualUpdate == null) return;

            manualUpdatesWaitToRemove.Remove(manualUpdate);
            manualUpdatesWaitToAdd.Add(manualUpdate);
        }

        public void Unregister(IManualUpdate manualUpdate)
        {
            if (manualUpdate == null) return;

            manualUpdatesWaitToAdd.Remove(manualUpdate);
            manualUpdatesWaitToRemove.Add(manualUpdate);
        }
    }
}

