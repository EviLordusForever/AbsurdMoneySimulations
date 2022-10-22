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

		public float _LEARNING_RATE;
		public float _MOMENTUM;

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

		public static NN CreateBasicNN()
		{
			NN nn = new NN();

			nn._layers = new List<Layer>();

			nn._horizon = 60;
			nn._inputWindow = 300;
			nn._weightsInitMin = -1f;
			nn._weightsInitMax = 1f;

			nn._gradientCutter = 10f;

			nn._biasInput = 0.01f;

			nn._LEARNING_RATE = 0.001f; //0.05f
			nn._MOMENTUM = 0f; //0.8f

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

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 8, 300, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers[0].FillWeightsRandomly();

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 8, 8, 0f, new SoftSign())); //40 x 15 = 600
			nn._layers[1].FillWeightsRandomly();

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 5, 8, 0, new SoftSign())); //40 x 15 = 600
			nn._layers[2].FillWeightsRandomly();

			nn._layers.Add(new LayerPerceptron(nn, nn._testerE._testsCount, 1, 5, 0f, new SoftSign())); //40 x 15 = 600
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
			jss.Converters.Add(new AbstractConverterOfActivationFunction());

			Log("Neural Network loaded from disk!");

			NN nn = JsonConvert.DeserializeObject<NN>(json, jss);
			nn._name = Text2.StringInsideLast(path, "\\", ".json");
			nn.Init();
			return nn;
		}

		public float Calculate(int test, float[] input, bool withDropout)
		{
			float[][] array = new float[1][];
			array[0] = input;

			_layers[0].Calculate(test, array, withDropout);

			for (int l = 1; l < _layers.Count; l++)
				_layers[l].Calculate(test, _layers[l - 1].GetValues(test), withDropout);

			return _layers[_layers.Count - 1].GetAnswer(test);
		}

		private void Init()
		{
			//Initialises values which are not saved to JSON
			InitLinksToMe();
			InitTesters();
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

				float vLossRecord = GetVLossRecord();
				Log("Validation loss record: " + vLossRecord);

				float vLoss = FindLoss(_testerV, false);
				Log("Current validation loss: " + vLoss);

				float tLoss = FindLoss(_testerE, false);
				Log("Current train loss: " + tLoss);

				float oldTLoss = tLoss;
				float oldVLoss = vLoss;

				for (int Generation = 1; ; Generation++)
				{
					Log($"G{Generation} b{Generation % 20}");

					_vanishedGradientsCount = 0;
					_cuttedGradientsCount = 0;

					FillBatch();
					UseMomentumForBPGradients(_testerE);
					FindBPGradients(_testerE);
					CorrectWeightsByBP(_testerE);

					oldVLoss = vLoss;
					vLoss = FindLoss(_testerV, false);

					Dropout();
					
					oldTLoss = tLoss;
					tLoss = FindLoss(_testerE, true);

					FindSpeed();
					FindAcceleration();

					LogAllInformation();
					SaveEvolveHistory();
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

				void FillBatch()
				{
					_testerE.FillBatchBy(200);
					//testerE.FillFullBatch();
					Log("Batch refilled");
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

				void FindSpeed()
				{
					old_v = v;
					v = tLoss - oldTLoss;
				}

				void FindAcceleration()
				{
					a = v - old_v;
				}

				void LogAllInformation()
				{
					Log($"validation loss: {string.Format("{0:F8}", vLoss)} (v {string.Format("{0:F8}", vLoss - oldVLoss)})");
					Log($"train loss: {string.Format("{0:F8}", tLoss)} (v {string.Format("{0:F8}", v)}) (a {string.Format("{0:F8}", a)}) (lmd {string.Format("{0:F7}", _LEARNING_RATE)})");
					Log($"vanished {_vanishedGradientsCount} cutted {_cuttedGradientsCount}");
				}

				void SaveEvolveHistory()
				{
					Disk2.WriteToProgramFiles("EvolveHistory", "csv", $"{tLoss}, {vLoss}\r\n", true);
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

		public void Dropout()
		{
			if (_layers.Count > 1)
			{
				for (int l = 1; l < _layers.Count - 1; l++)
					_layers[l].Dropout();

				Log("Dropped out!");
			}
		}

		private void UseMomentumForBPGradients(Tester tester)
		{
			for (int test = 0; test < tester._tests.Length; test++)
				for (int layer = _layers.Count - 2; layer >= 0; layer--)
					_layers[layer].UseMomentumForGradient(test);

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

		public float FindLoss(Tester tester, bool withDropout)
		{
			return FindLossSquared(tester, withDropout);
		}

		public float FindLossLinear(Tester tester, bool withDropout)
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
					float prediction = Calculate(test, tester._tests[test], withDropout);

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

		public float FindLossSquared(Tester tester, bool withDropout)
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
					float prediction = Calculate(test, tester._tests[test], withDropout);

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