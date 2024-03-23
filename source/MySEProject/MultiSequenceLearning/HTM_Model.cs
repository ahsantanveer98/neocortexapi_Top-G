using NeoCortexApi.Classifiers;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using System;

namespace ProjectMultiSequenceLearning
{
    public class HTM_Model
    {
       public  CortexLayer<object, object> CortexLayer { get; set; }

        public HtmClassifier<string, ComputeCycle> HtmClassifier { get; set; }
    }
}