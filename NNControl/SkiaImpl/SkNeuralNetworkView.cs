﻿using NeuralNetworkControl.Impl;
using NeuralNetworkControl.Model;
using SkiaSharp;

namespace NeuralNetworkControl.SkiaImpl
{
    internal class SkNeuralNetworkView : NeuralNetworkViewImpl
    {
        private SKImage _saved;

        private NeuralNetworkModel _neuralNetworkModel;
        private SkInternalNetworkLayerPainter _networkLayerPainter;
        private SkInternalNeuronPainter _neuronPainter;
        private SkInternalSynapsePainter _synapsePainter;


        public SkNeuralNetworkView()
        {
            
        }

        public override NeuralNetworkModel NeuralNetworkModel
        {
            get => _neuralNetworkModel;
            set
            {
                _neuralNetworkModel = value;
                _neuronPainter = new SkInternalNeuronPainter(NeuralNetworkModel.NeuronColor);
                _synapsePainter = new SkInternalSynapsePainter(NeuralNetworkModel.SynapseColor, NeuralNetworkModel.SynapseLabelColor);
                _networkLayerPainter = new SkInternalNetworkLayerPainter();
            }
        }


        private void ApplyZoom()
        {
            var ctx = SkNetworkPaintContextHolder.Context;

            var m = ctx.e.Surface.Canvas.TotalMatrix;
            m.ScaleX = (float) Zoom + 1;
            m.ScaleY = (float) Zoom + 1;
            ctx.e.Surface.Canvas.SetMatrix(m);
        }

        private void DrawAll()
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            var canvas = ctx.e.Surface.Canvas;

            canvas.Clear(SKColor.Parse(NeuralNetworkModel.BackgroundColor));

            ApplyZoom();

            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            foreach (var synapse in neuron.Synapses)
            {
                if (synapse.Excluded)
                {
                    continue;
                }

                _synapsePainter.Draw(this, layerView as SkLayerView, neuron as SkNeuronView,
                    synapse as SkSynapseView);
            }


            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            {
                if (neuron.Excluded)
                {
                    continue;
                }

                _neuronPainter.Draw(this, layerView as SkLayerView, neuron as SkNeuronView);
            }

        }

        public override LayerViewImpl CreateLayerInstance()
        {
            return new SkLayerView();
        }

        public override void DrawAndSave()
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            DrawAll();
            _saved = ctx.e.Surface.Snapshot();
        }

        public override void DrawFromSaved()
        {
            var ctx = SkNetworkPaintContextHolder.Context;
            ctx.e.Surface.Canvas.DrawImage(_saved, 0, 0);
        }

        public override void DrawExcluded()
        {
            ApplyZoom();

            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            foreach (var synapse in neuron.Synapses)
            {
                if (synapse.Excluded)
                {
                    _synapsePainter.Draw(this, layerView as SkLayerView, neuron as SkNeuronView,
                        synapse as SkSynapseView);
                }
            }


            foreach (var layerView in Layers)
            {
                foreach (var neuron in layerView.Neurons)
                {
                    if (neuron.Excluded)
                    {
                        _neuronPainter.Draw(this, layerView as SkLayerView, neuron as SkNeuronView);
                    }
                }
            }


            foreach (var layerView in Layers)
            foreach (var neuron in layerView.Neurons)
            foreach (var synapse in neuron.Synapses)
            {
                if (synapse.Excluded)
                {
                    _synapsePainter.DrawSynapseLabel(this, layerView as SkLayerView, neuron as SkNeuronView,
                        synapse as SkSynapseView);
                }
            }
        }
    }
}