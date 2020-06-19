using NNControl.Model;
using NNControl.Network;
using NNControl.Synapse;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NNControl.Adapter
{
    public class DefaultNeuralNetworkModelAdapter : INeuralNetworkModelAdapter
    {
        public DefaultNeuralNetworkModelAdapter(params int[] netsz)
        {
            NeuralNetworkModel = new NeuralNetworkModel();
            NeuralNetworkModel.NetworkLayerModels = new ObservableCollection<LayerModel>();
            foreach (var sz in netsz)
            {
                var l = new LayerModel();
                l.NeuronModels = new ObservableRangeCollection<NeuronModel>();
                for (int i = 0; i < sz; i++)
                {
                    l.NeuronModels.Add(new NeuronModel());
                }
                NeuralNetworkModel.NetworkLayerModels.Add(l);
            }

            var layerAdapters = new List<DefaultLayerModelAdapter>();

            foreach (var layerModel in NeuralNetworkModel.NetworkLayerModels)
            {
                layerAdapters.Add(new DefaultLayerModelAdapter()
                {
                    LayerModel = layerModel,
                });
            }

            LayerModelAdapters = layerAdapters;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public NeuralNetworkModel NeuralNetworkModel { get; }
        public IReadOnlyList<ILayerModelAdapter> LayerModelAdapters { get; }
        public NeuralNetworkController Controller { get; set; }
    }
}