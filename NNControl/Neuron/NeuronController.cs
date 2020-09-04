using NNControl.Layer;
using NNControl.Synapse;
using System.Collections.Generic;

namespace NNControl.Neuron
{
    public class NeuronController
    {
        /// <summary>
        /// Begining in neuron
        /// </summary>
        internal readonly List<SynapseController> ConnectedSynapses = new List<SynapseController>();
        /// <summary>
        /// Ending in neuron
        /// </summary>
        internal readonly List<SynapseController> Synapses = new List<SynapseController>();

        public NeuronController(int numberInLayer, NeuronView view, LayerController layer)
        {
            View = view;
            Layer = layer;
            View.NumberInLayer = numberInLayer;
            View.Layer = layer.View;
            View.NeuronModel = layer.LayerModel.NeuronModels[numberInLayer];
            CreateNeuron();
        }

        internal NeuronView View;
        internal LayerController Layer;

        private void CreateNeuron()
        {
            View.X = Layer.Network.PositionManager.GetNeuronX(Layer.Network, Layer.View, View);
            View.Y = Layer.Network.PositionManager.GetNeuronY(Layer.Network, Layer.View, View);
            View.OnPositionSet();
            View.Y = y;
        }

        public SynapseController GetSynapse(int number)
        {
            return Synapses[number];
        }

        public int TotalSynapses => Synapses.Count;

        internal void AddSynapse(NeuronController neuron2)
        {
            var newSynapse = View.CreateSynapseImpl();
            var newSynapseAbstr = new SynapseController(this, neuron2, newSynapse);

            newSynapse.NumberInNeuron = neuron2.Synapses.Count;

            //neuron2.ConnectedSynapses.Add(newSynapseAbstr);
            //neuron2.View.ConnectedSynapses.Add(newSynapse);

            neuron2.Synapses.Add(newSynapseAbstr);
            neuron2.View.Synapses.Add(newSynapse);


            ConnectedSynapses.Add(newSynapseAbstr);
            View.ConnectedSynapses.Add(newSynapse);

            //Synapses.Add(newSynapseAbstr);
            //View.Synapses.Add(newSynapse);
            //View.NeuronModel.SynapsesLabels.Add("1");
        }

        internal void Move(float dx, float dy)
        {
            View.X += dx;
            View.Y += dy;
            View.OnPositionSet();
            foreach (var synapse in Synapses)
            {
                synapse.SetArrowPos();
            }
            foreach (var synapse in ConnectedSynapses)
            {
                synapse.SetArrowPos();
            }
        }

        internal void RemoveSynapseTo(NeuronController neuron2)
        {
            Synapses.RemoveAll(s => s.Neuron2 == neuron2);
            View.Synapses.RemoveAll(s => s.Neuron2 == neuron2.View);

            ConnectedSynapses.RemoveAll(s => s.Neuron2 == neuron2);
            View.ConnectedSynapses.RemoveAll(s => s.Neuron2 == neuron2.View);
        }

        internal void RemoveSynapseFrom(NeuronController neuron1)
        {
            Synapses.RemoveAll(s => s.Neuron1 == neuron1);
            View.Synapses.RemoveAll(s => s.Neuron1 == neuron1.View);

            ConnectedSynapses.RemoveAll(s => s.Neuron1 == neuron1);
            View.ConnectedSynapses.RemoveAll(s => s.Neuron1 == neuron1.View);
        }

        public void Reposition()
        {
            View.X = Layer.Network.PositionManager.GetNeuronX(Layer.Network, Layer.View, View);
            View.Y = Layer.Network.PositionManager.GetNeuronY(Layer.Network, Layer.View, View);
            View.OnPositionSet();
            View.OnRepositioned();
            foreach (var synapse in Synapses)
            {
                synapse.SetArrowPos();
            }
        }
    }
}