using NeoCortexApi;
using System;

namespace ProjectMultiSequenceLearning
{
    public class HeartDiseasePredictionTrainingAndTesting
    {
        /// <summary>
        /// Runs MultiSequence Learning Experiment - To Carry out Sequence Learning for Heart Disease Scenario.
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
        private static object PredictElementAccuracy(Predictor predictor, string sequenceData, int predictionScenario)
        {
            int matchCount = 0;
            int predictions = 0;
            double accuracy = 0.0;
            char prevSeqItem = ' ';
            bool first = true;

            List<string> possibleModes = new List<string>();
            Console.WriteLine("------------------------------");

            foreach (var seqItem in sequenceData)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Console.WriteLine($"Element {prevSeqItem}");
                    var elementSDR = Helper.EncodeTestingElement(prevSeqItem.ToString(), predictionScenario);
                    Thread.Sleep(1000); 
                    var classifierPredictions = predictor.Predict(elementSDR);
                    
                    foreach (var cp in classifierPredictions)
                    {
                        var predictedSequence = cp.PredictedInput.Split('_');
                        var predictedElements = predictedSequence.Last().Split('-');

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
            accuracy = (double)matchCount / predictions * 100;

            Console.WriteLine("------------------------------");
            Console.WriteLine($"Sequence {sequenceData}");

            var predictedSequenceMode = possibleModes.GroupBy(x => x.Split("_")[0])
                   .Select(g => new { possibleMode = g.Key, Count = g.Count() })
                   .ToList()
                   .OrderByDescending(x => x.Count).FirstOrDefault();

            if (predictedSequenceMode != null)
                Console.WriteLine($"Heart Disease Status : " + (predictedSequenceMode.possibleMode == "1" ? "Active" : "Inactive"));

            return accuracy;
        }
    }
}
