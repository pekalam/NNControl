using System.Collections.Generic;
using NeuralNetworkControl.Impl;

namespace NeuralNetworkControl.Abstraction
{
    public class NeuronViewAbstraction
    {

        internal readonly List<SynapseViewAbstraction> ConnectedSynapses = new List<SynapseViewAbstraction>();
        internal readonly List<SynapseViewAbstraction> Synapses = new List<SynapseViewAbstraction>();

        public NeuronViewAbstraction(int numberInLayer, NeuronViewImpl impl, LayerViewAbstraction layer)
        {
            Impl = impl;
            Layer = layer;
            Impl.NumberInLayer = numberInLayer;
            Impl.Layer = layer.Impl;
            Impl.NeuronModel = layer.LayerModel.NeuronModels[numberInLayer];
            CreateNeuron();
        }

        internal NeuronViewImpl Impl { get; private set; }
        internal LayerViewAbstraction Layer { get; private set; }

        private void CreateNeuron()
        {
            var x = (int)Layer.Network.PositionManager.GetNeuronX(Layer.Network, Layer.Impl, Impl);
            var y = (int)Layer.Network.PositionManager.GetNeuronY(Layer.Network, Layer.Impl, Impl);
            Impl.X = x;
            Impl.Y = y;
        }

        internal void AddSynapse(NeuronViewAbstraction neuron2)
        {
            var newSynapse = Impl.CreateSynapseImpl();
            var newSynapseAbstr = new SynapseViewAbstraction(this, neuron2, newSynapse);

            newSynapse.NumberInNeuron = neuron2.Synapses.Count;

            neuron2.ConnectedSynapses.Add(newSynapseAbstr);
            neuron2.Impl.ConnectedSynapses.Add(newSynapse);

            neuron2.Synapses.Add(newSynapseAbstr);
            neuron2.Impl.Synapses.Add(newSynapse);

            neuron2.Impl.NeuronModel.SynapsesLabels.Add("");

            ConnectedSynapses.Add(newSynapseAbstr);
            Impl.ConnectedSynapses.Add(newSynapse);
        }

        private void SetArrowPos()
        {
            if (Layer.PreviousLayer != null)
            {
                foreach (var synapse in ConnectedSynapses)
                {
                    synapse.SetArrowPos();
                }
            }
        }

        internal void Move(float dx, float dy)
        {
            Impl.X += dx;
            Impl.Y += dy;
            SetArrowPos();
        }

        internal void RemoveSynapseTo(NeuronViewAbstraction neuron2)
        {
            ConnectedSynapses.RemoveAll(s => s.Neuron2 == neuron2);
            Impl.ConnectedSynapses.RemoveAll(s => s.Neuron2 == neuron2.Impl);
        }

        internal void RemoveSynapseFrom(NeuronViewAbstraction neuron1)
        {
            Synapses.RemoveAll(s => s.Neuron1 == neuron1);
            ConnectedSynapses.RemoveAll(s => s.Neuron1 == neuron1);
            Impl.ConnectedSynapses.RemoveAll(s => s.Neuron1 == neuron1.Impl);
            Impl.Synapses.RemoveAll(s => s.Neuron1 == neuron1.Impl);
        }

        public void Reposition()
        {
            Impl.X = (int)Layer.Network.PositionManager.GetNeuronX(Layer.Network, Layer.Impl, Impl);
            Impl.Y = (int)Layer.Network.PositionManager.GetNeuronY(Layer.Network, Layer.Impl, Impl);
            Impl.OnRepositioned();
            SetArrowPos();
        }

        public void OnZoomChanged()
        {
            Impl.OnZoomChanged();
        }
    }
}