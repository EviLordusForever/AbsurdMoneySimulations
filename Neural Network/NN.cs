﻿using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbsurdMoneySimulations.Logger;
using System.Text.Json;

namespace AbsurdMoneySimulations
{
	public static class NN
	{
		public const int horizon = 29;
		public const int testsCount = 2000;
		public const int inputWindow = 500;
		public static int layersCount = 7;
		public const double randomPower = 1.4;
		public const int jumpLimit = 9000;

		public static List<AbstractLayer> layers;

		public static double[] grafic; //[-1, 1]
		public static List<double> realGrafic;

		public static List<int> availableGraficPoints;
		public static double[] answers;
		public static int[] deltas;

		public static double[] randomMutates;

		public static List<int[]> linksToWeights;

		
		public static int globalWeightsCount;

		public static int mutationSeed;
		public static int lastMutatedLayer;
		public static int lastMutatedNode;
		public static int lastMutatedWeight;

		public static void CreateNewNN()
		{
			layers = new List<AbstractLayer>();

			layers.Add(new LayerPerceptron()); //300
			layers[0].FillRandomly(1, 300, 0);

			layers.Add(new LayerMegatron());   //55 x 30 x 15 = 24750
			(layers[1] as LayerMegatron).FillRandomly(55, 15, 30);

			layers.Add(new LayerPerceptron[15]); //15 x 55 x 10 = 8250
			for (int i = 0; i < 15; i++)
			{
				(layers[2] as LayerPerceptron[])[i] = new LayerPerceptron();
				(layers[2] as LayerPerceptron[])[i].FillRandomly(10, 55);
			}

			layers.Add(new LayerPerceptron()); //150 x 40 = 6000
			(layers[3] as LayerPerceptron).FillRandomly(40, 150);

			layers.Add(new LayerPerceptron()); //40 x 15 = 600
			(layers[4] as LayerPerceptron).FillRandomly(15, 40);

			layers.Add(new LayerPerceptron()); //15 x 1 = 15
			(layers[5] as LayerPerceptron).FillRandomly(1, 15);

			Log("Нейросеть создана !");
		}

		public static void Save()
		{
			var files = Directory.GetFiles(Disk.programFiles + "Trading\\NN");
			File.WriteAllText(files[0], string.Join("l", layers));
			//?????
			Log("Нейросеть успешно сохранена!");
		}

		public static void Load()
		{
			var files = Directory.GetFiles(Disk.programFiles + "Trading\\NN");
			string toLoad = File.ReadAllText(files[0]);

			FileStream stream = new FileStream(toLoad, FileMode.Open, FileAccess.Read);

			layers = JsonSerializer.Deserialize(stream, typeof(ArrayList)) as ArrayList;

/*			string[] layersStrs = toLoad.Split('l');
			layers = new ArrayList(layersStrs.Count());
			layersCount = layersStrs.Count();

			for (int l = 0; l < layersStrs.Count(); l++)
			{
				string[] nodesStrs = layersStrs[l].Split('n');
				if (nodesStrs.Contains("n"))
					layers[l] = new LayerPerceptron(nodesStrs);
				else
					layers[l] = new LayerMegatron(nodesStrs);
			}*/

			Log("ЗА ГРУ ЖЕ НО !");
		}

		public static double Think(int test, int delta)
		{
			(layers[1] as LayerPerceptron).Calculate(test, (layers[0] as LayerPerceptron).values[test], 0);

			return (layers[1] as LayerPerceptron).values[test][0] * 1000;
		}

		public static double ThinkNotFromBeginning(int test, int delta, int l, int n)
		{
			if (l > 0)
			{
				RecalcOnlyOneNode(test, delta, l, n);

				l++;

				for (; l < layersCount; l++)
					layers[l].Calculate(test, layers[l - 1].values[test], delta);

				return layers[l - 1].values[test][0] * 1000;
			}
			else
			{
				//Должно возникать в самом начале
				//Когда ничего еще не мутировало и
				//"Предыдущий мутировавший слой" == 0
				return Think(test, delta);
			}
		}

