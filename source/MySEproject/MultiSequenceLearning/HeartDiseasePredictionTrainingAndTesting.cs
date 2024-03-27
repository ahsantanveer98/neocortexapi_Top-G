﻿using NeoCortexApi;
using System;

namespace ProjectMultiSequenceLearning
{
    public class HeartDiseasePredictionTrainingAndTesting
    {
        /// <summary>
        /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for Cancer Prediction.
        /// </summary>
        /// <param name="predictionScenario"></param>
        /// <param name="trainingDataFilePath"></param>
        /// <param name="testingDataFilePath"></param>
        public void RunMultiSequenceLearningExperiment(int predictionScenario, string trainingDataFilePath, string testingDataFilePath)
        {
            //Get Sequence Data from file for training 
            var trainingData = Helper.ReadSequencesDataFromCSV(trainingDataFilePath);

            //Encode Sequence Training Data
            var trainingDataProcessed = Helper.TrainEncodeSequencesFromCSV(trainingData, predictionScenario);


            Console.WriteLine("Variables are being trained Please Wait....");
            /// <summary>
            /// Prototype for building the prediction engine.
            ///  </summary>
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(trainingDataProcessed);


            Console.WriteLine("Ready to Predict.....");

            //Get Testing Sequence Data from file
            var testingData = Helper.ReadTestingSequencesDataFromCSV(testingDataFilePath);

            //Testing Data Execute Row by Row
            foreach (var seqData in testingData)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Sequence {seqData}");
                predictor.Reset();
                var accuracy = PredictElementAccuracy(predictor, seqData, predictionScenario);
                Console.WriteLine($"Accuracy {accuracy}%");
            }
            Console.WriteLine("------------------------------");
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

            Console.WriteLine("------------------------------");

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

            //Every Row is Divide into single element for predict next element and calculate accuuracy
            foreach (var seqItem in sequenceData)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Console.WriteLine($"Element {prevSeqItem}");

                    //Encode Element of Testing Data
                    var elementSDR = Helper.EncodeTestingElement(prevSeqItem.ToString(), predictionScenario);

                    //Delary for 1 Sec
                    Thread.Sleep(1000);

                    //Predict Element using element SDR
                    var classifierPredictions = predictor.Predict(elementSDR);

                    //Read classifier Predictions one by one if match then matchCount increase 
                    foreach (var cp in classifierPredictions)
                    {
                        var predictedSequence = cp.PredictedInput.Split('_');
                        var predictedElements = predictedSequence.Last().Split('-');
                        //Debug.WriteLine($"Predicted Sequence: {predictedSequence.First()}, predicted next element {predictedElements.Last()}");

                        if (Convert.ToChar(predictedElements.Last()) == seqItem)
                        {
                            Console.WriteLine($"Predicted next element: {predictedElements.Last()}");
                            matchCount++;

                            var prediction = cp.PredictedInput.Split(",");
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

                    predictions++;
                }

                //save previous element to compare with upcoming element
                prevSeqItem = seqItem;
            }

            #region CalCulate Accuracy and Predict Mode

            /*
             * Accuracy is calculated as number of matching predictions made 
             * divided by total number of prediction made for an element in subsequence
             * 
             * accuracy = number of matching predictions/total number of prediction * 100
             */
            accuracy = (double)matchCount / predictions * 100;


            Console.WriteLine("------------------------------");
            Console.WriteLine($"Sequence {sequenceData}");
            //Predict Mode
            var predictedSequenceMode = possibleModes.GroupBy(x => x.Split("_")[0])
                   .Select(g => new { possibleMode = g.Key, Count = g.Count() })
                   .ToList()
                   .OrderByDescending(x => x.Count).FirstOrDefault();

            if (predictedSequenceMode != null)
                Console.WriteLine($"Predicted Mode : {predictedSequenceMode.possibleMode}");

            #endregion

            return accuracy;
        }

    }
}
