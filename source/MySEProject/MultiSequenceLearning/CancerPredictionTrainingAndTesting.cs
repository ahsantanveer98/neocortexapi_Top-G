using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Drawing;


using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using NeoCortexApi.Encoders;
using NeoCortexApi.Utility;

using HtmImageEncoder;

using Daenet.ImageBinarizerLib.Entities;
using Daenet.ImageBinarizerLib;

using Newtonsoft.Json;

using SkiaSharp;



namespace ProjectMultiSequenceLearning
{
    public class CancerPredictionTrainingAndTesting
    {
        /// <summary>
        ///     Fetch Data Sequence from the File 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadSequencesDataFromCSV(string dataFilePath)
        {
            List<Dictionary<string, string>> SequencesCollection = new List<Dictionary<string, string>>();

            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {
                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line != null)
                        {
                            string[] values = line.Split("\t");

                            Dictionary<string, string> Sequence = new Dictionary<string, string>();

                            string label = values[1];
                            string sequenceString = values[0];

                            foreach (var alphabet in sequenceString)
                            {
                                keyForUniqueIndexes++;
                                if (Sequence.ContainsKey(alphabet.ToString()))
                                {
                                    var newKey = alphabet.ToString() + "," + keyForUniqueIndexes;
                                    Sequence.Add(newKey, label);
                                }
                                else
                                {
                                    Sequence.Add(alphabet.ToString(), label);
                                }
                            }
                            SequencesCollection.Add(Sequence);
                        }
                    }
                }
                return SequencesCollection;
            }
            return SequencesCollection;
        }

        /// <summary>
        ///     Encoding Cancer_Alphabetic Sequences
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        /// 
        public static List<Dictionary<string, int[]>> TrainEncodeSequencesFromCSV(List<Dictionary<string, string>> trainingData)
        {
            List<Dictionary<string, int[]>> ListOfEncodedTrainingSDR = new List<Dictionary<string, int[]>>();

            EncoderBase encoder_Alphabets = FetchAlphabetEncoder();

            foreach (var sequence in trainingData)
            {
                int keyForUniqueIndex = 0;
                var tempDictionary = new Dictionary<string, int[]>();

                foreach (var element in sequence)
                {
                    keyForUniqueIndex++;
                    var elementLabel = element.Key + "," + element.Value;
                    var elementKey = element.Key;
                    int[] sdr = new int[0];
                    sdr = sdr.Concat(encoder_Alphabets.Encode(char.ToUpper(element.Key.ElementAt(0)) - 64)).ToArray();

                    if (tempDictionary.ContainsKey(elementLabel))
                    {
                        var newKey = elementLabel + "," + keyForUniqueIndex;
                        tempDictionary.Add(newKey, sdr);
                    }
                    else
                    {
                        tempDictionary.Add(elementLabel, sdr);
                    }
                }
                ListOfEncodedTrainingSDR.Add(tempDictionary);
            }
            return ListOfEncodedTrainingSDR;
        }

        /// <summary>
        ///         FetchAlphabetEncoder 
        /// </summary>
        /// <returns> SCALAR ENCODERS</returns>
        public static EncoderBase FetchAlphabetEncoder()
        {
            EncoderBase AlphabetEncoder = new ScalarEncoder(new Dictionary<string, object>()
                {
                    { "W", 5},
                    { "N", 31},
                    { "Radius", -1.0},
                    { "MinVal", (double)1},
                    { "Periodic", true},
                    { "Name", "scalar"},
                    { "ClipInput", false},
                    { "MaxVal", (double)27}
                });
            return AlphabetEncoder;
        }

        /// <summary>
        /// HTM Config for creating Connections
        /// </summary>
        /// <param name="inputBits">input bits</param>
        /// <param name="numColumns">number of columns</param>
        /// <returns>Object of HTMConfig</returns>
        public static HtmConfig FetchHTMConfig(int inputBits, int numColumns)
        {
            HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
            {
                Random = new ThreadSafeRandom(42),

                CellsPerColumn = 25,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 0.02 * numColumns,
                PotentialRadius = 65, /*(int)(0.15 * inputBits),*/
                InhibitionRadius = 15,

                MaxBoost = 10.0,
                DutyCyclePeriod = 25,
                MinPctOverlapDutyCycles = 0.75,
                MaxSynapsesPerSegment = 128, /*(int)(0.02 * numColumns),*/

                ActivationThreshold = 15,
                ConnectedPermanence = 0.5,

                // Learning is slower than forgetting in this case.
                PermanenceDecrement = 0.25,
                PermanenceIncrement = 0.15,

                // Used by punishing of segments.
                PredictedSegmentDecrement = 0.1,
            };

            return cfg;
        }

        /// <summary>
        ///     Fetch Testing Data Sequence from the File 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<string> ReadTestingSequencesDataFromCSV(string dataFilePath)
        {
            List<string> ListOfTestingSeqData = new List<string>();

            if (File.Exists(dataFilePath))
            {
                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line != null)
                        {
                            ListOfTestingSeqData.Add(line);
                        }
                    }
                }
            }
            return ListOfTestingSeqData;
        }

        /// <summary>
        ///     Fetch Testing Data Sequence from the File 
        /// </summary>
        /// <param name="testingAlphabet"></param>
        /// <returns></returns>
        public static int[] PredictTestingSequence(string testingAlphabet)
        {
            EncoderBase alphabetEncoder = FetchAlphabetEncoder();
            return alphabetEncoder.Encode(char.ToUpper(testingAlphabet.ElementAt(0)) - 64);
        }
    }
}