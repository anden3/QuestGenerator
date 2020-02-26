using System;

namespace QuestGenerator
{
    internal class Generator
    {
        private static int Main()
        {
            /*
            string[] path = Planner.PlanPath(
                problem: "blocks.pddl",
                domain: "domain.pddl"
            );

            foreach (string action in path)
            {
                Console.WriteLine(action);
            }
            */

            bool isValid = Planner.IsSolutionValid(
                problem: "blocks.pddl",
                domain: "domain.pddl",
                steps: new string[] {
                    "(PICK-UP B)",
                    "(STACK B A)",
                    "(PICK-UP C)",
                    "(STACK C B)",
                    "(PICK-UP D)",
                    "(STACK D A)"
            });

            Console.WriteLine("Solution is " + (isValid ? "valid" : "not valid"));
            return 0;
        }
    }
}
