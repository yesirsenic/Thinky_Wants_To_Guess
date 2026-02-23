using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorManager : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Image colorWheelImage;
    public Slider rSlider, gSlider, bSlider, aSlider;
    public Image previewImage;

    public DrawingCanvas drawingCanvas;

    private Texture2D wheelTexture;
    private Color currentColor;

    bool isUpdatingUI = false;

    void Start()
    {
        wheelTexture = colorWheelImage.sprite.texture;

        // 슬라이더 범위/정수 설정 (혹시 인스펙터에서 안 했을 때 대비)
        rSlider.minValue = 0; rSlider.maxValue = 255; rSlider.wholeNumbers = true;
        gSlider.minValue = 0; gSlider.maxValue = 255; gSlider.wholeNumbers = true;
        bSlider.minValue = 0; bSlider.maxValue = 255; bSlider.wholeNumbers = true;
        aSlider.minValue = 0; aSlider.maxValue = 255; aSlider.wholeNumbers = true;

        // ✅ 초기 색 지정 (원하는 값으로)
        currentColor = Color.black;
        currentColor.a = 1f;

        // ✅ UI(슬라이더/프리뷰/브러시)로 반영
        UpdateUIFromColor(updateAlpha: true);

        // 리스너는 "초기값 반영" 후에 붙이는 게 안전
        rSlider.onValueChanged.AddListener(_ => OnSliderChanged());
        gSlider.onValueChanged.AddListener(_ => OnSliderChanged());
        bSlider.onValueChanged.AddListener(_ => OnSliderChanged());
        aSlider.onValueChanged.AddListener(_ => OnSliderChanged());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PickColor(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        PickColor(eventData);
    }

    void PickColor(PointerEventData eventData)
    {
        RectTransform rect = colorWheelImage.rectTransform;
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Rect rectArea = rect.rect;

            float x = (localPoint.x - rectArea.x) / rectArea.width;
            float y = (localPoint.y - rectArea.y) / rectArea.height;

            int texX = Mathf.Clamp(Mathf.RoundToInt(x * wheelTexture.width), 0, wheelTexture.width - 1);
            int texY = Mathf.Clamp(Mathf.RoundToInt(y * wheelTexture.height), 0, wheelTexture.height - 1);

            Color picked = wheelTexture.GetPixel(texX, texY);

            // 🔥 기존 알파 유지
            picked.a = currentColor.a;

            currentColor = picked;

            UpdateUIFromColor(updateAlpha: false);
        }
    }

    void OnSliderChanged()
    {
        if (isUpdatingUI) return;

        currentColor = new Color(
            rSlider.value / 255f,
            gSlider.value / 255f,
            bSlider.value / 255f,
            aSlider.value / 255f
        );

        ApplyColor();
    }

    void UpdateUIFromColor(bool updateAlpha)
    {
        isUpdatingUI = true;

        rSlider.value = currentColor.r * 255f;
        gSlider.value = currentColor.g * 255f;
        bSlider.value = currentColor.b * 255f;

        if (updateAlpha)
            aSlider.value = currentColor.a * 255f;

        isUpdatingUI = false;

        ApplyColor();
    }

    void ApplyColor()
    {
        previewImage.color = currentColor;
        drawingCanvas.SetBrushColor(currentColor);
    }
}