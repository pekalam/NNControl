using System.Collections.Generic;
using System.Collections.Specialized;
using NNControl.Model;
using NNControl.Neuron;
using NNControl.Synapse;

namespace NNControl.Layer
{
    public class LayerViewAbstraction
    {
        internal LayerViewImpl Impl { get; private set; }
        private readonly List<NeuronViewAbstraction> _neurons = new List<NeuronViewAbstraction>();
        internal NNControl.Network.NeuralNetworkViewAbstraction Network { get; private set; }
        internal LayerViewAbstraction PreviousLayer { get; private set; }
        internal LayerViewAbstraction NextLayer { get; set; }

        public LayerViewAbstraction(LayerViewAbstraction previousLayer, int layerNum, LayerViewImpl impl, NNControl.Network.NeuralNetworkViewAbstraction network)
        {
            PreviousLayer = previousLayer;
            Impl = impl;
            Network = network;
            CreateLayer(layerNum);
        }

        public IReadOnlyList<NeuronViewAbstraction> Neurons => _neurons;

        public LayerModel LayerModel
        {
            get => Impl.LayerModel;
            set
            {
                Impl.LayerModel = value;
                for (int i = 0; i < value.NeuronModels.Count; i++)
                {
                    CreateAndAddNewNeuron(i);
                }

                value.NeuronModels.CollectionChanged += NeuronModelsOnCollectionChanged;
            }
        }

        private void CreateLayer(int layerNum)
        {
            Impl.PreviousLayer = Network.Layers.Count > 0 ? Network.Impl.Layers[layerNum - 1] : null;
            var x = (int) Network.PositionManager.GetLayerX(Network, layerNum);
            var y = (int) Network.PositionManager.GetLayerY(Network, layerNum);
            Impl.X = x;
            Impl.Y = y;
            Impl.Number = layerNum;
            Impl.Network = Network.Impl;

            LayerModel = Network.NeuralNetworkModel.NetworkLayerModels[layerNum];
            Impl.OnRepositioned();
        }

        private NeuronViewAbstraction CreateAndAddNewNeuron(int neuronNum)
        {
            var newNeuron = Impl.CreateNeuronInstance();
            var newNeuronAbstr = new NeuronViewAbstraction(_neurons.Count,newNeuron, this);
            LayerModel.NeuronModels[neuronNum].SynapsesLabels = new SynapsesLabelsCollection(Impl);

            if (Impl.PreviousLayer != null)
            {
                foreach (var prevNeuron in PreviousLayer.Neurons)
                {
                    prevNeuron.AddSynapse(newNeuronAbstr);
                }
            }

            Impl.Neurons.Add(newNeuron);
            _neurons.Add(newNeuronAbstr);

            return newNeuronAbstr;
        }

        private void RemoveNeuron(int neuronNum)
        {
            if (PreviousLayer != null)
            {
                foreach (var prevNeuron in PreviousLayer.Neurons)
                {
                    prevNeuron.RemoveSynapseTo(_neurons[neuronNum]);
                }
            }

            if (NextLayer != null)
            {
                foreach (var nextNeuron in NextLayer.Neurons)
                {
                    nextNeuron.RemoveSynapseFrom(_neurons[neuronNum]);
                }
            }
            
            Impl.Neurons.RemoveAt(neuronNum);
            _neurons.RemoveAt(neuronNum);
        }

        private void NeuronModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = e.NewStartingIndex; i < LayerModel.NeuronModels.Count; i++)
                {
                    var newNeuron = CreateAndAddNewNeuron(i);
                    if (NextLayer != null)
                    {
                        foreach (var nextNeuron in NextLayer.Neurons)
                        {
                            newNeuron.AddSynapse(nextNeuron);
                        }
                    }
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveNeuron(e.OldStartingIndex);
            }

        }

        public void Reposition()
        {
            Impl.X = Network.PositionManager.GetLayerX(Network, Impl.Number);
            Impl.Y = Network.PositionManager.GetLayerY(Network, Impl.Number);
            foreach (var neuron in _neurons)
            {
                neuron.Reposition();
            }
            Impl.OnRepositioned();
        }

        public void OnZoomChanged()
        {
            foreach (var neuron in Neurons)
            {
                neuron.OnZoomChanged();
            }
            Impl.OnZoomChanged();
        }
    }
}