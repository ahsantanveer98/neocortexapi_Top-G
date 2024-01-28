INitial Task:
First I have read and understood how Learning Sequence Work.

Secondly:

I have analyzed and understood the existing code and understand how learning sequences and predictions work.

Understanding Pridiction code.
        /// Runs the learning of sequences.
        /// </summary>
        /// <param name="sequences">Dictionary of sequences. KEY is the sewuence name, the VALUE is th elist of element of the sequence.</param>
        public Predictor Run(Dictionary<string, List<double>> sequences)
        {
            Console.WriteLine($"Hello NeocortexApi! Experiment {nameof(MultiSequenceLearning)}");

            int inputBits = 100;
            int numColumns = 1024;

            HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
            {
                Random = new ThreadSafeRandom(42),

                CellsPerColumn = 25,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 0.02 * numColumns,
                PotentialRadius = (int)(0.15 * inputBits),
                //InhibitionRadius = 15,

                MaxBoost = 10.0,
                DutyCyclePeriod = 25,
                MinPctOverlapDutyCycles = 0.75,
                MaxSynapsesPerSegment = (int)(0.02 * numColumns),

                ActivationThreshold = 15,
                ConnectedPermanence = 0.5,

                // Learning is slower than forgetting in this case.
                PermanenceDecrement = 0.25,
                PermanenceIncrement = 0.15,

                // Used by punishing of segments.
                PredictedSegmentDecrement = 0.1
            };

            double max = 20;

            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "W", 15},
                { "N", inputBits},
                { "Radius", -1.0},
                { "MinVal", 0.0},
                { "Periodic", false},
                { "Name", "scalar"},
                { "ClipInput", false},
                { "MaxVal", max}
            };

            EncoderBase encoder = new ScalarEncoder(settings);

            return RunExperiment(inputBits, cfg, encoder, sequences);
        }

