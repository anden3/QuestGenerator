using System;
using System.Collections.Generic;

namespace QuestGenerator
{
    public enum ActionType
    {
        Action1,
        Action2
    }

    public enum ActionCategory
    {
        None,
        Cost,
        Encounter
    }

    public class Precondition
    {
        public string name;
        public List<int> parameters = new List<int>();

        public bool Evaluate(WorldState state) => World.preconditions[name](parameters, state);
    }

    public class Action
    {
        public ActionType type;
        public ActionCategory category;

        public List<Precondition> preconditions = new List<Precondition>();

        public bool CanExecute(WorldState state) => preconditions.TrueForAll(p => p.Evaluate(state));

        public void Execute(WorldState state)
        {
            switch (type)
            {
                case ActionType.Action1:
                {
                    break;
                }
            }
        }
    }

    public class Node
    {
        public Action action;
        public List<int> parameters = new List<int>();

        public WorldState state;

        public Node parent = null;
        public List<Node> next = new List<Node>();
    }

    public class World
    {
        public static Dictionary<string, Func<List<int>, WorldState, bool>> preconditions
            = new Dictionary<string, Func<List<int>, WorldState, bool>>
        {
            {"alive", (List<int> arg, WorldState s) => s.characters[arg[0]].alive }
        };

        public static Action[] actions;
        public static WorldState startState;

        public List<Node> nodes = new List<Node>();
    }
}
