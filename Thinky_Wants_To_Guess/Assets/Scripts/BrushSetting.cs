public enum BrushType
{
    Pencil,
    Pen,
    Brush,
    Eraser
}

[System.Serializable]
public class BrushSetting
{
    public float size;
    public float alpha;
}