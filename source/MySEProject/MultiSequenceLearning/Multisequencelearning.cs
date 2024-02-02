using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace MultiSequenceLearning
{
    public class Multisequencelearning
    {


        public static void myproject()
        {

        back:
            Console.WriteLine("Enter number to start the Project..............");
            int uservalue = int.Parse(Console.ReadLine());
            if (uservalue == 1)
            {
                Console.WriteLine("MultiSequence Project starting.....");
                Console.WriteLine("In this Project we have 3 senarios");
                Console.WriteLine("1. Cancer Prediction\n2. Power Consumption\n3.Stocks Prediction");
            }
            else
            { 
                Console.WriteLine("Again");
                goto back;
            }

        }
    }
}
