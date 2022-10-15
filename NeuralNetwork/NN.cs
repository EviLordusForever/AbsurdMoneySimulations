using Newtonsoft.Json;
using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.Storage;


namespace AbsurdMoneySimulations
{
	public static class NN
	{
		public const int _horizon = 29;
		public const int _inputWindow = 300;
		public const float _weightsInitMin = -0.15f;
		public const float _weightsInitMax = 0.15f;
		public const int _jumpLimit = 9000;

		private const int _testsCount = 2000;
		private const int _batchSize = 1;

		public static int _randomMutatesCount = 2022;

		public static float _randomMutatesSharpness = 10;
		public static float _randomMutatesScaleV2 = 10;
		public static float _randomMutatesSmoothing = 0.03f;

		public static float _LYAMBDA = 0.02f; //0.05f
		public static float _INERTION = 0.8f; //0.8f

		public const float _cutter = 100f;

		public const float _biasInput = 0.1f;

		public static int _vanishedGradients;
		public static int _cuttedGradients;

		public static ActivationFunction _answersAF = new TanH();

		public static List<LayerAbstract> _layers;

		public static Tester _testerV;
		public static Tester _testerE;

		public static string _name;

		public static float[] _randomMutates;
		public static float _mutagen;
		public static int _mutationSeed;
		public static int _lastMutatedLayer;

		public static void Create()
		{
			_layers = new List<LayerAbstract>();

			_layers.Add(new LayerMegatron(_testerE._testsCount, 6, 136, 30, 2));   //136 x 30 x 10 = 
			_layers[0].FillWeightsRandomly();

			_layers.Add(new LayerCybertron(_testerE._testsCount, 6, 136, 10, 60)); //6 x 136 x 10 = 
			_layers[1].FillWeightsRandomly();

			_layers.Add(new LayerPerceptron(_testerE._testsCount, 5, 60)); //5 x 60 = 300
			_layers[2].FillWeightsRandomly();

			_layers.Add(new LayerPerceptron(_testerE._testsCount, 5, 5)); //5 x 5 =  25
			_layers[3].FillWeightsRandomly();

			_layers.Add(new LayerPerceptron(_testerE._testsCount, 1, 5)); //5 x 1 = 5
			_layers[4].FillWeightsRandomly();

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
			var files = Directory.GetFiles(Disk._programFiles + "\\NN");

			JsonSerializerSettings jss = new JsonSerializerSettings();
			jss.Formatting = Formatting.Indented;

			File.WriteAllText(files[0], JsonConvert.SerializeObject(_layers, jss));
			Log("Neural Network saved!");
		}

		public static void Load()
		{
			var files = Directory.GetFiles(Disk._programFiles + "\\NN");
			_name = TextMethods.StringInsideLast(files[0], "\\", ".json");
			string json = File.ReadAllText(files[0]);

			var jss = new JsonSerializerSettings();
			jss.Converters.Add(new LayerAbstractConverter());

			_layers = JsonConvert.DeserializeObject<List<LayerAbstract>>(json, jss);

			Log("Neural Network loaded from disk!");
		}

		public static float Calculate(int test, float[] input)
		{
			float[][] array = new float[1][];
			array[0] = input;

			_layers[0].Calculate(test, array);

			for (int l = 1; l < _layers.Count; l++)
				_layers[l].Calculate(test, _layers[l - 1].GetValues(test));

			return _layers[_layers.Count - 1].GetAnswer(test);
		}

		public static float Recalculate(int test, float[] input)
		{
			LayerRecalculateStatus lrs = LayerRecalculateStatus.OneWeightChanged;

			if (_lastMutatedLayer > 0)
				for (int layer = _lastMutatedLayer; layer < _layers.Count; layer++)
					lrs = _layers[layer].Recalculate(test, _layers[layer - 1].GetValues(test), lrs);
			else
			{
				float[][] array = new float[1][];
				array[0] = input;

				_layers[0].Calculate(test, array);

				for (int layer = 1; layer < _layers.Count; layer++)
					lrs = _layers[layer].Recalculate(test, _layers[layer - 1].GetValues(test), lrs);
			}

			return _layers[_layers.Count - 1].GetAnswer(test);
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
			_testerV = new Tester(_testsCount, _batchSize, "Grafic//ForValidation", "VALIDATION");
			_testerE = new Tester(_testsCount, _batchSize, "Grafic//ForEvolution", "EVOLUTION");
		}

		public static void InitActivationFunctions()
		{
			for (int l = 0; l < _layers.Count - 1; l++)
				_layers[l]._af = new TanH();
			_layers[_layers.Count - 1]._af = new TanH();
		}

