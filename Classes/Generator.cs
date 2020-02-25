namespace QuestGenerator
{
    internal class Generator
    {
        private static int Main()
        {
            string[] path = Planner.PlanPath("blocks.pddl", "domain.pddl");
            return 0;
        }
    }
}
