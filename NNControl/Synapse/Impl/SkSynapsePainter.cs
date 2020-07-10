using NNControl.Layer.Impl;
using NNControl.Model;
using NNControl.Network.Impl;
using NNControl.Neuron.Impl;
using SkiaSharp;

namespace NNControl.Synapse.Impl
{
    internal class SkSynapsePainter
    {
        private readonly SKPaint _synapsePaint = new SKPaint();
        private readonly SKPaint _synapseLabelPaint = new SKPaint();
        private readonly SKPaint _selectedSynapsePaint = new SKPaint();
        private readonly SKPaint _arrowPaint = new SKPaint();
        private readonly SKPaint _largeNetSynPaint = new SKPaint();
        private readonly SKPaint _largeNetArrowPaint = new SKPaint();

        public SkSynapsePainter(SynapseSettings settings)
        {
            _synapsePaint.Color = SKColor.Parse(settings.Color);
            _synapsePaint.IsAntialias = true;
            _synapsePaint.StrokeCap = SKStrokeCap.Round;

            _synapseLabelPaint.Color = SKColor.Parse(settings.LabelColor);
            _synapseLabelPaint.IsAntialias = true;
            _synapsePaint.StrokeCap = SKStrokeCap.Butt;

            _selectedSynapsePaint.Color = SKColor.Parse(settings.SelectedColor);
            _selectedSynapsePaint.IsAntialias = true;
            _selectedSynapsePaint.StrokeCap = SKStrokeCap.Butt;


            _arrowPaint.Style = _largeNetArrowPaint.Style = SKPaintStyle.Fill;
            _arrowPaint.Color = SKColor.Parse(settings.ArrowColor);
            _arrowPaint.IsAntialias = _largeNetArrowPaint.IsAntialias = true;


            _largeNetSynPaint.Color = SKColor.Parse(settings.LargetNetColor);
            _largeNetSynPaint.IsAntialias = true;
            _largeNetSynPaint.StrokeCap = SKStrokeCap.Butt;

            _largeNetArrowPaint.Color = SKColor.Parse(settings.LargetNetColor);

            _synapsePaint.StrokeWidth = _selectedSynapsePaint.StrokeWidth = settings.StrokeWidth;
        }

        public void SetColor(SKColor color)
        {
            _synapsePaint.Color = _largeNetSynPaint.Color = color;
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron, SkSynapseView synapse)
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            var canvas = ctx.e.Surface.Canvas;

            
            SKPaint synapsPaint = neuron.ConnectedSynapses.Count >= 30 ? _largeNetSynPaint : _synapsePaint;


            var arrowEnd = synapse.ArrowEnd;
            var arrowLeftEnd = synapse.ArrowLeftEnd;
            var arrowRightEnd = synapse.ArrowRightEnd;

            canvas.DrawLine(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.ArrowBeg.x, synapse.ArrowBeg.y,
                synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsPaint);


            var arrowPath = new SKPath();

            arrowPath.MoveTo(arrowEnd.x, arrowEnd.y);
            arrowPath.LineTo(arrowLeftEnd.x, arrowLeftEnd.y);
            arrowPath.LineTo(arrowRightEnd.x, arrowRightEnd.y);
            arrowPath.LineTo(arrowEnd.x, arrowEnd.y);
            arrowPath.Close();

            canvas.DrawPath(arrowPath, neuron.ConnectedSynapses.Count >= 30 ? _largeNetArrowPaint : _arrowPaint);
        }

        public void DrawSynapseLabel(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron,
            SkSynapseView synapse)
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            var canvas = ctx.e.Surface.Canvas;

            var txt = neuron.NeuronModel.SynapsesLabels[synapse.NumberInNeuron];

            var txtWidth = _synapseLabelPaint.MeasureText(txt);

            var p = new SKPoint()
            {
                X = -txtWidth / 2 + synapse.Neuron1.X + (synapse.Neuron2.X - synapse.Neuron1.X) / 2,
                Y = synapse.Neuron1.Y + (synapse.Neuron2.Y - synapse.Neuron1.Y) / 2
            };

            canvas.DrawText(txt, p, _synapseLabelPaint);
        }
    }
}