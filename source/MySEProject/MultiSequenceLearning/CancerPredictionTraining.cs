using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Drawing;


using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using NeoCortexApi.Encoders;
using NeoCortexApi.Utility;

using HtmImageEncoder;

using Daenet.ImageBinarizerLib.Entities;
using Daenet.ImageBinarizerLib;

using Newtonsoft.Json;

using SkiaSharp;



namespace ProjectMultiSequenceLearning
{
    public class CancerPredictionTraining
    {
        /// <summary>
        ///     Fetch Data Sequence from the File 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadSequencesDataFromCSV(string dataFilePath)
        {
            List<Dictionary<string, string>> SequencesCollection = new List<Dictionary<string, string>>();

            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {
                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line != null)
                        {
                            string[] values = line.Split("\t");

                            Dictionary<string, string> Sequence = new Dictionary<string, string>();

                            string label = values[1];
                            string sequenceString = values[0];

                            foreach (var alphabet in sequenceString)
                            {
                                keyForUniqueIndexes++;
                                if (Sequence.ContainsKey(alphabet.ToString()))
                                {
                                    var newKey = alphabet.ToString() + "," + keyForUniqueIndexes;
                                    Sequence.Add(newKey, label);
                                }
                                else
                                {
                                    Sequence.Add(alphabet.ToString(), label);
                                }
                            }
                            SequencesCollection.Add(Sequence);
                        }
                    }
                }
                return SequencesCollection;
            }
            return SequencesCollection;
        }


    }
}