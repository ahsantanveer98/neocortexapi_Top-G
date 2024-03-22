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
    public class HeartDiseasePredictionTrainingAndTesting
    {
        /// <summary>
        /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for HeartDisease Scenario.
        /// </summary>
        /// <param name="predictionScenario"></param>
        /// <param name="trainingDataFilePath"></param>
        /// <param name="testingDataFilePath"></param>
        public void RunMultiSequenceLearningExperiment(int predictionScenario, string trainingDataFilePath, string testingDataFilePath)
        {
            var trainingData = Helper.ReadSequencesDataFromCSV(trainingDataFilePath);
            var encodeTrainingData = Helper.TrainEncodeSequencesFromCSV(trainingData, predictionScenario);

            Console.WriteLine("Variables are being trained Please Wait....");
            /// <summary>
            /// Prototype for building the prediction engine.
            ///  </summary>
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(encodeTrainingData);

            Console.WriteLine("Ready to Predict.....");
            var testingData = Helper.ReadTestingSequencesDataFromCSV(testingDataFilePath);

            foreach (var seqData in testingData)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Sequence {seqData}");
                predictor.Reset();
                var accuracy = PredictElementAccuracy(predictor, seqData, predictionScenario);
                Console.WriteLine($"Accuracy {accuracy}");
            }
            Console.WriteLine("------------------------------");
        }

        /// <summary>
        /// Encoding HeartDisease_Alphabetic_Sequences
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
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
            /// After Alpha Sequence is Learnt, PredictInputSequence will carry out prediction of the HeartDisease Alphabets from the
            /// Sequence which is read from the sequence (CSV Folder) 
            /// </summary>
            /// <param name="list"></param>
            public static List<int[]> PredictInputSequence(string userInput, Boolean EncodeSingleAlphabet)
            {
            var alphabetEncoder = FetchAlphabetEncoder();
            var Encoded_Alphabet_SDRs = new List<int[]>();
            if (!EncodeSingleAlphabet)
            {
                if (userInput.Length < 33)
                {
                    int remainingLength = 33 - userInput.Length;
                    for (int i = 0; i < remainingLength; i++)
                    {
                        userInput = userInput + "Z";
                    }
                }
                foreach (var alphabet in userInput)
                {
                    Encoded_Alphabet_SDRs.Add(alphabetEncoder.Encode(char.ToUpper(alphabet) - 64));
                }
            }
            else
            {
                Encoded_Alphabet_SDRs.Add(alphabetEncoder.Encode(char.ToUpper(userInput.ElementAt(0)) - 64));
            }
            return Encoded_Alphabet_SDRs;
        }
        /// <summary>
        ///FetchAlphabetEncoder 
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
        /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for Alphabets.
        /// </summary>
        /// <param name="datafilepath"></param>
        public void MultiSequenceLearning_Alphabets(string datafilepath)
        {
            var trainingData = HelperMethod_Alphabets.ReadSequencesDataFromCSV(datafilepath);
            var trainingDataProcessed = HelperMethod_Alphabets.TrainEncodeSequencesFromCSV(trainingData);
            /// <summary>
            /// Prototype for building the prediction engine.
            ///  </summary>
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            Console.WriteLine("Variables are being trained Please Wait....");
            var trained_HTM_model = experiment.RunAlphabetsLearning(trainingDataProcessed, true);
            var trained_CortexLayer = trained_HTM_model.Keys.ElementAt(0);
            var trained_Classifier = trained_HTM_model.Values.ElementAt(0);
            Console.WriteLine("Ready to Predict.....");
            Console.WriteLine("Enter HeartDisease Sequence:   *note format {AlphabeticSequence}");
            var userInput = Console.ReadLine();
            while (userInput != null)
            {
                if ((userInput != "q") && (userInput != "Q"))
                {
                  var ElementSDRs = HelperMethod_Alphabets.PredictInputSequence(userInput, false);
                  List<string> possibleClasses = new List<string>();
                    for (int i = 0; i < userInput.Length; i++)
                    {
                        var element = userInput.ElementAt(i);
                        var elementSDR = HelperMethod_Alphabets.PredictInputSequence(element.ToString(), true);
                        var lyr_Output = trained_CortexLayer.Compute(elementSDR[0], false) as ComputeCycle;
                        if (lyr_Output != null)
                        {
                            var classifierPrediction = trained_Classifier.GetPredictedInputValues(lyr_Output.PredictiveCells.ToArray(), 5);
                            if (classifierPrediction.Count > 0)
                            {
                                foreach (var prediction in classifierPrediction)
                                {
                                    if (i < userInput.Length - 1)
                                    {
                                        var nextElement = userInput.ElementAt(i + 1).ToString();
                                        var nextElementString = nextElement.Split(",")[0];
                                        if (prediction.PredictedInput.Split(",")[0] == nextElementString)
                                        {
                                            if (prediction.PredictedInput.Split(",").Length == 3)
                                            {
                                                {
                                                    possibleClasses.Add(prediction.PredictedInput.Split(",")[2]);
                                                }
                                            }
                                            else if (prediction.PredictedInput.Split(",")[0] == nextElementString)
                                            {
                                                possibleClasses.Add(prediction.PredictedInput.Split(",")[1]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var Classcounts = possibleClasses.GroupBy(x => x.Split("_")[0])
                   .Select(g => new { possibleClass = g.Key, Count = g.Count() })
                   .ToList();
                    foreach (var class_ in Classcounts)
                    {
                        Console.WriteLine($"Predicted Class : {class_.possibleClass.Split("_")[0]} \t votes: {class_.Count}");
                    }
                    Console.WriteLine("Enter Next Sequence :");
                    userInput = Console.ReadLine();
                }
            }



            /// <summary>
            /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for Alphabets.
            /// </summary>
            /// <param name="datafilepath"></param>
            public void RunMultiSequenceLearningExperiment(string trainingDataFilePath, string testingDataFilePath)
            {
                var trainingData = ReadSequencesDataFromCSV(trainingDataFilePath);
                var trainingDataProcessed = TrainEncodeSequencesFromCSV(trainingData);


                Console.WriteLine("Variables are being trained Please Wait....");
                /// <summary>
                /// Prototype for building the prediction engine.
                ///  </summary>
                MultiSequenceLearning experiment = new MultiSequenceLearning();
                //var trained_HTM_model = experiment.Run(trainingDataProcessed);
                var predictor = experiment.Run(trainingDataProcessed);

                //var trained_HTM_model = experiment.Run(trainingDataProcessed);
                //var trained_CortexLayer = trained_HTM_model.Keys.ElementAt(0);
                //var trained_Classifier = trained_HTM_model.Values.ElementAt(0);

                Console.WriteLine("Ready to Predict.....");
                var testingData = ReadTestingSequencesDataFromCSV(testingDataFilePath);

                //List<Report> reports = new List<Report>();
                //Report report = new Report();
                foreach (var seqData in testingData)
                {

                    Console.WriteLine($"Sequence {seqData}");
                    var accuracy = PredictElementAccuracy(predictor, seqData);
                    Console.WriteLine($"Accuracy {accuracy}");
                    //Console.WriteLine($"Predicted Class : {predictedSequence.possibleClass.Split("_")[0]}, accuracy: {accuracy}");
                }
                //int rowNumber = 0;
                //foreach (var rowData in testingData)
                //{
                //    int count = 0;
                //    rowNumber++;
                //    foreach (var item in rowData)
                //    {
                //        count++;
                //        report.SequenceName = $"Row Data {rowNumber} Alpabat Index {count}";
                //        Debug.WriteLine($"Using test sequence: Row Data {rowNumber} Alpabat Index {count}");
                //        Console.WriteLine("------------------------------");
                //        Console.WriteLine($"Using test sequence: Row Data {rowNumber} Alpabat Index {count}");
                //        //predictor.Reset();
                //        report.SequenceData = item;
                //        //var accuracy = PredictNextElement(predictor, item, report);
                //        //reports.Add(report);
                //        //Console.WriteLine($"Accuracy for Row Data {rowNumber} Alpabat Index {count} sequence: {accuracy}%");
                //    }
                //}

                //var TestAllRep = reports;
            }
        }
    }
}