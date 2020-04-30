using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NNControl;
using NNControl.Adapter;
using NNControl.Model;
using NNControl.Synapse;
using NNLib;

namespace NNLibAdapter
{
    public class NeuralNetworkModelAdapter : INeuralNetworkModelAdapter
    {
        private readonly List<LayerModelAdapter> _layerModelAdapters = new List<LayerModelAdapter>();

        public IReadOnlyList<ILayerModelAdapter> LayerModelAdapters => _layerModelAdapters;
        public IReadOnlyList<Layer> Layers { get; private set; }
        public NeuralNetworkModel NeuralNetworkModel { get; private set; }

        public void SetNeuralNetwork<T>(Network<T> network) where T : Layer
        {
            Layers = network.ReadBaseLayers;
            NeuralNetworkModel = new NeuralNetworkModel();
            var inputLayer = Layers[0];
            //Forward layer
            var forwardLayerModel = new LayerModel();
            for (int i = 0; i < inputLayer.InputsCount; i++)
            {
                forwardLayerModel.NeuronModels.Add(new NeuronModel());
            }
            AddLayerModel(forwardLayerModel, null);

            foreach (var layer in network.ReadBaseLayers)
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
            _layerModelAdapters.Add(new LayerModelAdapter(layerModel, layer));
            NeuralNetworkModel.NetworkLayerModels.Add(layerModel);
        }

        public void AddLayer(Layer layer)
        {
            //todo func
            var layerModel = new LayerModel()
            {
                NeuronModels = new ObservableRangeCollection<NeuronModel>()
                {
                    new NeuronModel()
                }
            };
            _layerModelAdapters.Add(new LayerModelAdapter(layerModel, layer));
            NeuralNetworkModel.NetworkLayerModels.Add(layerModel);
        }

        public void UpdateWeights(NeuralNetworkControl networkControl, string format = "F3")
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                for (int j = 0; j < Layers[i].NeuronsCount; j++)
                {
                    for (int k = 0; k < Layers[i].InputsCount; k++)
                    {
                        NeuralNetworkModel.NetworkLayerModels[i].NeuronModels[j].SynapsesLabels[k] =
                            Layers[i].ReadWeights[j, k].ToString(format);
                    }
                }
            }

            networkControl.UpdateSynapseLabels();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}