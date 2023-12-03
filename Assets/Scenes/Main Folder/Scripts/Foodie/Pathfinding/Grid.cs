using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// tutorial: https://youtu.be/waEsGu--9P8?si=rO_SICX_V1nrJX1j

public class Grid<TGridObject>
{
    // Variables
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;

    private TGridObject[,] gridArray; // declare multi-dimensional array
    private TextMesh[,] debugTextArray;


    // Constructor -- constructs grid array with dimensions of given width and height
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];


        // initialize grid to avoid null spaces
        for (int x = 0; x < gridArray.GetLength(0); x ++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y ++)
            {
                gridArray[x, y] = createGridObject(x, y);
            }
        }


        
        if (FoodieSystem.inst.showDebug)
        {
            for ( int x = 0; x < gridArray.GetLength(0); x++ )
            {
                for (int y = 0; y < gridArray.GetLength(1); y++ )
                {
                    // creates the numbers
                    debugTextArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x,y) + new Vector3(cellSize, cellSize) * 0.5f, 80, Color.white, TextAnchor.MiddleCenter);
                    debugTextArray[x, y].transform.parent = GameObject.Find("~DebugTextGrid").transform; // puts the numbers under a gameObject

                    // draws the grid lines
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }

            
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    // returns the world position of a coordinate
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition; 
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) // out -- can return both x and y separately
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    public void SetGridObject(int x, int y, TGridObject value)
    {
        // check to make sure x and y are valid numbers
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            // set the cell at x and y equal to given value
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject); // if TGridObject is an int -- returns 0
        }
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public TextMesh[,] GetDebugTextArray()
    {
        return debugTextArray;
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    // CodeMonkey's CreateWorldText util functions
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 80, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, float characterSize = 0.05f, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, characterSize, sortingOrder);
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, float characterSize, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.characterSize = characterSize;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

}
