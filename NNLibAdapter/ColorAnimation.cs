using System.Collections.Generic;
using System.Diagnostics;
using NNControl.Network;
using NNLib;

namespace NNLibAdapter
{
    public class ColorAnimation
    {
        private NeuralNetworkController _controller;
        private MLPTrainer _trainer;

        public ColorAnimation(NeuralNetworkController controller)
        {
            _controller = controller;
        }

        public void SetupTrainer(MLPTrainer trainer)
        {
            _trainer = trainer;
            _trainer.EpochEnd += TrainerEpochEnd;
        }

        
        private (double mingz, double maxgz, double minlz, double maxlz) FindMinMaxW(IReadOnlyList<Layer> layers)
        {
            double mingz = 0, maxgz = 0, minlz = 0, maxlz = 0;
            foreach (var layer in layers)
            {
                for (int i = 0; i < layer.Weights.RowCount; i++)
                {
                    for (int j = 0; j < layer.Weights.ColumnCount; j++)
                    {
                        var el = layer.Weights[i, j];
                        if (el < 0)
                        {
                            if (el < minlz)
                            {
                                minlz = el;
                            }

                            if (el > maxlz)
                            {
                                maxlz = el;
                            }
                        }
                        else
                        {
                            if (el < mingz)
                            {
                                mingz = el;
                            }

                            if (el > maxgz)
                            {
                                maxgz = el;
                            }

                        }
                        
                    }
                }
            }

            return (mingz, maxgz, minlz, maxlz);
        }

        private (double mingz, double maxgz, double minlz, double maxlz) FindMinMaxB(IReadOnlyList<Layer> layers)
        {
            double mingz = 0, maxgz = 0, minlz = 0, maxlz = 0;
            foreach (var layer in layers)
            {
                for (int i = 0; i < layer.Biases.RowCount; i++)
                {
                    var el = layer.Biases[i, 0];
                    if (el < 0)
                    {
                        if (el < minlz)
                        {
                            minlz = el;
                        }

                        if (el > maxlz)
                        {
                            maxlz = el;
                        }
                    }
                    else
                    {
                        if (el < mingz)
                        {
                            mingz = el;
                        }

                        if (el > maxgz)
                        {
                            maxgz = el;
                        }

                    }
                }
            }

            return (mingz, maxgz, minlz, maxlz);
        }

        public int Scale(double x, double minx, double maxx, int miny, int maxy, int start = 0)
        {
            if (!double.IsFinite(x))
            {
                return 127;
            }
            var s = (int) ((x - minx) * (maxy - miny) / (maxx - minx)) + start;
            //Debug.WriteLine($"{s} for {x} max:{maxx} min: {minx}");
            // if (s >= 126 || s <= 128)
            // {
            //     Debug.WriteLine($"white for {x} max:{maxx} min: {minx}");
            // }
            return s;
        }


        private void TrainerEpochEnd()
        {
            var color = _controller.Color;

            var layers = _trainer.Network.Layers;

            var (mingz, maxgz, minlz, maxlz) = FindMinMaxW(layers);
            var (minbgz, maxbgz, minblz, maxblz) = FindMinMaxB(layers);


            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 0; j < layers[i].Weights.RowCount; j++)
                {
                    for (int k = 0; k < layers[i].Weights.ColumnCount; k++)
                    {
                        var el = layers[i].Weights[j, k];
                        if (el < 0)
                        {
                            color.SetSynapseColor(i + 1, j, k, Scale(el, minlz, 0, 0, 127));
                        }
                        else
                        {
                            color.SetSynapseColor(i + 1, j, k, Scale(el, 0, maxgz, 0, 127, 128));
                        }
                    }
                }
            }


            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 0; j < layers[i].Biases.RowCount; j++)
                {
                    var el = layers[i].Biases[j, 0];
                    if (el < 0)
                    {
                        color.SetNeuronColor(i + 1, j, Scale(el, minblz, 0, 0, 127));
                    }
                    else
                    {
                        color.SetNeuronColor(i + 1, j, Scale(el, 0, maxbgz, 0, 127,128));
                    }
                }
            }


            color.ApplyColors();
        }
    }
}