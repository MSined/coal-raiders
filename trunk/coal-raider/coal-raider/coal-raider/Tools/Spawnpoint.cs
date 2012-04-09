using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace coal_raider
{
    class Spawnpoint : Waypoint
    {
        public int team { get; protected set; }

        public Spawnpoint(Game game, Model model, Vector3 position, int team)
            : base(game, model, position)
        {
            this.team = team;
        }
    }
}
