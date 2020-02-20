using System;
using System.Runtime.InteropServices;

namespace QuestGenerator
{
    struct action_t
    {
        byte[] op;
    }

    struct result_t
    {
        byte[] problem;
        float timeTaken;
        int length;
        int cost;

        action_t[] actions;
    }

    class Planner
    {
        [DllImport("hsp2.dll", EntryPoint = "plan", CharSet = CharSet.Auto)]
        internal static extern int plan(int argc, byte[][] argv, result_t result);
    }
}
