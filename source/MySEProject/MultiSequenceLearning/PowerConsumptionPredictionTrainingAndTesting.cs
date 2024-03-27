using NeoCortexApi;
using System;
using System.Diagnostics;

namespace ProjectMultiSequenceLearning
{
    public class PowerConsumptionPredictionTrainingAndTesting
    {
        /// <summary>
        /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for Heart Disease Scenario.
        /// </summary>
        /// <param name="predictionScenario"></param>
        /// <param name="trainingDataFilePath"></param>
        /// <param name="testingDataFilePath"></param>
        public void RunMultiSequenceLearningExperiment(int predictionScenario, string trainingDataFilePath, string testingDataFilePath)
        {
            var trainingData = Helper.ReadSequencesDataFromCSVForPC(trainingDataFilePath);
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
                Console.WriteLine($"Sequence {seqData}");
                predictor.Reset();
                var accuracy = PredictElementAccuracy(predictor, seqData, predictionScenario);
                Console.WriteLine($"Accuracy {accuracy}%");
            }
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
            string prevSeqItem = "";
            bool first = true;

            List<string> possibleModes = new List<string>();
            Console.WriteLine("------------------------------");
            var sequenceDataList = sequenceData.Split(",");
            foreach (var seqItem in sequenceDataList)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Console.WriteLine($"Element {prevSeqItem}");
                    var elementSDR = Helper.EncodeTestingElement(prevSeqItem, predictionScenario);
                    Thread.Sleep(1000);
                    var classifierPredictions = predictor.Predict(elementSDR);
                    //Console.WriteLine($"Classifier Predictions Count {classifierPredictions.Count}");
                    //double lastSimilarity = 0;

                    foreach (var cp in classifierPredictions)
                    {
                        //if (cp.Similarity > lastSimilarity)
                        //{
                        //    lastSimilarity = cp.Similarity;

                        var predictedSequence = cp.PredictedInput.Split('_');
                        var predictedElements = predictedSequence.Last().Split('-');
                        //Debug.WriteLine($"Predicted Sequence: {predictedSequence.First()}, predicted next element {predictedElements.Last()}");

                        if (predictedElements.Last() == seqItem)
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
                        // }

                        //var prediction = cp.PredictedInput.Split(",");
                        //if (prediction.First() == seqItem)
                        //{
                        //    Console.WriteLine($"Predicted next element: {prediction.First()}");
                        //    matchCount++;

                        //    if (prediction.Length >= 3)
                        //    {
                        //        possibleModes.Add(prediction[2]);
                        //    }
                        //    else
                        //    {
                        //        possibleModes.Add(prediction[1]);
                        //    }
                        //    break;
                        //}
                    }
                    predictions++;
                }

                //save previous element to compare with upcoming element
                prevSeqItem = seqItem;
            }
            accuracy = (double)matchCount / predictions * 100;
            Console.WriteLine("------------------------------");

            var predictedSequenceMode = possibleModes.GroupBy(x => x.Split("_")[0])
                   .Select(g => new { possibleMode = g.Key, Count = g.Count() })
                   .ToList()
                   .OrderByDescending(x => x.Count).FirstOrDefault();

            if (predictedSequenceMode != null)
                Console.WriteLine($"Power Consumption : {predictedSequenceMode.possibleMode}");

            return accuracy;
        }
    }
}
