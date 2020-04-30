using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Documents;
using NeuralNetworkControl.Impl;
using NeuralNetworkControl.Model;

namespace NeuralNetworkControl.Abstraction
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

    public class SynapsesLabelsCollection : List<string>
    {
        private readonly LayerViewImpl _layerViewImpl;

        public SynapsesLabelsCollection(LayerViewImpl layerViewImpl)
        {
            _layerViewImpl = layerViewImpl;
        }

        public new void Clear()
        {
            throw new InvalidOperationException();
        }

        public new void Insert(int index, string item)
        {
            if (_layerViewImpl.Neurons.Sum(impl => impl.Synapses.Count) == Count)
            {
                throw new InvalidOperationException();
            }
            base.Insert(index, item);
        }

        public new void RemoveAt(int index)
        {
            if (_layerViewImpl.Neurons.Sum(impl => impl.Synapses.Count) <= Count)
            {
                throw new InvalidOperationException();
            }
            base.RemoveAt(index);
        }
    }
}