using System.Collections.Generic;
using UnityEngine;

public class CustomGridLayout : MonoBehaviour
{
    public List<Transform> objects = new List<Transform>(); // List of objects to align
    public int rows = 3;
    public int columns = 3;
    public Vector2 areaSize = new Vector2(5f, 5f); // Width and Height of the area
    public float spacingX = 0.1f; // Horizontal spacing between objects
    public float spacingY = 0.1f; // Vertical spacing between objects
    public bool scaleToFit = true; // If true, objects will be scaled to fit within grid cells

    void OnValidate()
    {
        //AlignObjectsInGrid();
    }
    public void SetValues(int _rows, int _coloms, List<Transform> _objects)
    {
        rows = _rows;
        columns = _coloms;
        columns = _coloms;
        objects = _objects;
        // Collect child objects automatically
        //objects.Clear();
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    objects.Add(transform.GetChild(i));
        //}

        if (objects.Count == 0)
        {
            Debug.LogWarning("No objects found in the parent transform.");
            return;
        }

        if (objects.Count < rows * columns)
        {
            Debug.LogWarning("Not enough objects to fill the entire grid.");
        }
        AlignObjectsInGrid();
    }
    void AlignObjectsInGrid()
    {
        // Calculate available cell size considering separate X and Y spacing
        float cellWidth = (areaSize.x - (columns - 1) * spacingX) / columns;
        float cellHeight = (areaSize.y - (rows - 1) * spacingY) / rows;

        int index = 0; // To track objects in the array

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (index >= objects.Count) return; // Stop if we run out of objects

                // Calculate position with separate spacing for X and Y
                float xPos = col * (cellWidth + spacingX) - (areaSize.x / 2) + cellWidth / 2;
                float yPos = row * (cellHeight + spacingY) - (areaSize.y / 2) + cellHeight / 2;
                Vector3 position = new Vector3(xPos, yPos, 0f);

                // Move the existing object to the calculated position
                Transform obj = objects[index];
                obj.position = position;

                // Reset scale before applying new one
                obj.localScale = Vector3.one;

                // Scale the object to fit within its cell
                if (scaleToFit && obj.GetComponent<Renderer>())
                {
                    Vector3 originalSize = obj.GetComponent<Renderer>().bounds.size;
                    float scaleX = cellWidth / originalSize.x;
                    float scaleY = cellHeight / originalSize.y;
                    float scale = Mathf.Min(scaleX, scaleY); // Maintain aspect ratio
                    obj.localScale *= scale;
                }

                index++; // Move to the next object
            }
        }
    }
}
