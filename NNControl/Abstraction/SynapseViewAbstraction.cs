using NeuralNetworkControl.Impl;
using NNLib;

namespace NeuralNetworkControl.Abstraction
{
    public class SynapseViewAbstraction
    {
        public SynapseViewAbstraction(NeuronViewAbstraction neuron1, NeuronViewAbstraction neuron2, SynapseViewImpl impl)
        {
            Neuron1 = neuron1;
            Neuron2 = neuron2;
            Impl = impl;
            Impl.Neuron1 = neuron1.Impl;
            Impl.Neuron2 = neuron2.Impl;
        }

        internal NeuronViewAbstraction Neuron1 { get; private set; }
        internal NeuronViewAbstraction Neuron2 { get; private set; }
        internal SynapseViewImpl Impl { get; private set; }

        internal void SetArrowPos()
        {
            var dst = MathHelpers.Distance(Neuron1.Impl.X, Neuron1.Impl.Y, Neuron2.Impl.X, Neuron2.Impl.Y);

            var arrowLen = 3f;

            var l = Neuron1.Layer.Network.NeuralNetworkModel.NeuronRadius / dst;
            var vp = (x: (Neuron2.Impl.X - Neuron1.Impl.X) * l, y: (Neuron2.Impl.Y - Neuron1.Impl.Y) * l);
            var arrowEnd = (x: Neuron2.Impl.X - vp.x, y: Neuron2.Impl.Y - vp.y);

            var l2 = (Neuron1.Layer.Network.NeuralNetworkModel.NeuronRadius + arrowLen) / dst;
            var vp2 = (x: (Neuron2.Impl.X - Neuron1.Impl.X) * l2,
                y: (Neuron2.Impl.Y - Neuron1.Impl.Y) * l2);

            var arrowBeg = (x: Neuron2.Impl.X - vp2.x, y: Neuron2.Impl.Y - vp2.y);

            var dif = (x: arrowBeg.x - arrowEnd.x, y: arrowBeg.y - arrowEnd.y);

            Impl.ArrowLeftEnd = (x: arrowBeg.x + dif.y, y: arrowBeg.y - dif.x);
            Impl.ArrowRightEnd = (x: arrowBeg.x - dif.y, y: arrowBeg.y + dif.x);
            Impl.ArrowEnd = arrowEnd;
        }
    }
}