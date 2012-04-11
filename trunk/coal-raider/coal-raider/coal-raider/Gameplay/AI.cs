using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class AI
    {
        public enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        List<Spawnpoint> aiSpawnpoints;
        DamageableObject target;
        Random random = new Random();

        Game game;

        int aiTeam = 1;
        int maxInSquad = 6;
        int someNum = 10;

        int spawnTime = 2000; //in miliseconds
        int spawnTimer = 0;

        int unitTime = 10000; //in miliseconds
        int unitTimer = 0;

        int numWarrior, numRanger, numMage;
        int maxAddWarrior, maxAddRanger, maxAddMage;
        Model[][] models;

        public AI(Game game, Model[][] models, List<Spawnpoint> spawnpoints, DamageableObject target, Difficulty difficulty)
        {
            this.game = game;
            this.models = models;
            this.target = target;
            setDifficulty(difficulty);

            aiSpawnpoints = new List<Spawnpoint>();
            foreach (Spawnpoint sp in spawnpoints)
            {
                if (sp.team == aiTeam) aiSpawnpoints.Add(sp);
            }
        }

        private void setDifficulty(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    maxAddWarrior = 3;
                    maxAddRanger = 2;
                    maxAddMage = 1;
                    break;
                case Difficulty.Medium:
                    maxAddWarrior = 5;
                    maxAddRanger = 3;
                    maxAddMage = 2;
                    break;
                case Difficulty.Hard:
                    maxAddWarrior = 10;
                    maxAddRanger = 7;
                    maxAddMage = 5;
                    break;
            }
        }

        public Squad Update(GameTime gametime, Camera camera)
        {
            Squad squad = null;
            spawnTimer += gametime.ElapsedGameTime.Milliseconds;
            unitTimer += gametime.ElapsedGameTime.Milliseconds;

            if (unitTimer > unitTime)
            {
                numWarrior += random.Next(maxAddWarrior);
                numRanger += random.Next(maxAddRanger);
                numMage += random.Next(maxAddMage);
                unitTimer = 0;
            }

            if (spawnTimer > spawnTime && (numWarrior + numRanger + numMage) > 0)
            {
                Spawnpoint s = aiSpawnpoints.ToArray()[random.Next(aiSpawnpoints.Count)];

                Vector3 squadComp = getSquadComp();

                List<Unit> uList = new List<Unit>();
                for (int j = 0; j < squadComp.X; ++j)
                {
                    uList.Add(UnitFactory.createUnit(game, models[0], s.position, UnitType.Warrior, aiTeam, camera));
                }
                for (int j = 0; j < squadComp.Y; ++j)
                {
                    uList.Add(UnitFactory.createUnit(game, models[1], s.position, UnitType.Ranger, aiTeam, camera));
                }
                for (int j = 0; j < squadComp.Z; ++j)
                {
                    uList.Add(UnitFactory.createUnit(game, models[2], s.position, UnitType.Mage, aiTeam, camera));
                }

                if (uList.Count != 0)
                {

                    squad = SquadFactory.createSquad(game, uList.ToArray(), aiTeam);
                    
                    //return any of the possible targets
                    squad.setTarget(target);
                }

                spawnTimer = 0;
            }

            return squad;
        }

        private Vector3 getSquadComp()
        {
            Vector3 comp = new Vector3();
            do
            {
                int numInSquad = random.Next(1, maxInSquad);

                if ((numWarrior + numRanger + numMage) <= numInSquad)
                {
                    comp.X = numWarrior;
                    comp.Y = numRanger;
                    comp.Z = numMage;

                    numWarrior = numRanger = numMage = 0;

                    return comp;
                }

                Vector3 rand = new Vector3();
                rand.X = random.Next(someNum);
                rand.Y = random.Next(someNum);
                rand.Z = random.Next(someNum);
                float total = rand.X + rand.Y + rand.Z;

                comp.X = (int)Math.Floor(rand.X / total * numInSquad);
                comp.Y = (int)Math.Floor(rand.Y / total * numInSquad);
                comp.Z = (int)Math.Floor(rand.Z / total * numInSquad);

            } while (comp.X > numWarrior || comp.Y > numRanger || comp.Z > numMage);

            numWarrior -= (int)comp.X;
            numRanger -= (int)comp.Y;
            numMage -= (int)comp.Z;

            return comp;
        }
    }
}
