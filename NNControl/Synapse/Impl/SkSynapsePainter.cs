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

        private readonly SKPath _arrowPath = new SKPath();

        public SkSynapsePainter(SynapseSettings settings)
        {
            _synapsePaint.Color = SKColor.Parse(settings.Color);
            _synapsePaint.IsAntialias = true;
            _synapsePaint.StrokeCap = SKStrokeCap.Butt;

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

        public void SetColor(in SKColor color)
        {
            _synapsePaint.Color = _largeNetSynPaint.Color = color;
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron, SkSynapseView synapse)
        {
            var canvas = network.PaintArgs.Surface.Canvas;

            
            SKPaint synapsPaint = neuron.ConnectedSynapses.Count >= 30 ? _largeNetSynPaint : _synapsePaint;


            // canvas.DrawLine(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.ArrowBeg.x, synapse.ArrowBeg.y,
            //     synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsPaint);


            canvas.DrawLine(synapse.SynapseData.GetX(synapse.Id),
                synapse.SynapseData.GetY(synapse.Id), 
                synapse.SynapseData.GetArrowBegX(synapse.Id),
                synapse.SynapseData.GetArrowBegY(synapse.Id),
                synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsPaint);


            // canvas.DrawLine(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.Neuron2.X, synapse.Neuron2.Y,
            //     synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsPaint);

            // _arrowPath.Rewind();
            //
            // _arrowPath.MoveTo(synapse.ArrowEnd.x, synapse.ArrowEnd.y);
            // _arrowPath.LineTo(synapse.ArrowLeftEnd.x, synapse.ArrowLeftEnd.y);
            // _arrowPath.LineTo(synapse.ArrowRightEnd.x, synapse.ArrowRightEnd.y);
            // _arrowPath.LineTo(synapse.ArrowEnd.x, synapse.ArrowEnd.y);
            // _arrowPath.Close();


            _arrowPath.Rewind();

            _arrowPath.MoveTo(synapse.SynapseData.GetArrowEndX(synapse.Id), synapse.SynapseData.GetArrowEndY(synapse.Id));
            _arrowPath.LineTo(synapse.SynapseData.GetArrowLeftX(synapse.Id), synapse.SynapseData.GetArrowLeftY(synapse.Id));
            _arrowPath.LineTo(synapse.SynapseData.GetArrowRightX(synapse.Id), synapse.SynapseData.GetArrowRightY(synapse.Id));
            _arrowPath.LineTo(synapse.SynapseData.GetArrowEndX(synapse.Id), synapse.SynapseData.GetArrowEndY(synapse.Id));
            _arrowPath.Close();


            canvas.DrawPath(_arrowPath, neuron.ConnectedSynapses.Count >= 30 ? _largeNetArrowPaint : _arrowPaint);
        }

        public void DrawSynapseLabel(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron,
            SkSynapseView synapse)
        {
            var canvas = network.PaintArgs.Surface.Canvas;

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