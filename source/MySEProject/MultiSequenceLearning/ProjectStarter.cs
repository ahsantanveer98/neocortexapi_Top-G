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
        /// Training File Paths For Images and Sequences
        /// </summary>
        ///



        /// <summary>
        /// Print Message During Startup of Program
        /// </summary>
        /// <param name="None"></param>
        public static void startingproject()
        {
            Console.WriteLine("\n");
            // Set the Foreground color to White
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("######## ########    ###    ##     ##    ########   ####     #########             ######    ");
            Console.WriteLine("   ##    ##         ## ##   ###   ###       ##     ##  ##    ##      ##           ##         ");
            Console.WriteLine("   ##    ##        ##   ##  #### ####       ##    ##    ##   ##      ##          ##         ");
            Console.WriteLine("   ##    ######   ##     ## ## ### ##       ##    ##    ##   #########    ####  ##    ##### ");
            Console.WriteLine("   ##    ##       ######### ##     ##       ##    ##    ##   ##                  ##   #  ## ");
            Console.WriteLine("   ##    ##       ##     ## ##     ##       ##     ##  ##    ##                   ##    ## ");
            Console.WriteLine("   ##    ######## ##     ## ##     ##       ##      ####     ##                    ######");

            Console.WriteLine("\n\n\n");

            // Set the Foreground color to Yellow
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine("**************          Multi Sequence Learning Project              ************** \n ");
            Console.WriteLine("**************  Project - 1 Multi Sequence Learning - Cancer_Prediction     ************** ");
            Console.WriteLine("**************  Project - 2 Multi Sequence Learning - Power_Prediction   ************** ");
            Console.WriteLine("**************  Project - 3 Multi Sequence Learning - Heart_Disease_Prediction      ************** ");

            Console.WriteLine("\n");
            Console.WriteLine("Please Enter A Project Number to Continue with MultiSequence Experiment");

            // Set the Foreground color to White
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Print Message During Startup of Program
        /// </summary>
        /// <param name="UserInput"></param>

        public static void UserSelection(int UserInput)
        {
            switch (UserInput)
            {
                case 1:
                {
                        Console.WriteLine("User Selected MultiSequence Experiment - Cancer_Prediction\n");
                        //CancerPredictionTraining.cancerprediction();
                        //HelperMethod_Numbers multiSeqLearn_Numbers = new HelperMethod_Numbers();
                        //multiSeqLearn_Numbers.MultiSequenceLearning_Numbers();              
                }
                break;

                case 2:
                {
                        Console.WriteLine("User Selected MultiSequence Experiment - Power_Prediction\n");
                        //HelperMethod_Numbers multiSeqLearn_Numbers = new HelperMethod_Numbers();
                        //multiSeqLearn_Numbers.MultiSequenceLearning_Numbers();              
                }
                break;

                case 3:
                {
                        Console.WriteLine("User Selected MultiSequence Experiment - Heart_Disease_Prediction\n");
                        //HelperMethod_Numbers multiSeqLearn_Numbers = new HelperMethod_Numbers();
                        //multiSeqLearn_Numbers.MultiSequenceLearning_Numbers();              
                }
                break;

                default:
                {
                     Console.WriteLine("User Entered Invalid Option");
                }
                break;
            }
        }

    }
}
