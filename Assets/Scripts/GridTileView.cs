using UnityEngine;
using UnityEngine.UI;
namespace Assignment.Grid
{
    public class GridTileView : MonoBehaviour
    {
        public Image image;
        public GameObject selectionIndicator;

        private GridNode node;

        public void UpdateView(GridNode node)
        {
            this.node = node;

            if (node.IsValid)
            {
                string spritePath = "Pieces/" + node.Type.ToString();
                Sprite sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null)
                    image.sprite = sprite;
            }
            else
            {
                image.color = new Color(0, 0, 0, 0);
            }
        }

        public void SetSelected(bool isSelected)
        {
            if (selectionIndicator != null)
                selectionIndicator.SetActive(isSelected);
        }
    }
}