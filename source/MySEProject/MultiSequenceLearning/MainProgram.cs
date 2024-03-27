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
            
            ProjectStarter.startingproject();

            int Option = Convert.ToInt16(Console.ReadLine());

            ProjectStarter.UserSelection(Option);
        }
    }
}
