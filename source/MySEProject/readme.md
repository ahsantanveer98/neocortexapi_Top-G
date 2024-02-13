Learning to implement Learning Sequence and prediction method.
Reading literature view to understand Multi sequence learning.
Tring to run Multisequence code in VS
understanding this part of code
studying prediction modeling

public class MultiSequenceLearning
{
    /// <summary>
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
  {  
    private static void RunMultiSimpleSequenceLearningExperiment()
{
    Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

    sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
    sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));
    sequences.Add("S3", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
    sequences.Add("S4", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));
    sequences.Add("S5", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
    sequences.Add("S6", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));


    //
    // Prototype for building the prediction engine.
    MultiSequenceLearning experiment = new MultiSequenceLearning();
    var predictor = experiment.Run(sequences);
}
