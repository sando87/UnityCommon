using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 Offset = new Vector3(0, 0, -10);

    private Camera mCamera = null;
    private GameObject mTarget = null;
    private int mIsOutOfControl = 0; // 외부나 다른 기능에 의해 카메라가 제어되는 중일경우(예로 보스가 죽었을때 슬로우 효과시)
    private Vector3 mLocalOriginPosCamera = Vector3.zero;

    public Camera Camera { get { return mCamera; } }
    public bool IsOutOfControl { get { return mIsOutOfControl > 0; } }
    public float CurrentHeight { get { return mCamera.orthographicSize * 2.0f; } }
    public float CurrentWidth { get { return CurrentHeight * mCamera.aspect; } }
    public float ToHeight(float width) { return width / mCamera.aspect; }
    public Rect CameraArea
    {
        get
        {
            Rect area = new Rect();
            area.size = new Vector2(CurrentWidth, CurrentHeight);
            area.center = new Vector2(transform.position.x, transform.position.y);
            return area;
        }
    }
    public Rect LimitArea { get; set; }

    void Start()
    {
        // PixelPerfectCamera ppc = GetComponentInChildren<PixelPerfectCamera>();
        // ppc.refResolutionX = (int)(ppc.refResolutionY * (Screen.width / (double)Screen.height));

        mCamera = GetComponentInChildren<Camera>();
        mLocalOriginPosCamera = mCamera.transform.localPosition;
    }

    void LateUpdate()
    {
        if(IsOutOfControl)
            return;

        if(mTarget != null)
        {
            transform.position = mTarget.transform.position + Offset;
            LimitCameraMovement();
            transform.Snap();
            // transform.LookAt(Target.transform);
        }
    }

    public void SetTarget(GameObject target)
    {
        mTarget = target;
    }
    private void LimitCameraMovement()
    {
        Rect limitedArea = CameraArea.LimitRectMovement(LimitArea);
        transform.ExSetPosition2D(limitedArea.center);
    }
    public void ShakeCamera(float strength)
    {
        float duration = strength * 0.5f;
        mCamera.transform.DOKill();
        mCamera.transform.localPosition = mLocalOriginPosCamera;
        mCamera.transform.DOShakePosition(duration, strength).SetEase(Ease.OutQuart);
    }
    public void VibrateCamera(float duration, float strength)
    {
        mCamera.transform.DOKill();
        mCamera.transform.localPosition = mLocalOriginPosCamera;
        mCamera.transform.DOShakePosition(duration, strength, 90, 0, false, true);
    }
    // public void StartFocusZoomming(BaseObject target, float rate, float durtaion)
    // {
    //     StartCoroutine(CoZoommingFocus(target, rate, durtaion));
    // }
    // IEnumerator CoZoommingFocus(BaseObject target, float rate, float duration)
    // {
    //     mIsOutOfControl++;
    //     float halfDuration = duration * 0.5f;
    //     float orthoSize = mCamera.orthographicSize;
    //     //mCamera.GetComponent<PixelPerfectCamera>().enabled = false;

    //     mCamera.DOOrthoSize(orthoSize * rate, halfDuration).SetEase(Ease.OutQuart);
    //     transform.DOMove(target.transform.position + Offset, halfDuration).SetEase(Ease.OutQuart);
    //     yield return new WaitForSeconds(halfDuration);
        
    //     mCamera.DOOrthoSize(orthoSize, halfDuration).SetEase(Ease.InOutQuad);
    //     transform.DOMove(mTarget.transform.position + Offset, halfDuration).SetEase(Ease.InOutQuad);
    //     yield return new WaitForSeconds(halfDuration);

    //     mCamera.DOKill();
    //     transform.DOKill();
    //     mCamera.orthographicSize = orthoSize;
    //     transform.position = mTarget.transform.position + Offset;
    //     //mCamera.GetComponent<PixelPerfectCamera>().enabled = true;
    //     mIsOutOfControl--;
    // }

}
