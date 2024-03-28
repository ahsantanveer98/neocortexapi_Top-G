using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NumSharp.Utilities;
using System;

namespace ProjectMultiSequenceLearning
{
    public class Helper
    {
        /// <summary>
        ///     Fetch Training Data Sequence from the File 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadSequencesDataFromCSV(string dataFilePath)
        {
            try
            {
                int keyForUniqueIndexes = 0;
                List<Dictionary<string, string>> sequencesCollection = new List<Dictionary<string, string>>();

                if (File.Exists(dataFilePath))
                {
                    using (StreamReader sr = new StreamReader(dataFilePath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var line = sr.ReadLine();
                            if (line != null)
                            {
                                string[] values = line.Replace("\"", "").Split(",");
                                var sequence = new Dictionary<string, string>();

                                string sequenceString = values[0];
                                string label = values[1];

                                foreach (var element in sequenceString)
                                {
                                    keyForUniqueIndexes++;
                                    if (sequence.ContainsKey(element.ToString()))
                                    {
                                        var newKey = element.ToString() + "," + keyForUniqueIndexes;
                                        sequence.Add(newKey, label);
                                    }
                                    else
                                    {
                                        sequence.Add(element.ToString(), label);
                                    }
                                }
                                sequencesCollection.Add(sequence);
                            }
                        }
                    }
                    return sequencesCollection;
                }
                return sequencesCollection;
            }
            catch (global::System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///     Encoding Sequences
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        /// 
        public static List<Dictionary<string, int[]>> TrainEncodeSequencesFromCSV(List<Dictionary<string, string>> trainingData, int predictionScenario)
        {
            List<Dictionary<string, int[]>> ListOfEncodedTrainingSDR = new List<Dictionary<string, int[]>>();
            EncoderBase encoder = FetchEncoder(predictionScenario);

            foreach (var sequence in trainingData)
            {
                int keyForUniqueIndex = 0;
                var tempDictionary = new Dictionary<string, int[]>();

                foreach (var element in sequence)
                {
                    keyForUniqueIndex++;
                    var elementLabel = element.Key + "," + element.Value;
                    var elementKey = element.Key;

                    int[] sdr;
                    if (predictionScenario == 2)
                        sdr = encoder.Encode(elementKey.Split(',').First());
                    else
                        sdr = encoder.Encode((int)char.ToUpper(elementKey.ElementAt(0)));

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
        ///         FetchEncoder 
        /// </summary>
        /// <returns> SCALAR ENCODERS</returns>
        public static EncoderBase FetchEncoder(int predictionScenario)
        {
            Dictionary<string, object> configDictionary = new Dictionary<string, object>();
            switch (predictionScenario)
            {
                case 1:
                    configDictionary.Add("W", 15);
                    configDictionary.Add("N", 200);
                    configDictionary.Add("Radius", -1.0);
                    configDictionary.Add("Periodic", true);
                    configDictionary.Add("Name", "scalar");
                    configDictionary.Add("ClipInput", false);
                    configDictionary.Add("MinVal", (double)65);
                    configDictionary.Add("MaxVal", (double)91);
                    break;
                case 2:
                    configDictionary.Add("W", 15);
                    configDictionary.Add("N", 200);
                    configDictionary.Add("Radius", -1.0);
                    configDictionary.Add("Periodic", false);
                    configDictionary.Add("Name", "scalar");
                    configDictionary.Add("ClipInput", false);
                    configDictionary.Add("MinVal", (double)1);
                    configDictionary.Add("MaxVal", (double)99);
                    break;
                case 3:
                    configDictionary.Add("W", 15);
                    configDictionary.Add("N", 200);
                    configDictionary.Add("Radius", -1.0);
                    configDictionary.Add("Periodic", true);
                    configDictionary.Add("Name", "scalar");
                    configDictionary.Add("ClipInput", false);
                    configDictionary.Add("MinVal", (double)47);
                    configDictionary.Add("MaxVal", (double)91);
                    break;
                default:
                    break;
            }

            EncoderBase encoder = new ScalarEncoder(configDictionary);
            return encoder;
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

                CellsPerColumn = 32,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 0.02 * numColumns,
                PotentialRadius = 65,
                //InhibitionRadius = 15,

                MaxBoost = 10.0,
                DutyCyclePeriod = 25,
                MinPctOverlapDutyCycles = 0.75,
                MaxSynapsesPerSegment = 128,

                ActivationThreshold = 15,
                ConnectedPermanence = 0.5,

                // Learning is slower than forgetting in this case.
                PermanenceDecrement = 0.0,
                PermanenceIncrement = 1.0,

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
            List<string> testingSeqDataList = new List<string>();

            if (File.Exists(dataFilePath))
            {
                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line != null)
                        {
                            testingSeqDataList.Add(line.Replace("\"", ""));
                        }
                    }
                }
            }
            return testingSeqDataList;
        }

        /// <summary>
        ///     Encode Testing Element 
        /// </summary>
        /// <param name="testingElement"></param>
        /// <param name="predictionScenario"></param>
        /// <returns></returns>
        public static int[] EncodeTestingElement(string testingElement, int predictionScenario)
        {
            EncoderBase encoder = FetchEncoder(predictionScenario);
            if (predictionScenario == 2)
                return encoder.Encode(testingElement);
            else
                return encoder.Encode((int)char.ToUpper(testingElement.ElementAt(0)));
        }

        /// <summary>
        ///     Fetch Training Data Sequence from the File for power consumption
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadSequencesDataFromCSVForPC(string dataFilePath)
        {
            try
            {
                List<Dictionary<string, string>> sequencesCollection = new List<Dictionary<string, string>>();

                if (File.Exists(dataFilePath))
                {
                    using (StreamReader sr = new StreamReader(dataFilePath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var line = sr.ReadLine();
                            if (line != null)
                            {
                                int keyForUniqueIndexes = 0;
                                string[] values = line.Replace("\"", "").Split(",");
                                var sequence = new Dictionary<string, string>();

                                string label = values[0];
                                var dataValue = values.RemoveAt(0);

                                foreach (var value in dataValue)
                                {
                                    keyForUniqueIndexes++;
                                    if (sequence.ContainsKey(value))
                                    {
                                        var newKey = value + "," + keyForUniqueIndexes;
                                        sequence.Add(newKey, label);
                                    }
                                    else
                                    {
                                        sequence.Add(value, label);
                                    }
                                }
                                sequencesCollection.Add(sequence);
                            }
                        }
                    }
                    return sequencesCollection;
                }
                return sequencesCollection;
            }
            catch (global::System.Exception)
            {
                throw;
            }
        }
    }
}
