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
        /// Print Message During Startup of Program
        /// </summary>
        /// <param name="None"></param>
        public static void startingproject()
        {
            Console.WriteLine("\n");
            // Set the Foreground color to green
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("######## ########    ###    ##     ##    ########   ####     #########             ######    ");
            Console.WriteLine("   ##    ##         ## ##   ###   ###       ##     ##  ##    ##      ##           ##         ");
            Console.WriteLine("   ##    ##        ##   ##  #### ####       ##    ##    ##   ##      ##          ##         ");
            Console.WriteLine("   ##    ######   ##     ## ## ### ##       ##    ##    ##   #########    ####  ##    ##### ");
            Console.WriteLine("   ##    ##       ######### ##     ##       ##    ##    ##   ##                  ##   #  ## ");
            Console.WriteLine("   ##    ##       ##     ## ##     ##       ##     ##  ##    ##                   ##    ## ");
            Console.WriteLine("   ##    ######## ##     ## ##     ##       ##      ####     ##                    ######");

            Console.WriteLine("\n\n\n");

            // Set the Foreground color to dark Blue
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine("**************          Approve Prediction of Multi Sequence Learning               ************** \n ");
            Console.WriteLine("**************  Scenario - 1 - Cancer_Prediction     ************** ");
            Console.WriteLine("**************  Scenario - 2 - Power_Prediction   ************** ");
            Console.WriteLine("**************  Scenario - 3 - Heart_Disease_Prediction      ************** ");

            Console.WriteLine("\n");
            Console.WriteLine("Please Enter a Scenario Number to Continue with MultiSequence Experiment");

            // Set the Foreground color to white
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

                    Console.WriteLine("User Selected MultiSequence Experiment - Cancer_Prediction\n");


                    break;

                case 2:

                    Console.WriteLine("User Selected MultiSequence Experiment - Power_Prediction\n");


                    break;

                case 3:

                    Console.WriteLine("User Selected MultiSequence Experiment - Heart_Disease_Prediction\n");


                    break;

                default:

                    Console.WriteLine("User Entered Invalid Option");

                    break;
            }
        }

    }
}
