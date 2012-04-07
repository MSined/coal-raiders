using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace coal_raider
{
    class SpatialHashGrid
    {
        List<List<Object>> dynamicCells;
        List<List<Object>> staticCells;
        int cellsPerRow;
        int cellsPerCol;
        float cellSize;
        int[] cellIDs = new int[4];
        List<Object> foundObjects;
        float worldLeftX, worldBottomY;
        int numCells;

        public SpatialHashGrid(float worldWidth, float worldHeight, float cellSize, float leftX, float bottomY)
        {
            this.cellSize = cellSize;
            this.cellsPerRow = (int)Math.Ceiling(worldWidth / cellSize);
            this.cellsPerCol = (int)Math.Ceiling(worldHeight / cellSize);
            numCells = cellsPerRow * cellsPerCol;
            dynamicCells = new List<List<Object>>(numCells);
            staticCells = new List<List<Object>>(numCells);
            for (int i = 0; i < numCells; ++i)
            {
                dynamicCells.Add(new List<Object>(10));
                staticCells.Add(new List<Object>(10));
            }
            foundObjects = new List<Object>(10);
            this.worldLeftX = leftX;
            this.worldBottomY = bottomY;
        }

        public void insertStaticObject(Object obj)
        {
            int[] cellIDs = getCellIDs(obj);
            int i = 0;
            int cellID = -1;
            while (i <= 3 && (cellID = cellIDs[i]) != -1)
            {
                staticCells[cellID].Add(obj);
                ++i;
            }
        }
        public void insertDynamicObject(Object obj)
        {
            int[] cellIDs = getCellIDs(obj);
            int i = 0;
            int cellID = -1;
            while (i <= 3 && (cellID = cellIDs[i]) != -1)
            {
                Object e = obj;
                dynamicCells[cellID].Add(e);
                ++i;
            }
        }

        public void removeDynamicObject(Object obj)
        {
            if (obj is Unit)
                obj.isAlive = false;
            int i = 0;
            int cellID = -1;
            while (i <= 3 && (cellID = obj.cellIDs[i]) != -1)
            {
                dynamicCells[cellID].Remove(obj);
                ++i;
            }
        }

        public void removeStaticObject(Object obj)
        {
            int[] cellIDs = getCellIDs(obj);
            int i = 0;
            int cellID = -1;
            while (i <= 3 && (cellID = cellIDs[i]) != -1)
            {
                staticCells[cellID].Remove(obj);
                ++i;
            }
        }

        public void clearDynamicCells()
        {
            int len = dynamicCells.Count;
            for (int i = 0; i < len; ++i)
            {
                dynamicCells[i].Clear();
            }
        }

        public List<Object> getPotentialColliders(Object obj)
        {
            #region Reset the objects cellIDs
            foundObjects.Clear();
            int[] cellIDs = getCellIDs(obj);
            // Check to see if the object has changed any cell location
            for (int k = 0; k < 4; ++k)
            {
                // If this objects cell location has changed
                if (cellIDs[k] != obj.cellIDs[k])
                {
                    // If the new location is -1 (old location should not be -1)
                    if (cellIDs[k] == -1)
                    {
                        // Remove obj from the old cellID location
                        dynamicCells[obj.cellIDs[k]].Remove(obj);
                        // Do not add anything into the list as the new location is null
                    }
                    // If the old location was -1
                    else if (obj.cellIDs[k] == -1)
                    {
                        // Don't remove anything because there is nothing to remove
                        // Add new object to location
                        dynamicCells[cellIDs[k]].Add(obj);
                    }
                    // If neither the new or the old is -1
                    else
                    {
                        // Remove obj from the old cellID location
                        dynamicCells[obj.cellIDs[k]].Remove(obj);
                        // And insert it at the new cellID location
                        dynamicCells[cellIDs[k]].Add(obj);
                    }
                }
                // Set the old cellIDs to the new ones
                obj.cellIDs[k] = cellIDs[k];
            }
            #endregion

            int i = 0;
            int cellID = -1;
            while (i <= 3 && (cellID = cellIDs[i]) != -1)
            {
                int len = dynamicCells[cellID].Count;
                for (int j = 0; j < len; ++j)
                {
                    // Get all objects that are part in a particular cell
                    Object collider = dynamicCells[cellID].ElementAt<Object>(j);
                    // If the object is already in the list, don't include it
                    // Also make sure the collider is no itself!!
                    if (!foundObjects.Contains(collider) && collider.objectID != obj.objectID)
                        foundObjects.Add(collider);
                }

                len = staticCells[cellID].Count;
                for (int j = 0; j < len; ++j)
                {
                    // Get all objects that are part in a particular cell
                    Object collider = staticCells[cellID].ElementAt<Object>(j);
                    // If the object is already in the list, don't include it
                    // Also make sure the collider is no itself!!
                    if (!foundObjects.Contains(collider) && collider.objectID != obj.objectID)
                        foundObjects.Add(collider);
                }
                ++i;
            }
            return foundObjects;
        }

        public int[] getCellIDs(Object obj)
        {
            // cellID corresponding to lower left corner of bounds
            // Uses the worldLeftX and worldBottomY to create a 3D offset relative to the maps position
            int x1 = (int)(Math.Floor(obj.bounds.Min.X / cellSize) - worldLeftX);
            int y1 = (int)(Math.Floor(obj.bounds.Min.Z / cellSize) + worldBottomY);
            // cellID corresponding to upper right corner of bounds
            // Uses the worldLeftX and worldBottomY to create a 3D offset relative to the maps position
            int x2 = (int)(Math.Floor(obj.bounds.Max.X / cellSize) - worldLeftX);
            int y2 = (int)(Math.Floor(obj.bounds.Max.Z / cellSize) + worldBottomY);

            // Various cases for collision detection cases
            #region Check for single cell case
            // Check for single cell case
            // If the lowerleft corner is in same cell as upper left corner
            // then we are in the same grid cell
            if (x1 == x2 && y1 == y2)
            {
                if (x1 >= 0 && x1 < cellsPerRow && y1 >= 0 && y1 < cellsPerRow)
                    cellIDs[0] = x1 + y1 * cellsPerRow;
                else
                    cellIDs[0] = -1;
                cellIDs[1] = -1;
                cellIDs[2] = -1;
                cellIDs[3] = -1;
            }
            #endregion

            #region Horizontal 2-cell overlap case
            // Horizontal 2-cell overlap case
            else if (x1 == x2)
            {
                int i = 0;
                if (x1 >= 0 && x1 < cellsPerRow)
                {
                    if (y1 >= 0 && y1 < cellsPerCol)
                    {
                        cellIDs[i] = x1 + y1 * cellsPerRow;
                        ++i;
                    }
                    if (y2 >= 0 && y2 < cellsPerCol)
                    {
                        cellIDs[i] = x1 + y2 * cellsPerRow;
                        ++i;
                    }
                }
                while (i <= 3)
                {
                    cellIDs[i] = -1; ++i;
                };
            }
            #endregion

            #region Vertical 2-cell overlap case
            // Vertical 2-cell overlap case
            else if (y1 == y2)
            {
                int i = 0;
                if (y1 >= 0 && y1 < cellsPerCol)
                {
                    if (x1 >= 0 && x1 < cellsPerRow)
                    {
                        cellIDs[i] = x1 + y1 * cellsPerRow;
                        ++i;
                    }
                    if (x2 >= 0 && x2 < cellsPerRow)
                    {
                        cellIDs[i] = x2 + y1 * cellsPerRow;
                        ++i;
                    }
                }
                while (i <= 3)
                {
                    cellIDs[i] = -1; ++i;
                }
            }
            #endregion

            #region Handle 4-cell overlap case
            // Handles 4-cell overlap case
            else
            {
                int i = 0;
                int y1CellsPerRow = y1 * cellsPerRow;
                int y2CellsPerRow = y2 * cellsPerRow;
                if (x1 >= 0 && x1 < cellsPerRow && y1 >= 0 && y1 < cellsPerCol)
                {
                    cellIDs[i] = x1 + y1CellsPerRow;
                    ++i;
                }
                if (x2 >= 0 && x2 < cellsPerRow && y1 >= 0 && y1 < cellsPerCol)
                {
                    cellIDs[i] = x2 + y1CellsPerRow;
                    ++i;
                }
                if (x2 >= 0 && x2 < cellsPerRow && y2 >= 0 && y2 < cellsPerCol)
                {
                    cellIDs[i] = x2 + y2CellsPerRow;
                    ++i;
                }
                if (x1 >= 0 && x1 < cellsPerRow && y2 >= 0 && y2 < cellsPerCol)
                {
                    cellIDs[i] = x1 + y2CellsPerRow;
                    ++i;
                }
                while (i <= 3)
                {
                    cellIDs[i] = -1; ++i;
                }
            }
            #endregion

            return cellIDs;
        }

        public List<Object> getAttackBoxColliders(BoundingBox bb)
        {
            // cellID corresponding to lower left corner of bounds
            // Uses the worldLeftX and worldBottomY to create a 3D offset relative to the maps position
            int x1 = (int)(Math.Floor(bb.Min.X / cellSize) - worldLeftX);
            int y1 = (int)(Math.Floor(bb.Min.Z / cellSize) + worldBottomY);
            // cellID corresponding to upper right corner of bounds
            // Uses the worldLeftX and worldBottomY to create a 3D offset relative to the maps position
            int x2 = (int)(Math.Floor(bb.Max.X / cellSize) - worldLeftX);
            int y2 = (int)(Math.Floor(bb.Max.Z / cellSize) + worldBottomY);

            // The previous gets the top left and bottom right corner cell locations of
            // the passed bounding box

            // The following will calculate all cell IDs that are contained in that 2D range
            List<int> cellIDs = new List<int>();
            // Check these values, from y1 to y2, it may be ++ and not --
            // Consequently the comparison operator as well
            for(int i = x1; i < x2; ++i)
            {
                for (int j = y1; j > y2; --j)
                {
                    cellIDs.Add(i + j * cellsPerRow);
                }
            }

            // Get the objects that are within that range of cells
            foundObjects.Clear();
            int index = 0;
            int cellID = -1;
            while (index <= 3 && (cellID = cellIDs[index]) != -1)
            {
                int len = dynamicCells[cellID].Count;
                for (int j = 0; j < len; ++j)
                {
                    // Get all objects that are part in a particular cell
                    Object collider = dynamicCells[cellID].ElementAt<Object>(j);
                    // If the object is already in the list, don't include it
                    // Also make sure the collider is no itself!!
                    foundObjects.Add(collider);
                }

                len = staticCells[cellID].Count;
                for (int j = 0; j < len; ++j)
                {
                    // Get all objects that are part in a particular cell
                    Object collider = staticCells[cellID].ElementAt<Object>(j);
                    // If the object is already in the list, don't include it
                    // Also make sure the collider is no itself!!
                    foundObjects.Add(collider);
                }
                ++index;
            }
            return foundObjects;
        }
    }
}