		public static void RecalcOnlyOneNode(int test, int delta, int l, int n)
		{
			if (l == 1)
				(layers[l] as LayerPerceptron).CalculateOneNode(test, grafic, 0, n); /////
			else
				(layers[l] as LayerPerceptron).CalculateOneNode(test, (layers[l + 1] as LayerPerceptron).values[test], 0, n);
		}

		public static void Born()
		{
			if (layers == null)
				Load();

			LoadGrafic();
			FillRandomMutations();
			FillLinksToWeights();
			FillDeltas();
			FillAnswers();

			void FillRandomMutations()
			{
				randomMutates = new double[1019];

				for (int i = 0; i < 1019; i++)
				{
					for (int j = 0; j < 10000; j++)
						randomMutates[i] += Storage.rnd.NextDouble();

					randomMutates[i] -= 5000;
					randomMutates[i] *= 0.1;
				}

				Log("Случайные мутации заполнены.");
			}

			void FillLinksToWeights()
			{
				//PLEASE, JUST BELIVE THAT IS NORMAL

				linksToWeights = new List<int[]>();

				for (int s = 0; s < (layers[1] as LayerMegatron).subs.Count(); s++)
					if ((layers[1] as LayerMegatron).subs[s].weights != null)
						for (int w = 0; w < (layers[1] as LayerMegatron).subs[s].weights.Count(); w++)
							linksToWeights.Add(new int[] { 1, s, w });


				for (int s = 0; s < (layers[1] as LayerMegatron).subs.Count(); s++)
				{
					for (int n = 0; n < (layers[2] as LayerPerceptron).nodes.Count(); n++)
						if ((layers[2] as LayerPerceptron).nodes[n].weights != null)
							for (int w = 0; w < (layers[2] as LayerPerceptron).nodes[n].weights.Count(); w++)
								linksToWeights.Add(new int[] { 2, n, w });
				}

				for (int l = 3; l <= 5; l++)
				{
					for (int n = 0; n < (layers[l] as LayerPerceptron).nodes.Count(); n++)
						if ((layers[l] as LayerPerceptron).nodes[n].weights != null)
							for (int w = 0; w < (layers[l] as LayerPerceptron).nodes[n].weights.Count(); w++)
								linksToWeights.Add(new int[] { l, n, w });
				}

				globalWeightsCount = linksToWeights.Count();

				Log("Ссылки на все веса поставлены. Весов: " + globalWeightsCount);
			}

			void FillDeltas()
			{
				int maximalDelta = availableGraficPoints.Count();
				double delta_delta = 1.02 * (double)maximalDelta / testsCount;

				deltas = new int[testsCount];

				int i = 0;
				for (double delta = 0; delta < maximalDelta; delta += delta_delta)
					deltas[i++] = availableGraficPoints[Convert.ToInt32(delta)];

				Log("Отступы для тестов заполнены.");
			}

			void FillAnswers()
			{
				answers = new double[grafic.Length];
				for (int i = NN.inputWindow - 1; i < grafic.Length - horizon - 1; i++)
					for (int j = 1; j <= horizon; j++)
						answers[i] += grafic[i + j];

				Log("Ответы заполнены.");
			}
		}

		public static void LoadGrafic()
		{
			var files = Directory.GetFiles(Disk.programFiles + "Trading\\Grafic");
			var graficL = new List<double>();
			availableGraficPoints = new List<int>();

			int g = 0;

			for (int f = 0; f < files.Length; f++)
			{
				string[] lines = File.ReadAllLines(files[f]);

				int l = 0;
				while (l < lines.Length)
				{
					graficL.Add(Brain.Normalize(Convert.ToDouble(lines[l])));

					if (l < lines.Length - inputWindow - horizon - 1)
						availableGraficPoints.Add(g);

					l++; g++;
				}
			}

			grafic = graficL.ToArray();

			Log("График (сборный) для обучения загружен.");
			Log("По совместимости также загружены доступные точки на графике.");
			Log("Полученная длина графика: " + grafic.Length);
		}

