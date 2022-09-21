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
		public const float randomPower = 2f;
		public const int jumpLimit = 9000;

		public static List<LayerAbstract> layers;

		public static float mutagen;
		public static float[] randomMutates;
		public static List<byte> linksToLayersToMutate;
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

			layers.Add(new LayerMegatron(15, 55, 5));   //55 x 30 x 15 = 24750
			layers[0].FillRandomly(15, 55, 30);

			layers.Add(new LayerCybertron(150)); //15 x 55 x 10 = 8250
			layers[1].FillRandomly(15, 10, 55);

			layers.Add(new LayerPerceptron(40)); //150 x 40 = 6000
			layers[2].FillRandomly(1, 40, 150);

			layers.Add(new LayerPerceptron(15)); //40 x 15 = 600
			layers[3].FillRandomly(1, 15, 40);

			layers.Add(new LayerPerceptron(1)); //15 x 1 = 15
			layers[4].FillRandomly(1, 1, 15);

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

			return layers[layers.Count - 1].GetAnswer(test) * 1000;
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

			return layers[layers.Count - 1].GetAnswer(test) * 1000;
		}

		public static void Init()
		{
			FillRandomMutations();
			FillLinksToAllWeights();
		}

		public static void FillRandomMutations()
		{
			randomMutates = new float[1019];

			for (int i = 0; i < 1019; i++)
			{
				for (int j = 0; j < 10000; j++)
					randomMutates[i] += (float)Storage.rnd.NextSingle();

				randomMutates[i] -= 5000;
				randomMutates[i] *= 0.1f;
			}

			Log("Случайные мутации заполнены.");
		}

		public static void FillLinksToAllWeights()
		{
			//PLEASE, JUST BELIVE THAT IS NORMAL

			linksToLayersToMutate = new List<byte>();

			for (byte l = 0; l < layers.Count; l++)
			{
				int weightsCount = layers[l].WeightsCount;

				for (int w = 0; w < weightsCount; w++)
					linksToLayersToMutate.Add(l);
			}

			Log("Ссылки на все веса поставлены. Весов: " + linksToLayersToMutate.Count);
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

				for (int Generation = 0; ; Generation++)
				{
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
						er = RefindErrorRate();
						Log("er_nfb back: " + er.ToString());
					}

					history += record + "\r\n";

					if (Generation % 100 == 99)
					{
						Save();
						Disk.WriteToProgramFiles("EvolveHistory", "csv", history, true);
						history = "";

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
			int testsPerThreadCount = NNTester.testsCount / coresCount;

			float er = 0;

			int alive = 4;

			Thread[] subThreads = new Thread[4];

			for (int core = 0; core < coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core * testsPerThreadCount);
			}

			void SubThread(object obj)
			{
				int x = (int)obj;

				for (int test = x; test < x + testsPerThreadCount; test++)
				{
					float prediction = Calculate(test, NNTester.tests[test]);

					float reality = NNTester.answers[test];

					er += MathF.Abs(prediction - reality);
				}

				alive--;
			}

			long ms = DateTime.Now.Ticks; 
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THREAD IS STACKED");
					for (int core = 0; core < coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}

			er /= NNTester.testsCount;

			return er;
		}

		public static float RefindErrorRate()
		{
			restart:

			int coresCount = 4;
			int testsPerThreadCount = NNTester.testsCount / coresCount;

			float er = 0;

			int alive = 4;

			Thread[] subThreads = new Thread[4];

			for (int core = 0; core < coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core * testsPerThreadCount);
			}

			void SubThread(object obj)
			{
				int x = (int)obj;

				for (int test = x; test < x + testsPerThreadCount; test++)
				{
					float prediction = Recalculate(test);

					float reality = NNTester.answers[test];

					er += MathF.Abs(prediction - reality);
				}

				alive--;
			}

			long ms = DateTime.Now.Ticks;
			while (alive > 0)
			{
				if (DateTime.Now.Ticks > ms + 10000 * 1000 * 10)
				{
					Log("THREAD IS STACKED");
					for (int core = 0; core < coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}

			er /= NNTester.testsCount;

			return er;
		}

		private static void SelectLayerForMutation()
		{
			//int number = linksToLayersToMutate[Storage.rnd.Next(linksToLayersToMutate.Count)];
			int number = Storage.rnd.Next(layers.Count);

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

