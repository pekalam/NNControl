using System;
using NNControl;
using NNControl.Adapter;
using NNControl.Model;
using NNControl.Network;
using NNControl.Synapse;
using NNLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NNLibAdapter
{
    public class NNLibModelAdapter : INeuralNetworkModelAdapter
    {
        private readonly List<NNLibLayerAdapter> _layerModelAdapters = new List<NNLibLayerAdapter>();
        private NeuralNetworkController _controller;

        public IReadOnlyList<ILayerModelAdapter> LayerModelAdapters => _layerModelAdapters;

        public NeuralNetworkController Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                ColorAnimation = new ColorAnimation(_controller);
            }
        }

        public IReadOnlyList<Layer> Layers { get; private set; }
        public NeuralNetworkModel NeuralNetworkModel { get; private set; }
        public ColorAnimation ColorAnimation { get; private set; }

        public void SetNeuralNetwork<T>(Network<T> network) where T : Layer
        {
            Layers = network.BaseLayers;
            NeuralNetworkModel = new NeuralNetworkModel();
            var inputLayer = Layers[0];
            //Forward layer
            var forwardLayerModel = new LayerModel();
            for (int i = 0; i < inputLayer.InputsCount; i++)
            {
                forwardLayerModel.NeuronModels.Add(new NeuronModel());
            }
            AddLayerModel(forwardLayerModel, null);

            foreach (var layer in network.BaseLayers)
            {
                var layerModel = new LayerModel();

                for (int i = 0; i < layer.NeuronsCount; i++)
                {
                    var neuronModel = new NeuronModel();
                    layerModel.NeuronModels.Add(neuronModel);
                }

                AddLayerModel(layerModel, layer);
            }

            OnPropertyChanged(nameof(NeuralNetworkModel));
        }

        private void AddLayerModel(LayerModel layerModel, Layer layer)
        {
            _layerModelAdapters.Add(new NNLibLayerAdapter(layerModel, layer));
            NeuralNetworkModel.NetworkLayerModels.Add(layerModel);
        }

        public void AddLayer(Layer layer)
        {
            //todo func
            var layerModel = new LayerModel()
            {
                NeuronModels = new ObservableRangeCollection<NeuronModel>()
            };
            for (int i = 0; i < layer.NeuronsCount; i++)
            {
                layerModel.NeuronModels.Add(new NeuronModel());
            }
            _layerModelAdapters.Add(new NNLibLayerAdapter(layerModel, layer));

            var outputLabels = NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels.Select(c => c.Label).ToArray();
            for (int i = 0; i < layerModel.NeuronModels.Count; i++)
            {
                layerModel.NeuronModels[i].Label = outputLabels[i];
            }

            if (_layerModelAdapters.Count > 2)
            {
                foreach (var neuron in NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels)
                {
                    neuron.Label = "";
                }
            }



            NeuralNetworkModel.NetworkLayerModels.Add(layerModel);
        }

        public void InsertAfter(int ind, Layer layer)
        {
            ind++;
            var layerModel = new LayerModel()
            {
                NeuronModels = new ObservableRangeCollection<NeuronModel>()
            };
            for (int i = 0; i < layer.NeuronsCount; i++)
            {
                layerModel.NeuronModels.Add(new NeuronModel());
            }
            _layerModelAdapters.Insert(ind, new NNLibLayerAdapter(layerModel, layer));


            if (ind == 0)
            {
                var inputLabels = NeuralNetworkModel.NetworkLayerModels[0].NeuronModels.Select(c => c.Label).ToArray();
                for (int i = 0; i < layerModel.NeuronModels.Count; i++)
                {
                    layerModel.NeuronModels[i].Label = inputLabels[i];
                }
            }
            else if (ind == _layerModelAdapters.Count - 1)
            {
                var outputLabels = NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels.Select(c => c.Label).ToArray();
                for (int i = 0; i < layerModel.NeuronModels.Count; i++)
                {
                    layerModel.NeuronModels[i].Label = outputLabels[i];
                }
            }

            //todo mv to ctrl
            if (_layerModelAdapters.Count > 2 && ind == _layerModelAdapters.Count - 1)
            {
                foreach (var neuron in NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels)
                {
                    neuron.Label = "";
                }
            }




            NeuralNetworkModel.NetworkLayerModels.Insert(ind,layerModel);
        }

        public void InsertBefore(int ind, Layer layer)
        {
            InsertAfter(ind-1,layer);
        }

        public void RemoveLayer(int layerIndex)
        {
            _layerModelAdapters.RemoveAt(layerIndex + 1);
            NeuralNetworkModel.NetworkLayerModels.RemoveAt(layerIndex + 1);
            if (layerIndex == 0)
            {
                _layerModelAdapters[0].SetNeuronsCount(_layerModelAdapters[1].Layer.InputsCount);
            }
        }

        public void UpdateWeights(NeuralNetworkControl networkControl, string format = "F3")
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                for (int j = 0; j < Layers[i].NeuronsCount; j++)
                {
                    for (int k = 0; k < Layers[i].InputsCount; k++)
                    {
                        NeuralNetworkModel.NetworkLayerModels[i+1].NeuronModels[j].SynapsesLabels[k] =
                            Layers[i].Weights[j, k].ToString(format);
                    }
                }
            }

            networkControl.UpdateSynapseLabels();
        }

        public void SetInputLabels(string[] labels)
        {
            for (int i = 0; i < NeuralNetworkModel.NetworkLayerModels[0].NeuronModels.Count; i++)
            {
                NeuralNetworkModel.NetworkLayerModels[0].NeuronModels[i].Label = labels[i];
            }

            Controller?.Reposition();
        }

        public void SetOutputLabels(string[] labels)
        {
            for (int i = 0; i < NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels.Count; i++)
            {
                NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels[i].Label = labels[i];
            }

            Controller?.Reposition();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}