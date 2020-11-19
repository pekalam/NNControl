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
        //private SKImage _saved;
        private SKPicture _saved;

        private NeuralNetworkModel _neuralNetworkModel;
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
                value.BackgroundColorChanged += s => _bgColor = SKColor.Parse(s);
                _bgColor = SKColor.Parse(value.BackgroundColor);
            }
        }

        private void ApplyTranslation(SKCanvas canvas)
        {

        }

        private void ApplyZoom(SKCanvas canvas)
        {
            var m = SKMatrix.CreateScale(Zoom, Zoom, canvas.DeviceClipBounds.MidX, canvas.DeviceClipBounds.MidY);

            _transX = m.TransX;
            _transY = m.TransY;

            canvas.SetMatrix(m);
        }

        private void DrawAll(SKCanvas canvas, bool aa = true)
        {
            canvas.Clear(_bgColor);

            for (int i = 0; i < Layers.Count; i++)
            {
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    for (int k = 0; k < Layers[i].Neurons[j].ConnectedSynapses.Count; k++)
                    {
                        if (Layers[i].Neurons[j].ConnectedSynapses[k].Excluded)
                        {
                            continue;
                        }


                        (Layers[i].Neurons[j].ConnectedSynapses[k] as SkSynapseView)!.Draw(this, Layers[i] as SkLayerView, Layers[i].Neurons[j] as SkNeuronView, canvas);
                    }
                }
            }


            for (int i = 0; i < Layers.Count; i++)
            {
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    if (Layers[i].Neurons[j].Excluded)
                    {
                        continue;
                    }

                    (Layers[i].Neurons[j] as SkNeuronView)!.Draw(this, Layers[i] as SkLayerView, canvas);
                }
            }
        }

        public override (float x, float y) ToCanvasPoints(float x, float y)
        {
            return ((x - _transX) / Zoom, (y - _transY) / Zoom);
        }


        public override void DrawAndSave()
        {
            var pictureRecorder = new SKPictureRecorder();
            var canvas = pictureRecorder.BeginRecording(PaintArgs.Info.Rect);
            DrawAll(canvas, false);
            //_saved = PaintArgs.Surface.Snapshot();
            _saved = pictureRecorder.EndRecording();
            pictureRecorder.Dispose();

            var surfCanvas = PaintArgs.Surface.Canvas;
            surfCanvas.Clear(_bgColor);
            ApplyZoom(surfCanvas);
            surfCanvas.DrawPicture(_saved);
        }

        public override void DrawFromSaved()
        {
            var canvas = PaintArgs.Surface.Canvas;
            ApplyZoom(canvas);
            canvas.DrawPicture(_saved);
            //PaintArgs.Surface.Canvas.DrawImage(_saved, 0, 0);
        }

        public override void DrawExcluded()
        {
            var canvas = PaintArgs.Surface.Canvas;
            ApplyZoom(canvas);


            for (int i = 0; i < Layers.Count; i++)
            {
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    for (int k = 0; k < Layers[i].Neurons[j].ConnectedSynapses.Count; k++)
                    {
                        if (Layers[i].Neurons[j].ConnectedSynapses[k].Excluded)
                        {
                            (Layers[i].Neurons[j].ConnectedSynapses[k] as SkSynapseView)!.Draw(this, Layers[i] as SkLayerView, Layers[i].Neurons[j] as SkNeuronView, canvas);
                        }
                    }
                }
            }


            for (int i = 0; i < Layers.Count; i++)
            {
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    if (Layers[i].Neurons[j].Excluded)
                    {
                        (Layers[i].Neurons[j] as SkNeuronView)!.Draw(this, Layers[i] as SkLayerView, canvas);
                    }
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

        public override void DrawDirectly()
        {
            var canvas = PaintArgs.Surface.Canvas;
            ApplyZoom(canvas);
            DrawAll(canvas);
        }
    }
}