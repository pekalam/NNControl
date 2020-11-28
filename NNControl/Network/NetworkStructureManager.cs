using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NNControl.Layer;
using NNControl.Model;

namespace NNControl.Network
{
    internal class NetworkStructureManager
    {
        internal readonly NeuralNetworkView View;
        internal readonly List<LayerController> LayerControllers = new List<LayerController>();

        private readonly NeuralNetworkController _networkController;

        public NetworkStructureManager(NeuralNetworkView view, NeuralNetworkController networkController)
        {
            View = view;
            _networkController = networkController;
        }

        public void SetNeuralNetworkModel(NeuralNetworkModel model)
        {
            LayerControllers.Clear();
            View.Layers.Clear();

            if (View.NeuralNetworkModel != null)
            {
                View.NeuralNetworkModel.NetworkLayerModels.CollectionChanged -= LayerModelsOnCollectionChanged;
            }

            View.NeuralNetworkModel = model;


            model.NetworkLayerModels.CollectionChanged -= LayerModelsOnCollectionChanged;
            model.NetworkLayerModels.CollectionChanged += LayerModelsOnCollectionChanged;
        }

        private void LayerModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewStartingIndex >= View.Layers.Count)
                {
                    AddNewLayers(e.NewStartingIndex, e.NewItems.Count);
                }
                else
                {
                    InsertNewLayers(e.NewStartingIndex, e.NewItems.Count);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                LayerController next = e.OldStartingIndex < LayerControllers.Count - 1 ? LayerControllers[e.OldStartingIndex + 1] : null;
                next?.RemoveSynapsesFromPrevious();

                LayerController toRemove = LayerControllers[e.OldStartingIndex];
                //toRemove.RemoveSynapsesFromPrevious();
                toRemove.RemoveNeurons();

                View.Layers.RemoveAt(e.OldStartingIndex);
                LayerControllers.RemoveAt(e.OldStartingIndex);

                for (int i = 0; i < LayerControllers.Count; i++)
                {
                    LayerControllers[i].View.Number = i;
                    View.Layers[i].PreviousLayer = i > 0 ? View.Layers[i - 1] : null;
                    LayerControllers[i].PreviousLayer = i > 0 ? LayerControllers[i - 1] : null;
                    LayerControllers[i].NextLayer = i < LayerControllers.Count - 1 ? LayerControllers[i + 1] : null;
                }

                next?.AddSynapsesToPrevious();
            }

            _networkController.Reposition();
        }

        public void CreateNewLayer(int layerNum)
        {
            var newLayerView = View.CreateLayerInstance();
            View.Layers.Add(newLayerView);

            var previousLayer = layerNum == 0 ? null : LayerControllers[layerNum - 1];
            var newAbstractLayer = new LayerController(previousLayer, layerNum, newLayerView, _networkController);
            if (previousLayer != null)
            {
                previousLayer.NextLayer = newAbstractLayer;
            }

            LayerControllers.Add(newAbstractLayer);


            View.NeuralNetworkModel.NetworkLayerModels[layerNum].NeuronModels.CollectionChanged += NeuronModelsOnCollectionChanged;

            foreach (var neuron in newLayerView.Neurons)
            {
                neuron.Number = View.NeuronsCount++;
            }
        }

        public void CreateInsertedLayer(int layerNum)
        {
            var newLayerView = View.CreateLayerInstance();
            View.Layers.Insert(layerNum,newLayerView);

            int i = 0;
            foreach (var layer in View.Layers)
            {
                layer.Number = i++;
            }

            var previousLayer = layerNum == 0 ? null : LayerControllers[layerNum - 1];
            var nextLayer = layerNum >= LayerControllers.Count ? null : LayerControllers[layerNum];

            if (previousLayer != null && nextLayer != null)
            {
                previousLayer.RemoveSynapsesTo(nextLayer);
            }

            if (layerNum == 0)
            {
                nextLayer.View.PreviousLayer = newLayerView;
            }



            var newAbstractLayer = new LayerController(previousLayer, layerNum, newLayerView, _networkController, nextLayer);
            if (previousLayer != null)
            {
                previousLayer.NextLayer = newAbstractLayer;
            }

            if (nextLayer != null)
            {
                nextLayer.PreviousLayer = newAbstractLayer;
            }

            LayerControllers.Insert(layerNum,newAbstractLayer);


            View.NeuralNetworkModel.NetworkLayerModels[layerNum].NeuronModels.CollectionChanged += NeuronModelsOnCollectionChanged;

            ResetNeuronsNumbers();
        }

        public void InsertNewLayers(int collectionStartingIndex, int count)
        {
            for (int i = collectionStartingIndex; i < collectionStartingIndex + count; i++)
            {
                CreateInsertedLayer(i);
            }
        }

        public void AddNewLayers(int collectionStartingIndex, int count)
        {
            for (int i = collectionStartingIndex; i < collectionStartingIndex + count; i++)
            {
                CreateNewLayer(i);
            }
        }

        private void ResetNeuronsNumbers()
        {
            int n = 0;
            foreach (var layer in View.Layers)
            {
                int l = 0;
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Number = n++;
                    neuron.NumberInLayer = l++;
                }
            }

            View.NeuronsCount = n;
        }

        private void NeuronModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetNeuronsNumbers();
            _networkController.Reposition();
        }
    }
}