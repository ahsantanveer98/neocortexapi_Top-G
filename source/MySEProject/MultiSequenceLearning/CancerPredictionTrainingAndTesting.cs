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
        /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for Cancer Prediction.
        /// </summary>
        /// <param name="predictionScenario"></param>
        /// <param name="trainingDataFilePath"></param>
        /// <param name="testingDataFilePath"></param>
        public void RunMultiSequenceLearningExperiment(int predictionScenario, string trainingDataFilePath, string testingDataFilePath)
        {
            var trainingData = Helper.ReadSequencesDataFromCSV(trainingDataFilePath);
            var trainingDataProcessed = Helper.TrainEncodeSequencesFromCSV(trainingData, predictionScenario);


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
            var testingData = Helper.ReadTestingSequencesDataFromCSV(testingDataFilePath);

            //List<Report> reports = new List<Report>();
            //Report report = new Report();
            foreach (var seqData in testingData)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Sequence {seqData}");
                predictor.Reset();
                var accuracy = PredictElementAccuracy(predictor, seqData, predictionScenario);
                Console.WriteLine($"Accuracy {accuracy}");
            }
            Console.WriteLine("------------------------------");




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

        /// <summary>
        /// Takes predicted model, subsequence and predict accuracy
        /// </summary>
        /// <param name="predictor">Object of Predictor</param>
        /// <param name="sequenceData">sequence to be tested</param>
        /// <param name="predictionScenario">Prediction Scenario</param>
        /// <returns>accuracy of predicting elements in %</returns>
        private static double PredictElementAccuracy(Predictor predictor, string sequenceData, int predictionScenario)
        {
            int matchCount = 0;
            int predictions = 0;
            double accuracy = 0.0;

            char prevSeqItem = ' ';
            bool first = true;

            List<string> possibleModes = new List<string>();
            //List<string> logs = new List<string>();
            Console.WriteLine("------------------------------");

            //int prev = -1;

            /*
             * Pseudo code for calculating accuracy:
             * 
             * 1.      loop for each element in the sub-sequence
             * 1.1     if the element is first element do nothing and save the element as 'previous' for further comparision
             * 1.2     take previous element and predict the next element
             * 1.2.1   get the predicted element with highest similarity and compare with 'next' element
             * 1.2.1.1 if predicted element matches the next element increment the counter of matched elements
             * 1.2.2   increment the count for number of calls made to predict an element
             * 1.2     update the 'previous' element with 'next' element
             * 2.      calculate the accuracy as (number of matched elements)/(total number of calls for prediction) * 100
             * 3.      end the loop
             */


            //for (int i = 0; i < sequenceData.Length-1; i++)
            foreach (var seqItem in sequenceData)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    //var element = sequenceData.ElementAt(i);
                    Console.WriteLine($"Element {prevSeqItem}");
                    var elementSDR = Helper.EncodeTestingElement(prevSeqItem.ToString(), predictionScenario);
                    var classifierPredictions = predictor.Predict(elementSDR);
                    //string log = "";

                    //if (classifierPrediction.Count > 0)
                    //{
                    //foreach (var pred in res)
                    //{
                    //    Debug.WriteLine($"Predicted Input: {pred.PredictedInput} - Similarity: {pred.Similarity}%");
                    //}
                    //Console.WriteLine($"Classifier Predictions Count {classifierPredictions.Count}");
                    foreach (var cp in classifierPredictions)
                    {
                        var prediction = cp.PredictedInput.Split(",");
                        if (Convert.ToChar(prediction.First()) == seqItem)
                        {
                            Console.WriteLine($"Predicted next element: {prediction.First()}");
                            matchCount++;

                            if (prediction.Length >= 3)
                            {
                                possibleModes.Add(prediction[2]);
                            }
                            else
                            {
                                possibleModes.Add(prediction[1]);
                            }
                            break;
                        }
                    }

                    //var sequence = res.First().PredictedInput.Split('_');
                    //var prediction = res.First().PredictedInput.Split(',');
                    //Console.WriteLine($"Predicted Sequence: {sequence.First()} - Predicted next element: {prediction.First()}");
                    //log = $"Input: {prev}, Predicted Sequence: {sequence.First()}, Predicted next element: {prediction.First()}";



                    //var nextElement = sequenceData.ElementAt(i + 1).ToString();
                    //compare current element with prediction of previous element
                    //if (seqItem == Convert.ToChar(prediction.First()))
                    //{
                    //    matchCount++;

                    //    if (prediction.Length == 3)
                    //    {
                    //        possibleModes.Add(prediction[2]);
                    //    }
                    //    else //if (prediction.PredictedInput.Split(",")[0] == nextElement)
                    //    {
                    //        possibleModes.Add(prediction[1]);
                    //    }
                    //}
                    //}
                    //else
                    //{
                    //    //Console.WriteLine("Nothing predicted :(");
                    //    //log = $"Input: {prev}, Nothing predicted";
                    //}

                    //logs.Add(log);
                    predictions++;
                }

                //save previous element to compare with upcoming element
                prevSeqItem = seqItem;
            }

            //report.PredictionLog = logs;

            /*
             * Accuracy is calculated as number of matching predictions made 
             * divided by total number of prediction made for an element in subsequence
             * 
             * accuracy = number of matching predictions/total number of prediction * 100
             */
            accuracy = (double)matchCount / predictions * 100;
            //report.Accuracy = accuracy;

            Console.WriteLine("------------------------------");
            Console.WriteLine($"Sequence {sequenceData}");
            var predictedSequenceMode = possibleModes.GroupBy(x => x.Split("_")[0])
                   .Select(g => new { possibleMode = g.Key, Count = g.Count() })
                   .ToList()
                   .OrderByDescending(x => x.Count).FirstOrDefault();
            //foreach (var class_ in Classcounts.OrderByDescending(x=>x.Count))
            //{
            if (predictedSequenceMode != null)
                Console.WriteLine($"Predicted Mode : {predictedSequenceMode.possibleMode}");



            return accuracy;
        }

    }
}