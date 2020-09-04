using System;
using NNControl.Neuron;

namespace NNControl.Network
{
    public class ColorManager
    {
        private readonly NeuralNetworkController _controller;

        public ColorManager(NeuralNetworkController controller)
        {
            _controller = controller;
        }

        public void SetNeuronColor(int layerNumber, int numberInLayer, string hexColor)
        {
            _controller.Layers[layerNumber].Neurons[numberInLayer].View.ResetColor(hexColor);
        }

        public void SetNeuronColor(int number, string hexColor)
        {
            NeuronController neuron = null;
            foreach (var layer in _controller.Layers)
            {
                foreach (var n in layer.Neurons)
                {
                    if (n.View.Number == number)
                    {
                        neuron = n;
                        break;
                    }
                }
            }

            neuron?.View.ResetColor(hexColor);
        }

        public void SetSynapseColor(int layerNumber, int neuronNumberInLayer, int numberInNeuron, string hexColor)
        {
            if (_controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses.Count == 0)
            {
                return;
            }

            _controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses[numberInNeuron].View
                .ResetColor(hexColor);
        }


        public void SetNeuronColor(int layerNumber, int numberInLayer, int scale)
        {
            _controller.Layers[layerNumber].Neurons[numberInLayer].View.SetColor(scale);
        }

        public void SetNeuronColor(int number, int scale)
        {
            throw new NotImplementedException();
        }

        public void SetSynapseColor(int layerNumber, int neuronNumberInLayer, int numberInNeuron, int scale)
        {
            if (_controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses.Count == 0)
            {
                return;
            }

            _controller.Layers[layerNumber].Neurons[neuronNumberInLayer].Synapses[numberInNeuron].View
                .SetColor(scale);
        }

        public void ResetColorsToDefault()
        {
            foreach (var layer in _controller.Layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    neuron.View.ResetColor(_controller.NeuralNetworkModel.NeuronSettings.Color);
                    foreach (var synapse in neuron.Synapses)
                    {
                        synapse.View.ResetColor(_controller.NeuralNetworkModel.SynapseSettings.Color);
                    }
                }
            }

            ApplyColors();
        }

        public void ApplyColors()
        {
            _controller.RequestRedraw(NeuralNetworkController.ViewTrig.FORCE_DRAW);
        }
    }
}