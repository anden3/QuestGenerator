using System;
using System.Linq;
using System.Collections.Generic;

using PathList = System.Collections.Generic.List<QuestGenerator.Node[]>;

namespace QuestGenerator
{
    interface IEvaluator
    {
        float Evaluate(World world, PathList paths);
    }

    public static class Evaluator
    {
        private static readonly IEvaluator[] evaluators = new IEvaluator[]
        {
            new NarrativeContentEvaluator(),
            new PathLengthEvaluator(),
            new BranchCountEvaluator(),
            new CostEvaluator(),
            new EncounterEvaluator(),
            new UniquenessEvaluator(),
            new WeightOfChoicesEvaluator()
        };

        public static float EvaluateWorld(World world)
        {
            var paths = GetAllPossiblePaths(world);
            return evaluators.Sum(e => e.Evaluate(world, paths));
        }

        private static PathList GetAllPossiblePaths(World world)
        {
            PathList foundPaths = new PathList();

            Queue<(Node node, Node[] path)> queue = new Queue<(Node, Node[])>();
            queue.Enqueue((world.nodes[0], new Node[] { world.nodes[0] }));

            while (queue.Count > 0)
            {
                (Node node, Node[] path) = queue.Dequeue();

                if (node.next.Count == 0)
                {
                    foundPaths.Add(path);
                    continue;
                }

                foreach (Node n in node.next)
                {
                    Node[] newPath = new Node[path.Length + 1];
                    Array.Copy(path, 0, path, 0, path.Length);
                    newPath[path.Length + 1] = n;

                    queue.Enqueue((n, newPath));
                }
            }

            return foundPaths;
        }
    }

    internal class NarrativeContentEvaluator : IEvaluator
    {
        private const float weight = 1.0f;
        public float Evaluate(World world, PathList paths) => world.nodes.Count * weight;
    }

    internal class PathLengthEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            IEnumerable<int> pathLengths = paths.Select(p => p.Length);

            return pathLengths.Min()     * minWeight +
            (float)pathLengths.Average() * avgWeight +
                   pathLengths.Max()     * maxWeight;
        }
    }

    internal class BranchCountEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            IEnumerable<int> branchCounts = paths.Select(p => p.Count(n => n.next.Count > 1));

            return branchCounts.Min()     * minWeight +
            (float)branchCounts.Average() * avgWeight +
                   branchCounts.Max()     * maxWeight;
        }
    }

    internal class CostEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            IEnumerable<int> pathCost = paths.Select(p => p.Count(n => n.action.category == ActionCategory.Cost));

            return pathCost.Min()     * minWeight +
            (float)pathCost.Average() * avgWeight +
                   pathCost.Max()     * maxWeight;
        }
    }

    internal class EncounterEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            IEnumerable<int> pathEncounters = paths.Select(p => 
                p.Count(n => n.action.category == ActionCategory.Encounter));

            return pathEncounters.Min()     * minWeight +
            (float)pathEncounters.Average() * avgWeight +
                   pathEncounters.Max()     * maxWeight;
        }
    }

    internal class UniquenessEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            IEnumerable<int> uniqueness = paths.Select(p =>
                p.Select(n => n.action).Distinct().Count() / p.Length);

            return uniqueness.Min()     * minWeight +
            (float)uniqueness.Average() * avgWeight +
                   uniqueness.Max()     * maxWeight;
        }
    }

    internal class WeightOfChoicesEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            List<WorldState> endStates = GetEndStates(world);

            // Handshake problem.
            List<float> differences = new List<float>(endStates.Count * (endStates.Count - 1) / 2);

            for (int i = 0; i < endStates.Count; ++i)
            {
                for (int j = i + 1; j < endStates.Count; ++j)
                {
                    WorldState a = endStates[i];
                    WorldState b = endStates[j];

                    int allAttributes = 0;
                    int commonAttributes = 0;

                    allAttributes += Utility.GetFullAttributeCount(a.characters, b.characters);
                    commonAttributes += Utility.GetCommonAttributeCount(a.characters, b.characters);

                    int allRelations = a.relations.Union(b.relations).Count();
                    int commonRelations = a.relations.Intersect(b.relations).Count();

                    differences.Add((commonAttributes + commonRelations) / (allAttributes + allRelations));
                }
            }

            return differences.Min()     * minWeight +
                   differences.Average() * avgWeight +
                   differences.Max()     * maxWeight;
        }

        private List<WorldState> GetEndStates(World world)
        {

        }
    }

    internal class NarrativeRichnessEvaluator : IEvaluator
    {
        private const float minWeight = 1.0f;
        private const float avgWeight = 1.0f;
        private const float maxWeight = 1.0f;

        public float Evaluate(World world, PathList paths)
        {
            List<WorldState> endStates = GetEndStates(world);

            // Handshake problem.
            List<float> differences = new List<float>(endStates.Count * (endStates.Count - 1) / 2);

            foreach (WorldState state in endStates)
            {
                // Find how many preconditions in start state has been changed in state.
            }

            return differences.Min()     * minWeight +
                   differences.Average() * avgWeight +
                   differences.Max()     * maxWeight;
        }

        private List<WorldState> GetEndStates(World world)
        {

        }
    }
}
