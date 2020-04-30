using System.Collections.Generic;
using NeuralNetworkControl.Model;

namespace NeuralNetworkControl.Impl
{
    public abstract class LayerViewImpl
    {
        public List<NeuronViewImpl> Neurons { get; } = new List<NeuronViewImpl>();
        public LayerModel LayerModel { get; set; }
        public virtual float X { get; set; }
        public virtual float Y { get; set; }

        public LayerViewImpl PreviousLayer { get; set; }
        public NeuralNetworkViewImpl Network { get; set; }
        public int Number { get; set; }

        public abstract NeuronViewImpl CreateNeuronInstance();
        public abstract void OnRepositioned();
        public abstract void OnZoomChanged();
    }
}