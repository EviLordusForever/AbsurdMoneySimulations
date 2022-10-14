using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.Storage;


namespace AbsurdMoneySimulations
{
	public static class NN
	{
		public const int horizon = 29;
		public const int inputWindow = 300;
		public const float weightsInitMin = -0.3f;
		public const float weightsInitScale = 0.6f;
		public const int jumpLimit = 9000;

		private const int testsCount = 2000;
		private const int batchSize = 1;

		public static int randomMutatesCount = 2022;

		public static float randomMutatesSharpness =  10;
		public static float randomMutatesScaleV2 = 10;
		public static float randomMutatesSmoothing = 0.03f;

		public static float LYAMBDA = 0.01f; //0.05f
		public static float INERTION = 0.9f; //0.8f

		public static int vanishedGradients = 0;
		public static int cuttedGradients = 0;
		public const float cutter = 100f;

		public static ActivationFunction AnswersAF = new TanH();

		public static List<LayerAbstract> layers;

		public static Tester testerV;
		public static Tester testerE;

		public static string name;

		public static float[] randomMutates;
		public static float mutagen;
		public static int mutationSeed;
		public static int lastMutatedLayer;

		public static void Create()
		{
			layers = new List<LayerAbstract>();

			layers.Add(new LayerMegatron(testerE.testsCount, 10, 136, 30, 2));   //55 x 30 x 10 = 
			layers[0].FillWeightsRandomly();

			layers.Add(new LayerCybertron(testerE.testsCount, 10, 136, 10, 100)); //10 x 55 x 10 = 
			layers[1].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(testerE.testsCount, 30, 100)); //100 x 30 = 3000
			layers[2].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(testerE.testsCount, 10, 30)); //30 x 15 =  450
			layers[3].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(testerE.testsCount, 1, 10)); //10 x 1 = 10
			layers[4].FillWeightsRandomly();

			/*layers.Add(new LayerMegatron(testerE.testsCount, 2, 271, 30, 1));   //271 x 30 x 2 = 
			layers[0].FillWeightsRandomly();

			layers.Add(new LayerCybertron(testerE.testsCount, 2, 271, 5, 10)); //2 x 271 x 5 =
			layers[1].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(testerE.testsCount, 1, 10)); //10 x 1 = 10
			layers[2].FillWeightsRandomly();*/

			/*			layers.Add(new LayerPerceptron(3, 300)); //40 x 15 = 600
						layers[0].FillWeightsRandomly();

						layers.Add(new LayerPerceptron(1, 3)); //15 x 1 = 15
						layers[0].FillWeightsRandomly();*/

			Disk.DeleteFileFromProgramFiles("EvolveHistory.csv");
			Disk.DeleteFileFromProgramFiles("weights.csv");
			Log("Neural Network created!");
		}

		public static void Save()
		{
			var files = Directory.GetFiles(Disk.programFiles + "\\NN");

			JsonSerializerSettings jss = new JsonSerializerSettings();
			jss.Formatting = Formatting.Indented;

			File.WriteAllText(files[0], JsonConvert.SerializeObject(layers, jss));
			Log("Neural Network saved!");
		}

		public static void Load()
		{
			var files = Directory.GetFiles(Disk.programFiles + "\\NN");
			name = TextMethods.StringInsideLast(files[0], "\\", ".json");
			string json = File.ReadAllText(files[0]);
			
			var jss = new JsonSerializerSettings();
			jss.Converters.Add(new LayerAbstractConverter());

			layers = JsonConvert.DeserializeObject<List<LayerAbstract>>(json, jss);

			Log("Neural Network loaded from disk!");
		}

		public static float Calculate(int test, Tester tester, float[] input)
		{
			float[][] array = new float[1][];
			array[0] = input;

			layers[0].Calculate(test,  array);

			for (int l = 1; l < layers.Count; l++)
				layers[l].Calculate(test, layers[l - 1].GetValues(test));

			return layers[layers.Count - 1].GetAnswer(test);
		}

		public static float Recalculate(int test, Tester tester)
		{
			LayerRecalculateStatus lrs = LayerRecalculateStatus.First;

			if (lastMutatedLayer > 0)
				for (int layer = lastMutatedLayer; layer < layers.Count; layer++)
					lrs = layers[layer].Recalculate(test, layers[layer - 1].GetValues(test), lrs);
			else
			{
				float[][] array = new float[1][];
				array[0] = tester.tests[test];

				layers[0].Calculate(test, array);

				for (int layer = 1; layer < layers.Count; layer++)
					lrs = layers[layer].Recalculate(test, layers[layer - 1].GetValues(test), lrs);
			}

			return layers[layers.Count - 1].GetAnswer(test);
		}

		public static void Init()
		{
			InitTesters();
			FillRandomMutations();
			InitValues();
			InitActivationFunctions();
		}

		public static void InitTesters()
		{
			testerV = new Tester(testsCount, batchSize, "Grafic//ForValidation", "VALIDATION");
			testerE = new Tester(testsCount, batchSize, "Grafic//ForEvolution", "EVOLUTION");
		}

		public static void InitActivationFunctions()
		{
			for (int l = 0; l < layers.Count - 1; l++)
				layers[l].af = new TanH();
			layers[layers.Count - 1].af = new TanH();
		}

		public static void InitValues()
		{
			for (int l = 0; l < layers.Count; l++)
				layers[l].InitValues(testerE.testsCount);

			Log("NN values were initialized");
		}

		public static void FillRandomMutations()
		{
			randomMutates = new float[randomMutatesCount];

			for (int m = 0; m < randomMutatesCount; m++)
				randomMutates[m] = Extensions.NormalDistribution(randomMutatesScaleV2, randomMutatesSharpness, randomMutatesSmoothing);

			Log("Random mutations are filled.");
		}

		public static void EvolveByRandomMutations()
		{
			short previous = 0;
			string history = "";
			float er = 0;
			float record = FindErrorRateSquared(testerE);
			Log("Received current er_fb: " + record);

			for (int Generation = 0; ; Generation++)
			{
				Log("G" + Generation);

				SelectLayerForMutation();
				Mutate();

				er = RefindErrorRateSquared(testerE);

				if (er < record)
				{
					Log("er_nfb: " + er.ToString());
					Log($" ▲ Good mutation. (mutagen {mutagen})");
					record = er;
				}
				else if (er == record)
				{
					Log($" - Neutral mutation. Leave it. ({mutagen})");
				}
				else
				{
					Log($" ▽ Bad mutation. Go back. ({mutagen})");
					Demutate();
					er = FindErrorRateSquared(testerE);
				}

				history += record + "\r\n";

				if (Generation % 100 == 99)
				{
					Save();
					Disk.WriteToProgramFiles("EvolveHistory", "csv", history, true);
					history = "";

					Log("(!) er_nfb: " + er);
					er = FindErrorRateSquared(testerE);
					Log("(!) er_fb: " + er);

					string validation = Stat.CalculateStatistics(testerV);
					Disk.WriteToProgramFiles("Stat", "csv", Stat.StatToCsv("Validation") + "\n", true);
					string evolition = Stat.CalculateStatistics(testerE);
					Disk.WriteToProgramFiles("Stat", "csv", Stat.StatToCsv("Evolution"), true);

					Log("Evolution dataset:\n" + evolition);
					Log("Validation dataset:\n" + validation);
				}
			}
		}

		public static void EvolveByBackPropagtion()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
				float v = 0;
				float old_v = 0;
				float a = 0;

				float ert = FindErrorRateSquared(testerV);
				Log("Current ert: " + ert);
				float er = FindErrorRateSquared(testerE);
				Log("Current er: " + er);
				float old_er = er;
				float old_ert = ert;
				float ert_record = ert;

				for (int Generation = 0; ; Generation++)
				{
					Log("G" + Generation);

					vanishedGradients = 0;
					cuttedGradients = 0;
					for (int b = 0; b < testerE.batchesCount; b++)
					{
						testerE.FillBatch();
						FindBPGradients(testerE);
						CorrectWeightsByBP(testerE);
					}

					old_ert = ert;
					ert = FindErrorRateSquared(testerV);
					Log($"ert: {string.Format("{0:F8}", ert)} (v {string.Format("{0:F8}", ert - old_ert)})");
					old_er = er;
					er = FindErrorRateSquared(testerE);

					old_v = v;
					v = er - old_er;
					a = v - old_v;					

					Log($"er: {string.Format("{0:F8}", er)} (v {string.Format("{0:F8}", v)}) (a {string.Format("{0:F8}", a)}) (lmd {string.Format("{0:F7}", LYAMBDA)})");
					Log($"vanished {vanishedGradients} cutted {cuttedGradients}");
					Disk.WriteToProgramFiles("EvolveHistory", "csv", $"{er}, {ert}\r\n", true);

					Save();
					EarlyStopping();

					if (Generation % 20 == 19)
					{
						string validation = Stat.CalculateStatistics(testerV);
						Disk.WriteToProgramFiles("Stat", "csv", Stat.StatToCsv("Validation") + "\n", true);
						string evolition = Stat.CalculateStatistics(testerE);
						Disk.WriteToProgramFiles("Stat", "csv", Stat.StatToCsv("Evolution"), true);

						Log("Evolution dataset:\n" + evolition);
						Log("Validation dataset:\n" + validation);
					}
				}

				void EarlyStopping()
				{
					if (ert <= ert_record)
					{
						ert_record = ert;
						Disk.ClearDirectory($"{Disk.programFiles}\\NN\\EarlyStopping");
						File.Copy($"{Disk.programFiles}\\NN\\{name}.json", $"{Disk.programFiles}\\NN\\EarlyStopping\\{name} ({ert}).json");
						Log("NN copied for early stopping.");
					}
				}
			}
		}

		private static void FindBPGradients(Tester tester)
		{
			for (int test = 0; test < tester.tests.Length; test++)
				if (tester.batch[test] == 1)
				{
					layers[layers.Count - 1].FindBPGradient(test, tester.answers[test]);
					for (int layer = layers.Count - 2; layer >= 0; layer--)
						layers[layer].FindBPGradient(test, layers[layer + 1].AllBPGradients(test), layers[layer + 1].AllWeights);
				}

			SaveLastLayerWeights(); //
			Log("Gradients are found!");

			void SaveLastLayerWeights()
			{
				string str = "";

				for (int w = 0; w < (layers[layers.Count - 1] as LayerPerceptron).nodes[0].weights.Length; w++)
					str += (layers[layers.Count - 1] as LayerPerceptron).nodes[0].weights[w] + ",";

				Disk.WriteToProgramFiles("weights", "csv", str + "\n", true);
			}
		}

		private static void CorrectWeightsByBP(Tester tester)
		{
			for (int test = 0; test < tester.testsCount; test++)
			{
				if (tester.batch[test] == 1)
				{
					float[][] array = new float[1][];
					array[0] = tester.tests[test];

					layers[0].CorrectWeightsByBP(test, array);

					for (int l = 1; l < layers.Count; l++)
						layers[l].CorrectWeightsByBP(test, layers[l - 1].GetValues(test));
				}
			}
			Log("Weights are corrected!");
		}

		public static float FindErrorRateLinear(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester.testsCount / coresCount;

			float er = 0;
			float[] suber = new float[coresCount];

			int alive = coresCount;

			Thread[] subThreads = new Thread[coresCount];

			for (int core = 0; core < coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core);
			}

			void SubThread(object obj)
			{
				int core = (int)obj;

				for (int test = core * testsPerCoreCount; test < core * testsPerCoreCount + testsPerCoreCount; test++)
				{
					float prediction = Calculate(test, tester, tester.tests[test]);

					float reality = tester.answers[test];

					suber[core] += MathF.Abs(prediction - reality);
				}

				alive--;
			}

			long ms = DateTime.Now.Ticks;
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THE THREAD IS STACKED");
					for (int core = 0; core < coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < coresCount; core++)
				er += suber[core];

			er /= tester.testsCount;

			return er;
		}

		public static float FindErrorRateSquared(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester.testsCount / coresCount;

			float er = 0;
			float[] suber = new float[coresCount];

			int alive = coresCount;

			Thread[] subThreads = new Thread[coresCount];

			for (int core = 0; core < coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core);
			}

			void SubThread(object obj)
			{
				int core = (int)obj;

				for (int test = core * testsPerCoreCount; test < core * testsPerCoreCount + testsPerCoreCount; test++)
				{
					float prediction = Calculate(test, tester, tester.tests[test]);

					float reality = tester.answers[test];

					suber[core] += MathF.Pow(prediction - reality, 2);
				}
				
				alive--;
			}

			long ms = DateTime.Now.Ticks; 
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THE THREAD IS STACKED");
					for (int core = 0; core < coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < coresCount; core++)
				er += suber[core];

			er /= tester.testsCount;

			return er;
		}

		public static float RefindErrorRateSquared(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester.testsCount / coresCount;

			float er = 0;
			float[] suber = new float[coresCount];

			int alive = coresCount;

			Thread[] subThreads = new Thread[coresCount];

			for (int core = 0; core < coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core);
			}

			void SubThread(object obj)
			{
				int core = (int)obj;

				for (int test = core * testsPerCoreCount; test < core * testsPerCoreCount + testsPerCoreCount; test++)
				{
					float prediction = Recalculate(test, tester);

					float reality = tester.answers[test];

					suber[core] += MathF.Pow(prediction - reality, 2);
				}

				alive--;
			}

			long ms = DateTime.Now.Ticks;
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THE THREAD IS STACKED");
					for (int core = 0; core < coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}

			for (int core = 0; core < coresCount; core++)
				er += suber[core];

			er /= tester.testsCount;

			return er;
		}

		private static void SelectLayerForMutation()
		{
			//int number = linksToLayersToMutate[Storage.rnd.Next(linksToLayersToMutate.Count)];
			int number = Storage.rnd.Next(layers.Count - 0) + 0;

			lastMutatedLayer = number;
		}

		public static void NeuralBattle()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
				Init();

				float record = FindErrorRateSquared(testerE);
				Log($"record {record}");
				var files = Directory.GetFiles(Disk.programFiles + "NN");

				for (int n = 0; ; n++)
				{
					Create();
					Init();

					float er = FindErrorRateSquared(testerE);
					Log($"er {er}");

					if (er < record)
					{
						Logger.Log("Эта лучше!");
						record = er;
						Save();						
					}
					else
						Log("Эта не лучше!");
				}
			}
		}

		public static void Mutate()
		{
			SelectLayerForMutation();
			mutagen = randomMutates[Storage.rnd.Next(randomMutates.Length)];
			layers[lastMutatedLayer].Mutate(mutagen);
			Log($"Layer {lastMutatedLayer} is mutated.");
		}

		public static void Demutate()
		{
			layers[lastMutatedLayer].Demutate(mutagen);
		}

		public static float CutGradient(float g)
		{
			if (MathF.Abs(g) > cutter)
			{
				cuttedGradients++;
				return cutter * (g / g);
			}
			else
				return g;
		}
	}
}

