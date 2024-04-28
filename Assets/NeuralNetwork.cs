using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    public class NeuralNetwork
    {
        int MaxInputs;

        public Layer[] layers;

        public NeuralNetwork(int[] n_networkShape)
        {
            layers = new Layer[n_networkShape.Length - 1];

            MaxInputs = n_networkShape[0];

            int inputs = n_networkShape[0];

            for(int i = 0; i < layers.Length; i++)
            {
                layers[i] = new Layer(inputs, n_networkShape[i + 1]);
                inputs = n_networkShape[i];
            }
        }

        public void MutateNetwork(float mutationChance, float mutationAmount)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].MutateLayer(mutationChance, mutationAmount);
            }
        }

        public float[] Think(float[] inputs)
        {
            List<float> inputsTrimmed = new List<float>();

            if (inputs.Length < MaxInputs) { Debug.LogWarning("Passed inputs are smaller than the max allowed inputs in Netowrk. Reduce the first layer node count for better results. Max inputs = " + MaxInputs + " Passed inputs = " + inputs.Length); }

            if (inputs.Length > MaxInputs) { Debug.LogWarning("Passed inputs are bigger than the max allowed inputs in Netowrk. Some inputs will be ignored. Max inputs = " + MaxInputs + " Passed inputs = " + inputs.Length); }

            for (int i = 0; i < MaxInputs; i++)
            {
                if (inputs.Length <= i) 
                { inputsTrimmed.Add(0); }
                else 
                { inputsTrimmed.Add(inputs[i]); }
            }

            float[] fowarding = inputsTrimmed.ToArray();

            for(int i = 0; i < layers.Length; i++)
            {
                layers[i].ForwardPass(fowarding);
                if (i != layers.Length - 1) { layers[i].Activation(); }
                fowarding = layers[i].nodes;
            }

            return layers[layers.Length - 1].nodes;
        }

        public Layer[] CopyLayers()
        {
            Layer[] newLayers = new Layer[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                newLayers[i] = new Layer(layers[i].weights.GetLength(1), layers[i].nodes.Length);
                System.Array.Copy(layers[i].weights, newLayers[i].weights, layers[i].weights.GetLength(0) * layers[i].weights.GetLength(1));
                System.Array.Copy(layers[i].bias, newLayers[i].bias, layers[i].bias.GetLength(0));
            }

            return newLayers;
        }
    }

    public class Layer
    {
        public float[,] weights;
        public float[] bias;
        public float[] nodes;

        int n_nodes;
        int n_inputs;

        public Layer(int n_inputs, int n_nodes)
        {
            this.n_nodes = n_nodes;
            this.n_inputs = n_inputs;

            weights = new float[n_nodes, n_inputs];
            bias = new float[n_nodes];
            nodes = new float[n_nodes];
        }

        public void MutateLayer(float mutationChance, float mutationAmount)
        {
            for (int i = 0; i < n_nodes; i++)
            {
                for (int j = 0; j < n_inputs; j++)
                {
                    if (Random.value < mutationChance)
                    {
                        weights[i, j] += Random.Range(-1f, 1f) * mutationAmount;
                    }
                }

                if(Random.value < mutationChance)
                {
                    bias[i] += Random.Range(-1f, 1f) * mutationAmount;
                }
            }
        }

        public void ForwardPass(float[] inputs)
        {
            nodes = new float[n_nodes];

            for(int i = 0; i < n_nodes; i++)
            {
                for(int j = 0; j < n_inputs; j++)
                {
                    nodes[i] += weights[i, j] * inputs[j];
                }

                nodes[i] += bias[i];
            }
        }

        public void Activation()
        {
            for (int i = 0; i < n_nodes; i++)
            {
                if (nodes[i] < 0)
                {
                    nodes[i] = 0;
                }
            }
        }
    }
}

