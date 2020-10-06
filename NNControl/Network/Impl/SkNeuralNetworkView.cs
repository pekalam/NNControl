using NNControl.Layer;
using NNControl.Layer.Impl;
using NNControl.Model;
using NNControl.Neuron.Impl;
using NNControl.Synapse.Impl;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace NNControl.Network.Impl
{
    internal class SkNeuralNetworkView : NeuralNetworkView
    {
        private SKImage _saved;

        private NeuralNetworkModel _neuralNetworkModel;
        private SkNetworkLayerPainter _networkLayerPainter;
        private SKColor _bgColor;

        private float _transX;
        private float _transY;

        public SKPaintSurfaceEventArgs PaintArgs;

        public override LayerView CreateLayerInstance()
        {
            return new SkLayerView();
        }

        public override NeuralNetworkModel NeuralNetworkModel
        {
            get => _neuralNetworkModel;
            set
            {
                _neuralNetworkModel = value;
                _bgColor = SKColor.Parse(value.BackgroundColor);
                _networkLayerPainter = new SkNetworkLayerPainter();
            }
        }


        private void ApplyZoom(SKCanvas canvas)
        {
            var m = canvas.TotalMatrix;
            m.ScaleX = (float) Zoom + 1;
            m.ScaleY = (float) Zoom + 1;

            // m.TransX = (float) (ZoomCenterX * Zoom);
            // m.TransY = (float)(ZoomCenterY * Zoom);


            _transX = m.TransX = -canvas.DeviceClipBounds.MidX * (float) Zoom;
            _transY = m.TransY = -canvas.DeviceClipBounds.MidY * (float) Zoom;

            canvas.SetMatrix(m);
        }

        private void DrawAll(SKCanvas canvas)
        {
            canvas.Clear(_bgColor);
            ApplyZoom(canvas);

            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            {
                foreach (var synapse in neuron.ConnectedSynapses)
                {
                    if (synapse.Excluded)
                    {
                        continue;
                    }


                    (synapse as SkSynapseView).Draw(this, layerView as SkLayerView, neuron as SkNeuronView, canvas);
                }
            }


            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            {
                if (neuron.Excluded)
                {
                    continue;
                }

                (neuron as SkNeuronView).Draw(this, layerView as SkLayerView, canvas);
            }
        }

        public override (float x, float y) ToCanvasPoints(float x, float y)
        {
            return ((x - _transX) / (float) (Zoom + 1), (y - _transY) / (float) (Zoom + 1));
        }


        public override void DrawAndSave()
        {
            var canvas = PaintArgs.Surface.Canvas;
            DrawAll(canvas);
            _saved = PaintArgs.Surface.Snapshot();
        }

        public override void DrawFromSaved()
        {
            PaintArgs.Surface.Canvas.DrawImage(_saved, 0, 0);
        }

        public override void DrawExcluded()
        {
            var canvas = PaintArgs.Surface.Canvas;
            ApplyZoom(canvas);

            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            {
                foreach (var synapse in neuron.ConnectedSynapses)
                {
                    if (synapse.Excluded)
                    {
                        (synapse as SkSynapseView).Draw(this, layerView as SkLayerView, neuron as SkNeuronView, canvas);
                    }
                }
            }

            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            {
                if (neuron.Excluded)
                {
                    (neuron as SkNeuronView).Draw(this, layerView as SkLayerView, canvas);
                }
            }


            //
            // foreach (var layerView in Layers)
            // foreach (var neuron in layerView.Neurons)
            // foreach (var synapse in neuron.ConnectedSynapses)
            // {
            //     if (synapse.Excluded)
            //     {
            //         (synapse as SkSynapseView).DrawSynapseLabel(this, layerView as SkLayerView, neuron as SkNeuronView,
            //             synapse as SkSynapseView);
            //     }
            // }
        }
    }
}