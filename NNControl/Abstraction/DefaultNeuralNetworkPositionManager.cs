using System;
using System.Diagnostics;
using NeuralNetworkControl.Impl;

namespace NeuralNetworkControl.Abstraction
{
    public class DefaultNeuralNetworkPositionManager : NeuralNetworkPositionManagerBase
    {
        public override float GetLayerX(NeuralNetworkViewAbstraction network, int layerNum)
        {
            Debug.Assert(network != null);

            return network.NeuralNetworkModel.Padding.Left +
                   (2 * network.NeuralNetworkModel.NeuronRadius + network.NeuralNetworkModel.LayerXSpaceBetween) *
                   layerNum + network.ViewportPosition.Left;
        }

        public override float GetLayerY(NeuralNetworkViewAbstraction network, int layerNum)
        {
            Debug.Assert(network != null);

            return network.NeuralNetworkModel.Padding.Top + network.ViewportPosition.Top;
        }

        public override float GetNeuronX(NeuralNetworkViewAbstraction network, LayerViewImpl layer,
            NeuronViewImpl neuron)
        {
            Debug.Assert(network != null);
            Debug.Assert(layer != null);
            Debug.Assert(neuron != null);

            return layer.X;
        }

        public override float GetNeuronY(NeuralNetworkViewAbstraction network, LayerViewImpl layer,
            NeuronViewImpl neuron)
        {
            Debug.Assert(network != null);
            Debug.Assert(layer != null);
            Debug.Assert(neuron != null);

            var offsetFromPrev = (neuron.NumberInLayer * (2 * network.NeuralNetworkModel.NeuronRadius +
                                                          network.NeuralNetworkModel.LayerYSpaceBetween));
            return layer.Y + offsetFromPrev +
                   network.NeuralNetworkModel.LayerYSpaceBetween;
        }

        private void CenterPositions(NeuralNetworkViewAbstraction network)
        {
            int CalcLayerHeight(int i) =>
                network.Layers[i].Neurons.Count * (network.NeuralNetworkModel.NeuronRadius * 2) +
                (network.NeuralNetworkModel.LayerYSpaceBetween * network.Layers[i].Neurons.Count -
                 1);

            int maxHeight = 0;
            int maxInd = 0;
            for (int i = 0; i < network.Layers.Count; i++)
            {
                var currentHeight = CalcLayerHeight(i);
                maxInd = currentHeight > maxHeight ? i : maxInd;
                maxHeight = currentHeight > maxHeight ? currentHeight : maxHeight;
            }


            for (int i = 0; i < network.Layers.Count; i++)
            {
                if (i == maxInd)
                {
                    continue;
                }

                var dy = (maxHeight - CalcLayerHeight(i)) / 2f;
                network.Layers[i].Impl.Y += dy;
                foreach (var neuron in network.Layers[i].Neurons)
                {
                    neuron.Move(0, dy);
                }
            }


        }

        private void AdjustToNotOverlap(NeuralNetworkViewAbstraction network)
        {
            for (int i = 1; i < network.Layers.Count; i++)
            {
                var prevLayer = network.Layers[i - 1].Impl;

                if (prevLayer.Neurons.Count <= 2)
                {
                    continue;
                }

                var p1 = (x: prevLayer.Neurons[^1].X,
                    y: prevLayer.Neurons[^1].Y);
                var p2 = (x: network.Layers[i].Impl.Neurons[0].X, y: network.Layers[i].Impl.Neurons[0].Y);

                var s = (x: prevLayer.Neurons[^2].X,
                    y: prevLayer.Neurons[^2].Y);


                var u = ((s.x - p1.x) * (p2.x - p1.x) + (s.y - p1.y) * (p2.y - p1.y)) /
                        (float) (Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));

                var s2 = (x: p1.x + (p2.x - p1.x) * u, y: p1.y + (p2.y - p1.y) * u);

                var sDist = MathHelpers.Distance(s2, s);
                var offsetX = network.NeuralNetworkModel.NeuronRadius - sDist;

                if (offsetX > 0)
                {
                    var tp2 = (x: p2.x + offsetX, y: p2.y);
                    var tu = ((s.x - p1.x) * (tp2.x - p1.x) + (s.y - p1.y) * (tp2.y - p1.y)) /
                             (float) (Math.Pow(p1.x - tp2.x, 2) + Math.Pow(p1.y - tp2.y, 2));

                    var ts2 = (x: p1.x + (tp2.x - p1.x) * tu, y: p1.y + (tp2.y - p1.y) * tu);

                    var tsDist = MathHelpers.Distance(ts2, s);

                    var mv = tsDist - sDist;

                    var xx = offsetX * offsetX / mv;


                    for (int j = i; j < network.Layers.Count; j++)
                    {
                        network.Layers[j].Impl.X += xx;
                        foreach (var neuron in network.Layers[j].Neurons)
                        {
                            neuron.Move((int)xx, 0);
                            //neuron.Impl.X += (int) xx;
                        }
                    }
                }
            }
        }

        public override void InvokeActionsAfterPositionsSet(NeuralNetworkViewAbstraction network)
        {
            AdjustToNotOverlap(network);
            CenterPositions(network);
        }
    }
}