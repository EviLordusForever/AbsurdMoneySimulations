using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
	public static class NN
	{
		public const int horizon = 29;
		public const int inputWindow = 300;
		public const float randomPower = 1.4f;
		public const int jumpLimit = 9000;

		public static List<LayerAbstract> layers;

		public static float mutagen;
		public static float[] randomMutates;
		public static int mutationSeed;
		public static int lastMutatedLayer;

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

			layers.Add(new LayerMegatron(15, 55, 30, 5));   //55 x 30 x 15 = 24750
			layers[0].FillWeightsRandomly();

			layers.Add(new LayerCybertron(15, 55, 10, 150)); //15 x 55 x 10 = 8250
			layers[1].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(40, 150)); //150 x 40 = 6000
			layers[2].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(15, 40)); //40 x 15 = 600
			layers[3].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(1, 15)); //15 x 1 = 15
			layers[4].FillWeightsRandomly();

			/*			layers.Add(new LayerPerceptron(40, 300));
						layers[0].FillWeightsRandomly();

						layers.Add(new LayerPerceptron(15, 40)); //40 x 15 = 600
						layers[1].FillWeightsRandomly();

						layers.Add(new LayerPerceptron(1, 15)); //15 x 1 = 15
						layers[2].FillWeightsRandomly();*/

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

			return layers[layers.Count - 1].GetAnswer(test) * 100;
			//What? |
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

			return layers[layers.Count - 1].GetAnswer(test) * 100;
		}

		public static void Init()
		{
			FillRandomMutations();
			InitValues();
		}

		public static void InitValues()
		{
			for (int l = 0; l < layers.Count; l++)
				layers[l].InitValues();

			Log("NN values were initialized");
		}

		public static void FillRandomMutations()
		{
			randomMutates = new float[1019];

			for (int i = 0; i < 1019; i++)
			{
				for (int j = 0; j < 10000; j++)
					randomMutates[i] += (float)Storage.rnd.NextSingle();

				randomMutates[i] -= 5000;
				randomMutates[i] *= 0.05f;
			}

			Log("Random mutations are filled.");
		}

		public static void Evolve()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			float l;
			float r;

			void SoThread()
			{
				short previous = 0;
				string history = "";
				float er = 0;
				float record = FindErrorRate();
				Log("Received current er_fb: " + record);
				Log("1\n2\n3\naaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

				for (int Generation = 0; ; Generation++)
				{
					Log("er_nfb: " + RefindErrorRate());
					Log("er_fb: " + FindErrorRate());
					Log("er_nfb: " + RefindErrorRate());
					Log(NNStatManager.GetStatistics());
					Log("er_nfb: " + RefindErrorRate());
					Log("er_fb: " + FindErrorRate());
					Log("er_nfb: " + RefindErrorRate());
					Log(NNStatManager.GetStatistics());
					Log(NNStatManager.GetStatistics());
					Log(NNStatManager.GetStatistics());
					Thread.Sleep(666000);

					Log("Generation " + Generation);

					SelectLayerForMutation();
					Mutate();

					er = RefindErrorRate();
					Log("er_nfb: " + er.ToString());

					if (er < record)
					{
						Log(" ▲ Good mutation.");
						record = er;
					}
					else if (er == record)
					{
						Log(" - Neutral mutation. Leave it as it is.");
					}
					else
					{
						Log(" ▽ Bad mutation. Go back.");
						Demutate();
						er = FindErrorRate();
						Log("er_nfb back: " + er.ToString());
					}

					history += record + "\r\n";

					if (Generation % 100 == 99)
					{
						Save();
						Disk.WriteToProgramFiles("EvolveHistory", "csv", history, true);
						history = "";

						Log("(!) er_nfb: " + er.ToString());
						er = FindErrorRate();
						Log("(!) er_fb: " + er.ToString());

						GetStatistics();

						void Test()
						{
							Log("ПРОВЕДУ ТЕСТ!");
							Log("Часть 1:");
							Log("Провека: ");
							float error1 = RefindErrorRate();
							Log("er not from beginning: " + error1);
							float error2 = FindErrorRate();
							Log("er from beginning: " + error2);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Часть 2:");
							Log("Сохраню ее.");
							Save();
							Log("Теперь загружу ее.");
							Load();
							Log("Инит занчений.");
							InitValues();
							Log("Провека: ");
							float error3 = RefindErrorRate();
							Log("er not from beginning: " + error3);
							float error4 = FindErrorRate();
							Log("er from beginning: " + error4);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Часть 3:");
							Log("Теперь переинициализирую: ");
							Init();
							Log("Провека: ");
							float error5 = RefindErrorRate();
							Log("er not from beginning: " + error5);
							float error6 = FindErrorRate();
							Log("er from beginning: " + error6);
							float error7 = RefindErrorRate();
							Log("er not from beginning: " + error7);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Такие дела. Жду 60 сек.");
							Thread.Sleep(60000);
						}
					}
				}
			}
		}

		public static float FindErrorRate()
		{
			restart:

			int coresCount = 4;
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

		public static float RefindErrorRate()
		{
			restart:

			int coresCount = 4;
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

					suber[core] += MathF.Abs(prediction - reality);
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
				//Create(); ///////////
				//Save();
				//Load();
				Init();
				NNTester.LoadGrafic();
				NNTester.FillTests();

				float record = FindErrorRate();
				Log($"record {record}");
				var files = Directory.GetFiles(Disk.programFiles + "NN");

				for (int n = 0; ; n++)
				{
					Create();
					Init();

					float er = FindErrorRate();
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

		public static float GetStatistics()
		{
			return 404;
		} //

		public static void Mutate()
		{
			SelectLayerForMutation();
			mutagen = randomMutates[Storage.rnd.Next(randomMutates.Length)];
			layers[lastMutatedLayer].Mutate(mutagen);
			Log($"Layer {lastMutatedLayer} is mutated");
		}

		public static void Demutate()
		{
			layers[lastMutatedLayer].Demutate(mutagen);
			Log("Demutated");
		}
	}
}

