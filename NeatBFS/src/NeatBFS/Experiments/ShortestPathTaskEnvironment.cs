﻿using System;
using System.Linq;
using ENTM.Base;
using ENTM.Replay;
using NeatBFS.Graph;

namespace NeatBFS.Experiments
{
    public class ShortestPathTaskEnvironment : BaseEnvironment
    {
        #region environment
        public override IController Controller { get; set; }
        public override int InputCount { get; }
        public override int OutputCount { get; }
        public override double[] InitialObservation { get; }
        private double _currentScore;
        public override double CurrentScore => _currentScore;

        public override double MaxScore { get; }

        public override double NormalizedScore => CurrentScore / MaxScore;
        public override bool IsTerminated => DistanceToArray[CurrentVertex] == 0 || _step >= TotalTimeSteps;
        public override int TotalTimeSteps { get; } = 1;
        public override int MaxTimeSteps { get; } = 1;

        public override int NoveltyVectorLength
        {
            get
            {
                throw new Exception("novelty not implemented");
            }
        }

        public override int NoveltyVectorDimensions
        {
            get
            {
                throw new Exception("novelty not implemented");
            }
        }

        public override int MinimumCriteriaLength { get; } = 0;
        #endregion environment

        #region graph

        public IGraph Graph { get; set; }
        public double[] EncodedGraph { get; set; }
        public int[] DistanceToArray { get; set; }
        public int Goal { get; set; }
        public int CurrentVertex { get; set; }
        private readonly int _startVertex;
        #endregion graph

        public ShortestPathTaskEnvironment(IGraph graph, int goal, int startVertex)
        {
            Graph = graph;
            EncodedGraph = graph.EncodedGraph;
            DistanceToArray = graph.DistanceToArray(goal);
            Goal = goal;
            _startVertex = startVertex;
            CurrentVertex = startVertex;

            OutputCount = graph.NumberOfVertices * graph.NumberOfVertices + 2 * graph.NumberOfVertices;
            InputCount = graph.NumberOfVertices;
            InitialObservation = GetOutput(startVertex);
            TotalTimeSteps = DistanceToArray[startVertex];
            MaxTimeSteps = DistanceToArray[startVertex];
            MaxScore = DistanceToArray[startVertex] * 3;
        }

        public override double[] PerformAction(double[] action)
        {
            var next = GetMaxIndex(action);
            var observation = GetOutput(next);
            var thisScore = Evaluate(CurrentVertex, next);

            _currentScore += thisScore;

            if (RecordTimeSteps)
            {
                PrevTimeStep = new EnvironmentTimeStep(action, observation, thisScore);
            }
            CurrentVertex = next;
            _step++;
            return GetOutput(next);
        }

        protected double Evaluate(int current, int next)
        {
            var score = 0;
            if (!Graph.HasEdge(current, next)) // took non edge
            {
                score += 0;
            }
            else if (DistanceToArray[next] > DistanceToArray[current]) // took an edge away from goal
            {
                score += 1;
            }
            else if (DistanceToArray[next] == DistanceToArray[current]) // took an edge on the current fridge
            {
                score += 2;
            }
            else if (DistanceToArray[next] < DistanceToArray[current]) // took an edge towards the goal.
            {
                score += 3;
            }
            return score;
        }

        private static int GetMaxIndex(double[] action)
        {
            var index = -1;
            var oneHotVal = double.NegativeInfinity;

            for (var i = 0; i < action.Length; i++)
            {
                if (action[i] > oneHotVal)
                {
                    index = i;
                    oneHotVal = action[i];
                }
            }

            return index;
        }

        private double[] OneHotOfVertex(int vertex)
        {
            var result = new double[Graph.NumberOfVertices];
            result[vertex] = 1d;
            return result;
        }

        public override int RandomSeed { get; set; }

        protected double[] GetOutput(int source)
        {
            var n = Graph.NumberOfVertices;
            var arr = new double[n*n + 2*n];
            
            EncodedGraph.CopyTo(arr, 0);
            arr[n * n + source] = 1d;
            arr[n * n + n + Goal] = 1d;

            return arr;
        }

        public override void ResetIteration()
        {
            CurrentVertex = _startVertex;
            _currentScore = 0;
            _step = 0;
        }

        public override void ResetAll()
        {
            CurrentVertex = _startVertex;
            _currentScore = 0;
            _step = 0;
        }

        #region RecordTimesteps

        private int _step;
        public override bool RecordTimeSteps { get; set; }
        protected EnvironmentTimeStep PrevTimeStep;

        public override EnvironmentTimeStep InitialTimeStep
        {
            get
            {
                return PrevTimeStep = new EnvironmentTimeStep(new double[InputCount], GetOutput(CurrentVertex), _currentScore);
            }
        }
        public override EnvironmentTimeStep PreviousTimeStep => PrevTimeStep;
        #endregion
    }
}