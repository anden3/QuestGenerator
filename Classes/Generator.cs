namespace QuestGenerator
{
    internal class Generator
    {
        private static int Main()
        {
            Planner.PlanPath("blocks.pddl", "domain.pddl");
            return 0;
        }
    }
}
