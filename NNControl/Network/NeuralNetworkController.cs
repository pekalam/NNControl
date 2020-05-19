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
    public partial class NeuralNetworkController
    {
        internal NeuralNetworkPositionManager PositionManager =
            new NeuralNetworkPositionManager();

        private readonly List<LayerController> _abstrLayers = new List<LayerController>();
        private readonly List<NeuronController> _selectedNeurons = new List<NeuronController>();

        public NeuralNetworkController(NeuralNetworkView view) : this()
        {
            View = view;
            Color = new ColorManager(this);
        }

        public event Action OnRequestRedraw;

        internal NeuralNetworkView View { get; private set; }

        public IReadOnlyList<LayerController> Layers => _abstrLayers;



        public float CanvasWidth { get; private set; }
        public float CanvasHeight { get; private set; }

        private Box _viewportPosition;
        internal Box ViewportPosition => _viewportPosition;

        public ColorManager Color { get; }

        public NeuralNetworkModel NeuralNetworkModel
        {
            get => View.NeuralNetworkModel;
            set
            {
                _abstrLayers.Clear();
                View.Layers.Clear();
                View.NeuralNetworkModel = value;

                AddNewLayers();

                PositionManager.InvokeActionsAfterPositionsSet(this);

                value.NetworkLayerModels.CollectionChanged += NetworkLayerModelsOnCollectionChanged;

                RequestRedraw(ViewTrig.FORCE_DRAW);
            }
        }

        private void AddNewLayers(int collectionStartingIndex = 0)
        {
            for (int i = collectionStartingIndex; i < View.NeuralNetworkModel.NetworkLayerModels.Count; i++)
            {
                CreateAndAddNewLayer(i);
                View.NeuralNetworkModel.NetworkLayerModels[i].NeuronModels.CollectionChanged +=
                    NeuronModelsOnCollectionChanged;
            }

            SetNeuronsNumbers();
            View.NeuronsCount = NeuralNetworkModel.NetworkLayerModels.Select(model => model.NeuronModels.Count).Sum();
        }

        private void CreateAndAddNewLayer(int layerNum)
        {
            var newLayer = View.CreateLayerInstance();
            View.Layers.Add(newLayer);

            var previousLayer = layerNum == 0 ? null : _abstrLayers[layerNum - 1];
            var newAbstractLayer = new LayerController(previousLayer, layerNum,
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
            View.NeuronsCount = View.NeuralNetworkModel.NetworkLayerModels.Select(model => model.NeuronModels.Count)
                .Sum();
            SetNeuronsNumbers();

            Reposition();
        }

        private void SetNeuronsNumbers()
        {
            int n = 0;
            foreach (var layer in View.Layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Number = n++;
                }
            }
        }


        public void SetZoom(double value)
        {
            if (value < -1.0d)
            {
                return;
            }

            View.Zoom = value;
            foreach (var layer in Layers)
            {
                layer.OnZoomChanged();
            }

            RequestRedraw(ViewTrig.ZOOM);
        }

        public void MoveSelectedNeurons(NeuronView clicked, float nx, float ny)
        {
            (nx, ny) = View.ToCanvasPoints(nx, ny);

            var dx = nx - clicked.X;
            var dy = ny - clicked.Y;

            foreach (var neuron in _selectedNeurons)
            {
                neuron.Move(dx, dy);
            }


            RequestRedraw(ViewTrig.MV);
        }

        public NeuronController SelectNeuronAt(float x, float y)
        {
            var foundNeuron = FindNeuronAt(x, y);
            if (foundNeuron == null)
            {
                return null;
            }

            if (View.SelectedNeuron.Contains(foundNeuron.View))
            {
                return foundNeuron;
            }

            _selectedNeurons.Add(foundNeuron);
            View.SelectedNeuron.Add(foundNeuron.View);
            RequestRedraw(ViewTrig.SELECT);
            return foundNeuron;
        }

        public NeuronController FindNeuronAt(float x, float y)
        {
            (x, y) = View.ToCanvasPoints(x, y);

            foreach (var layer in Layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    if (neuron.View.Contains(x, y))
                    {
                        return neuron;
                    }
                }
            }

            return null;
        }


        public void DeselectNeuron()
        {
            if (View.SelectedNeuron.Count > 0)
            {
                _selectedNeurons.Clear();
                View.SelectedNeuron.Clear();
                RequestRedraw(ViewTrig.DESELECT);
            }
        }

        public void SelectedNeuronMoveEnd()
        {
            if (View.SelectedNeuron.Count > 0)
            {
                RequestRedraw(ViewTrig.MV_END);
            }
        }

        public void Draw(float canvasWidth, float canvasHeight)
        {
            SetCanvasSz(canvasWidth, canvasHeight);
            _redrawStateMachine.Next(ViewTrig.DRAW);
        }

        public void SetCanvasSz(float canvasWidth, float canvasHeight)
        {
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
        }

        public void ForceDraw(float canvasWidth, float canvasHeight)
        {
            SetCanvasSz(canvasWidth, canvasHeight);
            RequestRedraw(ViewTrig.FORCE_DRAW);
        }

        public void Move(int dx, int dy)
        {
            _viewportPosition.Left += dx;
            _viewportPosition.Top += dy;
            foreach (var layer in Layers)
            {
                layer.View.X += dx;
                layer.View.Y += dy;
                foreach (var neuron in layer.Neurons)
                {
                    neuron.Move(dx, dy);
                }
            }

            RequestRedraw(ViewTrig.FORCE_DRAW);
        }

        public void Reposition()
        {
            foreach (var layer in Layers)
            {
                layer.Reposition();
            }

            PositionManager.InvokeActionsAfterPositionsSet(this);
            RequestRedraw(ViewTrig.REPOSITION);
        }

        public void UpdatePositionParameters()
        {
            foreach (var layer in Layers)
            {
                layer.Reposition();
            }
            PositionManager.InvokeActionsAfterPositionsSet(this, false);
            RequestRedraw(ViewTrig.REPOSITION);
        }


        public SynapseController SelectSynapseAt(float x, float y)
        {
            (x, y) = View.ToCanvasPoints(x, y);

            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            foreach (var synapse in neuron.Synapses)
            {
                if (synapse.View.Contains(x, y))
                {
                    if (synapse.View != View.SelectedSynapse)
                    {
                        View.SelectedSynapse = synapse.View;
                        RequestRedraw(ViewTrig.SELECT_SYNAPSE);
                        return synapse;
                    }
                }
            }

            return null;
        }


        public void DeselectSynapse()
        {
            if (View.SelectedSynapse != null)
            {
                RequestRedraw(NeuralNetworkController.ViewTrig.DESELECT_SYNAPSE);
            }
        }


        public void RequestRedraw(NeuralNetworkController.ViewTrig next)
        {
            Check.NotNull(OnRequestRedraw);
            _redrawStateMachine.Next(next);
            // ReSharper disable once PossibleNullReferenceException
            OnRequestRedraw.Invoke();
        }



    }

    public partial class NeuralNetworkController
    {
        public class ColorManager
        {
            private NeuralNetworkController _controller;

            public ColorManager(NeuralNetworkController controller)
            {
                _controller = controller;
            }

            public void SetNeuronColor(int layerNumber, int numberInLayer, string hexColor)
            {
                _controller.Layers[layerNumber].Neurons[numberInLayer].View.SetColor(hexColor);
            }

            public void SetNeuronColor(int number, string hexColor)
            {
                NeuronController neuron = null;
                foreach (var layer in _controller.Layers)
                {
                    foreach (var n in layer.Neurons)
                    {
                        if (n.View.Number == number)
                        {
                            neuron = n;
                            break;
                        }
                    }
                }

                neuron?.View.SetColor(hexColor);
            }

            public void SetSynapseColor(int layerNumber, int neuronNumberInLayer, int numberInNeuron, string hexColor)
            {
                if (_controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses.Count == 0)
                {
                    return;
                }

                _controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses[numberInNeuron].View.SetColor(hexColor);
            }


            public void SetNeuronColor(int layerNumber, int numberInLayer, int scale)
            {
                _controller.Layers[layerNumber].Neurons[numberInLayer].View.SetColor(scale);
            }

            public void SetNeuronColor(int number, int scale)
            {
                throw new NotImplementedException();
            }

            public void SetSynapseColor(int layerNumber, int neuronNumberInLayer, int numberInNeuron, int scale)
            {
                if (_controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses.Count == 0)
                {
                    return;
                }

                _controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses[numberInNeuron].View.SetColor(scale);
            }

            public void ResetColorsToDefault()
            {
                foreach (var layer in _controller.Layers)
                {
                    foreach (var neuron in layer.Neurons)
                    {
                        neuron.View.SetColor(_controller.NeuralNetworkModel.NeuronSettings.Color);
                        foreach (var synapse in neuron.Synapses)
                        {
                            synapse.View.SetColor(_controller.NeuralNetworkModel.SynapseSettings.Color);
                        }
                    }
                }
                ApplyColors();
            }

            public void ApplyColors()
            {
                _controller.RequestRedraw(ViewTrig.FORCE_DRAW);
            }
        }
    }
}