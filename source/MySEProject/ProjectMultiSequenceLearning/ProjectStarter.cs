using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMultiSequenceLearning
{
    public class ProjectStarter
    {
        /// <summary>
        /// Training File Paths For Sequences
        /// </summary>
        ///
#pragma warning disable CS8602 //disable the warning 
        static readonly string FilePath = Path.GetFullPath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName);

        static readonly string CancerTrainingDataFile = FilePath + @"\TrainingData\CancerTrainingData.csv";
        static readonly string HeartDiseaseTrainingDataFile = FilePath + @"\TrainingData\HeartDiseaseTrainingData.csv";
        static readonly string PowerConsumptionTrainingDataFile = FilePath + @"\TrainingData\PowerTrainingData.csv";

        static readonly string CancerTestingDataFile = FilePath + @"\TestingData\CancerTestingData.csv";
        static readonly string HeartDiseaseTestingDataFile = FilePath + @"\TestingData\HeartDiseaseTestingData.csv";
        static readonly string PowerConsumptionTestingDataFile = FilePath + @"\TestingData\PowerTestingData.csv";


        /// <summary>
        /// Print Message During Startup of Program
        /// </summary>
        /// <param name="None"></param>
        public static void startingproject()
        {
            Console.WriteLine("\n");
            // Set the Foreground color to blue
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("######## ########    ###    ##     ##    ########   ####     #########             ######    ");
            Console.WriteLine("   ##    ##         ## ##   ###   ###       ##     ##  ##    ##      ##           ##         ");
            Console.WriteLine("   ##    ##        ##   ##  #### ####       ##    ##    ##   ##      ##          ##         ");
            Console.WriteLine("   ##    ######   ##     ## ## ### ##       ##    ##    ##   #########    ####  ##    ##### ");
            Console.WriteLine("   ##    ##       ######### ##     ##       ##    ##    ##   ##                  ##   #  ## ");
            Console.WriteLine("   ##    ##       ##     ## ##     ##       ##     ##  ##    ##                   ##    ## ");
            Console.WriteLine("   ##    ######## ##     ## ##     ##       ##      ####     ##                    ######");

            Console.WriteLine("\n\n\n");

            // Set the Foreground color to blue
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine("**************          Approve Prediction of Multi Sequence Learning               ************** \n ");
            Console.WriteLine("**************  Scenario - 1 - Cancer_Prediction     ************** ");
            Console.WriteLine("**************  Scenario - 2 - Power_Consumption_Prediction   ************** ");
            Console.WriteLine("**************  Scenario - 3 - Heart_Disease_Prediction      ************** ");

            Console.WriteLine("\n");
            Console.WriteLine("Please Enter A Scenario Number to Continue with MultiSequence Experiment");

            // Set the Foreground color to blue
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Print Message During Startup of Program
        /// </summary>
        /// <param name="UserInput"></param>

        public static void UserSelection(int userInput)
        {
            switch (userInput)
            {
                case 1:
                    Console.WriteLine("User Selected MultiSequence Experiment - Cancer_Prediction\n");
                    //CancerPredictionTrainingAndTesting cancerPrediction = new CancerPredictionTrainingAndTesting();
                    //cancerPrediction.RunMultiSequenceLearningExperiment(userInput, CancerTrainingDataFile, CancerTestingDataFile);
                    break;
                case 2:
                    Console.WriteLine("User Selected MultiSequence Experiment - Power_Consumption_Prediction\n");
                    PowerConsumptionPredictionTrainingAndTesting powerConsumptionPrediction = new PowerConsumptionPredictionTrainingAndTesting();
                    powerConsumptionPrediction.RunMultiSequenceLearningExperiment(userInput, PowerConsumptionTrainingDataFile, PowerConsumptionTestingDataFile);
                    break;
                case 3:
                    Console.WriteLine("User Selected MultiSequence Experiment - Heart_Disease_Prediction\n");
                    //HeartDiseasePredictionTrainingAndTesting heartDiseasePrediction = new HeartDiseasePredictionTrainingAndTesting();
                    //heartDiseasePrediction.RunMultiSequenceLearningExperiment(userInput, HeartDiseaseTrainingDataFile, HeartDiseaseTestingDataFile);   
                    break;
                default:
                    Console.WriteLine("User Entered Invalid Option");
                    break;
            }
        }
    }
}
