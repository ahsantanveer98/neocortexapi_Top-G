using NeoCortexApi;
using System;

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
            var trainingData = Helper.ReadSequencesDataFromCSV(trainingDataFilePath);
            var encodeTrainingData = Helper.TrainEncodeSequencesFromCSV(trainingData, predictionScenario);

            Console.WriteLine("Variables are being trained Please Wait....");
            /// <summary>
            /// Prototype for building the prediction engine.
            ///  </summary>
            MultiSequenceLearning1 experiment = new MultiSequenceLearning1();
            var predictor = experiment.Run(encodeTrainingData);

            Console.WriteLine("Ready to Predict.....");
            var testingData = Helper.ReadTestingSequencesDataFromCSV(testingDataFilePath);

            foreach (var seqData in testingData)
            {
                Console.WriteLine($"Sequence {seqData}");
                predictor.Reset();
                var accuracy = PredictElementAccuracy(predictor, seqData, predictionScenario);
                Console.WriteLine($"Accuracy {accuracy}");
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
                    var classifierPredictions = predictor.Predict(elementSDR);
                    Console.WriteLine($"Classifier Predictions Count {classifierPredictions.Count}");

                }
            }
        }
    }
}

    

