using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuestGenerator
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Result
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string problem;

        public float timeTaken;
        public int length;
        public int cost;

        public IntPtr actions;
    }

    public static class Planner
    {
        public enum Algorithm
        {
            BFS,
            GBFS
        }

        public enum Direction
        {
            Forward,
            Backward
        }

        public enum Heuristic
        {
            H1Plus,
            H1Max,
            H2Plus,
            H2Max
        }

        public struct ScheduleStep
        {
            public Direction dir;
            public Heuristic heuristic;
            public int milliseconds;
        }

        public static string[] PlanPath(
            string problem,
            string domain,

            Algorithm algorithm = Algorithm.GBFS,
            Direction direction = Direction.Backward,
            Heuristic heuristic = Heuristic.H1Plus,

            int verbosity = 1,
            float costWeight = 5.0f,

            ScheduleStep[] schedule = null)
        {
            /*
             * usage:
             * hsp <flags>* [ <algorithm> | -S <schedule> ]
             * <problem.pddl> <domain.pddl>
             */
            List<string> arguments = new List<string>
            {
                "hsp"
            };

            arguments.AddRange(
                $"-v {verbosity} -w {costWeight}".Split(' '));

            if (schedule != null)
            {
                /*
                 * <schedule> is a colon separated <option> list where each option has
                 * [<direction>,<heuristic>,<msecs>].
                 */
                string scheduleString = string.Join(":", schedule.Select(s =>
                    $"[{s.dir:g},".ToLower() +
                    $"{s.heuristic:g},".ToLower() +
                    $"{s.milliseconds}]"
                ));

                arguments.Add("-S");
                arguments.Add(scheduleString);
            }
            else
            {
                arguments.AddRange((
                    $"-a {algorithm:g}".ToLower() +
                    $" -d {direction:g}".ToLower() +
                    $" -h {heuristic:g}".ToLower()
                ).Split(' '));
            }

            arguments.Add(problem);
            arguments.Add(domain);

            string[] argArray = arguments.ToArray();

            IntPtr ptr = IntPtr.Zero;
            int count;

            using (StreamWriter outStream = NativeMethods.RedirectConsoleToFile("solution.txt"))
            {
                count = NativeMethods.Plan(arguments.Count, argArray, out ptr);
                NativeMethods.ResetConsole();
            }

            return NativeMethods.MarshalStringArray(ptr, count);
        }

        public static bool IsSolutionValid(string problem, string domain, string[] steps)
        {
            if (steps == null)
                return false;

            return NativeMethods.IsPathValid(problem, domain, steps, steps.Length) != 0;
        }
    }

    internal static class NativeMethods
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const int STD_ERROR_HANDLE = -12;

        private static TextWriter STDOUT;
        private static TextWriter STDERR;

        [DllImport("HSP2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Plan(int argc, string[] argv, out IntPtr result);

        [DllImport("HSP2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IsPathValid(string problem, string domain, string[] path, int pathLength);

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern int SetStdHandle(int device, IntPtr handle);

        internal static StreamWriter RedirectConsoleToFile(string fileName)
        {
            STDOUT = Console.Out;
            STDERR = Console.Error;

            FileStream filestream = new FileStream(fileName, FileMode.Create);
            StreamWriter streamwriter = new StreamWriter(filestream);

            IntPtr handle = filestream.SafeFileHandle.DangerousGetHandle();

            if (SetStdHandle(STD_OUTPUT_HANDLE, handle) == 0)
                Console.WriteLine("[ERROR] Redirection of stdout failed.");
            if (SetStdHandle(STD_ERROR_HANDLE, handle) == 0)
                Console.WriteLine("[ERROR] Redirection of stderr failed.");

            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            return streamwriter;
        }

        internal static void ResetConsole()
        {
            Console.Out.Flush();
            Console.Error.Flush();

            Console.SetOut(STDOUT ?? Console.Out);
            Console.SetError(STDERR ?? Console.Error);
        }

        internal static string[] MarshalStringArray(IntPtr array, int count)
        {
            IntPtr[] ptrArray = new IntPtr[count];
            string[] strings = new string[count];

            Marshal.Copy(array, ptrArray, 0, count);

            for (int i = 0; i < count; i++)
            {
                strings[i] = Marshal.PtrToStringAnsi(ptrArray[i]);
                Marshal.FreeCoTaskMem(ptrArray[i]);
            }

            Marshal.FreeCoTaskMem(array);
            return strings;
        }
    }
}
