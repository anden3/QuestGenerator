using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestGenerator.Classes.World;

namespace QuestGenerator.Classes
{
    class PopulationGenerator
    {
        public class Quest
        {
            List<Action> Actions;
        }

        public class Encounter
        {
            public World.Action action;
            public List<Encounter> child = new List<Encounter>();
        }

        static Encounter newNode(World.Action action)
        {
            encounter temp = new Encounter();
            temp.action = action;
            return temp;
        }

        private int Main()
        {
            int numberOfIndividuals;
            List<Quest> population;

            for(int i = 0; i <= numberOfIndividuals; i++)
            {
                population[i] = GenerateIndividual();

            }
        }

        private void GenerateIndividual()
        {

        }

    }
}
