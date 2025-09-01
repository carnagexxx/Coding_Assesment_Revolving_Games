using Assignment.Grid;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Assignment.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GridLayoutGroup gridLayout;

        [SerializeField]
        private GameObject piecePrefab;

        [SerializeField]
        private Button initializeGridButton;

        [SerializeField]
        private Button analyzeButton;

        private GridController gridController;
        private List<int> validPlaces;
        private int rows;
        private int columns;

        private void Start()
        {
            validPlaces = ReadLayout();

            initializeGridButton.onClick.AddListener(InitializeGrid);
            analyzeButton.onClick.AddListener(AnalyzeGrid);

            gridController = new GridController(piecePrefab, gridLayout);
            gridController.Initialize(validPlaces, rows, columns);
        }

        private List<int> ReadLayout()
        {
            var textFile = Resources.Load<TextAsset>("layout");
            List<string> lines = new List<string>(textFile.text.Split('\n'));
            List<int> validPlaces = new List<int>();
            rows = 0;
            columns = 0;
            foreach (var line in lines)
            {
                if (line == null)
                {
                    break;
                }
                string[] s = line.Split(',');
                rows++;
                columns = s.Length;
                for (int i = 0; i < s.Length; i++)
                {
                    validPlaces.Add(int.Parse(s[i]));
                }
            }
            return validPlaces;
        }

        private void InitializeGrid()
        {
            gridController.Initialize(validPlaces, rows, columns);
        }
        private void AnalyzeGrid()
        {
            gridController.Analyze();
        }
    }
}