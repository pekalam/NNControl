using System;
using System.Collections.Generic;
using System.Linq;
using NNControl.Layer;

namespace NNControl.Synapse
{
    public class SynapsesLabelsCollection : List<string>
    {
        private readonly LayerView _layerView;

        public SynapsesLabelsCollection(LayerView layerView)
        {
            _layerView = layerView;
        }

        public new void Clear()
        {
            throw new InvalidOperationException();
        }

        public new void Insert(int index, string item)
        {
            if (_layerView.Neurons.Sum(impl => impl.Synapses.Count) == Count)
            {
                throw new InvalidOperationException();
            }
            base.Insert(index, item);
        }

        public new void RemoveAt(int index)
        {
            if (_layerView.Neurons.Sum(impl => impl.Synapses.Count) <= Count)
            {
                throw new InvalidOperationException();
            }
            base.RemoveAt(index);
        }
    }
}