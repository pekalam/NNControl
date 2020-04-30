using NNControl.Layer.Impl;
using NNControl.Network.Impl;
using NNControl.Neuron.Impl;
using SkiaSharp;

namespace NNControl.Synapse.Impl
{
    internal class SkInternalSynapsePainter
    {
        private readonly SKPaint _synapsePaint = new SKPaint();
        private readonly SKPaint _synapseLabelPaint = new SKPaint();
        private readonly SKPaint _selectedSynapsePaint = new SKPaint();
        private readonly SKPaint _arrowFillPaint = new SKPaint();
        private readonly SKPaint _largeNetSynPaint = new SKPaint();

        public SkInternalSynapsePainter(string synapseColor = "#000000", string synapseLabelColor = "#FFFFFF",
            string selectedSynapseColor = "#FFFFFF")
        {
            _synapsePaint.IsAntialias = true;
            _synapsePaint.Color = SKColor.Parse(synapseColor);
            _synapsePaint.StrokeCap = SKStrokeCap.Round;

            _synapseLabelPaint.Color = SKColor.Parse(synapseLabelColor);
            _synapseLabelPaint.IsAntialias = true;
            _synapsePaint.StrokeCap = SKStrokeCap.Butt;

            _selectedSynapsePaint.Color = SKColor.Parse(selectedSynapseColor);
            _selectedSynapsePaint.IsAntialias = true;
            _selectedSynapsePaint.StrokeCap = SKStrokeCap.Butt;


            _arrowFillPaint.Style = SKPaintStyle.Fill;
            _arrowFillPaint.Color = SKColors.Black;
            _arrowFillPaint.IsAntialias = true;


            _largeNetSynPaint.Color = SKColor.Parse("#f2f2f2");
            _largeNetSynPaint.IsAntialias = true;
            _largeNetSynPaint.StrokeCap = SKStrokeCap.Butt;
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron, SkSynapseView synapse)
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            var canvas = ctx.e.Surface.Canvas;

            
            SKPaint synapsPaint = neuron.ConnectedSynapses.Count >= 30 ? _largeNetSynPaint : _synapsePaint;
            canvas.DrawLine(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.Neuron2.X, synapse.Neuron2.Y,
                synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsPaint);


            // var dst = MathHelpers.Distance(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.Neuron2.X, synapse.Neuron2.Y);
            //
            // var arrowLen = 3f;
            //
            // var l = network.NeuralNetworkModel.NeuronRadius / dst;
            // var vp = (x: (synapse.Neuron2.X - synapse.Neuron1.X) * l, y: (synapse.Neuron2.Y - synapse.Neuron1.Y) * l);
            // var arrowEnd = (x: synapse.Neuron2.X - vp.x, y: synapse.Neuron2.Y - vp.y);
            //
            // var l2 = (network.NeuralNetworkModel.NeuronRadius + arrowLen) / dst;
            // var vp2 = (x: (synapse.Neuron2.X - synapse.Neuron1.X) * l2,
            //     y: (synapse.Neuron2.Y - synapse.Neuron1.Y) * l2);
            //
            // var arrowBeg = (x: synapse.Neuron2.X - vp2.x, y: synapse.Neuron2.Y - vp2.y);
            //
            // var dif = (x: arrowBeg.x - arrowEnd.x, y: arrowBeg.y - arrowEnd.y);
            //
            // var arrowLeftEnd = (x: arrowBeg.x + dif.y, y: arrowBeg.y - dif.x);
            // var arrowRightEnd = (x: arrowBeg.x - dif.y, y: arrowBeg.y + dif.x);

            var arrowPath = new SKPath();

            var arrowEnd = synapse.ArrowEnd;
            var arrowLeftEnd = synapse.ArrowLeftEnd;
            var arrowRightEnd = synapse.ArrowRightEnd;

            arrowPath.MoveTo(arrowEnd.x, arrowEnd.y);
            arrowPath.LineTo(arrowLeftEnd.x, arrowLeftEnd.y);
            arrowPath.LineTo(arrowRightEnd.x, arrowRightEnd.y);
            arrowPath.LineTo(arrowEnd.x, arrowEnd.y);
            arrowPath.Close();

            canvas.DrawPath(arrowPath, _arrowFillPaint);
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