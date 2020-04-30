using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NNControl.Layer;
using NNControl.Model;
using NNControl.Neuron;
using NNControl.Synapse;

namespace NNControl.Network
{


    public partial class NeuralNetworkViewAbstraction
    {
        private readonly List<LayerViewAbstraction> _abstrLayers = new List<LayerViewAbstraction>();
        private readonly List<NeuronViewAbstraction> _selectedNeurons = new List<NeuronViewAbstraction>();

        public NeuralNetworkViewAbstraction(NeuralNetworkViewImpl impl) : this()
        {
            Impl = impl;
        }

        public event Action OnRequestRedraw;

        internal NeuralNetworkViewImpl Impl { get; private set; }

        public IReadOnlyList<LayerViewAbstraction> Layers => _abstrLayers;

        public NeuralNetworkPositionManagerBase PositionManager { get; set; } =
            new DefaultNeuralNetworkPositionManager();


        private Box _viewportPosition;
        internal Box ViewportPosition => _viewportPosition;

        public NeuralNetworkModel NeuralNetworkModel
        {
            get => Impl.NeuralNetworkModel;
            set
            {
                _abstrLayers.Clear();
                Impl.Layers.Clear();
                Impl.NeuralNetworkModel = value;

                AddNewLayers();

                PositionManager.InvokeActionsAfterPositionsSet(this);

                value.NetworkLayerModels.CollectionChanged += NetworkLayerModelsOnCollectionChanged;

                RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.FORCE_DRAW);
            }
        }

        private void AddNewLayers(int collectionStartingIndex = 0)
        {
            for (int i = collectionStartingIndex; i < Impl.NeuralNetworkModel.NetworkLayerModels.Count; i++)
            {
                CreateAndAddNewLayer(i);
                Impl.NeuralNetworkModel.NetworkLayerModels[i].NeuronModels.CollectionChanged += NeuronModelsOnCollectionChanged;
            }

            SetNeuronsNumbers();
            Impl.NeuronsCount = NeuralNetworkModel.NetworkLayerModels.Select(model => model.NeuronModels.Count).Sum();
        }

        private void CreateAndAddNewLayer(int layerNum)
        {
            var newLayer = Impl.CreateLayerInstance();
            Impl.Layers.Add(newLayer);

            var previousLayer = layerNum == 0 ? null : _abstrLayers[layerNum - 1];
            var newAbstractLayer = new LayerViewAbstraction(previousLayer, layerNum,
                newLayer, this);
            if (previousLayer != null)
            {
                previousLayer.NextLayer = newAbstractLayer;
            }
            _abstrLayers.Add(newAbstractLayer);
        }

        private void NetworkLayerModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AddNewLayers(e.NewStartingIndex);

            Reposition();
        }

        private void NeuronModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Impl.NeuronsCount = Impl.NeuralNetworkModel.NetworkLayerModels.Select(model => model.NeuronModels.Count)
                .Sum();
            SetNeuronsNumbers();

            Reposition();
        }

        private void SetNeuronsNumbers()
        {
            int n = 0;
            foreach (var layer in Impl.Layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Number = n++;
                }
            }
        }



        public void SetZoom(double value)
        {
            if (Impl.SelectedNeuron.Count > 0 || Impl.SelectedSynapse != null)
            {
                SetAllNeuronAndSynapsesExcluded(false);

                Impl.SelectedSynapse = null;
                Impl.SelectedNeuron.Clear();
            }

            Impl.Zoom = value;
            foreach (var layer in Layers)
            {
                layer.OnZoomChanged();
            }
            RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.ZOOM);
        }

        public void MoveSelectedNeurons(NeuronViewImpl clicked, float nx, float ny)
        {
            nx = nx / (float) (Impl.Zoom + 1);
            ny = ny / (float) (Impl.Zoom + 1);

            var dx = nx - clicked.X;
            var dy = ny - clicked.Y;

            foreach (var neuron in _selectedNeurons)
            {
                neuron.Move(dx, dy);
            }


            RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.MV);
        }

        public NeuronViewAbstraction SelectNeuronAt(int x, int y)
        {
            var foundNeuron = FindNeuronAt(x, y);
            if (foundNeuron == null)
            {
                return null;
            }

            if (Impl.SelectedNeuron.Contains(foundNeuron.Impl))
            {
                return foundNeuron;
            }

            _selectedNeurons.Add(foundNeuron);
            Impl.SelectedNeuron.Add(foundNeuron.Impl);
            RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.SELECT);
            return foundNeuron;
        }

        public NeuronViewAbstraction FindNeuronAt(int x, int y)
        {
            foreach (var layer in Layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    if (neuron.Impl.Contains(x, y))
                    {
                        return neuron;
                    }
                }
            }

            return null;
        }


        public void DeselectNeuron()
        {
            if (Impl.SelectedNeuron.Count > 0)
            {
                _selectedNeurons.Clear();
                Impl.SelectedNeuron.Clear();
                RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.DESELECT);
            }
        }

        public void SelectedNeuronMoveEnd()
        {
            if (Impl.SelectedNeuron.Count > 0)
            {
                RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.MV_END);
            }
        }

        public void Draw()
        {
            _redrawStateMachine.Next(NeuralNetworkViewAbstraction.ViewTrig.DRAW);
        }

        public void ForceDraw()
        {
            RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.FORCE_DRAW);
        }

        public void Move(int dx, int dy)
        {
            _viewportPosition.Left += dx;
            _viewportPosition.Top += dy;
            foreach (var layer in Layers)
            {
                layer.Impl.X += dx;
                layer.Impl.Y += dy;
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Impl.X += dx;
                    neuron.Impl.Y += dy;
                }
            }
            RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.FORCE_DRAW);
        }

        public void Reposition()
        {
            foreach (var layer in Layers)
            {
                layer.Reposition();
            }

            PositionManager.InvokeActionsAfterPositionsSet(this);
            RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.FORCE_DRAW);
        }


        public SynapseViewAbstraction SelectSynapseAt(float x, float y)
        {
            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            foreach (var synapse in neuron.Synapses)
            {
                if (synapse.Impl.Contains(x, y))
                {
                    if (synapse.Impl != Impl.SelectedSynapse)
                    {
                        Impl.SelectedSynapse = synapse.Impl;
                        RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.SELECT_SYNAPSE);
                        return synapse;
                    }
                }
            }

            return null;
        }


        public void DeselectSynapse()
        {
            if (Impl.SelectedSynapse != null)
            {
                RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig.DESELECT_SYNAPSE);
            }
        }


        public void RequestRedraw(NeuralNetworkViewAbstraction.ViewTrig next)
        {
            Check.NotNull(OnRequestRedraw);
            _redrawStateMachine.Next(next);
            // ReSharper disable once PossibleNullReferenceException
            OnRequestRedraw.Invoke();
        }
    }
}