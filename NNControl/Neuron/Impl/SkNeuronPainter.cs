using System.Diagnostics;
using NNControl.Layer.Impl;
using NNControl.Model;
using NNControl.Network.Impl;
using SkiaSharp;

namespace NNControl.Neuron.Impl
{
    internal class SkNeuronPainter
    {
        private readonly SKPaint _defaultPaint = new SKPaint();
        private readonly SKPaint _selectedPaint = new SKPaint();
        private readonly SKPaint _defaultInputPaint = new SKPaint();
        private readonly SKPaint _highlightedPaint = new SKPaint();
        private readonly SKPaint _defaultLabelPaint = new SKPaint();

        public SkNeuronPainter(NeuronSettings settings)
        {
            _defaultPaint.IsAntialias = _selectedPaint.IsAntialias = _defaultInputPaint.IsAntialias = _highlightedPaint.IsAntialias = _defaultLabelPaint.IsAntialias = true;
            _defaultPaint.Color = SKColor.Parse(settings.Color);
            _defaultInputPaint.Color = SKColor.Parse(settings.InputColor);
            _selectedPaint.Color = SKColor.Parse(settings.SelectedColor);
            _highlightedPaint.Color = SKColor.Parse(settings.HighlightedColor);
            _defaultLabelPaint.Color = SKColor.Parse(settings.LabelColor);
            _defaultPaint.StrokeCap = _selectedPaint.StrokeCap = _highlightedPaint.StrokeCap = SKStrokeCap.Round;

            _defaultInputPaint.Style = _defaultLabelPaint.Style = SKPaintStyle.Fill;

            //            var selectedShadowFilter = CreateSelectedShadow();

            //            _selectedNeuronPaint.ImageFilter = selectedShadowFilter;

            var neuronShadowFilter = CreateDefaultShadow();
            _defaultPaint.ImageFilter = neuronShadowFilter;
            _defaultInputPaint.ImageFilter = neuronShadowFilter;
        }

        internal void SetColor(SKColor color)
        {
            _defaultPaint.Color = _defaultInputPaint.Color = color;
        }

        private SKImageFilter CreateSelectedShadow()
        {
            var shadowCOlor = new SKColor(255, 255, 105, 255);

            var neuronShadowFilter = SKImageFilter.CreateDropShadow(0f, 0f, 1.2f, 1.2f,
                shadowCOlor, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground, null, null);
            return neuronShadowFilter;
        }

        private SKImageFilter CreateDefaultShadow()
        {
            var shadowCOlor = new SKColor(0, 0, 0, 255);

            var neuronShadowFilter = SKImageFilter.CreateDropShadow(0f, 0f, 1.2f, 1.2f,
                shadowCOlor, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground, null, null);
            return neuronShadowFilter;
        }

        private void DrawNeuron(SKCanvas canvas, SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron,
            bool selected, bool highlighted)
        {
            SKPaint paint;
            if (highlighted)
            {
                paint = _highlightedPaint;
            }
            else
            {
                paint = selected ? _selectedPaint : (layer.PreviousLayer == null ? _defaultInputPaint : _defaultPaint);
            }

            if (layer.PreviousLayer == null)
            {
                canvas.DrawRect(neuron.X - network.NeuralNetworkModel.NeuronRadius / 2f,
                    neuron.Y - network.NeuralNetworkModel.NeuronRadius / 2f, network.NeuralNetworkModel.NeuronRadius,
                    network.NeuralNetworkModel.NeuronRadius, paint);
            }
            else
            {
                canvas.DrawCircle(neuron.X, neuron.Y, network.NeuralNetworkModel.NeuronRadius, paint);
            }


            if (neuron.NeuronModel.Label != "")
            {
                if (layer.PreviousLayer == null)
                {
                    var l = _defaultLabelPaint.MeasureText(neuron.NeuronModel.Label);
                    canvas.DrawText(neuron.NeuronModel.Label, neuron.X - network.NeuralNetworkModel.NeuronRadius - l - 10, neuron.Y + _defaultLabelPaint.FontSpacing / 4f, _defaultLabelPaint);
                }
                if (neuron.ConnectedSynapses.Count == 0)
                {
                    var l = _defaultLabelPaint.MeasureText(neuron.NeuronModel.Label);
                    canvas.DrawText(neuron.NeuronModel.Label, neuron.X + network.NeuralNetworkModel.NeuronRadius + 10, neuron.Y + _defaultLabelPaint.FontSpacing / 4f, _defaultLabelPaint);
                }
            }
        }


        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron, SKCanvas canvas)
        {
#if DEBUG2
            canvas.DrawRect(neuron.ReferenceRect, _defaultInputPaint);
#endif

            DrawNeuron(canvas, network, layer, neuron, network.SelectedNeuron.Contains(neuron), layer.HighlightedNeurons.Contains(neuron));
        }
    }
}