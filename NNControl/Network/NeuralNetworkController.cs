using NNControl.Layer;
using NNControl.Model;
using NNControl.Neuron;
using NNControl.Synapse;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NNControl.Network
{
    public partial class NeuralNetworkController
    {
        private readonly List<NeuronController> _selectedNeurons = new List<NeuronController>();
        private readonly Action _onRedraw;

        internal NetworkPositionManager PositionManager = new NetworkPositionManager();
        internal NetworkStructureManager StructureManager;
        public ColorManager Color;
        internal NeuralNetworkView View;
        internal Box ViewportPosition;


        public IReadOnlyList<LayerController> Layers => StructureManager.LayerControllers;
        public float CanvasWidth;
        public float CanvasHeight;

        public NeuralNetworkController(NeuralNetworkView view, Action onRedraw) : this(onRedraw)
        {
            StructureManager = new NetworkStructureManager(view, this);
            View = view;
            Color = new ColorManager(this);
        }

        public NeuralNetworkModel NeuralNetworkModel
        {
            get => StructureManager.View.NeuralNetworkModel;
            set
            {
                StructureManager.SetNeuralNetworkModel(value);
                StructureManager.AddNewLayers(0, value.NetworkLayerModels.Count);

                PositionManager.AdjustNetworkPosition(this);

                value.NetworkLayerModels.CollectionChanged += (_, arg) =>
                {
                    if (arg.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (var item in arg.NewItems)
                        {
                            (item as LayerModel).NeuronModels.CollectionChanged += (_, __) => Reposition();
                        }
                    }

                    Reposition();
                };
                foreach (var layerModel in value.NetworkLayerModels)
                {
                    layerModel.NeuronModels.CollectionChanged += (_, __) => Reposition();
                }

                RequestRedraw(ViewTrig.FORCE_DRAW);
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

        public void HighlightLayer(int layerNum)
        {
            var layer = Layers[layerNum];
            layer.HighlightLayer();
            RequestRedraw(ViewTrig.FORCE_DRAW);
        }

        public void ClearLayerHighlight(int layerNum)
        {
            var layer = Layers[layerNum];
            layer.ClearHighlight();
            RequestRedraw(ViewTrig.FORCE_DRAW);
        }

        public void ClearHighlight()
        {
            foreach (var layer in Layers)
            {
                layer.ClearHighlight();
            }
            RequestRedraw(ViewTrig.FORCE_DRAW);
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
            ViewportPosition.Left += dx;
            ViewportPosition.Top += dy;
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
            ViewportPosition.Left = 0;
            ViewportPosition.Top = 0;
            foreach (var layer in Layers)
            {
                layer.Reposition();
            }

            PositionManager.AdjustNetworkPosition(this);
            RequestRedraw(ViewTrig.REPOSITION);
        }

        public void UpdatePositionParameters()
        {
            foreach (var layer in Layers)
            {
                layer.Reposition();
            }

            PositionManager.AdjustNetworkPosition(this, false);
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
            _redrawStateMachine.Next(next);
            _onRedraw();
        }
    }
}