using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AreaController : MonoBehaviour
{
    public enum AreaEditMode
    {
        None = 0,
        ResizeRight = 1,
        ResizeLeft = 2,
        ResizeTop = 4,
        ResizeBottom = 8,
        Move = 16,
    }

    [SerializeField] BoxCollider EdgeLeft = null;
    [SerializeField] BoxCollider EdgeRight = null;
    [SerializeField] BoxCollider EdgeTop = null;
    [SerializeField] BoxCollider EdgeBottom = null;
    [SerializeField] BoxCollider EdgeCenter = null;

    [SerializeField] Texture2D CursorEdge = null;
    [SerializeField] Texture2D CursorMove = null;

    private AreaEditMode mCurrentAreaEditMode = AreaEditMode.None;
    private bool mIsDragging = false;
    private Camera mCamera = null;
    private Vector2 mLeftBottomCornerPosition = new Vector2(-0.5f, -0.5f);
    private Vector2 mSize = Vector2.one;
    private System.Action<Rect> mEventRectEdit = null;

    private float Thickness { get { return mCamera.orthographicSize * 0.05f; } }

    public Rect Rect { get { return new Rect(mLeftBottomCornerPosition, mSize); } }

    public void Init(Rect rect, System.Action<Rect> eventRectEdit)
    {
        mLeftBottomCornerPosition = rect.min;
        mSize = rect.size;
        mEventRectEdit = eventRectEdit;
    }

    // Start is called before the first frame update
    void Start()
    {
        mCamera = Camera.main;
        
        UpdateTranform();
        UpdateCurrentEditMode();
        UpdateCursor();
    }

    // Update is called once per frame
    void Update()
    {
        DetectMouseClickDown();

        UpdateTranform();
        UpdateCurrentEditMode();
        // UpdateCursor();
    }

    private void UpdateTranform()
    {
        Vector3 centerPos = mLeftBottomCornerPosition + (mSize * 0.5f);
        transform.position = centerPos;
        EdgeCenter.transform.localScale = new Vector3(mSize.x, mSize.y, 1);
        EdgeCenter.transform.position = centerPos;
        EdgeRight.transform.localScale = new Vector3(Thickness, mSize.y + Thickness, 1);
        EdgeRight.transform.position = EdgeCenter.bounds.ExRight();
        EdgeLeft.transform.localScale = new Vector3(Thickness, mSize.y + Thickness, 1);
        EdgeLeft.transform.position = EdgeCenter.bounds.ExLeft();
        EdgeTop.transform.localScale = new Vector3(mSize.x + Thickness, Thickness, 1);
        EdgeTop.transform.position = EdgeCenter.bounds.ExTop();
        EdgeBottom.transform.localScale = new Vector3(mSize.x + Thickness, Thickness, 1);
        EdgeBottom.transform.position = EdgeCenter.bounds.ExBottom();
    }
    private void UpdateCurrentEditMode()
    {
        if(mIsDragging) return;
        mCurrentAreaEditMode = AreaEditMode.None;

        float radius = 0.1f;
        Vector3 mouseWorldPos = mCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Collider[] cols = Physics.OverlapSphere(mouseWorldPos, radius);
        foreach (Collider col in cols)
        {
            if (col == EdgeRight) mCurrentAreaEditMode |= AreaEditMode.ResizeRight;
            else if (col == EdgeLeft) mCurrentAreaEditMode |= AreaEditMode.ResizeLeft;
            else if (col == EdgeTop) mCurrentAreaEditMode |= AreaEditMode.ResizeTop;
            else if (col == EdgeBottom) mCurrentAreaEditMode |= AreaEditMode.ResizeBottom;
            else if (col == EdgeCenter) mCurrentAreaEditMode |= AreaEditMode.Move;
        }
    }
    private void UpdateCursor()
    {
        if (mIsDragging) return;

        if(mCurrentAreaEditMode == AreaEditMode.None)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        }
        else if (mCurrentAreaEditMode == AreaEditMode.Move)
        {
            Cursor.SetCursor(CursorMove, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(CursorEdge, Vector2.zero, CursorMode.ForceSoftware);
        }
    }

    // NewInputSystem : 마우스 클릭시마다 호출됨(드래깅 시작시 사용)
    private void DetectMouseClickDown()
    {
        if (mCurrentAreaEditMode == AreaEditMode.None) return;

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(CoMouseDragging());
        }
    }

    IEnumerator CoMouseDragging()
    {
        mIsDragging = true;
        Vector2 startPos = mLeftBottomCornerPosition;
        Vector2 startSize = mSize;
        Vector3 clickDownPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        // 드래깅중일때까지 while루프 안에서 동작
        while (Mouse.current.leftButton.ReadValue() != 0)
        {
            Vector2 deltaPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - clickDownPos;
            if((mCurrentAreaEditMode & AreaEditMode.ResizeRight) > 0)
            {
                mSize.x = startSize.x + deltaPos.x;
                mSize.x = Mathf.Max(mSize.x, 1);
            }
            else if ((mCurrentAreaEditMode & AreaEditMode.ResizeLeft) > 0)
            {
                mSize.x = startSize.x - deltaPos.x;
                if (mSize.x < 1)
                {
                    mSize.x = 1;
                }
                else
                {
                    mLeftBottomCornerPosition.x = startPos.x + deltaPos.x;
                }
            }
            
            if ((mCurrentAreaEditMode & AreaEditMode.ResizeTop) > 0)
            {
                mSize.y = startSize.y + deltaPos.y;
                mSize.y = Mathf.Max(mSize.y, 1);
            }
            else if ((mCurrentAreaEditMode & AreaEditMode.ResizeBottom) > 0)
            {
                mSize.y = startSize.y - deltaPos.y;
                if (mSize.y < 1)
                {
                    mSize.y = 1;
                }
                else
                {
                    mLeftBottomCornerPosition.y = startPos.y + deltaPos.y;
                }
            }

            if (mCurrentAreaEditMode == AreaEditMode.Move)
            {
                mLeftBottomCornerPosition = startPos + deltaPos;
            }

            yield return null;
        }
        mEventRectEdit?.Invoke(Rect);
        mIsDragging = false;
    }
}
