using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public BrushSetting eraserSetting;

    private RenderTexture drawingRT;
    private RenderTexture tempRT;
    private Vector2? lastUV = null;

    [SerializeField]
    private Sprite[] drawingToolsImages;

    [SerializeField]
    private Image drawingToolButtonImage;

    [SerializeField]
    private Slider eraserSizeSlider;

    [SerializeField]
    private RectTransform eraserPreview;

    void Start()
    {
        


        drawingRT = new RenderTexture(
                                        1400,
                                        600,
                                        0,
                                        RenderTextureFormat.ARGB32
                                     );

        drawingRT.filterMode = FilterMode.Point;
        drawingRT.wrapMode = TextureWrapMode.Clamp;
        drawingRT.Create();

        tempRT = new RenderTexture(
        drawingRT.width,
        drawingRT.height,
        0,
        RenderTextureFormat.ARGB32
        );

        tempRT.filterMode = FilterMode.Point;
        tempRT.wrapMode = TextureWrapMode.Clamp;

        tempRT.Create();



        drawingView.texture = drawingRT;
        Clear();

        brushColor = Color.black;
        brushMaterial.SetColor("_Color", brushColor);

        if (eraserSizeSlider != null)
        {
            eraserSizeSlider.onValueChanged.AddListener(SetEraserSize);
            eraserSizeSlider.value = eraserSetting.size;
        }

        SetBrushPencil();
    }

    void Update()
    {
        UpdateEraserPreview();

        if (IsPointerOverUIExceptDrawing())
        {
            lastUV = null;
            return;
        }

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

    bool IsPointerOverUIExceptDrawing()
    {
        if (EventSystem.current == null) return false;

        // PointerEventData 세팅 (마우스/터치 공통)
        PointerEventData ped = new PointerEventData(EventSystem.current);

        if (Input.touchCount > 0)
            ped.position = Input.GetTouch(0).position;
        else
            ped.position = Input.mousePosition;

        // UI 레이캐스트
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        if (results.Count == 0) return false;

        // "도화지(drawingView)" 위라면 허용
        var drawingGO = drawingView != null ? drawingView.gameObject : null;

        foreach (var r in results)
        {
            if (r.gameObject == null) continue;

            // 도화지 자신 또는 도화지의 자식(UI 오버레이 등)이면 막지 않음
            if (drawingGO != null && (r.gameObject == drawingGO || r.gameObject.transform.IsChildOf(drawingGO.transform)))
                continue;

            // 그 외 UI가 하나라도 걸리면 "막기"
            return true;
        }

        return false;
    }

    void UpdateEraserPreview()
    {
        if (eraserPreview == null) return;

        if (currentBrush != BrushType.Eraser)
        {
            eraserPreview.gameObject.SetActive(false);
            return;
        }

        eraserPreview.gameObject.SetActive(true);

        Vector2 localPoint;
        RectTransform rect = drawingView.rectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            Input.mousePosition,
            null,
            out localPoint))
        {
            eraserPreview.localPosition = localPoint;

            float size = brushSize;
            eraserPreview.sizeDelta = new Vector2(size, size);
        }
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

    public void SetBrushEraser()
    {
        currentBrush = BrushType.Eraser;

        brushSize = eraserSizeSlider.value;

        eraserSetting.size = brushSize;

        Debug.Log(brushSize);
    }

    public void SetEraserSize(float value)
    {
        if (currentBrush == BrushType.Eraser)
        {
            brushSize = value;
        }

        eraserSetting.size = value;
    }

    public void SetBrushColor(Color color)
    {
        brushColor = color;
    }

    public Texture2D GetTexture()
    {
        RenderTexture current = RenderTexture.active;
        RenderTexture.active = drawingRT;

        Texture2D tex = new Texture2D(drawingRT.width, drawingRT.height);
        tex.ReadPixels(new Rect(0, 0, drawingRT.width, drawingRT.height), 0, 0);
        tex.Apply();

        RenderTexture.active = current;

        return tex;
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);

        int steps = Mathf.CeilToInt(distance / (brushSize * 0.5f));

        if (steps <= 0)
        {
            Draw(to);
            return;
        }

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 lerp = Vector2.Lerp(from, to, t);
            Draw(lerp);
        }
    }

    void Draw(Vector2 uv)
    {
        if (float.IsNaN(uv.x) || float.IsNaN(uv.y))
            return;

        float sizeUV = brushSize / drawingRT.width;

        brushMaterial.SetVector("_Coordinate", uv);
        brushMaterial.SetFloat("_Size", sizeUV);

        Color finalColor = currentBrush == BrushType.Eraser
            ? Color.white
            : brushColor;

        brushMaterial.SetColor("_Color", finalColor);

        Graphics.Blit(drawingRT, tempRT);
        Graphics.Blit(tempRT, drawingRT, brushMaterial);
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

            case BrushType.Eraser:
                setting = eraserSetting;
                break;
        }

        drawingToolButtonImage.sprite = drawingToolsImages[(int)currentBrush];

        brushSize = setting.size;

    }
}