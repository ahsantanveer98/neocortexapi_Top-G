using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMultiSequenceLearning
{
    public class Helper
    {
        /// <summary>
        ///     Fetch Training Data Sequence from the File 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadSequencesDataFromCSV(string dataFilePath)
        {
            try
            {
                int keyForUniqueIndexes = 0;
                List<Dictionary<string, string>> sequencesCollection = new List<Dictionary<string, string>>();

                if (File.Exists(dataFilePath))
                {
                    using (StreamReader sr = new StreamReader(dataFilePath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var line = sr.ReadLine();
                            if (line != null)
                            {
                                string[] values = line.Replace("\"", "").Split(",");
                                var sequence = new Dictionary<string, string>();

                                string sequenceString = values[0];
                                string label = values[1];

                                foreach (var element in sequenceString)
                                {
                                    keyForUniqueIndexes++;
                                    if (sequence.ContainsKey(element.ToString()))
                                    {
                                        var newKey = element.ToString() + "," + keyForUniqueIndexes;
                                        sequence.Add(newKey, label);
                                    }
                                    else
                                    {
                                        sequence.Add(element.ToString(), label);
                                    }
                                }
                                sequencesCollection.Add(sequence);
                            }
                        }
                    }
                    return sequencesCollection;
                }
                return sequencesCollection;
            }
            catch (global::System.Exception)
            {
                throw;
            }
        }


    }
}
