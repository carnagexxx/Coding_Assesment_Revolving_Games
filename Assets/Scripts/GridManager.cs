using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assignment.Grid
{
    [Serializable]
    public enum PieceType
    {
        None,
        P1,
        P2,
        P3,
        P4,
        P5,
        P6,
        P7
    }

    [Serializable]
    public struct GridNode
    {
        public bool IsValid;
        public PieceType Type;
    }
    /// <summary>
    /// This class manages all operations on the grid.
    /// </summary>
    public class GridController
    {
        private GridModel gridModel;
        private GridView gridView;

        public GridController(GameObject piecePrefab, GridLayoutGroup gridLayout)
        {
            this.gridModel = new GridModel();
            this.gridView = new GridView(piecePrefab, gridLayout);
        }

        public void Initialize(List<int> validPlaces, int rows, int columns)
        {
            this.gridView.Clear();
            this.gridModel.Initialize(validPlaces, rows, columns);
            this.gridView.SpawnTiles(this.gridModel.GetNodes());
        }

        public void Analyze()
        {
            List<Vector2Int> nodesToCheck = new List<Vector2Int>();
            for (int i = 0; i < this.gridModel.Rows; i++)
            {
                for (int j = 0; j < this.gridModel.Columns; j++)
                {
                    if (this.gridModel.GetNode(i, j).IsValid)
                    { nodesToCheck.Add(new Vector2Int(i, j)); }
                }
            }
            Queue<Vector2Int> frontier = new Queue<Vector2Int>();
            Queue<Vector2Int> optimalMoveSet = new Queue<Vector2Int>();
            Queue<Vector2Int> contiguousNodes;

            while (nodesToCheck.Count > 0)
            {
                Vector2Int currentNode = nodesToCheck[0];
                nodesToCheck.RemoveAt(0);
                frontier.Enqueue(currentNode);
                contiguousNodes = new Queue<Vector2Int>();

                while (frontier.Count > 0)
                {
                    currentNode = frontier.Dequeue();
                    contiguousNodes.Enqueue(currentNode);
                    foreach (Vector2Int nextPos in this.GetNeighbours(currentNode.x, currentNode.y))
                    {
                        if (this.gridModel.GetNode(nextPos.x, nextPos.y).Type == this.gridModel.GetNode(currentNode.x, currentNode.y).Type
                            && nodesToCheck.Contains(nextPos))
                        {
                            frontier.Enqueue(nextPos);
                            nodesToCheck.Remove(nextPos);
                        }
                    }
                }

                if (contiguousNodes.Count > optimalMoveSet.Count)
                {
                    optimalMoveSet = contiguousNodes;
                }
            }

            Debug.Log("OptimalMoves:");
            if (optimalMoveSet.Count < 3) { return; }
            Vector2Int[] optimalMoveArray = optimalMoveSet.ToArray();
            foreach (var node in optimalMoveArray)
            {
                this.gridView.SetTileActive(node, true);
            }
        }

        private List<Vector2Int> GetNeighbours(int x, int y)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            if (x < this.gridModel.Rows - 1)
            {
                neighbours.Add(new Vector2Int(x + 1, y));
            }

            if (y < this.gridModel.Columns - 1)
            {
                neighbours.Add(new Vector2Int(x, y + 1));
            }

            if (x > 0)
            {
                neighbours.Add(new Vector2Int(x - 1, y));
            }

            if (y > 0)
            {
                neighbours.Add(new Vector2Int(x, y - 1));
            }

            return neighbours;
        }

        public class GridModel
        {
            private GridNode[][] nodes;
            private int rows;
            private int columns;
            public int Rows { get { return rows; } }
            public int Columns { get { return columns; } }
            public void Initialize(List<int> validPlaces, int rowCount, int columnCount)
            {
                rows = rowCount;
                columns = columnCount;
                nodes = new GridNode[rows][];
                for (int i = 0; i < rows; i++)
                {
                    nodes[i] = new GridNode[columns];
                }
                int k = 0;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        nodes[i][j] = new GridNode();
                        if (validPlaces[k++] != 0)
                        {
                            nodes[i][j].IsValid = true;
                            nodes[i][j].Type = (PieceType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(PieceType)).Length);
                        }
                    }
                }
            }
            public GridNode GetNode(int x, int y)
            {
                return nodes[x][y];
            }
            public GridNode[][] GetNodes()
            {
                return nodes;
            }
        }

        public class GridView
        {
            private GameObject tilePrefab;
            private GridLayoutGroup layout;
            private Dictionary<Vector2Int, GridTileView> tileObjects;

            public GridView(GameObject _tilePrefab, GridLayoutGroup _parent)
            {
                tilePrefab = _tilePrefab;
                layout = _parent;
                tileObjects = new Dictionary<Vector2Int, GridTileView>();
            }

            public void SpawnTiles(GridNode[][] nodes)
            {
                layout.constraintCount = nodes.Length;
                if (nodes.Length > 0)
                    layout.constraintCount = nodes[0].Length;
                for (int i = 0; i < nodes.Length; i++)
                {
                    for (int j = 0; j < nodes[i].Length; j++)
                    {
                        GameObject gameObject = UnityEngine.Object.Instantiate(tilePrefab, layout.transform);
                        gameObject.transform.localPosition = Vector3.zero;
                        GridTileView tileView = gameObject.GetComponent<GridTileView>();

                        tileView.UpdateView(nodes[i][j]);
                        tileObjects.Add(new Vector2Int(i, j), tileView);
                    }
                }
            }

            public void SetTileActive(Vector2Int position, bool active)
            {
                if (tileObjects.TryGetValue(position, out GridTileView tileView))
                {
                    tileView.SetSelected(active);
                }
            }

            public void Clear()
            {
                foreach (var tileObject in tileObjects.Values)
                {
                    UnityEngine.Object.Destroy(tileObject.gameObject);
                }
                tileObjects.Clear();
            }
        }
    }
}