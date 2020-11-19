using NNControl;
using NNControl.Adapter;
using NNControl.Model;
using NNControl.Network;
using NNControl.Synapse;
using NNLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
// ReSharper disable InconsistentNaming

namespace NNLibAdapter
{
    public class NNLibModelAdapter : INeuralNetworkModelAdapter
    {
        private readonly List<NNLibLayerAdapter> _layerModelAdapters = new List<NNLibLayerAdapter>();
        private NeuralNetworkController _controller = null!;
        private INetwork? _network;

        public NNLibModelAdapter(INetwork network)
        {
            SetNeuralNetwork(network);
        }

        public INetwork Network => _network!;

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

        public NeuralNetworkModel NeuralNetworkModel { get; private set; } = null!;
        public ColorAnimation ColorAnimation { get; private set; } = null!;

        public void SetNeuralNetwork(INetwork network)
        {
            void AddLayerModel(LayerModel layerModel, Layer? layer)
            {
                _layerModelAdapters.Add(new NNLibLayerAdapter(layerModel, layer));
                NeuralNetworkModel.NetworkLayerModels.Add(layerModel);
            }

            if (_network != null)
            {
                _network.StructureChanged -= NetworkOnStructureChanged;
            }
            _network = network;
            _network.StructureChanged += NetworkOnStructureChanged;
            NeuralNetworkModel = new NeuralNetworkModel();
            _layerModelAdapters.Clear();

            var inputLayer = network.BaseLayers[0];
            inputLayer.InputsCountChanged += LayerOnInputsCountChanged;
            var inputLayerModel = new LayerModel();
            for (int i = 0; i < inputLayer.InputsCount; i++)
            {
                inputLayerModel.NeuronModels.Add(new NeuronModel());
            }
            AddLayerModel(inputLayerModel, null);

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
        
        private void NetworkOnStructureChanged(INetwork obj)
        {
            if (_layerModelAdapters.Count < obj.TotalLayers + 1)
            {
                if (obj.TotalLayers > 1 && obj.BaseLayers[^2] == _layerModelAdapters[^1].Layer)
                {
                    AddLayer(obj.BaseLayers[^1]);
                    return;
                }

                for (int i = 1; i < _layerModelAdapters.Count; i++)
                {
                    if (_layerModelAdapters[i].Layer == obj.BaseLayers[i])
                    {
                        InsertBefore(i, obj.BaseLayers[i-1]);
                        return;
                    }

                    if (_layerModelAdapters[i].Layer != obj.BaseLayers[i - 1])
                    {
                        InsertAfter(i-1, obj.BaseLayers[i-1]);
                        return;
                    }
                }
            }
            else if (_layerModelAdapters.Count > obj.TotalLayers + 1)
            {
                for (int i = 1; i < obj.TotalLayers + 1; i++)
                {
                    if (_layerModelAdapters[i].Layer != obj.BaseLayers[i-1])
                    {
                        RemoveLayer(i);
                        return;
                    }
                }
                RemoveLayer(_layerModelAdapters.Count - 1);
            }
        }
        private void LayerOnInputsCountChanged(Layer obj)
        {
            _layerModelAdapters[0].SetNeuronsCount(obj.InputsCount);
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
            if (ind == 1)
            {
                _layerModelAdapters[1].Layer!.InputsCountChanged -= LayerOnInputsCountChanged; 
                layer.InputsCountChanged += LayerOnInputsCountChanged;
            }

            InsertAfter(ind-1,layer);
        }

        public void RemoveLayer(int layerIndex)
        {
            _layerModelAdapters.RemoveAt(layerIndex);
            NeuralNetworkModel.NetworkLayerModels.RemoveAt(layerIndex);
            if (layerIndex == 1)
            {
                _layerModelAdapters[1].Layer!.InputsCountChanged += LayerOnInputsCountChanged;
                _layerModelAdapters[0].SetNeuronsCount(_layerModelAdapters[1].Layer!.InputsCount);
            }
        }



        public void UpdateWeights(NeuralNetworkControl networkControl, string format = "F3")
        {
            for (int i = 1; i < _network!.BaseLayers.Count; i++)
            {
                for (int j = 0; j < _network.BaseLayers[i].NeuronsCount; j++)
                {
                    for (int k = 0; k < _network.BaseLayers[i].InputsCount; k++)
                    {
                        NeuralNetworkModel.NetworkLayerModels[i+1].NeuronModels[j].SynapsesLabels[k] =
                            _network.BaseLayers[i].Weights.At(j, k).ToString(format);
                    }
                }
            }

            networkControl.UpdateSynapseLabels();
        }

        public void SetInputLabels(string[] labels)
        {
            if (labels.Length != NeuralNetworkModel.NetworkLayerModels[0].NeuronModels.Count)
            {
                throw new ArgumentException("Invalid input labels length");
            }

            for (int i = 0; i < NeuralNetworkModel.NetworkLayerModels[0].NeuronModels.Count; i++)
            {
                NeuralNetworkModel.NetworkLayerModels[0].NeuronModels[i].Label = labels[i];
            }

        }

        public void SetOutputLabels(string[] labels)
        {
            if (labels.Length != NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels.Count)
            {
                throw new ArgumentException("Invalid output labels length");
            }

            for (int i = 0; i < NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels.Count; i++)
            {
                NeuralNetworkModel.NetworkLayerModels[^1].NeuronModels[i].Label = labels[i];
            }

        }



        public event PropertyChangedEventHandler PropertyChanged = null!;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}