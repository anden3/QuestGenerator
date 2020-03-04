using System;
using System.Linq;
using System.Collections.Generic;

using CharRelations = System.Collections.Generic.Dictionary
    <(QuestGenerator.Character a, QuestGenerator.Character b), float>;

namespace QuestGenerator
{
    public class Character
    {
        public bool alive;
    }

    public class WorldState
    {
        public Character[] characters;

        public CharRelations relations = new CharRelations();

        public static float GetDifference(WorldState a, WorldState b)
        {
            int allAttributes = 0;
            int commonAttributes = 0;

            allAttributes += Utility.GetFullAttributeCount(a.characters, b.characters);
            commonAttributes += Utility.GetCommonAttributeCount(a.characters, b.characters);

            int allRelations = a.relations.Union(b.relations).Count();
            int commonRelations = a.relations.Intersect(b.relations).Count();

            return (commonAttributes + commonRelations) / (allAttributes + allRelations);
        }
    }
}
