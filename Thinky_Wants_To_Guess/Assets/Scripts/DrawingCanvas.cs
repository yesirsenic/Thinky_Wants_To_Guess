using UnityEngine;
using UnityEngine.UI;

public class DrawingCanvas : MonoBehaviour
{
    public RawImage drawingView;
    public Material brushMaterial;
    public float brushSize = 10f;
    public Color brushColor = Color.black;

    public BrushType currentBrush;

    public BrushSetting pencilSetting;
    public BrushSetting penSetting;
    public BrushSetting brushSetting;

    private RenderTexture drawingRT;
    private Vector2? lastUV = null;

    void Start()
    {
        drawingRT = new RenderTexture(1400, 600, 0);
        drawingRT.Create();

        drawingView.texture = drawingRT;
        Clear();

        SetBrushPencil();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 uv;
            if (GetMouseUV(out uv))
            {
                if (lastUV.HasValue)
                {
                    DrawLine(lastUV.Value, uv);
                }
                else
                {
                    Draw(uv);
                }

                lastUV = uv;
            }
        }
        else
        {
            lastUV = null;
        }
    }

    bool GetMouseUV(out Vector2 uv)
    {
        uv = Vector2.zero;

        Vector2 localPoint;
        RectTransform rect = drawingView.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            Input.mousePosition,
            null,
            out localPoint))
            return false;

        Rect rectArea = rect.rect;

        // 도화지 범위 밖이면 무시
        if (!rectArea.Contains(localPoint))
            return false;

        // local 좌표를 0~1 UV로 변환
        float x = (localPoint.x - rectArea.x) / rectArea.width;
        float y = (localPoint.y - rectArea.y) / rectArea.height;

        uv = new Vector2(x, y);
        return true;
    }

    public void Clear()
    {
        var prev = RenderTexture.active;
        RenderTexture.active = drawingRT;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = prev;
    }
    public void SetBrushPencil()
    {
        currentBrush = BrushType.Pencil;
        ApplyBrushSettings();
    }

    public void SetBrushPen()
    {
        currentBrush = BrushType.Pen;
        ApplyBrushSettings();
    }

    public void SetBrushBrush()
    {
        currentBrush = BrushType.Brush;
        ApplyBrushSettings();
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);
        int steps = Mathf.CeilToInt(distance / (brushSize * 0.5f));

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 lerp = Vector2.Lerp(from, to, t);
            Draw(lerp);
        }
    }

    void Draw(Vector2 uv)
    {
        float sizeUV = brushSize / drawingRT.width;

        brushMaterial.SetVector("_Coordinate", uv);
        brushMaterial.SetFloat("_Size", sizeUV);
        brushMaterial.SetColor("_Color", brushColor);

        RenderTexture temp = RenderTexture.GetTemporary(drawingRT.width, drawingRT.height);

        Graphics.Blit(drawingRT, temp);
        Graphics.Blit(temp, drawingRT, brushMaterial);

        RenderTexture.ReleaseTemporary(temp);
    }

    void ApplyBrushSettings()
    {
        BrushSetting setting = null;

        switch (currentBrush)
        {
            case BrushType.Pencil:
                setting = pencilSetting;
                break;

            case BrushType.Pen:
                setting = penSetting;
                break;

            case BrushType.Brush:
                setting = brushSetting;
                break;
        }

        brushSize = setting.size;

        // 알파 적용
        brushColor = new Color(
            brushColor.r,
            brushColor.g,
            brushColor.b,
            setting.alpha
        );
    }
}