		public static void InitValues()
		{
			for (int l = 0; l < _layers.Count; l++)
				_layers[l].InitValues(_testerE._testsCount);

			Log("NN values were initialized");
		}

		public static void FillRandomMutations()
		{
			_randomMutates = new float[_randomMutatesCount];

			for (int m = 0; m < _randomMutatesCount; m++)
				_randomMutates[m] = Extensions.NormalDistribution(_randomMutatesScaleV2, _randomMutatesSharpness, _randomMutatesSmoothing);

			Log("Random mutations are filled.");
		}

		public static void EvolveByRandomMutations()
		{
			short previous = 0;
			string history = "";
			float er = 0;
			float record = FindErrorRateSquared(_testerE);
			Log("Received current er_fb: " + record);

			for (int Generation = 0; ; Generation++)
			{
				Log("G" + Generation);

				SelectLayerForMutation();
				Mutate();

				er = RefindErrorRateSquared(_testerE);

				if (er < record)
				{
					Log("er_nfb: " + er.ToString());
					Log($" ▲ Good mutation. (mutagen {_mutagen})");
					record = er;
				}
				else if (er == record)
				{
					Log($" - Neutral mutation. Leave it. ({_mutagen})");
				}
				else
				{
					Log($" ▽ Bad mutation. Go back. ({_mutagen})");
					Demutate();
					er = FindErrorRateSquared(_testerE);
				}

				history += record + "\r\n";

				if (Generation % 100 == 99)
				{
					Save();
					Disk.WriteToProgramFiles("EvolveHistory", "csv", history, true);
					history = "";

					Log("(!) er_nfb: " + er);
					er = FindErrorRateSquared(_testerE);
					Log("(!) er_fb: " + er);

					string validation = Stat.CalculateStatistics(_testerV);
					Disk.WriteToProgramFiles("Stat", "csv", Stat.StatToCsv("Validation") + "\n", true);
					string evolition = Stat.CalculateStatistics(_testerE);
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

				float ert = FindErrorRateSquared(_testerV);
				Log("Current ert: " + ert);
				float er = FindErrorRateSquared(_testerE);
				Log("Current er: " + er);
				float old_er = er;
				float old_ert = ert;
				float ert_record = ert;

				for (int Generation = 0; ; Generation++)
				{
					Log($"G{Generation} b{Generation % 30}");

					if (Generation % 30 == 0)
					{
						_testerE.FillBatchBy(500);
						Log("Batch refilled");
					}

					_vanishedGradients = 0;
					_cuttedGradients = 0;
					for (int b = 0; b < _testerE._batchesCount; b++)
					{
						_testerE.FillBatch();
						UseInertionForBPGradients(_testerE);
						FindBPGradients(_testerE);
						CorrectWeightsByBP(_testerE);
					}

					old_ert = ert;
					ert = FindErrorRateSquared(_testerV);
					Log($"ert: {string.Format("{0:F8}", ert)} (v {string.Format("{0:F8}", ert - old_ert)})");
					old_er = er;
					er = FindErrorRateSquared(_testerE);

					old_v = v;
					v = er - old_er;
					a = v - old_v;

					Log($"er: {string.Format("{0:F8}", er)} (v {string.Format("{0:F8}", v)}) (a {string.Format("{0:F8}", a)}) (lmd {string.Format("{0:F7}", _LYAMBDA)})");
					Log($"vanished {_vanishedGradients} cutted {_cuttedGradients}");
					Disk.WriteToProgramFiles("EvolveHistory", "csv", $"{er}, {ert}\r\n", true);

					Save();
					EarlyStopping();

					if (Generation % 20 == 19)
					{
						string validation = Stat.CalculateStatistics(_testerV);
						Disk.WriteToProgramFiles("Stat", "csv", Stat.StatToCsv("Validation") + "\n", true);
						string evolition = Stat.CalculateStatistics(_testerE);
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
						Disk.ClearDirectory($"{Disk._programFiles}\\NN\\EarlyStopping");
						File.Copy($"{Disk._programFiles}\\NN\\{_name}.json", $"{Disk._programFiles}\\NN\\EarlyStopping\\{_name} ({ert}).json");
						Log("NN copied for early stopping.");
					}
				}
			}
		}

		private static void UseInertionForBPGradients(Tester tester)
		{
			if (_INERTION != 1f)
			{
				for (int test = 0; test < tester._tests.Length; test++)
					for (int layer = _layers.Count - 2; layer >= 0; layer--)
						_layers[layer].UseInertionForGradient(test);

				Log("Inertion for gradients used!");
			}
		}

		private static void FindBPGradients(Tester tester)
		{
			for (int test = 0; test < tester._tests.Length; test++)
				if (tester._batch[test] == 1)
				{
					_layers[_layers.Count - 1].FindBPGradient(test, tester._answers[test]);
					for (int layer = _layers.Count - 2; layer >= 0; layer--)
						_layers[layer].FindBPGradient(test, _layers[layer + 1].AllBPGradients(test), _layers[layer + 1].AllWeights);
				}

			SaveLastLayerWeights(); //
			Log("Gradients are found!");

			void SaveLastLayerWeights()
			{
				string str = "";

				for (int w = 0; w < (_layers[_layers.Count - 1] as LayerPerceptron)._nodes[0]._weights.Length; w++)
					str += (_layers[_layers.Count - 1] as LayerPerceptron)._nodes[0]._weights[w] + ",";

				Disk.WriteToProgramFiles("weights", "csv", str + "\n", true);
			}
		}

		private static void CorrectWeightsByBP(Tester tester)
		{
			for (int test = 0; test < tester._testsCount; test++)
			{
				if (tester._batch[test] == 1)
				{
					float[][] array = new float[1][];
					array[0] = tester._tests[test];

					_layers[0].CorrectWeightsByBP(test, array);

					for (int l = 1; l < _layers.Count; l++)
						_layers[l].CorrectWeightsByBP(test, _layers[l - 1].GetValues(test));
				}
			}
			Log("Weights are corrected!");
		}

		public static float FindErrorRateLinear(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester._testsCount / _coresCount;

			float er = 0;
			float[] suber = new float[_coresCount];

			int alive = _coresCount;

			Thread[] subThreads = new Thread[_coresCount];

			for (int core = 0; core < _coresCount; core++)
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
					float prediction = Calculate(test, tester._tests[test]);

					float reality = tester._answers[test];

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
					for (int core = 0; core < _coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < _coresCount; core++)
				er += suber[core];

			er /= tester._testsCount;

			return er;
		}

		public static float FindErrorRateSquared(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester._testsCount / _coresCount;

			float er = 0;
			float[] suber = new float[_coresCount];

			int alive = _coresCount;

			Thread[] subThreads = new Thread[_coresCount];

			for (int core = 0; core < _coresCount; core++)
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
					float prediction = Calculate(test, tester._tests[test]);

					float reality = tester._answers[test];

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
					for (int core = 0; core < _coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}


			for (int core = 0; core < _coresCount; core++)
				er += suber[core];

			er /= tester._testsCount;

			return er;
		}

		public static float RefindErrorRateSquared(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester._testsCount / _coresCount;

			float er = 0;
			float[] suber = new float[_coresCount];

			int alive = _coresCount;

			Thread[] subThreads = new Thread[_coresCount];

			for (int core = 0; core < _coresCount; core++)
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
					float prediction = Recalculate(test, tester._tests[test]);

					float reality = tester._answers[test];

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
					for (int core = 0; core < _coresCount; core++)
						Log($"Thread / core {core}: {subThreads[core].ThreadState}");
					Log("AGAIN");

					goto restart;
				}
			}

