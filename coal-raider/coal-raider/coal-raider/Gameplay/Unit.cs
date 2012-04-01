using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class Unit : Object
    {
        struct NodeRecord
        {
            public Waypoint currentWaypoint;
            public Waypoint fromWaypoint;
            public NodeRecord[] fromNode;
            public float costSoFar;
            public float estimatedTotalCost;
        }

        public Object target { get; set; }
        Waypoint subTarget;
        public bool newTarget = false;
        int pathStep = 0;
        List<Waypoint> pathToTarget = new List<Waypoint>();

        public int topHP;
        public int hp;// { get; protected set; }
        public int topSP;
        public int sp;// { get; protected set; }

        private float spRecoverTimer = 0f;
        private float spRecoverInterval = 1000f;
        private int spRecoverRate = 5;

        protected int[] attributes { get; private set; }

        public Vector3 lookDirection = new Vector3(1, 0, 0);
        public Vector3 velocity = new Vector3(0, 0, 0);
        public float speed;

        //public Boolean poisoned = false, checkBoxCollision = false;
        //public int burningStacks = 0;
        //public bool wasUpdated = false;

        // Characters initial position is defined by the spawnpoint ther are associated with
        public Unit(Game game, Model[] modelComponents, Vector3 position,
            int topHP, int topSP, float speed, bool isAlive)
            : base(game, modelComponents, position, isAlive)
        {
            this.topHP = topHP;
            this.hp = topHP;

            this.topSP = topSP;
            this.sp = topSP;

            this.speed = speed;

            this.target = null;
        }

        public override void Update(GameTime gameTime, List<Object> colliders, List<Waypoint> waypointList)
        {
            if (target != null)
            {
                // Follow path or chase player
                moveToTarget(waypointList);
            }

            lookDirection.Normalize();

            float angle = (float)Math.Asin(lookDirection.X) + MathHelper.ToRadians(180);
            if (lookDirection.Z > 0)
            {
                angle = MathHelper.ToRadians(180) - angle;
            }

            world = Matrix.CreateRotationY(-angle) * Matrix.CreateTranslation(position);

            if (!(velocity.X == 0 && velocity.Y == 0 && velocity.Z == 0))
            {
                velocity.Normalize();
                position += speed * velocity;
            }
            //check collisions after moved
            CheckCollisions(colliders);

            checkIfDead();

            /*
            if (this is Enemy)
                checkBox.Update(gameTime, colliders, cameraTarget);
            */

            spRecoverTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (sp < topSP && spRecoverTimer > spRecoverInterval)
            {
                sp += spRecoverRate;
                spRecoverTimer = 0;
            }

            //base.Update(gameTime, colliders, cameraTarget, waypointList);
        }

        public void checkIfDead()
        {
            if (hp <= 0)
            {
                isAlive = false;
            }
        }

        private void moveToTarget(List<Waypoint> waypointsList)
        {
            // If target acquired
            if (target != null)
            {
                // If the target is a new one
                if (newTarget)
                {
                    // Find path to target
                    pathToTarget = aStarToTarget(waypointsList);

                    // Get first subTarget goal
                    pathStep = 0;
                    if (pathToTarget.Count != 0)
                    {
                        if (pathStep < pathToTarget.Count)
                            subTarget = pathToTarget[pathStep++];
                        else
                        {
                            target = null;
                        }
                    }
                    else
                    {
                        target = null;
                        return;
                    }
                    newTarget = false;
                }

                // If the intended target is within reach
                float distToTargetSquared = (target.position - this.position).LengthSquared();
                float distToSubTargetSquared = (subTarget.position - this.position).LengthSquared();

                // Start following the path
                if (distToSubTargetSquared < 1)
                {
                    // If there are still subTargets in the list
                    // Go to the next one
                    if (pathStep < pathToTarget.Count)
                        subTarget = pathToTarget[pathStep++];
                    // Otherwise head back to the spawnpoint
                    else if (distToSubTargetSquared <= 0.5)
                    {
                        velocity = Vector3.Zero;
                        //target = this.spawnPoint;
                        subTarget = null;
                        newTarget = true;
                    }
                }

                // If target is not within reach, and next subgoal is not reached
                // keep moving towards current subgoal
                else
                {
                    velocity = subTarget.position - this.position;
                    velocity.Normalize();
                    lookDirection = velocity;
                }
            }
        }
    
        private List<Waypoint> aStarToTarget(List<Waypoint> waypointsList)
        {
            List<Waypoint> pathToTake = new List<Waypoint>();

            // Find the nearest waypoint as a start point for the search
            Waypoint nearestWaypoint = getNearestWaypoint(this, waypointsList);
            Waypoint nearestToTarget = getNearestWaypoint(target, waypointsList);

            List<NodeRecord> openList = new List<NodeRecord>();
            List<NodeRecord> closedList = new List<NodeRecord>();
            NodeRecord current = new NodeRecord();

            NodeRecord startRecord = new NodeRecord();
            startRecord.currentWaypoint = nearestWaypoint;
            startRecord.costSoFar = 0;
            startRecord.fromWaypoint = null;
            startRecord.fromNode = new NodeRecord[1];
            startRecord.estimatedTotalCost = (startRecord.currentWaypoint.position - nearestToTarget.position).LengthSquared();

            openList.Add(startRecord);

            float endNodeCost = 0;
            float endNodeHeuristic = 0;
            while (openList.Count > 0)
            {
                float smallestCost = float.MaxValue;
                foreach (NodeRecord n in openList)
                    if (n.estimatedTotalCost < smallestCost)
                    {
                        current = n;
                        smallestCost = n.estimatedTotalCost;
                    }

                if (current.currentWaypoint.ID == nearestToTarget.ID)
                    break;

                foreach (Waypoint.Edge connection in current.currentWaypoint.connectedEdges)
                {
                    NodeRecord endNode = new NodeRecord();
                    endNode.currentWaypoint = connection.connectedTo;
                    endNode.fromWaypoint = current.currentWaypoint;
                    endNode.fromNode = new NodeRecord[1];
                    endNode.fromNode[0] = current;
                    endNodeCost = current.costSoFar + connection.length;

                    NodeRecord endNodeRecord = new NodeRecord();

                    if (containedInList(closedList, endNode))
                    {
                        endNodeRecord = new NodeRecord();

                        // Equivalent to closedList.Find(endNode)
                        foreach (NodeRecord n in closedList)
                        {
                            if (n.currentWaypoint.ID == endNode.currentWaypoint.ID)
                            {
                                endNodeRecord.costSoFar = n.costSoFar;
                                endNodeRecord.currentWaypoint = n.currentWaypoint;
                                endNodeRecord.estimatedTotalCost = n.estimatedTotalCost;
                                endNodeRecord.fromWaypoint = n.fromWaypoint;
                                endNodeRecord.fromNode = n.fromNode;
                            }

                        }

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        closedList.Remove(endNodeRecord);

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }

                    else if (containedInList(openList, endNode))
                    {
                        endNodeRecord = new NodeRecord();

                        // Equivalent to closedList.Find(endNode)
                        foreach (NodeRecord n in openList)
                        {
                            if (n.currentWaypoint.ID == endNode.currentWaypoint.ID)
                            {
                                endNodeRecord.costSoFar = n.costSoFar;
                                endNodeRecord.currentWaypoint = n.currentWaypoint;
                                endNodeRecord.estimatedTotalCost = n.estimatedTotalCost;
                                endNodeRecord.fromWaypoint = n.fromWaypoint;
                                endNodeRecord.fromNode = n.fromNode;
                            }
                        }

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }

                    else
                    {
                        endNodeRecord = endNode;

                        endNodeHeuristic = (endNode.currentWaypoint.position - nearestToTarget.position).LengthSquared();
                    }

                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.currentWaypoint = connection.connectedTo;
                    endNodeRecord.fromWaypoint = endNode.fromWaypoint;
                    // might cause problem?
                    endNodeRecord.fromNode[0] = endNode.fromNode[0];
                    endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                    if (!containedInList(openList, endNode))
                        openList.Add(endNodeRecord);
                }

                openList.Remove(current);
                closedList.Add(current);
            }

            if (current.currentWaypoint.ID != nearestToTarget.ID)
                return pathToTake;
            else
            {
                while (current.fromNode != null)
                {
                    pathToTake.Add(current.currentWaypoint);
                    current = current.fromNode[0];
                }
                pathToTake.Add(nearestWaypoint);
                pathToTake.Reverse();
            }

            return pathToTake;
        }

        private bool containedInList(List<NodeRecord> list, NodeRecord node)
        {
            foreach (NodeRecord n in list)
                if (node.currentWaypoint.ID == n.currentWaypoint.ID)
                    return true;

            return false;
        }

        private Waypoint getNearestWaypoint(Object o, List<Waypoint> waypointsList)
        {
            float dist = float.MaxValue; ;
            Waypoint nearest = null;
            foreach (Waypoint w in waypointsList)
            {
                float t = (o.position - w.position).LengthSquared();
                if (t < dist)
                {
                    nearest = w;
                    dist = t;
                }
            }
            return nearest;
        }

        public override void Draw(Camera camera)
        {
            // Required to stop drawing players that are dead and should not be drawn
            if (this.isAlive)
            {
                Matrix[] transforms = new Matrix[modelComponents[0].Bones.Count];
                modelComponents[0].CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in modelComponents[0].Meshes)
                {
                    foreach (BasicEffect be in mesh.Effects)
                    {
                        be.EnableDefaultLighting();
                        be.SpecularPower = 10f;
                        be.Projection = camera.projection;
                        be.View = camera.view;
                        be.World = world * mesh.ParentBone.Transform;
                    }
                    mesh.Draw();
                }
            }
        }

        public void drawHealth(Camera camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D healthTexture)
        {
            if (this.isAlive)
            {
                int healthBarWidth = 20;
                int healthBarHeight = 5;
                Rectangle srcRect, destRect;

                Vector3 screenPos = graphicsDevice.Viewport.Project(this.position + new Vector3(0, 0.8f, 0), camera.projection, camera.view, Matrix.Identity);

                srcRect = new Rectangle(0, 0, 1, 1);
                destRect = new Rectangle((int)screenPos.X - healthBarWidth / 2, (int)screenPos.Y, healthBarWidth, healthBarHeight);
                spriteBatch.Draw(healthTexture, destRect, srcRect, Color.LightGray, 0f, Vector2.Zero, SpriteEffects.None, 0.81f);

                float healthPercentage = (float)hp / (float)topHP;

                Color healthColor = new Color(new Vector3(1 - healthPercentage, healthPercentage, 0));

                srcRect = new Rectangle(0, 0, 1, 1);
                destRect = new Rectangle((int)screenPos.X - healthBarWidth / 2, (int)screenPos.Y, (int)(healthPercentage * healthBarWidth), healthBarHeight);
                spriteBatch.Draw(healthTexture, destRect, srcRect, healthColor, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);
            }
        }

        //collision vs buildings/ all bullet collisions are in Bullet
        public void CheckCollisions(List<Object> colliders)
        {
            foreach (Object o in colliders)
            {
                if (bounds.Intersects(o.bounds))
                {
                    if (o is StaticObject || o is Unit)
                    {
                        //neutralize the Z movement if going in a collision by moving up/down
                        if (position.X > o.bounds.Min.X && position.X < o.bounds.Max.X)
                        {
                            position -= speed * new Vector3(0, 0, velocity.Z);
                        }

                        //neutralize the X movement if going in a collision by moving left/right
                        if (position.Z > o.bounds.Min.Z && position.Z < o.bounds.Max.Z)
                        {
                            position -= speed * new Vector3(velocity.X, 0, 0);
                        }

                        /*update bounds again to make sure Character does not get stuck
                        if (bounds.FloatIntersects(o.bounds))
                        {//push against the building
                            Vector3 moveBack = position - o.position;
                            moveBack.Normalize();
                            position += moveBack * speed;
                        }*/
                    }
                }
            }
        }
    }
}