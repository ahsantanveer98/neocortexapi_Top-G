using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;


using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using System.Text.RegularExpressions;



namespace ProjectMultiSequenceLearning
{
    /// <summary>
    /// Implements an experiment that demonstrates how to learn sequences.
    /// </summary>
    public class MultiSequenceLearning
    {
        /// <summary>
        /// Runs the learning of sequences.
        /// </summary>
        /// <param name="sequences">Dictionary of sequences. KEY is the sequence name, the VALUE is th elist of element of the sequence.</param>
        public Predictor Run(List<Dictionary<string, int[]>> sequences)
        {
            //input bits increase
            int inputBits = 200;
            int numColumns = 1024;

            //Code Refectoring and move HtmConfig in seperate/general class
            HtmConfig cfg = Helper.FetchHTMConfig(inputBits, numColumns);

            //Remove Encoder from parameter, 
            //because we generate encoded SDR data during fetching data from file and update sequence 
            return RunExperiment(inputBits, cfg, sequences);
        }

        /// <summary>
        /// Learning of Provided Sequence of encoded SDR
        /// </summary>
        private Predictor RunExperiment(int inputBits, HtmConfig cfg, List<Dictionary<string, int[]>> sequences)
        {
            int maxCycles = 35;
            int maxMatchCnt = 0;
            //bool classVotingEnabled = true;

            // Starting experiment
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //--------- CONNECTIONS
            var mem = new Connections(cfg);

            // HTM CLASSIFIER
            HtmClassifier<string, ComputeCycle> cls = new HtmClassifier<string, ComputeCycle>();

            // CORTEX LAYER
            CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");

            // HPA IS_IN_STABLE STATE FLAG
            bool isInStableState = false;

            // LEARNING ACTIVATION FLAG
            bool learn = true;

            // NUMBER OF NEW BORN CYCLES
            int newbornCycle = 0;

            // HOMOSTATICPLASTICITY CONTROLLER
            HomeostaticPlasticityController hpa = new HomeostaticPlasticityController(mem, sequences.Count * 150, (isStable, numPatterns, actColAvg, seenInputs) =>
            {
                if (isStable)
                    // Event should be fired when entering the stable state.
                    Debug.WriteLine($"STABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
                else
                    // Ideal SP should never enter unstable state after stable state.
                    Debug.WriteLine($"INSTABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");

                // We are not learning in instable state.
                isInStableState = isStable; //learn = isInStableState = isStable;

                // Clear all learned patterns in the classifier.
                //cls.ClearState();

            }, numOfCyclesToWaitOnChange: 50);

            // SPATIAL POOLER initialization with HomoPlassiticityController using connections.
            SpatialPoolerMT sp = new SpatialPoolerMT(hpa);
            sp.Init(mem);

            // TEMPORAL MEMORY initialization using connections.
            TemporalMemory tm = new TemporalMemory();
            tm.Init(mem);

            //layer1.HtmModules.Add("encoder", encoder);
            // ADDING SPATIAL POOLER TO CORTEX LAYER
            layer1.HtmModules.Add("sp", sp);

            // CONTRAINER FOR Previous Active Columns
            int[] prevActiveCols = new int[0];

            // TRAINING SP till STATBLE STATE IS ACHIEVED
            while (isInStableState == false) // STABLE CONDITION LOOP ::: LOOP - 0
            {
                newbornCycle++;
                Debug.WriteLine($"-------------- Newborn Cycle {newbornCycle} ---------------");
                Console.WriteLine($"-------------- Newborn Cycle {newbornCycle} ---------------");

                foreach (var sequence in sequences) // FOR EACH SEQUENCE IN SEQUNECS LOOP ::: LOOP - 1
                {
                    foreach (var element in sequence) // FOR EACH dictionary containing single sequence Details LOOP ::: LOOP - 2
                    {
                        var observationClass = element.Key; // OBSERVATION LABEL || SEQUENCE LABEL
                        var elementSDR = element.Value; // ALL ELEMENT IN ONE SEQUENCE 

                        Console.WriteLine($"-------------- {observationClass} ---------------");
                        // CORTEX LAYER OUTPUT with elementSDR as INPUT and LEARN = TRUE
                        var lyrOut = layer1.Compute(elementSDR, learn);

                        // IF STABLE STATE ACHIEVED BREAK LOOP - 3
                        if (isInStableState)
                            break;

                    }
                    if (isInStableState)
                        break;
                }
            }

            // Clear all learned patterns in the classifier.
            cls.ClearState();

            // ADDING TEMPORAL MEMEORY to CORTEX LAYER
            layer1.HtmModules.Add("tm", tm);

            //string lastPredictedValue = "-1";
            //double lastCycleAccuracy = 0;
            //double accuracy = 0;

            //List<List<string>> possibleSequence = new List<List<string>>();

            List<string> lastPredictedValues = new List<string>();

            // TRAINING SP+TM TOGETHER
            foreach (var sequence in sequences)  // SEQUENCE LOOP
            {
                //Debug.WriteLine($"-------------- Sequences {sequence.Key} ---------------");
                int cycle = 0;
                int maxPrevInputs = sequence.Count;

                // Set on true if the system has learned the sequence with a maximum acurracy.
                bool isLearningCompleted = false;

                for (int i = 0; i < maxCycles; i++) // MAXCYCLE LOOP 
                {
                    List<string> previousInputs = new List<string>();
                    cycle++;
                    int elementMatches = 0; // ELEMENT IN SEQUENCE MATCHES COUNT
                    Debug.WriteLine($"-------------- Cycle SP+TM{cycle} ---------------");
                    Console.WriteLine($"-------------- Cycle SP+TM {cycle} ---------------");

                    foreach (var elements in sequence) // SEQUENCE DICTIONARY LOOP
                    {
                        Debug.WriteLine($"-------------- Sequence Element {elements.Key} ---------------");
                        //Console.WriteLine($"-------------- Sequence Element {elements.Key} ---------------");
                        var observationLabel = elements.Key; // OBSERVATION LABEl
                        var elementSdr = elements.Value; // ELEMENT SDR LIST FOR A SINGLE SEQUENCE

                        var observationLabels = observationLabel.Split(",");
                        var element = observationLabels[0];

                        var lyrOut = layer1.Compute(elementSdr, learn) as ComputeCycle;
                        if (lyrOut != null)
                        {
                            Debug.WriteLine(string.Join(',', lyrOut.ActivColumnIndicies));
                            var activeColumns = layer1.GetResult("sp") as int[];
                            previousInputs.Add(element);

                            //// In the pretrained SP with HPC, the TM will quickly learn cells for patterns
                            //// In that case the starting sequence 4-5-6 might have the sam SDR as 1-2-3-4-5-6,
                            //// Which will result in returning of 4-5-6 instead of 1-2-3-4-5-6.
                            //// HtmClassifier allways return the first matching sequence. Because 4-5-6 will be as first
                            //// memorized, it will match as the first one.
                            //if (previousInputs.Count < maxPrevInputs)
                            //    continue;

                            // Modification to Generate Key
                            string key = GetKey(previousInputs, element, observationLabel);

                            List<Cell> actCells = new List<Cell>();
                            // Active Cells
                            actCells = (lyrOut.ActiveCells.Count == lyrOut.WinnerCells.Count) ? lyrOut.ActiveCells : lyrOut.WinnerCells;

                            cls.Learn(key, actCells.ToArray());

                            Debug.WriteLine($"Col  SDR: {Helpers.StringifyVector(lyrOut.ActivColumnIndicies)}");
                            Debug.WriteLine($"Cell SDR: {Helpers.StringifyVector(actCells.Select(c => c.Index).ToArray())}");

                            // If the list of predicted values from the previous step contains the currently presenting value,
                            // we have a match.
                            if (lastPredictedValues.Contains(key))
                            {
                                elementMatches++;
                                Debug.WriteLine($"Match. Actual value: {key} - Predicted value: {lastPredictedValues.FirstOrDefault(key)}.");
                            }
                            else
                            {
                                Debug.WriteLine($"Missmatch! Actual value: {key} - Predicted values: {String.Join(',', lastPredictedValues)}");
                            }

                            if (lyrOut.PredictiveCells.Count > 0)
                            {
                                var predictedInputValues = cls.GetPredictedInputValues(lyrOut.PredictiveCells.ToArray(), 5);
                                foreach (var predictedInputValue in predictedInputValues)
                                {
                                    Debug.WriteLine($"Current Input: {key} \t| Predicted Input: {predictedInputValue.PredictedInput} - {predictedInputValue.Similarity}");
                                }

                                lastPredictedValues = predictedInputValues.Select(v => v.PredictedInput).ToList();
                            }
                            else
                            {
                                Debug.WriteLine($"NO CELLS PREDICTED for next cycle.");
                                lastPredictedValues = new List<string>();
                            }
                        }
                    }

                    // The first element (a single element) in the sequence cannot be predicted
                    double maxPossibleAccuraccy = (double)((double)sequence.Count - 1) / (double)sequence.Count * 100.0;

                    double accuracy = (double)elementMatches / (double)sequence.Count * 100.0;

                    Debug.WriteLine($"Cycle: {cycle}\tMatches={elementMatches} of {sequence.Count}\t {accuracy}%");
                    Console.WriteLine($"Cycle: {cycle}\tMatches={elementMatches} of {sequence.Count}\t {accuracy}%");

                    if (accuracy >= maxPossibleAccuraccy)
                    {
                        maxMatchCnt++;
                        Debug.WriteLine($"100% accuracy reched {maxMatchCnt} times.");

                        //
                        // Experiment is completed if we are 30 cycles long at the 100% accuracy.
                        if (maxMatchCnt >= 30)
                        {
                            sw.Stop();
                            Debug.WriteLine($"Sequence learned. The algorithm is in the stable state after 30 repeats with with accuracy {accuracy} of maximum possible {maxMatchCnt}. learning time: {sw.Elapsed}.");
                            isLearningCompleted = true; 
                            break;
                        }
                    }
                    else if (maxMatchCnt > 0)
                    {
                        Debug.WriteLine($"At 100% accuracy after {maxMatchCnt} repeats we get a drop of accuracy with accuracy {accuracy}. This indicates instable state. Learning will be continued.");
                        maxMatchCnt = 0;
                    }

                    // This resets the learned state, so the first element starts allways from the beginning.
                    tm.Reset(mem);
                    //lastPredictedValues.Clear();
                }

                //if (isLearningCompleted == false)
                //    throw new Exception($"The system didn't learn with expected acurracy!");
            }

            //sw.Stop();

            //****************DISPLAY STATUS OF EXPERIMENT
            Debug.WriteLine("-------------------TRAINING END------------------------");
            Console.WriteLine("-------------------TRAINING END------------------------");

            return new Predictor(layer1, mem, cls);
        }

        /// <summary>
        /// Gets the number of all unique inputs.
        /// </summary>
        /// <param name="sequences">Alle sequences.</param>
        /// <returns></returns>
        private int GetNumberOfInputs(Dictionary<string, List<double>> sequences)
        {
            int num = 0;

            foreach (var inputs in sequences)
            {

                num += inputs.Value.Count;
            }

            return num;
        }

        /// <summary>
        /// Constracts the unique key of the element of an sequece. This key is used as input for HtmClassifier.
        /// It makes sure that alle elements that belong to the same sequence are prefixed with the sequence.
        /// The prediction code can then extract the sequence prefix to the predicted element.
        /// </summary>
        /// <param name="prevInputs"></param>
        /// <param name="input"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static string GetKey(List<string> prevInputs, string input, string sequence)
        {
            string key = String.Empty;

            for (int i = 0; i < prevInputs.Count; i++)
            {
                if (i > 0)
                    key += "-";

                key += (prevInputs[i]);
            }

            return $"{sequence}_{key}";
        }

    }
}
