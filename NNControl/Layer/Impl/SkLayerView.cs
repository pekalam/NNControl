﻿using NNControl.Neuron;
using NNControl.Neuron.Impl;

namespace NNControl.Layer.Impl
{
    internal class SkLayerView : LayerViewImpl
    {
        public SkLayerView() : base()
        {
        }

        public override NeuronViewImpl CreateNeuronInstance()
        {
            var neuron = new SkNeuronView(new SkInternalNeuronPainter(Network.NeuralNetworkModel.NeuronSettings));
            return neuron;
        }

        public override void OnRepositioned()
        {
        }

        public override void OnZoomChanged()
        {
            
        }
    }
}