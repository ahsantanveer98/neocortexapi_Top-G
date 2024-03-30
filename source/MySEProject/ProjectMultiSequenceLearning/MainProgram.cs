using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMultiSequenceLearning
{
    class MainProgram
    {
        /// <summary>
        /// Main Program Start
        /// </summary>
        /// 
        public static void Main(string[] args)
        {

            //Method used for printing scenario(s).
            ProjectStarter.StartingProject();

            //Get user input for scenario that want execute
            int Option = Convert.ToInt16(Console.ReadLine());

            //Method run the scenario that user selected 
            ProjectStarter.UserSelection(Option);
        }
    }
}