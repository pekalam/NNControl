using NNControl.Neuron;

namespace NNControl.Synapse
{
    public class SynapseController
    {
        public SynapseController(NeuronController neuron1, NeuronController neuron2, SynapseView view)
        {
            Neuron1 = neuron1;
            Neuron2 = neuron2;
            View = view;
            View.Neuron1 = neuron1.View;
            View.Neuron2 = neuron2.View;
        }

        internal NeuronController Neuron1;
        internal NeuronController Neuron2;
        internal SynapseView View;

        internal void SetArrowPos()
        {
            var dst = MathHelpers.Distance(Neuron1.View.X, Neuron1.View.Y, Neuron2.View.X, Neuron2.View.Y);

            var arrowLen = Neuron1.Layer.Network.NeuralNetworkModel.SynapseSettings.ArrowLength;

            var l = Neuron1.Layer.Network.NeuralNetworkModel.NeuronRadius / dst;
            var vp = (x: (Neuron2.View.X - Neuron1.View.X) * l, y: (Neuron2.View.Y - Neuron1.View.Y) * l);
            var arrowEnd = (x: Neuron2.View.X - vp.x, y: Neuron2.View.Y - vp.y);

            var l2 = (Neuron1.Layer.Network.NeuralNetworkModel.NeuronRadius + arrowLen) / dst;
            var vp2 = (x: (Neuron2.View.X - Neuron1.View.X) * l2,
                y: (Neuron2.View.Y - Neuron1.View.Y) * l2);

            var arrowBeg = (x: Neuron2.View.X - vp2.x, y: Neuron2.View.Y - vp2.y);

            var dif = (x: arrowBeg.x - arrowEnd.x, y: arrowBeg.y - arrowEnd.y);

            View.ArrowLeftEnd = (x: arrowBeg.x + dif.y, y: arrowBeg.y - dif.x);
            View.ArrowRightEnd = (x: arrowBeg.x - dif.y, y: arrowBeg.y + dif.x);
            View.ArrowEnd = arrowEnd;
            View.ArrowBeg = arrowBeg;
        }
    }
}