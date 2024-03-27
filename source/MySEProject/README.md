# ML23/24-09   Approve Prediction of Multisequence Learning Team Top-G

## Objective

In this project the group of students must implement  The sample implemented in **MultisequenceLearning.cs** and the method **RunMultiSequenceLearningExperiment** demonstrates how sequences are learned and then predicted.
Your first task is to analyse the existing code and understand how learning sequences and prediction work. Then implement a new method **RunPredictionMultiSequenceExperiment**, that improves the existing **RunMultiSequenceLearningExperiment**. The new method should automatically read learning sequences from a file and learn them. After learning is completed, the sample should read testing subsequences from another file and calculate the prediction accuracy.
The existing example already works. Your task is to start the learning (**RunExperiment method**) and then to implement the code which approves your scenario. For example, you learn some sequence of elements and then load inferring samples as subsequences (see how Predictor is used) and then show how next elements are predicted.


## Introduction

In this project, each member of the group implemented different scenarios (scenarios like **Cancer Prediction**, **Power Consumption Prediction** and **Heart Disease Prediction**.)  we have tried to implement new methods along **MultisequenceLearning** alogrithm. The new methods are automatically reading the training dataset from the given path in **Helper.ReadSequencesDataFromCSV(trainingDataFilePath)**, and also can automatically reading the testing dataset from the given path 
**Helper.ReadTestingSequencesDataFromCSV(testingDataFilePath)**. The method **RunMultiSequenceLearningExperiment(int predictionScenario, string trainingDataFilePath, string testingDataFilePath)** takes the multiple training sequences from `trainingDataFilePath` and test subsequences from `testingDataFilePath` and is passed to **RunMultiSequenceLearningExperiment(int predictionScenario, string trainingDataFilePath, string testingDataFilePath)**. After learning is completed, calculate the accuracy of the predicted elements.

## Implementation

![image](./Images/overview.png)

Fig: Architecture of Approve Prediction of Multisequence Learning

Above the flow of implementation of our project.

`Sequence collection` this line collect the sequences from training CSV file and save into list of dictionary.

```csharp

List<Dictionary<string, string>> sequencesCollection = new List<Dictionary<string, string>>();

```

**- Training Dataset**

- Training Dataset in `CSV` of **Senario_1-Cancer Prediction**

1.  UBUJBUUBUJBJBBJ,mod. active
2.  NDSNDNDNNDNNSDSD,inactive - exp
3.  FLFFGGLGLGFFGLFG,very active
4.  WAAWKKAKWAKAAKWAKA,inactive - virtual

- Training Dataset in `CSV` of **Senario_2-Power Consumption Prediction**

1.  High,10,12,15,20,25,30,35,40,45,50
2.  Low,60,66,68,70,73,76,79,82,84,86
3.  Normal,88,89,90,92,94,95,96,97,98
4.  Normal,10,15,18,25,30,35,40,45,50
5.  Low,62,65,66,72,75,80,83,85,88,90
6.  High,32,34,36,38,43,44,45,50,53,55

- Training Dataset in `CSV` of **Senario_3-Heart Disease Prediction**

1.  ldukkw,0
2.  daa9kp,1
3.  3nwy2n,1
4.  1r508r,0
5.  ldg4b9,0
6.  xc17yq,0
7.  mpggsq,1
8.  zlyac8,0
9.  2gbyz9,1
10. f06u72,1

**- Test Dataset**

- Testing Dataset in `CSV` of **Senario_1-Cancer Prediction**

1.  UJBUUBUJ
2.  GGLGLGFF
3.  AKWAKAAK

- Testing Dataset in `CSV` of **Senario_2-Power Consumption Prediction**

1.  10,12,15,20
2.  92,94,95,96
3.  65,66,72,75

- Training Dataset in `CSV` of **Senario_3-Heart Disease Prediction**

1.  lyac
2.  06u7
3.  2gby


Our implemented methods are in `Helper.cs` and can be found [here](../Helper.cs):

1. FetchHTMConfig()

Here we save the HTMConfig which is used for Hierarchical Temporal Memory to `Connections`

