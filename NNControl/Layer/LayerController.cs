﻿using NNControl.Model;
using NNControl.Network;
using NNControl.Neuron;
using NNControl.Synapse;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace NNControl.Layer
{
    public class LayerController
    {
        internal LayerView View { get; private set; }
        private readonly List<NeuronController> _neurons = new List<NeuronController>();
        internal NeuralNetworkController Network { get; private set; }
        internal LayerController PreviousLayer { get; set; }
        internal LayerController NextLayer { get; set; }

        public LayerController(LayerController previousLayer, int layerNum, LayerView view,
            NeuralNetworkController network)
        {
            PreviousLayer = previousLayer;
            View = view;
            Network = network;
            CreateLayer(layerNum);
        }

        public IReadOnlyList<NeuronController> Neurons => _neurons;

        public LayerModel LayerModel
        {
            get => View.LayerModel;
            set
            {
                View.LayerModel = value;
                for (int i = 0; i < value.NeuronModels.Count; i++)
                {
                    CreateAndAddNewNeuron(i);
                }

                value.NeuronModels.CollectionChanged += NeuronModelsOnCollectionChanged;
            }
        }

        private void CreateLayer(int layerNum)
        {
            View.PreviousLayer = Network.Layers.Count > 0 ? Network.View.Layers[layerNum - 1] : null;
            var x = (int) Network.PositionManager.GetLayerX(Network, layerNum);
            var y = (int) Network.PositionManager.GetLayerY(Network, layerNum);
            View.X = x;
            View.Y = y;
            View.Number = layerNum;
            View.Network = Network.View;

            LayerModel = Network.NeuralNetworkModel.NetworkLayerModels[layerNum];
            View.OnRepositioned();
        }

        private NeuronController CreateAndAddNewNeuron(int neuronNum)
        {
            var newNeuron = View.CreateNeuronInstance();
            var newNeuronAbstr = new NeuronController(_neurons.Count, newNeuron, this);
            LayerModel.NeuronModels[neuronNum].SynapsesLabels = new SynapsesLabelsCollection(View);

            if (View.PreviousLayer != null)
            {
                foreach (var prevNeuron in PreviousLayer.Neurons)
                {
                    prevNeuron.AddSynapse(newNeuronAbstr);
                }
            }

            View.Neurons.Add(newNeuron);
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

            View.Neurons.RemoveAt(neuronNum);
            _neurons.RemoveAt(neuronNum);
        }

        public void RemoveNeurons()
        {
            for (int i = 0; i < Neurons.Count; i++)
            {
                RemoveNeuron(i);
            }
        }

        public void RemoveSynapsesFromPrevious()
        {
            foreach (var neuron in Neurons)
            {
                foreach (var prevNeuron in PreviousLayer.Neurons)
                {
                    neuron.RemoveSynapseFrom(prevNeuron);
                }
            }
        }

        public void AddSynapsesToPrevious()
        {
            if (PreviousLayer == null)
            {
                return;
            }
            foreach (var neuron in Neurons)
            {
                foreach (var prevNeuron in PreviousLayer.Neurons)
                {
                    prevNeuron.AddSynapse(neuron);
                }
            }
        }

        public void AddSynapsesToNext(LayerController next)
        {
            foreach (var neuron in Neurons)
            {
                foreach (var nextNeuron in next.Neurons)
                {
                    neuron.AddSynapse(nextNeuron);
                }
            }
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
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                RemoveNeuron(e.OldStartingIndex);
            }
        }

        public void Reposition()
        {
            View.X = Network.PositionManager.GetLayerX(Network, View.Number);
            View.Y = Network.PositionManager.GetLayerY(Network, View.Number);
            foreach (var neuron in _neurons)
            {
                neuron.Reposition();
            }

            View.OnRepositioned();
        }

        public void OnZoomChanged()
        {
            foreach (var neuron in Neurons)
            {
                neuron.OnZoomChanged();
            }

            View.OnZoomChanged();
        }
    }
}