﻿using System;
using NNControl.Synapse;

namespace NNControl.Model
{
    public class NeuronModel
    {
        public SynapsesLabelsCollection SynapsesLabels { get; internal set; }
        public string Label { get; set; }
    }
}