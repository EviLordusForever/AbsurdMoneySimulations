﻿using System;
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
		public static int layersCount = 7;
		public const float randomPower = 1.4f;
		public const int jumpLimit = 9000;

		public static List<LayerAbstract> layers;

		public static float[] randomMutates;
		public static List<byte> linksToLayersToMutate;
		public static int mutationSeed;
		public static int lastMutatedLayer;

		public static void Test()
		{
			Thread myThread = new Thread(TestThread);
			myThread.Start();

			void TestThread()
			{
/*				Create();
				Init();
				Save();*/
				Load();
				Init();
				NNTester.LoadGrafic();
				NNTester.FillTests();
				NNTester.FillAnswersForTests();
				Log(Calculate(0, NNTester.tests[0]).ToString());
				Log(Calculate(1, NNTester.tests[1]).ToString());
				Log(Calculate(0, NNTester.tests[0]).ToString());
				Log(Calculate(1, NNTester.tests[1]).ToString());
			}
		}

		public static void Create()
		{
			layers = new List<LayerAbstract>();

			//input 300

			layers.Add(new LayerMegatron(15, 55));   //55 x 30 x 15 = 24750
			layers[0].FillRandomly(15, 55, 30);

			layers.Add(new LayerCybertron(150)); //15 x 55 x 10 = 8250
			layers[1].FillRandomly(15, 10, 55);

			layers.Add(new LayerPerceptron(40)); //150 x 40 = 6000
			layers[2].FillRandomly(1, 40, 150);

			layers.Add(new LayerPerceptron(15)); //40 x 15 = 600
			layers[3].FillRandomly(1, 15, 40);

			layers.Add(new LayerPerceptron(1)); //15 x 1 = 15
			layers[4].FillRandomly(1, 1, 15);

			Log("Нейросеть создана !");
		}

		public static void Save()
		{
			var files = Directory.GetFiles(Disk.programFiles + "\\NN");

			JsonSerializerSettings jss = new JsonSerializerSettings();
			jss.Formatting = Formatting.Indented;

			File.WriteAllText(files[0], JsonConvert.SerializeObject(layers, jss));
			Log("Нейросеть успешно сохранена!");
		}

		public static void Load()
		{
			var files = Directory.GetFiles(Disk.programFiles + "\\NN");
			string toLoad = File.ReadAllText(files[0]);

			var jss = new JsonSerializerSettings();
			jss.Converters.Add(new LayerAbstractConverter());

			layers = JsonConvert.DeserializeObject<List<LayerAbstract>>(toLoad, jss);

			Log("ЗА ГРУ ЖЕ НО !");


		}

		public static float Calculate(int test, float[] input)
		{
			float[][] array = new float[1][];
			array[0] = input;

			layers[0].Calculate(test, array);

			for (int l = 1; l < layers.Count; l++)
				layers[l].Calculate(test, layers[l - 1].GetValues(test));

			return layers[layers.Count - 1].values[test][0][0] * 1000;
		}

		public static float ThinkNotFromBeginning(int test)
		{
			layers[lastMutatedLayer].CalculateOneNode(test); /////


			l++;

			for (; l < layersCount; l++)
				layers[l].Calculate(test, layers[l - 1].values[test]);
				/////////////////////////////////

			return layers[l - 1].values[test][0][0] * 1000;
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

		public static void Educate()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			float l;
			float r;

			void SoThread()
			{
				NN.Init();
				float error_rate = 0;
				float mutagen = 0;
				short previous = 0;
				string history = "";
				float record = FindErrorRate();
				Log("Получен текущий er_fb: " + record);
				int previous_mutated_layer = 0;
				int previous_mutated_node = 0;
				lastMutatedLayer = 0;
				float er = 0;

				for (int Generation = 0; ; Generation++)
				{
					Log("Поколение " + Generation);

					mutationSeed = Storage.rnd.Next(1000000000);
					mutagen = randomMutates[mutationSeed % randomMutates.Length];
					previous_mutated_layer = lastMutatedLayer;
					//previous_mutated_node = lastMutatedNode;
					//////////////////////////////////
					SelectLayerForMutation();

					Mutate(1, mutagen);

					er = FindErrorRateNotFromBeginning();

					Log("er_nfb: " + er.ToString());

					if (er < record)
					{
						Log(" ▲ Хорошая мутация.");
						record = er;
						//Optimize();
					}
					else
					{
						Log(" ▽ Плохая мутация. Откат.");
						Mutate(1, -mutagen);
					}

					history += record + "\r\n";

					if (Generation % 100 == 99)
					{
						Save();
						Disk.WriteToProgramFiles("Trading\\history", history, true);
						history = "";

						GetStatistics();

						//Test();

						void Test()
						{
							Log("ПРОВЕДУ ТЕСТ!");
							Log("Часть 1:");
							Log("Провека: ");
							float error1 = FindErrorRateNotFromBeginning();
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
							float error3 = FindErrorRateNotFromBeginning();
							Log("er not from beginning: " + error3);
							float error4 = FindErrorRate();
							Log("er from beginning: " + error4);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Часть 3:");
							Log("Теперь переинициализирую: ");
							Init();
							Log("Провека: ");
							float error5 = FindErrorRateNotFromBeginning();
							Log("er not from beginning: " + error5);
							float error6 = FindErrorRate();
							Log("er from beginning: " + error6);
							float error7 = FindErrorRateNotFromBeginning();
							Log("er not from beginning: " + error7);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Такие дела. Жду 60 сек.");
							Thread.Sleep(60000);
						}
					}
				}

				void Optimize()
				{
					//Эта оптимизация почему-то быстро загоняет его
					//в локальный минимуми ни к чему хорошему не приводит(

					string res = "";
					float n = 0;
					float speed = 2;

					while (Math.Abs(mutagen) > 0.0025)
					{
						if (Math.Abs(mutagen) > 20)
							break;

						n++;

						Log("er: " + record + "; mutagen: " + mutagen.ToString());

						history += record + "\r\n";

						NN.Mutate(10, mutagen);
						r = FindErrorRate();
						NN.Mutate(10, mutagen * -2);
						l = FindErrorRate();
						NN.Mutate(10, mutagen);

						if (l > record && r > record)
						{
							mutagen /= speed;
							previous = 0;
						}
						else if (r < l)
						{
							record = r;

							if (mutagen < 0)
								mutagen = -mutagen;

							NN.Mutate(1, mutagen);

							if (previous == 1)
								mutagen *= speed;
							previous = 1;
						}
						else
						{
							record = l;

							if (mutagen > 0)
								mutagen = -mutagen;

							NN.Mutate(1, -mutagen);

							if (previous == -1)
								mutagen *= speed;
							previous = -1;
						}
					}

					//System.IO.File.AppendAllText(Memory.programFiles + "\\Trading\\TEST.csv", res, Encoding.UTF8);
					//Log(Memory.programFiles + "\\Trading\\TEST.csv");
				}

				float FindErrorRateNotFromBeginning()
				{ //По поводу l n запутанно получилось, но это крутая оптимизация

					int l_;
					int n_;

					//Вершина запутанности, но она таки нужна для оптимизации
					if (previous_mutated_layer < lastMutatedLayer)
					{
						l_ = previous_mutated_layer;
						n_ = previous_mutated_node;
					}
					else if (previous_mutated_layer > lastMutatedLayer)
					{
						l_ = lastMutatedLayer;
						//n_ = lastMutatedNode;
						/////////////////////////
					}
					else
					{
						l_ = previous_mutated_layer;
						n_ = previous_mutated_node;

						//for (int test = 0; test < NNTester.testsCount; test++)
						//	RecalcOnlyOneNode(test, NNTester.testStartPoints[test], l_, n_);
						////////////////////////////
						l_ = lastMutatedLayer;
						//n_ = lastMutatedNode;
						////////////////////////////////
					}

					error_rate = 0;
					for (int test = 0; test < NNTester.testsCount; test++)
					{
						//float prediction = ThinkNotFromBeginning(test, deltas[test], l_, n_);
						////////////////////////

						float reality = NNTester.answers[test];

						//error_rate += Math.Abs(prediction - reality);
						/////////////////////////////////
					}

					error_rate /= NNTester.testsCount;

					return error_rate;
				}

				float FindErrorRate()
				{
					error_rate = 0;
					for (int test = 0; test < NNTester.testsCount; test++)
					{
						float prediction = 0;// Calculate(test, NNTester.testStartPoints[test]);
						///////////////////////////////////

						float reality = NNTester.answers[test];

						error_rate += MathF.Abs(prediction - reality);
					}

					error_rate /= NNTester.testsCount;

					return error_rate;
				}
			}
		}

		public static void SelectLayerForMutation()
		{
			int number = mutationSeed % linksToLayersToMutate.Count;

			lastMutatedLayer = linksToLayersToMutate[number];
		}

		public static void Neural_battle()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
/*				if (NNTester.testStartPoints == null)
					Init();*/

				Load();

				float record = GetStatistics();

				for (int n = 0; n < 30; n++)
				{
					var files = Directory.GetFiles(Disk.programFiles + "Trading\\NN");
					string recordsmen = File.ReadAllText(files[0]);

					Create();
					Save();
					Load();
					var er = GetStatistics();
					if (er < record)
					{
						Logger.Log("Эта лучше!");
						record = er;
						recordsmen = File.ReadAllText(files[0]);
						File.WriteAllText(files[0] + "_recordsmen_copy.txt", recordsmen);
					}
					else
					{
						Log("Эта не лучше!");
						File.WriteAllText(files[0], recordsmen);
						File.WriteAllText(files[0] + "_recordsmen_copy.txt", recordsmen);
					}
				}
			}
		}

		public static float GetStatistics()
		{
			return 404;
		}

		public static void Mutate(int count, float mutagen)
		{
			for (int i = 0; i < count; i++)
				layers[lastMutatedLayer].Mutate(mutagen);
		}
	}
}