```csharp
/// <summary>
/// HTM Config for creating Connections
/// </summary>
/// <param name="inputBits">input bits</param>
/// <param name="numColumns">number of columns</param>
/// <returns>Object of HTMConfig</returns>        
public static HtmConfig FetchHTMConfig(int inputBits, int numColumns)
{
    HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
    {
        Random = new ThreadSafeRandom(42),

        CellsPerColumn = 32,
        GlobalInhibition = true,
        LocalAreaDensity = -1,
        NumActiveColumnsPerInhArea = 0.02 * numColumns,
        PotentialRadius = 65,
       

        MaxBoost = 10.0,
        DutyCyclePeriod = 25,
        MinPctOverlapDutyCycles = 0.75,
        MaxSynapsesPerSegment = 128,

        ActivationThreshold = 15,
        ConnectedPermanence = 0.5,

        // Learning is slower than forgetting in this case.
        PermanenceDecrement = 0.0,
        PermanenceIncrement = 1.0,

        // Used by punishing of segments.
        PredictedSegmentDecrement = 0.1,
    };

    return cfg;
}

All the fields are self explanotary as per HTM theory.

2. FetchEncoder()

We have used `ScalarEncoder` since we are encoding both Alphabetic and Nummeric Data.

Remember that `inputBits` is same as `HTMConfig`.

```csharp
/// <summary>
///         Fetch Encoder 
/// </summary>
/// <returns> SCALAR ENCODERS</returns>
public static EncoderBase FetchEncoder(int predictionScenario)
{
    Dictionary<string, object> configDictionary = new Dictionary<string, object>();
    switch (predictionScenario)
    {
        case 1:
            configDictionary.Add("W", 15);
            configDictionary.Add("N", 200);
            configDictionary.Add("Radius", -1.0);
            configDictionary.Add("Periodic", true);
            configDictionary.Add("Name", "scalar");
            configDictionary.Add("ClipInput", false);
            configDictionary.Add("MinVal", (double)65);
            configDictionary.Add("MaxVal", (double)91);
            break;
        case 2:
            configDictionary.Add("W", 15);
            configDictionary.Add("N", 200);
            configDictionary.Add("Radius", -1.0);
            configDictionary.Add("Periodic", false);
            configDictionary.Add("Name", "scalar");
            configDictionary.Add("ClipInput", false);
            configDictionary.Add("MinVal", (double)1);
            configDictionary.Add("MaxVal", (double)99);
            break;
        case 3:
            configDictionary.Add("W", 15);
            configDictionary.Add("N", 200);
            configDictionary.Add("Radius", -1.0);
            configDictionary.Add("Periodic", true);
            configDictionary.Add("Name", "scalar");
            configDictionary.Add("ClipInput", false);
            configDictionary.Add("MinVal", (double)47);
            configDictionary.Add("MaxVal", (double)91);
            break;
        default:
            break;
    }

    EncoderBase encoder = new ScalarEncoder(configDictionary);
    return encoder;
}
```



3. ReadTestingSequencesDataFromCSV()

Reads the CSV file wehen passed as full path and retuns the object of list of `testingSeqDataList`

```csharp
/// <summary>
///     Fetch Testing Data Sequence from the File 
/// </summary>
/// <param name="dataFilePath"></param>
/// <returns></returns>
public static List<string> ReadTestingSequencesDataFromCSV(string dataFilePath)
{
    List<string> testingSeqDataList = new List<string>();

    //
    if (File.Exists(dataFilePath))
    {
        //
        using (StreamReader sr = new StreamReader(dataFilePath))
        {
            //
            while (sr.Peek() >= 0)
            {
                //
                var line = sr.ReadLine();
                if (line != null)
                {
                    testingSeqDataList.Add(line.Replace("\"", ""));
                }
            }
        }
    }
    return testingSeqDataList;
}
```

4. Calculating accuracy in PredictElementAccuracy() in `CancerPredictionTrainingAndTesting.cs`, `PowerConsumptionPredictionTrainingAndTesting.cs` and `HeartDiseasePredictionTrainingAndTesting.cs`

![image](./Images/approve_prediction.png)

Fig: Prediction and calculating accuracy

```csharp
{
    int matchCount = 0;
    int predictions = 0;
    double accuracy = 0.0;
    char prevSeqItem = ' ';
    bool first = true;

    List<string> possibleModes = new List<string>();

    Console.WriteLine("------------------------------");

    //Every Row is Divide into single element for predict next element and calculate accuuracy
    foreach (var seqItem in sequenceData)
    {
        if (first)
        {
            first = false;
        }
        else
        {
            Console.WriteLine($"Element {prevSeqItem}");

            //Encode Element of Testing Data
            var elementSDR = Helper.EncodeTestingElement(prevSeqItem.ToString(), predictionScenario);

            //Delary for 1 Sec
            Thread.Sleep(1000);

            //Predict Element using element SDR
            var classifierPredictions = predictor.Predict(elementSDR);

            //Read classifier Predictions one by one if match then matchCount increase 
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

    #region CalCulate Accuracy and Predict Mode

    accuracy = (double)matchCount / predictions * 100;
    Console.WriteLine("------------------------------");
    Console.WriteLine($"Sequence {sequenceData}");

    //Predict Mode

    var predictedSequenceMode = possibleModes.GroupBy(x => x.Split("_")[0])
           .Select(g => new { possibleMode = g.Key, Count = g.Count() })
           .ToList()
           .OrderByDescending(x => x.Count).FirstOrDefault();

    if (predictedSequenceMode != null)
        Console.WriteLine($"Predicted Mode : {predictedSequenceMode.possibleMode}");

    #endregion

    return accuracy;
}
```

Note that prediction code is omitted.

## How to run the project

### To create synthetic dataset

1. Open the [sln](../../../NeoCortexApi.sln) and select `ProjectMultiSequenceLearning` as startup project.

2. In `MainProgram.cs` we have the `Main()`.

```csharp
        public static void Main(string[] args)
        {
            //Method used for printing scenario(s).
            ProjectStarter.StartingProject();

            //Get user input for scenario that want execute
            int Option = Convert.ToInt16(Console.ReadLine());

            //Method run the scenario that user selected 
            ProjectStarter.UserSelection(Option);
        }
```

*and comment rest of the lines*.

3. Run to read dataset from csv


## Results

We have run the experiment max possible number of times with different dataset. We have tried to keep the size of dataset small and number of sequences also small due to large time in execution.

![results](./Images/Cancer_Prediction_Result.png)
![results](./Images/HeartDiseasePredictionResult.jpeg)
![results](./Images/PowerConsumptionPredictionResult.jpeg)

## Reference

- Forked from [ddobric/neocortexapi](https://github.com/ddobric/neocortexapi)

- [Machine Learning Guide to HTM](https://www.numenta.com/blog/2019/10/24/machine-learning-guide-to-htm/)