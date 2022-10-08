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
		public const float randomPower = 0.4f;
		public const int jumpLimit = 9000;

		public static int randomMutatesCount = 2022;

		public static float randomMutatesSharpness =  10;
		public static float randomMutatesScaleV2 = 10;
		public static float randomMutatesSmoothing = 0.03f;

		public static List<LayerAbstract> layers;

		public static float mutagen;
		public static float[] randomMutates;
		public static int mutationSeed;
		public static int lastMutatedLayer;

		public static float LYAMBDA = 0.01f;
		public static float INERTION = 0f;

		public static void Create()
		{
			layers = new List<LayerAbstract>();

			//30*15
			//55*10*15
			//150*40

			//300
			//55 x 15 = 825
			//10 x 15 = 150
			//40
			//15
			//1

			/*			layers.Add(new LayerMegatron(15, 55, 30, 5));   //55 x 30 x 15 = 24750
						layers[0].FillWeightsRandomly();

						layers.Add(new LayerCybertron(15, 55, 10, 150)); //15 x 55 x 10 = 8250
						layers[1].FillWeightsRandomly();

						layers.Add(new LayerPerceptron(40, 150)); //150 x 40 = 6000
						layers[2].FillWeightsRandomly();

						layers.Add(new LayerPerceptron(15, 40)); //40 x 15 = 600
						layers[3].FillWeightsRandomly();

						layers.Add(new LayerPerceptron(1, 15)); //15 x 1 = 15
						layers[4].FillWeightsRandomly();*/

			//layers.Add(new LayerPerceptron(10, 300)); //40 x 15 = 600
			//layers[0].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(1, 300)); //15 x 1 = 15
			layers[0].FillWeightsRandomly();

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
			string toLoad = File.ReadAllText(files[0]);

			var jss = new JsonSerializerSettings();
			jss.Converters.Add(new LayerAbstractConverter());

			layers = JsonConvert.DeserializeObject<List<LayerAbstract>>(toLoad, jss);

			Log("Neural Network loaded from disk!");
		}

		public static float Calculate(int test, float[] input)
		{
			float[][] array = new float[1][];
			array[0] = input;

			layers[0].Calculate(test, array);

			for (int l = 1; l < layers.Count; l++)
				layers[l].Calculate(test, layers[l - 1].GetValues(test));

			return layers[layers.Count - 1].GetAnswer(test);
		}

		public static float Recalculate(int test)
		{
			LayerRecalculateStatus lrs = LayerRecalculateStatus.First;

			if (lastMutatedLayer > 0)
				for (int layer = lastMutatedLayer; layer < layers.Count; layer++)
					lrs = layers[layer].Recalculate(test, layers[layer - 1].GetValues(test), lrs);
			else
			{
				float[][] array = new float[1][];
				array[0] = NNTester.tests[test];

				layers[0].Calculate(test, array);

				for (int layer = 1; layer < layers.Count; layer++)
					lrs = layers[layer].Recalculate(test, layers[layer - 1].GetValues(test), lrs);
			}

			return layers[layers.Count - 1].GetAnswer(test);
		}

		public static void Init()
		{
			FillRandomMutations();
			InitValues();
			InitActivationFunctions();
		}

		public static void InitActivationFunctions()
		{
			for (int l = 0; l < layers.Count - 1; l++)
				layers[l].af = new ClassicAF();
			layers[layers.Count - 1].af = new ClassicAF();
		}

		public static void InitValues()
		{
			for (int l = 0; l < layers.Count; l++)
				layers[l].InitValues();

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
			float record = FindErrorRateSquared();
			Log("Received current er_fb: " + record);

			for (int Generation = 0; ; Generation++)
			{
				Log("G" + Generation);

				SelectLayerForMutation();
				Mutate();

				er = RefindErrorRate();

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
					er = FindErrorRateSquared();
				}

				history += record + "\r\n";

				if (Generation % 100 == 99)
				{
					Save();
					Disk.WriteToProgramFiles("EvolveHistory", "csv", history, true);
					history = "";

					Log("(!) er_nfb: " + er.ToString());
					er = FindErrorRateSquared();
					Log("(!) er_fb: " + er.ToString());

					Log("Evolution dataset:\n" + NNStatManager.CalculateStatistics());
					Disk.WriteToProgramFiles("Stat", "csv", NNStatManager.StatToCsv("Evolution"), true);

					NNTester.InitForTesting();
					Log("Testing dataset:\n" + NNStatManager.CalculateStatistics());
					Disk.WriteToProgramFiles("Stat", "csv", NNStatManager.StatToCsv("Testing") + "\n", true);
					NNTester.InitForEvolution();
				}
			}
		}

		public static void EvolveByBackPropagtion()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
				short previous = 0;
				string history = "";
				float er = FindErrorRateSquared();
				float old_er = er;
				Log("Current er_fb: " + er);

				for (int Generation = 0; ; Generation++)
				{
					Log("G" + Generation);

					for (int b = 0; b < NNTester.batchesCount; b++)
					{
						NNTester.FillBatch();
						FindBPGradients();
						CorrectWeightsByBP();
					}

					er = FindErrorRateSquared();
					Log($"er: {er}");
					history += er + "\r\n";

					if (Generation % 100 == 99)
					{
						Save();
						Disk.WriteToProgramFiles("EvolveHistory", "csv", history, true);
						history = "";

						Log("Evolution dataset:\n" + NNStatManager.CalculateStatistics());
						Disk.WriteToProgramFiles("Stat", "csv", NNStatManager.StatToCsv("Evolution"), true);

						NNTester.InitForTesting();
						Log("Testing dataset:\n" + NNStatManager.CalculateStatistics());
						Disk.WriteToProgramFiles("Stat", "csv", NNStatManager.StatToCsv("Testing") + "\n", true);
						NNTester.InitForEvolution();
						FindErrorRateSquared();
					}
				}
			}
		}

		private static void FindBPGradients()
		{
			for (int test = 0; test < NNTester.tests.Length; test++)
			{
				if (NNTester.batch[test] == 1)
				{
					layers[layers.Count - 1].FindBPGradient(test, NNTester.answers[test]);
					for (int layer = layers.Count - 2; layer >= 0; layer--)
						layers[layer].FindBPGradient(test, layers[layer + 1].AllBPGradients(test), layers[layer + 1].AllWeights);
				}
			}

			//SaveLastLayerWeights();
			Log("Gradients are found!");

			void SaveLastLayerWeights()
			{
				string str = "";

				for (int w = 0; w < (layers[layers.Count - 1] as LayerPerceptron).nodes[0].weights.Length; w++)
					str += (layers[layers.Count - 1] as LayerPerceptron).nodes[0].weights[w] + ",";

				Disk.WriteToProgramFiles("weights", "csv", str + "\n", true);
			}
		}

		private static void CorrectWeightsByBP()
		{
			for (int test = 0; test < NNTester.testsCount; test++)
			{
				if (NNTester.batch[test] == 1)
				{
					float[][] array = new float[1][];
					array[0] = NNTester.tests[test];

					layers[0].CorrectWeightsByBP(test, array);

					for (int l = 1; l < layers.Count; l++)
						layers[l].CorrectWeightsByBP(test, layers[l - 1].GetValues(test));
				}
			}
			Log("Weights are corrected!");
		}

		public static float FindErrorRateSquared()
		{
			restart:

			int testsPerCoreCount = NNTester.testsCount / coresCount;

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
					float prediction = Calculate(test, NNTester.tests[test]);

					float reality = NNTester.answers[test];

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

			er /= NNTester.testsCount;

			return er;
		}

		public static float RefindErrorRate()
		{
			restart:

			int testsPerCoreCount = NNTester.testsCount / coresCount;

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
					float prediction = Recalculate(test);

					float reality = NNTester.answers[test];

					suber[core] += MathF.Pow(prediction - reality, 2);
				}

				alive--;
				//Log($"core{core}:" + suber[core]);
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

			er /= NNTester.testsCount;

			return er;
		}

		public static float FindErrorRateLinear()
		{
			restart:

			int testsPerCoreCount = NNTester.testsCount / coresCount;

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
					float prediction = Calculate(test, NNTester.tests[test]);

					float reality = NNTester.answers[test];

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

			er /= NNTester.testsCount;

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
				NNTester.InitForEvolution();

				float record = FindErrorRateSquared();
				Log($"record {record}");
				var files = Directory.GetFiles(Disk.programFiles + "NN");

				for (int n = 0; ; n++)
				{
					Create();
					Init();

					float er = FindErrorRateSquared();
					Log($"er {er}");

					if (er < record)
					{
						Logger.Log("Эта лучше!");
						record = er;
						Save();						
					}
					else
					{
						Log("Эта не лучше!");
					}
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
	}
}

