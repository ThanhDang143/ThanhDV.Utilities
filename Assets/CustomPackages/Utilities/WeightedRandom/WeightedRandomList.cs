using System.Collections.Generic;

namespace ThanhDV.Utilities
{
    [System.Serializable]
    public class WeightedRandomList<T>
    {
        [System.Serializable]
        public struct Pair
        {
            public T item;

            public float weight;

            public Pair(T _item, float _weight)
            {
                item = _item;
                weight = _weight;
            }
        }

        public List<Pair> list = new();
        public int Count => list.Count;


        /// <summary>
        /// Adds a new item with the specified weight.
        /// </summary>
        /// <param name="item">Element to add.</param>
        /// <param name="weight">Weight influencing probability (should be > 0).</param>
        public void Add(T item, float weight)
        {
            list.Add(new Pair(item, weight));
        }

        /// <summary>
        /// Removes the first occurrence of an item.
        /// </summary>
        /// <param name="item">Element to remove.</param>
        /// <returns>True if the item was found and removed.</returns>
        public bool Remove(T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(list[i].item, item))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all entries.
        /// </summary>
        public void Clear() => list.Clear();

        /// <summary>
        /// Attempts to set the weight of the first occurrence of an item.
        /// </summary>
        /// <param name="item">Element to update.</param>
        /// <param name="newWeight">New weight value.</param>
        /// <returns>True if the item was found and updated.</returns>
        public bool TrySetWeight(T item, float newWeight)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(list[i].item, item))
                {
                    list[i] = new Pair(list[i].item, newWeight);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a random item based on weights. If the list is empty returns default(T).
        /// </summary>
        /// <remarks>
        /// Probability of each item = item.weight / totalWeight.
        /// </remarks>
        public T Random()
        {
            float totalWeight = 0;

            foreach (Pair p in list)
            {
                totalWeight += p.weight;
            }

            float value = UnityEngine.Random.value * totalWeight;

            float sumWeight = 0;

            foreach (Pair p in list)
            {
                sumWeight += p.weight;

                if (sumWeight >= value)
                {
                    return p.item;
                }
            }

            return default;
        }
    }
}