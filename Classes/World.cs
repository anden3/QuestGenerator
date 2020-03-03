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
        public string precondition;
        public List<int> parameters = new List<int>();

        public bool Evaluate() => World.preconditions[precondition](this);
    }

    public class Action
    {
        public ActionType type;
        public ActionCategory category;

        public List<Precondition> preconditions = new List<Precondition>();

        public bool CanExecute() => preconditions.TrueForAll(p => p.Evaluate());

        public void Execute(World world)
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
        public Node parent = null;

        public List<int> parameters = new List<int>();
        public List<Node> next = new List<Node>();
    }

    public class Character
    {
        public bool alive;
    }

    public class World
    {
        public static Dictionary<string, Predicate<Precondition>> preconditions
            = new Dictionary<string, Predicate<Precondition>>
        {
            {"alive", (Precondition p) => characters[p.parameters[0]].alive }
        };

        public static Character[] characters;

        public static Action[] actions;
        public static Precondition[] startState;

        public List<Node> nodes = new List<Node>();
    }
}
