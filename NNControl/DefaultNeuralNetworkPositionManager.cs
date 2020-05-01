using System;
using System.Diagnostics;
using NNControl.Layer;
using NNControl.Network;
using NNControl.Neuron;

namespace NNControl
{
    public class DefaultNeuralNetworkPositionManager : NeuralNetworkPositionManagerBase
    {
        public override float GetLayerX(NeuralNetworkController network, int layerNum)
        {
            Debug.Assert(network != null);

            return network.NeuralNetworkModel.Padding.Left +
                   (2 * network.NeuralNetworkModel.NeuronRadius + network.NeuralNetworkModel.LayerXSpaceBetween) *
                   layerNum + network.ViewportPosition.Left;
        }

        public override float GetLayerY(NeuralNetworkController network, int layerNum)
        {
            Debug.Assert(network != null);

            return network.NeuralNetworkModel.Padding.Top + network.ViewportPosition.Top;
        }

        public override float GetNeuronX(NeuralNetworkController network, LayerView layer,
            NeuronView neuron)
        {
            Debug.Assert(network != null);
            Debug.Assert(layer != null);
            Debug.Assert(neuron != null);

            return layer.X;
        }

        public override float GetNeuronY(NeuralNetworkController network, LayerView layer,
            NeuronView neuron)
        {
            Debug.Assert(network != null);
            Debug.Assert(layer != null);
            Debug.Assert(neuron != null);

            var offsetFromPrev = (neuron.NumberInLayer * (2 * network.NeuralNetworkModel.NeuronRadius +
                                                          network.NeuralNetworkModel.LayerYSpaceBetween));
            return layer.Y + offsetFromPrev +
                   network.NeuralNetworkModel.LayerYSpaceBetween;
        }

        private void CenterPositions(NeuralNetworkController network)
        {
            int CalcLayerHeight(int i) =>
                network.Layers[i].Neurons.Count * (network.NeuralNetworkModel.NeuronRadius * 2) +
                (network.NeuralNetworkModel.LayerYSpaceBetween * network.Layers[i].Neurons.Count - 1);


            float netWidth = (network.Layers[^1].View.X + network.NeuralNetworkModel.NeuronRadius) - network.Layers[0].View.X;
            float newScale = 0f;
            float netStart = 0f;
            int maxHeight = 0;
            for (int i = 0; i < network.Layers.Count; i++)
            {
                var currentHeight = CalcLayerHeight(i);
                maxHeight = Math.Max(currentHeight, maxHeight);
            }

            
            if (maxHeight > network.CanvasHeight)
            {
                var scale = -(1 - network.CanvasHeight / maxHeight);
                newScale = Math.Min(scale, newScale);
            }
            if (netWidth > network.CanvasWidth)
            {
                var scale = -(1 - network.CanvasWidth / netWidth);
                newScale = Math.Min(scale, newScale);
            }
            else
            {
                netStart = (network.CanvasWidth - netWidth) / 2f;
            }


            for (int i = 0; i < network.Layers.Count; i++)
            {
                float dy = (network.CanvasHeight - CalcLayerHeight(i)) / 2f - network.NeuralNetworkModel.NeuronRadius * 2
                                                                            - network.NeuralNetworkModel.LayerYSpaceBetween;

                foreach (var neuron in network.Layers[i].Neurons)
                {
                    neuron.Move(netStart - neuron.View.X + (neuron.View.X - network.Layers[0].View.X), dy);
                }
            }

            if (network.CanvasWidth > 0 && network.CanvasHeight > 0)
            {
                network.SetZoom(newScale);
            }
        }

        private void AdjustToNotOverlap(NeuralNetworkController network)
        {
            for (int i = 1; i < network.Layers.Count; i++)
            {
                var prevLayer = network.Layers[i - 1].View;

                if (prevLayer.Neurons.Count <= 2)
                {
                    continue;
                }

                var p1 = (x: prevLayer.Neurons[^1].X,
                    y: prevLayer.Neurons[^1].Y);
                var p2 = (x: network.Layers[i].View.Neurons[0].X, y: network.Layers[i].View.Neurons[0].Y);

                var s = (x: prevLayer.Neurons[^2].X,
                    y: prevLayer.Neurons[^2].Y);


                var u = ((s.x - p1.x) * (p2.x - p1.x) + (s.y - p1.y) * (p2.y - p1.y)) /
                        (float) (Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));

                var s2 = (x: p1.x + (p2.x - p1.x) * u, y: p1.y + (p2.y - p1.y) * u);

                var sDist = MathHelpers.Distance(s2, s);
                var offsetX = network.NeuralNetworkModel.NeuronRadius - sDist;

                if (offsetX > 0)
                {
                    var tp2 = (x: p2.x + offsetX, p2.y);
                    var tu = ((s.x - p1.x) * (tp2.x - p1.x) + (s.y - p1.y) * (tp2.y - p1.y)) /
                             (float) (Math.Pow(p1.x - tp2.x, 2) + Math.Pow(p1.y - tp2.y, 2));

                    var ts2 = (x: p1.x + (tp2.x - p1.x) * tu, y: p1.y + (tp2.y - p1.y) * tu);

                    var tsDist = MathHelpers.Distance(ts2, s);

                    var mv = tsDist - sDist;

                    var xx = offsetX * offsetX / mv;


                    for (int j = i; j < network.Layers.Count; j++)
                    {
                        network.Layers[j].View.X += xx;
                        foreach (var neuron in network.Layers[j].Neurons)
                        {
                            neuron.Move((int)xx, 0);
                        }
                    }
                }
            }
        }

        public override void InvokeActionsAfterPositionsSet(NeuralNetworkController network)
        {
            if (network.Layers.Count == 0)
            {
                return;
            }

            AdjustToNotOverlap(network);
            CenterPositions(network);
        }
    }
}