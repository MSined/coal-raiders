﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace coal_raider
{
    class NavigatingObject : Object
    {
        struct NodeRecord
        {
            public Waypoint currentWaypoint;
            public Waypoint fromWaypoint;
            public NodeRecord[] fromNode;
            public float costSoFar;
            public float estimatedTotalCost;
        }

        public Vector3? targetPosition { get; protected set; }
        Waypoint subTarget;
        public bool newTargetPosition = true;
        int pathStep = 0;
        List<Waypoint> pathToTarget = new List<Waypoint>();

        public Vector3 lookDirection = new Vector3(1, 0, 0);
        public Vector3 velocity = new Vector3(0, 0, 0);
        
        // Characters initial position is defined by the spawnpoint ther are associated with
        public NavigatingObject(Game game, Model[] modelComponents, Vector3 position, bool isAlive, bool canCollide)
            : base(game, modelComponents, position, isAlive, canCollide)
        {
            lookDirection = new Vector3(1, 0, 0);
            velocity = new Vector3(1, 0, 0);
            this.position = position;
        }

        protected void moveToTargetPosition(List<Waypoint> waypointsList)
        {
            Vector3 myTarget = (Vector3)targetPosition;
            bool isNaN = false;
            if (float.IsNaN(this.position.X) ||float.IsNaN(this.position.Y)||float.IsNaN(this.position.Z) ||
                float.IsNaN(myTarget.X) || float.IsNaN(myTarget.Y) || float.IsNaN(myTarget.Z))
                isNaN = true;

            // If target acquired
            if (targetPosition != null && !isNaN)
            {
                // If the target is a new one
                if (newTargetPosition)

                {
                    // Find path to target
                    pathToTarget = aStarToPosition(waypointsList);

                    // Get first subTarget goal
                    pathStep = 0;
                    if (pathToTarget.Count != 0)
                    {
                        if (pathStep < pathToTarget.Count)
                        {
                            pathToTarget[pathStep].closed = false;
                            subTarget = pathToTarget[pathStep++];
                        }
                        else
                        {
                            targetPosition = null;
                            return;
                        }
                    }
                    else
                    {
                        targetPosition = null;
                        return;
                    }
                    newTargetPosition = false;
                }

                Vector3 targetPos = (Vector3)targetPosition;

                // If the intended target is within reach
                float distToTarget = (targetPos - this.position).Length();
                float distToSubTarget = (subTarget.position - this.position).Length();

                // Start following the path
                if (distToSubTarget < 1)
                {
                    // If there are still subTargets in the list
                    // Go to the next one
                    if (pathStep < pathToTarget.Count)
                        subTarget = pathToTarget[pathStep++];
                    // Otherwise head back to the spawnpoint
                    else if (distToSubTarget <= 1)
                    {
                        velocity = Vector3.Zero;
                        //target = this.spawnPoint;
                        subTarget = null;
                        newTargetPosition = true;
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

        private List<Waypoint> aStarToPosition(List<Waypoint> waypointsList)
        {
            List<Waypoint> pathToTake = new List<Waypoint>();

            // Find the nearest waypoint as a start point for the search
            Waypoint nearestWaypoint = getNearestWaypoint(this.position, waypointsList);
            Waypoint nearestToTarget = getNearestWaypoint((Vector3)targetPosition, waypointsList);

            List<NodeRecord> openList = new List<NodeRecord>();
            List<NodeRecord> closedList = new List<NodeRecord>();
            NodeRecord current = new NodeRecord();

            NodeRecord startRecord = new NodeRecord();
            startRecord.currentWaypoint = nearestWaypoint;
            startRecord.costSoFar = 0;
            startRecord.fromWaypoint = null;
            startRecord.fromNode = new NodeRecord[1];
            startRecord.estimatedTotalCost = (startRecord.currentWaypoint.position - nearestToTarget.position).Length();

            openList.Add(startRecord);

            float endNodeCost = 0;
            float endNodeHeuristic = 0;
            NodeRecord nodeFoundInList;
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

                    if (containedInList(closedList, endNode, out nodeFoundInList))
                    {
                        endNodeRecord = new NodeRecord();
                        endNodeRecord.costSoFar = nodeFoundInList.costSoFar;
                        endNodeRecord.currentWaypoint = nodeFoundInList.currentWaypoint;
                        endNodeRecord.estimatedTotalCost = nodeFoundInList.estimatedTotalCost;
                        endNodeRecord.fromWaypoint = nodeFoundInList.fromWaypoint;
                        endNodeRecord.fromNode = nodeFoundInList.fromNode;

                        // Equivalent to closedList.Find(endNode)
                        // When containedInList is run, if it finds the node
                        // It will store it at an out variable so we dont need to 
                        // loop through the list AGAIN

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        closedList.Remove(endNodeRecord);

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else if (containedInList(openList, endNode, out nodeFoundInList))
                    {
                        endNodeRecord = new NodeRecord();
                        endNodeRecord.costSoFar = nodeFoundInList.costSoFar;
                        endNodeRecord.currentWaypoint = nodeFoundInList.currentWaypoint;
                        endNodeRecord.estimatedTotalCost = nodeFoundInList.estimatedTotalCost;
                        endNodeRecord.fromWaypoint = nodeFoundInList.fromWaypoint;
                        endNodeRecord.fromNode = nodeFoundInList.fromNode;

                        // Equivalent to closedList.Find(endNode)
                        // When containedInList is run, if it finds the node
                        // It will store it at an out variable so we dont need to 
                        // loop through the list AGAIN

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }

                    else
                    {
                        endNodeRecord = endNode;

                        endNodeHeuristic = (endNode.currentWaypoint.position - nearestToTarget.position).Length();
                    }

                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.currentWaypoint = connection.connectedTo;
                    endNodeRecord.fromWaypoint = endNode.fromWaypoint;
                    // might cause problem?
                    endNodeRecord.fromNode[0] = endNode.fromNode[0];
                    endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                    if (!containedInList(openList, endNode, out nodeFoundInList))
                    {
                        openList.Add(endNodeRecord);
                        // DEBUG STUFF
                        if (endNodeRecord.currentWaypoint != null)
                            endNodeRecord.currentWaypoint.open = true;
                    }
                }

                openList.Remove(current);
                closedList.Add(current);
                // DEBUG STUFF
                if (current.currentWaypoint != null)
                    current.currentWaypoint.closed = true;
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

        private bool containedInList(List<NodeRecord> list, NodeRecord node, out NodeRecord foundNode)
        {
            foundNode = new NodeRecord();
            foreach (NodeRecord n in list)
                if (node.currentWaypoint.ID == n.currentWaypoint.ID)
                {
                    foundNode.costSoFar = n.costSoFar;
                    foundNode.currentWaypoint = n.currentWaypoint;
                    foundNode.estimatedTotalCost = n.estimatedTotalCost;
                    foundNode.fromWaypoint = n.fromWaypoint;
                    foundNode.fromNode = n.fromNode;

                    return true;
                }

            return false;
        }

        private Waypoint getNearestWaypoint(Vector3 position, List<Waypoint> waypointsList)
        {
            float dist = float.MaxValue; ;
            Waypoint nearest = null;
            foreach (Waypoint w in waypointsList)
            {
                float t = (position - w.position).Length();
                if (t < dist)
                {
                    nearest = w;
                    dist = t;
                }
            }
            return nearest;
        }
    }
}