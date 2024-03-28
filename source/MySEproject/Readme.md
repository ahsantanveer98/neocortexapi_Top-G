
ML-23/24-09 Approved prediction of Multisequence Learning - Team_Top-G


       Project objective:

                        **The group of students must implement different scenarios (every student one scenario like cancer sequence prediction, power consumption prediction or similar.)**
                        
                        The sample implemented in MultisequenceLearning.cs and the method RunMultiSequenceLearningExperiment demonstrates how sequences are learned and then predicted.

                        Your first task is to analyse the existing code and understand how learning sequences and prediction work. Then implement a new method RunPredictionMultiSequenceExperiment, that improves the existing RunMultiSequenceLearningExperiment. The new method should automatically read learning sequences from a file and learn them. After learning is completed, the sample should read testing subsequences from another file and calculate the prediction accuracy.

                        The existing example already works. Your task is to start the learning (RunExperiment method) and then to implement the code which approves your scenario. For example, you learn some sequence of elements and then load inferring samples as subsequences (see how Predictor is used) and then show how next elements are predicted.




Senario 3: HeartDisease Prediction

Number of files: 

1. HeartDiseasePredictionTrainingAndTesting.cs
2. MainProgram.cs
3. MultiSequenceLearning.cs
4. ProjectStarter.cs
5. Helper.cs

Number Of Folders

1. TrainingData
2. TestingData

Code For File (Training and Testing) Path:

static readonly string FilePath = Path.GetFullPath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName);
static readonly string HeartDiseaseTrainingDatafile = FilePath + @"\TrainingData\HeartDiseaseTrainingData.csv";
static readonly string HeartDiseaseTestingDataFile = FilePath + @"\TestingData\HeartDiseaseTestingData.csv";

Rest Coding of the Project in .cs Files

        Scenario 3_ HeartDisease Prediction *Completed*