			for (int core = 0; core < _coresCount; core++)
				er += suber[core];

			er /= tester._testsCount;

			return er;
		}

		private static void SelectLayerForMutation()
		{
			//int number = linksToLayersToMutate[Storage.rnd.Next(linksToLayersToMutate.Count)];
			int number = Storage.rnd.Next(_layers.Count - 0) + 0;

			_lastMutatedLayer = number;
		}

		public static void NeuralBattle()
		{
			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
				Init();

				float record = FindErrorRateSquared(_testerE);
				Log($"record {record}");
				var files = Directory.GetFiles(Disk._programFiles + "NN");

				for (int n = 0; ; n++)
				{
					Create();
					Init();

					float er = FindErrorRateSquared(_testerE);
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
			_mutagen = _randomMutates[Storage.rnd.Next(_randomMutates.Length)];
			_layers[_lastMutatedLayer].Mutate(_mutagen);
			Log($"Layer {_lastMutatedLayer} is mutated.");
		}

		public static void Demutate()
		{
			_layers[_lastMutatedLayer].Demutate(_mutagen);
		}

		public static float CutGradient(float g)
		{
			if (MathF.Abs(g) > _cutter)
			{
				_cuttedGradients++;
				return _cutter * (g / g);
			}
			else
				return g;
		}
	}
}

