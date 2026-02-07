using UnityEngine;

public class UIConveyorBelt : MonoBehaviour
{
    [Header("Tiles")]
    public RectTransform tileA;
    public RectTransform tileB;

    [Header("Wrap Lines (place these in the UI)")]
    public RectTransform wrapStart; 
    public RectTransform wrapEnd;   

    [Header("Motion")]
    public Vector2 speedPixelsPerSecond = new Vector2(0f, -200f); 
    public bool useUnscaledTime = false;

    RectTransform parentRect;
    float tileWLocal;
    float tileHLocal;

    void Awake()
    {
        if (!tileA || !tileB) return;

        parentRect = tileA.parent as RectTransform;

        Canvas.ForceUpdateCanvases();
        tileWLocal = tileA.rect.width;
        tileHLocal = tileA.rect.height;

        if (Mathf.Abs(speedPixelsPerSecond.x) >= Mathf.Abs(speedPixelsPerSecond.y))
        {
            tileA.anchoredPosition = Vector2.zero;
            tileB.anchoredPosition = new Vector2(tileWLocal * Mathf.Sign(-speedPixelsPerSecond.x), 0f);
        }
        else
        {
            tileA.anchoredPosition = Vector2.zero;
            tileB.anchoredPosition = new Vector2(0f, tileHLocal * Mathf.Sign(-speedPixelsPerSecond.y));
        }
    }

    void Update()
    {
        if (!tileA || !tileB || !wrapStart || !wrapEnd) return;

        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        Vector2 delta = speedPixelsPerSecond * dt;

        tileA.anchoredPosition += delta;
        tileB.anchoredPosition += delta;

        bool horizontal = Mathf.Abs(speedPixelsPerSecond.x) >= Mathf.Abs(speedPixelsPerSecond.y);

        if (horizontal)
        {
            HandleHorizontalWrap(tileA, tileB);
            HandleHorizontalWrap(tileB, tileA);
        }
        else
        {
            HandleVerticalWrap(tileA, tileB);
            HandleVerticalWrap(tileB, tileA);
        }
    }
    
    void HandleVerticalWrap(RectTransform tile, RectTransform other)
    {
        float dir = Mathf.Sign(speedPixelsPerSecond.y);
        float startY = GetLineYLocal(wrapStart);
        float endY   = GetLineYLocal(wrapEnd);
        
        float tileTop    = GetTopLocal(tile);
        float tileBottom = GetBottomLocal(tile);

        if (dir < 0f) 
        {
            if (tileTop <= endY)
            {
          
                float shift = startY - tileBottom;
                tile.anchoredPosition += new Vector2(0f, shift);
            }
        }
        else 
        {
            if (tileBottom >= endY)
            {
                float shift = startY - tileTop;
                tile.anchoredPosition += new Vector2(0f, shift);
            }
        }
    }
    void HandleHorizontalWrap(RectTransform tile, RectTransform other)
    {
        float dir = Mathf.Sign(speedPixelsPerSecond.x);
        float startX = GetLineXLocal(wrapStart);
        float endX   = GetLineXLocal(wrapEnd);

        float tileLeft  = GetLeftLocal(tile);
        float tileRight = GetRightLocal(tile);

        if (dir < 0f) 
        {
            if (tileRight <= endX)
            {
                float shift = startX - tileLeft;
                tile.anchoredPosition += new Vector2(shift, 0f);
            }
        }
        else
        {
            if (tileLeft >= endX)
            {
                float shift = startX - tileRight;
                tile.anchoredPosition += new Vector2(shift, 0f);
            }
        }
    }

    float GetLineYLocal(RectTransform rt)
    {
        Vector3 world = rt.TransformPoint(rt.rect.center);
        Vector3 local = parentRect.InverseTransformPoint(world);
        return local.y;
    }

    float GetLineXLocal(RectTransform rt)
    {
        Vector3 world = rt.TransformPoint(rt.rect.center);
        Vector3 local = parentRect.InverseTransformPoint(world);
        return local.x;
    }

    float GetTopLocal(RectTransform rt)
    {
        var c = new Vector3[4];
        rt.GetWorldCorners(c);
        float y0 = parentRect.InverseTransformPoint(c[0]).y;
        float y1 = parentRect.InverseTransformPoint(c[1]).y;
        float y2 = parentRect.InverseTransformPoint(c[2]).y;
        float y3 = parentRect.InverseTransformPoint(c[3]).y;
        return Mathf.Max(y0, y1, y2, y3);
    }

    float GetBottomLocal(RectTransform rt)
    {
        var c = new Vector3[4];
        rt.GetWorldCorners(c);
        float y0 = parentRect.InverseTransformPoint(c[0]).y;
        float y1 = parentRect.InverseTransformPoint(c[1]).y;
        float y2 = parentRect.InverseTransformPoint(c[2]).y;
        float y3 = parentRect.InverseTransformPoint(c[3]).y;
        return Mathf.Min(y0, y1, y2, y3);
    }

    float GetLeftLocal(RectTransform rt)
    {
        var c = new Vector3[4];
        rt.GetWorldCorners(c);
        float x0 = parentRect.InverseTransformPoint(c[0]).x;
        float x1 = parentRect.InverseTransformPoint(c[1]).x;
        float x2 = parentRect.InverseTransformPoint(c[2]).x;
        float x3 = parentRect.InverseTransformPoint(c[3]).x;
        return Mathf.Min(x0, x1, x2, x3);
    }

    float GetRightLocal(RectTransform rt)
    {
        var c = new Vector3[4];
        rt.GetWorldCorners(c);
        float x0 = parentRect.InverseTransformPoint(c[0]).x;
        float x1 = parentRect.InverseTransformPoint(c[1]).x;
        float x2 = parentRect.InverseTransformPoint(c[2]).x;
        float x3 = parentRect.InverseTransformPoint(c[3]).x;
        return Mathf.Max(x0, x1, x2, x3);
    }
}
