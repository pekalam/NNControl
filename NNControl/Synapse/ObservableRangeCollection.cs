using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NNControl.Synapse
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> collection)
        {
            var startInd = Items.Count;
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection, startInd));
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            var removedIndexes = new List<int>();
            foreach (var item in collection)
            {
                var ind = Items.IndexOf(item);
                removedIndexes.Add(ind);
                Items.RemoveAt(ind);
            };
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedIndexes));
        }
    }
}