using NNControl.Layer.Impl;
using NNControl.Model;
using NNControl.Network.Impl;
using NNControl.Neuron.Impl;
using SkiaSharp;

namespace NNControl.Synapse.Impl
{
    internal class SkSynapsePainter
    {
        private static readonly SKPaint _synapseLabelPaint = new SKPaint();
        private static readonly SKPaint _selectedSynapsePaint = new SKPaint();
        private static readonly SKPaint _arrowPaint = new SKPaint();
        private static readonly SKPaint _largeNetArrowPaint = new SKPaint();

        private static readonly SKPath _arrowPath = new SKPath();

        private readonly SKPaint _synapsePaint = new SKPaint();
        private readonly SKPaint _largeNetSynPaint = new SKPaint();


        private static bool _initialized;

        public SkSynapsePainter(SynapseSettings settings)
        {
            if (!_initialized)
            {
                _synapseLabelPaint.Color = SKColor.Parse(settings.LabelColor);
                _synapseLabelPaint.IsAntialias = true;

                _selectedSynapsePaint.Color = SKColor.Parse(settings.SelectedColor);
                _selectedSynapsePaint.IsAntialias = true;
                _selectedSynapsePaint.StrokeCap = SKStrokeCap.Butt;


                _arrowPaint.Style = _largeNetArrowPaint.Style = SKPaintStyle.Fill;
                _arrowPaint.Color = SKColor.Parse(settings.ArrowColor);
                _arrowPaint.IsAntialias = _largeNetArrowPaint.IsAntialias = true;

                _largeNetArrowPaint.Color = SKColor.Parse(settings.LargetNetColor);

                _initialized = true;
            }

            _synapsePaint.Color = SKColor.Parse(settings.Color);
            _synapsePaint.IsAntialias = true;
            _synapsePaint.StrokeCap = SKStrokeCap.Butt;
            _synapsePaint.StrokeCap = SKStrokeCap.Butt;
            _synapsePaint.StrokeWidth = _selectedSynapsePaint.StrokeWidth = settings.StrokeWidth;

            _largeNetSynPaint.Color = SKColor.Parse(settings.LargetNetColor);
            _largeNetSynPaint.IsAntialias = true;
            _largeNetSynPaint.StrokeCap = SKStrokeCap.Butt;
        }

        public void SetColor(in SKColor color)
        {
            _synapsePaint.Color = _largeNetSynPaint.Color = color;
        }

        public void Draw(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron, SkSynapseView synapse, SKCanvas canvas)
        {
            SKPaint synapsePaint = neuron.ConnectedSynapses.Count >= 30 ? _largeNetSynPaint : _synapsePaint;



            canvas.DrawLine(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.ArrowBeg.x, synapse.ArrowBeg.y,
                synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsePaint);


            // canvas.DrawLine(synapse.Neuron1.X, synapse.Neuron1.Y, synapse.Neuron2.X, synapse.Neuron2.Y,
            //     synapse == network.SelectedSynapse ? _selectedSynapsePaint : synapsPaint);

            _arrowPath.Rewind();

            _arrowPath.MoveTo(synapse.ArrowEnd.x, synapse.ArrowEnd.y);
            _arrowPath.LineTo(synapse.ArrowLeftEnd.x, synapse.ArrowLeftEnd.y);
            _arrowPath.LineTo(synapse.ArrowRightEnd.x, synapse.ArrowRightEnd.y);
            _arrowPath.LineTo(synapse.ArrowEnd.x, synapse.ArrowEnd.y);
            _arrowPath.Close();

            canvas.DrawPath(_arrowPath, neuron.ConnectedSynapses.Count >= 30 ? _largeNetArrowPaint : _arrowPaint);
        }

        public void DrawSynapseLabel(SkNeuralNetworkView network, SkLayerView layer, SkNeuronView neuron,
            SkSynapseView synapse, SKCanvas canvas)
        {

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