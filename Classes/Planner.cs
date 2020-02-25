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

            FileStream filestream = new FileStream("logfile.txt", FileMode.Create);
            using StreamWriter streamwriter = new StreamWriter(filestream)
            {
                AutoFlush = true
            };

            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            IntPtr handle = filestream.SafeFileHandle.DangerousGetHandle();
            int status = NativeMethods.SetStdHandle(-11, handle);
            status = NativeMethods.SetStdHandle(-12, handle);

            string[] argArray = arguments.ToArray();

            IntPtr ptr = IntPtr.Zero;
            int count = NativeMethods.Plan(arguments.Count, argArray, out ptr);

            return NativeMethods.MarshalStringArray(ptr, count);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("HSP2.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Plan(int argc, string[] argv, out IntPtr result);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern int SetStdHandle(int device, IntPtr handle);

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
