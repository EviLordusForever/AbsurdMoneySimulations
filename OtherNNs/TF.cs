using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumSharp;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;
using Tensorflow;
using static AbsurdMoneySimulations.Logger;

namespace AbsurdMoneySimulations
{
    public static class TF
    {
        public static Model model;
        public static Tensorflow.NumPy.NDArray in_train, out_train, in_test, out_test;

        public static void LetsGo()
        {
            PrepareData();
            BuildModel();
            Train();
        }

        public static void PrepareData()
        {
            NNTester.InitForEvolution();

            Tensor t = new Tensor(Extensions.Convert2DArrayTo1D(NNTester.tests), new Tensorflow.Shape(2000, 300));
            in_train = new Tensorflow.NumPy.NDArray(t);
            Tensor t2 = new Tensor(NNTester.answers, shape: 1);
            out_train = new Tensorflow.NumPy.NDArray(t2);

            NNTester.InitForTesting();

            Tensor t3 = new Tensor(Extensions.Convert2DArrayTo1D(NNTester.tests), new Tensorflow.Shape(2000, 300));
            in_test = new Tensorflow.NumPy.NDArray(t3);
            Tensor t4 = new Tensor(NNTester.answers, shape: 1);
            out_test = new Tensorflow.NumPy.NDArray(t4);
            Log(t3.ToString());
        }

        public static void BuildModel()
        {
            Tensor inputs = keras.Input(shape: 784);

            LayersApi layers = new LayersApi();

            Tensors outputs = layers.Dense(64, activation: keras.activations.Relu).Apply(inputs);
            ////////////////////////

            outputs = layers.Dense(10).Apply(outputs);

            model = keras.Model(inputs, outputs, name: "mnist_model");
            model.summary();

            model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
                optimizer: keras.optimizers.Adam(),
                metrics: new[] { "accuracy" });
        }

        public static void Train()
        {
            model.fit(in_train, out_train, batch_size: 10, epochs: 2);
            model.evaluate(in_test, out_test);
        }
    }
}