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


        public SkNeuronPainter(NeuronSettings settings)
        {
            _defaultPaint.IsAntialias = _selectedPaint.IsAntialias = _defaultInputPaint.IsAntialias = true;
            _defaultPaint.Color = SKColor.Parse(settings.Color);
            _defaultInputPaint.Color = SKColor.Parse(settings.InputColor);
            _selectedPaint.Color = SKColor.Parse(settings.SelectedColor);
            _defaultPaint.StrokeCap = _selectedPaint.StrokeCap = SKStrokeCap.Round;

            _defaultInputPaint.Style = SKPaintStyle.Fill;

            var neuronShadowFilter = CreateDefaultShadow();
//            var selectedShadowFilter = CreateSelectedShadow();

//            _selectedNeuronPaint.ImageFilter = selectedShadowFilter;

            _defaultPaint.ImageFilter = neuronShadowFilter;
            _defaultInputPaint.ImageFilter = neuronShadowFilter;
        }

        internal void SetColor(string hexColor)
        {
            _defaultPaint.Color = _defaultInputPaint.Color = SKColor.Parse(hexColor);
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
            bool selected)
        {
            if (layer.PreviousLayer == null)
            {
                canvas.DrawRect(neuron.X - network.NeuralNetworkModel.NeuronRadius / 2f,
                    neuron.Y - network.NeuralNetworkModel.NeuronRadius / 2f, network.NeuralNetworkModel.NeuronRadius,
                    network.NeuralNetworkModel.NeuronRadius, selected ? _selectedPaint : _defaultInputPaint);
            }
            else
            {
                canvas.DrawCircle(neuron.X, neuron.Y, network.NeuralNetworkModel.NeuronRadius,
                    selected ? _selectedPaint : _defaultPaint);
            }
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron)
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            var canvas = ctx.e.Surface.Canvas;

#if DEBUG2
            canvas.DrawRect(neuron.ReferenceRect, _defaultInputPaint);
#endif

            if (network.SelectedNeuron.Count > 0 && network.SelectedNeuron.Contains(neuron))
            {
                DrawNeuron(canvas, network, layer, neuron, true);
            }
            else
            {
                DrawNeuron(canvas, network, layer, neuron, false);
            }
        }
    }
}