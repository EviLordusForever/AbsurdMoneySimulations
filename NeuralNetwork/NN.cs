using Newtonsoft.Json;
using static AbsurdMoneySimulations.Logger;
using static AbsurdMoneySimulations.Storage;
using Library;

namespace AbsurdMoneySimulations
{
	public class NN
	{
		public int _horizon;
		public int _inputWindow;
		public float _weightsInitMin;
		public float _weightsInitMax;

		public int _randomMutatesCount;

		public float _randomMutatesSharpness;
		public float _randomMutatesScaleV2;
		public float _randomMutatesSmoothing;

		public float _LEARNING_RATE;
		public float _INERTION;

		public float _gradientCutter;

		public float _biasInput;

		public ActivationFunction _inputAF;
		public ActivationFunction _answersAF;

		public List<Layer> _layers;

		public Tester _testerV;
		public Tester _testerE;

		public string _name;

		[JsonIgnore] public int _vanishedGradientsCount;
		[JsonIgnore] public int _cuttedGradientsCount;
		[JsonIgnore] public float[] _randomMutations;
		[JsonIgnore] public float _mutagen;
		[JsonIgnore] public int _lastMutatedLayer;

		public static NN CreateBasicNN()
		{
			NN nn = new NN();

			nn._layers = new List<Layer>();

			nn._horizon = 60; //
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._randomMutatesCount = 2022;

			nn._randomMutatesSharpness = 10;
			nn._randomMutatesScaleV2 = 10;
			nn._randomMutatesSmoothing = 0.03f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._INERTION = 0f; //0.8f

			nn._inputAF = new SoftSign();
			nn._answersAF = new SoftSign();

			nn._testerE = new Tester(nn, 4000, 1, "Grafic//ForEvolution", "EVOLUTION", 2, 0, 0);
			nn._testerV = new Tester(nn, 2000, 1, "Grafic//ForValidation", "VALIDATION", 2, 0, 0);


			/*			nn._layers.Add(new LayerMegatron(nn, nn._testerE._testsCount, 3, 271, 30, 1, new SoftSign()));   //136 x 30 x 10 = 
						nn._layers[0].FillWeightsRandomly();

						nn._layers.Add(new LayerCybertron(nn, nn._testerE._testsCount, 3, 271, 5, 15, new SoftSign())); //6 x 136 x 10 = 
						nn._layers[1].FillWeightsRandomly();

						nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 10, 15, new SoftSign())); //5 x 60 = 300
						nn._layers[2].FillWeightsRandomly();

						nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 5, 10, new SoftSign())); //5 x 5 =  25
						nn._layers[3].FillWeightsRandomly();

						nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 1, 5, new SoftSign())); //5 x 5 =  25
						nn._layers[4].FillWeightsRandomly();*/


			/*layers.Add(new LayerMegatron(testerE.testsCount, 2, 271, 30, 1));   //271 x 30 x 2 = 
			layers[0].FillWeightsRandomly();

			layers.Add(new LayerCybertron(testerE.testsCount, 2, 271, 5, 10)); //2 x 271 x 5 =
			layers[1].FillWeightsRandomly();

			layers.Add(new LayerPerceptron(testerE.testsCount, 1, 10)); //10 x 1 = 10
			layers[2].FillWeightsRandomly();*/

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 10, 300, new SoftSign())); //40 x 15 = 600
			nn._layers[0].FillWeightsRandomly();

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 10, 10, new SoftSign())); //40 x 15 = 600
			nn._layers[1].FillWeightsRandomly();

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 5, 10, new SoftSign())); //40 x 15 = 600
			nn._layers[2].FillWeightsRandomly();

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 1, 5, new SoftSign())); //40 x 15 = 600
			nn._layers[3].FillWeightsRandomly();

			nn.Init();

			Log("New Neural Network created!");
			return nn;
		}

		public static void Save(NN nn)
		{
			var files = Directory.GetFiles(Disk2._programFiles + "\\NN");

			JsonSerializerSettings jss = new JsonSerializerSettings();
			jss.Formatting = Formatting.Indented;

			File.WriteAllText(files[0], JsonConvert.SerializeObject(nn, jss));
			Log("Neural Network saved!");
		}

		public static NN Load()
		{
			var files = Directory.GetFiles(Disk2._programFiles + "\\NN");

			string json = File.ReadAllText(files[0]);

			var jss = new JsonSerializerSettings();
			jss.Converters.Add(new AbstractConverterOfLayer());
			jss.Converters.Add(new AbstractConverterOfActivationFunction());

			Log("Neural Network loaded from disk!");

			NN nn = JsonConvert.DeserializeObject<NN>(json, jss);
			nn._name = Text2.StringInsideLast(files[0], "\\", ".json");
			nn.Init();
			return nn;
		}

		public static NN Load(string path)
		{
			string json = File.ReadAllText(path);

			var jss = new JsonSerializerSettings();
			jss.Converters.Add(new AbstractConverterOfLayer());

			Log("Neural Network loaded from disk!");

			NN nn = JsonConvert.DeserializeObject<NN>(json, jss);

			nn._name = Text2.StringInsideLast(path, "\\", ".json");
			return nn;
		}

		public float Calculate(int test, float[] input)
		{
			float[][] array = new float[1][];
			array[0] = input;

			_layers[0].Calculate(test, array);

			for (int l = 1; l < _layers.Count; l++)
				_layers[l].Calculate(test, _layers[l - 1].GetValues(test));

			return _layers[_layers.Count - 1].GetAnswer(test);
		}

		public float Recalculate(int test, float[] input)
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

		private void Init()
		{
			//Initialises values which are not saved to JSON
			InitLinksToMe();
			InitTesters();
			FillRandomMutations();
			InitValues();
		}

		private void InitTesters()
		{
			_testerV.Init(this, "Grafic//ForValidation", "VALIDATION");
			_testerE.Init(this, "Grafic//ForEvolution", "EVOLUTION");
			Log("Testers were initialized");
		}

		private void InitValues()
		{
			for (int l = 0; l < _layers.Count; l++)
				_layers[l].InitValues(_testerE._testsCount);

			Log("NN values were initialized");
		}

		private void InitLinksToMe()
		{
			for (int l = 0; l < _layers.Count; l++)
				_layers[l].InitLinksToOwnerNN(this);

			Log("Links to this NN were initialized");
		}

		private void FillRandomMutations()
		{
			_randomMutations = new float[_randomMutatesCount];

			for (int m = 0; m < _randomMutatesCount; m++)
				_randomMutations[m] = Math2.LikeNormalDistribution(_randomMutatesScaleV2, _randomMutatesSharpness, _randomMutatesSmoothing, rnd);

			Log("Random mutations are filled.");
		}

		public void EvolveByRandomMutations()
		{
			short previous = 0;
			string history = "";
			float er = 0;
			float record = FindLoss(_testerE);
			Log("Current train loss: " + record);

			for (int Generation = 0; ; Generation++)
			{
				Log("G" + Generation);

				SelectLayerForMutation();
				Mutate();

				er = RefindLossSquared(_testerE);

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
					er = FindLoss(_testerE);
				}

				history += record + "\r\n";

				if (Generation % 100 == 99)
				{
					Save(this);
					Disk2.WriteToProgramFiles("EvolveHistory", "csv", history, true);
					history = "";

					Log("(!) er_nfb: " + er);
					er = FindLoss(_testerE);
					Log("(!) er_fb: " + er);

					string validation = Statistics.CalculateStatistics(this, _testerV);
					Disk2.WriteToProgramFiles("Stat", "csv", Statistics.StatToCsv("Validation") + "\n", true);
					string evolition = Statistics.CalculateStatistics(this, _testerE);
					Disk2.WriteToProgramFiles("Stat", "csv", Statistics.StatToCsv("Evolution"), true);

					Log("Evolution dataset:\n" + evolition);
					Log("Validation dataset:\n" + validation);
				}
			}
		}

		public void EvolveByBackPropagtion()
		{
			//Just very very important function

			Thread myThread = new Thread(SoThread);
			myThread.Start();

			void SoThread()
			{
				float v = 0;
				float old_v = 0;
				float a = 0;

				float vLoss = FindLoss(_testerV);
				Log("Current validation loss: " + vLoss);
				float vLossRecord = GetVLossRecord();
				Log("Validation loss record: " + vLossRecord);
				float tLoss = FindLoss(_testerE);
				Log("Current train loss: " + tLoss);
				float oldTLoss = tLoss;
				float oldVLoss = vLoss;

				for (int Generation = 1; ; Generation++)
				{
					Log($"G{Generation} b{Generation % 20}");

					if (Generation % 1 == 0)
					{
						//_testerE.FillBatchBy(1000);
						_testerE.FillFullBatch();
						Log("Batch refilled");
					}

					_vanishedGradientsCount = 0;
					_cuttedGradientsCount = 0;

					UseInertionForBPGradients(_testerE);
					FindBPGradients(_testerE);
					CorrectWeightsByBP(_testerE);

					oldVLoss = vLoss;
					vLoss = FindLoss(_testerV);
					Log($"validation loss: {string.Format("{0:F8}", vLoss)} (v {string.Format("{0:F8}", vLoss - oldVLoss)})");
					oldTLoss = tLoss;
					tLoss = FindLoss(_testerE);

					old_v = v;
					v = tLoss - oldTLoss;
					a = v - old_v;

					Log($"train loss: {string.Format("{0:F8}", tLoss)} (v {string.Format("{0:F8}", v)}) (a {string.Format("{0:F8}", a)}) (lmd {string.Format("{0:F7}", _LEARNING_RATE)})");
					Log($"vanished {_vanishedGradientsCount} cutted {_cuttedGradientsCount}");
					Disk2.WriteToProgramFiles("EvolveHistory", "csv", $"{tLoss}, {vLoss}\r\n", true);

					Save(this);
					EarlyStopping();

					if (Generation % 20 == 19)
					{
						string validation = Statistics.CalculateStatistics(this, _testerV);
						Disk2.WriteToProgramFiles("Stat", "csv", Statistics.StatToCsv("Validation") + "\n", true);
						string evolition = Statistics.CalculateStatistics(this, _testerE);
						Disk2.WriteToProgramFiles("Stat", "csv", Statistics.StatToCsv("Evolution"), true);

						Log("Evolution dataset:\n" + evolition);
						Log("Validation dataset:\n" + validation);
					}
				}

				void EarlyStopping()
				{
					if (vLoss <= vLossRecord)
					{
						vLossRecord = vLoss;
						Disk2.ClearDirectory($"{Disk2._programFiles}\\NN\\EarlyStopping");
						File.Copy($"{Disk2._programFiles}\\NN\\{_name}.json", $"{Disk2._programFiles}\\NN\\EarlyStopping\\{_name} ({vLoss}).json");
						Log(" ▲ NN copied for early stopping.");
					}
				}

				float GetVLossRecord()
				{
					string[] files = Directory.GetFiles(Disk2._programFiles + "NN\\EarlyStopping");
					if (files.Length > 0)
					{
						string record = Text2.StringInsideLast(files[0], " (", ").json");
						return Convert.ToSingle(record);
					}
					else
						return 1f;
				}
			}
		}

		private void UseInertionForBPGradients(Tester tester)
		{
			for (int test = 0; test < tester._tests.Length; test++)
				for (int layer = _layers.Count - 2; layer >= 0; layer--)
					_layers[layer].UseInertionForGradient(test);

			Log("Inertion for gradients used!");
		}

		private void FindBPGradients(Tester tester)
		{
			restart:

			int testsPerCoreCount = tester._testsCount / _coresCount;

			int alive = _coresCount;

			Thread[] subThreads = new Thread[_coresCount];

			for (int core = 0; core < _coresCount; core++)
			{
				subThreads[core] = new Thread(new ParameterizedThreadStart(SubThread));
				subThreads[core].Name = "Core " + core;
				subThreads[core].Priority = ThreadPriority.Highest;
				subThreads[core].Start(core);
			}

			void SubThread(object obj)
			{
				int core = (int)obj;

				for (int test = core * testsPerCoreCount; test < core * testsPerCoreCount + testsPerCoreCount; test++)
					if (tester._batch[test] == 1)
					{
						_layers[_layers.Count - 1].FindBPGradient(test, tester._answers[test]);
						for (int layer = _layers.Count - 2; layer >= 0; layer--)
							_layers[layer].FindBPGradient(test, _layers[layer + 1].AllBPGradients(test), _layers[layer + 1].AllWeights);
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

			SaveLastLayerWeights();
			Log("Gradients are found!");

			void SaveLastLayerWeights()
			{
				string str = "";

				for (int w = 0; w < (_layers[_layers.Count - 1] as LayerPerceptron)._nodes[0]._weights.Length; w++)
					str += (_layers[_layers.Count - 1] as LayerPerceptron)._nodes[0]._weights[w] + ",";

				Disk2.WriteToProgramFiles("weights", "csv", str + "\n", true);
			}
		}

		private void CorrectWeightsByBP(Tester tester)
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

		public float FindLoss(Tester tester)
		{
			return FindLossSquared(tester);
		}

		public float FindLossLinear(Tester tester)
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
				subThreads[core].Name = "Core " + core;
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

		public float FindLossSquared(Tester tester)
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
				subThreads[core].Name = "Core " + core;
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

		public float RefindLossSquared(Tester tester)
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
				subThreads[core].Name = "Core " + core;
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

		private void SelectLayerForMutation()
		{
			//int number = linksToLayersToMutate[Storage.rnd.Next(linksToLayersToMutate.Count)];
			int number = Storage.rnd.Next(_layers.Count - 0) + 0;

			_lastMutatedLayer = number;
		}

		public void Mutate()
		{
			SelectLayerForMutation();
			_mutagen = _randomMutations[Storage.rnd.Next(_randomMutations.Length)];
			_layers[_lastMutatedLayer].Mutate(_mutagen);
			Log($"Layer {_lastMutatedLayer} is mutated.");
		}

		public void Demutate()
		{
			_layers[_lastMutatedLayer].Demutate(_mutagen);
		}

		public float CutGradient(float g)
		{
			if (MathF.Abs(g) > _gradientCutter)
			{
				_cuttedGradientsCount++;
				return _gradientCutter * (g / g);
			}
			else
				return g;
		}
	}
}