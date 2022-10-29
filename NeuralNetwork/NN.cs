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
		public Tester _testerT;

		public string _name;

		public int _generation;

		[JsonIgnore] public int _vanishedGradientsCount;
		[JsonIgnore] public int _cuttedGradientsCount;

		public static void Save(NN nn)
		{
			string path = Directory.GetFiles(Disk2._programFiles + "\\NN")[0];
			Save(nn, path);
		}

		public static void Save(NN nn, string path)
		{
			JsonSerializerSettings jss = new JsonSerializerSettings();
			jss.Formatting = Formatting.Indented;

			File.WriteAllText(path, JsonConvert.SerializeObject(nn, jss));
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
			_testerV.Init(this, "Graph//ForValidation", "VALIDATION");
			_testerT.Init(this, "Graph//ForTraining", "TRAINING");
			Log("Testers were initialized");
		}

		public void InitValues()
		{
			for (int l = 0; l < _layers.Count; l++)
				_layers[l].InitValues(_testerT._testsCount);

			Log("NN values were initialized");
		}

		private void InitLinksToMe()
		{
			for (int l = 0; l < _layers.Count; l++)
				_layers[l].InitLinksToOwnerNN(this);

			Log("Links to this NN were initialized");
		}

		public void FillWeightsRandomly()
		{
			for (int l = 0; l < _layers.Count; l++)
				_layers[l].FillWeightsRandomly();
		}

		public void FitByBackPropagtion(bool useBatchForTLoss, bool needValidationLoss)
		{
			FitByBackPropagtion(1000000000, useBatchForTLoss, needValidationLoss);
		}

		public void FitByBackPropagtion(int count, bool useBatchForTLoss, bool needValidationLoss)
		{
			//Just very very important function

			bool needDropout = DefineIfNeedDropout();

			float v = 0;
			float old_v = 0;
			float a = 0;

			float vLossRecord = GetVLossRecord();
			Log("Validation loss record: " + vLossRecord);

			float vLoss = FindLoss(_testerV, false, false);
			Log("Current validation loss: " + vLoss);

			float tLoss = FindLoss(_testerT, false, useBatchForTLoss);
			Log("Current train loss: " + tLoss);

			float oldTLoss = tLoss;
			float oldVLoss = vLoss;

			for (int localGeneration = 0; localGeneration < count; localGeneration++)
			{
				_generation++;

				Log($"G{_generation} ({localGeneration + 1}) ({localGeneration % 20})");

				_vanishedGradientsCount = 0;
				_cuttedGradientsCount = 0;

				FillBatch();
				UseMomentumForBPGradients(_testerT);
				FindBPGradients(_testerT);
				CorrectWeightsByBP(_testerT);

				if (needValidationLoss)
				{
					oldVLoss = vLoss;
					vLoss = FindLoss(_testerV, false, false);
				}

				if (needDropout)
					Dropout();

				oldTLoss = tLoss;
				tLoss = FindLoss(_testerT, true, useBatchForTLoss);

				FindSpeed();
				FindAcceleration();

				LogAllInformation();
				SaveFittingHistory();
				Save(this);
				EarlyStopping();

				if (localGeneration % 50 == 49)
				{
					if (!needValidationLoss)
					{
						oldVLoss = vLoss;
						vLoss = FindLoss(_testerV, false, false);
					}

					string validation = Statistics.CalculateStatistics(this, _testerV);
					Disk2.WriteToProgramFiles("Stat", "csv", Statistics.StatToCsv("Validation") + "\n", true);
					string training = Statistics.CalculateStatistics(this, _testerT);
					Disk2.WriteToProgramFiles("Stat", "csv", Statistics.StatToCsv("Training"), true);

					Log("Training dataset:\n" + training);
					Log("Validation dataset:\n" + validation);
				}
			}

			Log($"SUCCESSFULLY FITTED {count} GENERATIONS");

			void FillBatch()
			{
				int count = 700;
				_testerT.FillBatchBy(count);
				//testerE.FillFullBatch();
				Log($"Batch refilled ({count})");
			}

			void EarlyStopping()
			{
				if (vLoss <= vLossRecord)
				{
					vLossRecord = vLoss;
					var path = Directory.GetFiles(Disk2._programFiles + "\\NN")[0];
					Disk2.ClearDirectory($"{Disk2._programFiles}\\NN\\EarlyStopping");
					File.Copy(path, $"{Disk2._programFiles}\\NN\\EarlyStopping\\{_name} ({vLoss}).json");
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

			void SaveFittingHistory()
			{
				Disk2.WriteToProgramFiles("FittingHistory", "csv", $"{tLoss}, {vLoss}\r\n", true);
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

			bool DefineIfNeedDropout()
			{
				if (_layers.Count > 1)
				{
					float dropoutRange = 0;
					for (int l = 1; l < _layers.Count - 1; l++)
						dropoutRange += _layers[l]._dropoutProbability;

					return dropoutRange > 0;
				}
				else
					return false;
			}
		}

		public void Dropout()
		{
			for (int l = 1; l < _layers.Count - 1; l++)
				_layers[l].Dropout();

			Log("Dropped out!");
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

		public float FindLoss(Tester tester, bool withDropout, bool useBatch)
		{
			if (useBatch)
				return FindLossSquaredForBatch(tester, withDropout);
			else
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

		public float FindLossSquaredForBatch(Tester tester, bool withDropout)
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
					if (tester._batch[test] == 1)
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

			er /= tester._batchSize;

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

		public NN()
		{
			_layers = new List<Layer>();
			_generation = 1;
		}
	}
}