		public static void Educate()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			double l;
			double r;

			void SoThread()
			{
				NN.Born();
				double error_rate = 0;
				double mutagen = 0;
				short previous = 0;
				string history = "";
				double record = FindErrorRate();
				Log("Получен текущий er_fb: " + record);
				int previous_mutated_layer = 0;
				int previous_mutated_node = 0;
				lastMutatedLayer = 0;
				double er = 0;

				for (int Generation = 0; ; Generation++)
				{
					Log("Поколение " + Generation);

					mutationSeed = Storage.rnd.Next(1000000000);
					mutagen = randomMutates[mutationSeed % randomMutates.Length];
					previous_mutated_layer = lastMutatedLayer;
					previous_mutated_node = lastMutatedNode;
					SelectLNW();

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
							double error1 = FindErrorRateNotFromBeginning();
							Log("er not from beginning: " + error1);
							double error2 = FindErrorRate();
							Log("er from beginning: " + error2);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Часть 2:");
							Log("Сохраню ее.");
							Save();
							Log("Теперь загружу ее.");
							Load();
							Log("Провека: ");
							double error3 = FindErrorRateNotFromBeginning();
							Log("er not from beginning: " + error3);
							double error4 = FindErrorRate();
							Log("er from beginning: " + error4);
							Log("Теперь статистика: ");
							GetStatistics();
							Log("Часть 3:");
							Log("Теперь переинициализирую: ");
							Born();
							Log("Провека: ");
							double error5 = FindErrorRateNotFromBeginning();
							Log("er not from beginning: " + error5);
							double error6 = FindErrorRate();
							Log("er from beginning: " + error6);
							double error7 = FindErrorRateNotFromBeginning();
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
					double n = 0;
					double speed = 2;

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

				double FindErrorRateNotFromBeginning()
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
						n_ = lastMutatedNode;
					}
					else
					{
						l_ = previous_mutated_layer;
						n_ = previous_mutated_node;

						for (int test = 0; test < testsCount; test++)
							RecalcOnlyOneNode(test, deltas[test], l_, n_);

						l_ = lastMutatedLayer;
						n_ = lastMutatedNode;
					}

					error_rate = 0;
					for (int test = 0; test < testsCount; test++)
					{
						double prediction = ThinkNotFromBeginning(test, deltas[test], l_, n_);

						double reality = answers[deltas[test] + NN.inputWindow];

						error_rate += Math.Abs(prediction - reality);
					}

					error_rate /= testsCount;

					return error_rate;
				}

				double FindErrorRate()
				{
					error_rate = 0;
					for (int test = 0; test < testsCount; test++)
					{
						double prediction = Think(test, deltas[test]);

						double reality = answers[deltas[test] + NN.inputWindow];

						error_rate += Math.Abs(prediction - reality);
					}

					error_rate /= testsCount;

					return error_rate;
				}
			}
		}

		public static void SelectLNW()
		{
			int number = mutationSeed % globalWeightsCount;

			lastMutatedLayer = linksToWeights[number][0];
			lastMutatedNode = linksToWeights[number][1];
			lastMutatedWeight = linksToWeights[number][2];
		}

		public static void Neural_battle()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
				if (deltas == null)
					Born();

				Load();

				double record = GetStatistics();

				for (int n = 0; n < 30; n++)
				{
					var files = Directory.GetFiles(Disk.programFiles + "Trading\\NN");
					string recordsmen = File.ReadAllText(files[0]);

					CreateNewNN();
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

		public static double GetStatistics()
		{
			return 404;
		}

		public static void Mutate(int count, double mutagen)
		{
			for (int i = 0; i < count; i++)
				layers[lastMutatedLayer].nodes[lastMutatedNode].weights[lastMutatedWeight] += mutagen;
		}
	}
}

