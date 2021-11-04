using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BaseBtu : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public UnityEvent<string> pointDownEvent;
    public UnityEvent<string> pointUpEvent;
    public UnityEvent<string> drawEvent;
    
    //背景板
    public Transform baseTransform;

    //操作杆
    public Transform handleTransform;

    //操作杆移动半径
    public float maxRadius;

    //是否手指位置呼出（背景板）
    public bool fingerExhale;

    //背景板默认位置（fingerExhale = true 有效）
    protected Vector3 _baseRootPos;

    //手指按下位置
    protected Vector2 _pointDownPos;

    protected bool _canHandle = true;
    //移动向量
    protected Vector2 _dir;
    public Vector2 Dir => (_dir);

    protected virtual void Awake()
    {
        if (baseTransform == null || handleTransform == null)
        {
            //如果没有设置操纵杆或者背景板 则自动获取
            //背景板默认为panel的第一孩子
            baseTransform = transform.GetChild(0);
            //操纵杆默认为背景板的第一个孩子
            handleTransform = baseTransform.GetChild(0);
        }

        if (maxRadius == 0)
        {
            //如果没设置可移动范围，则取操作杆的半径
            maxRadius = handleTransform.gameObject.GetComponent<RectTransform>().rect.width;
        }

        _baseRootPos = baseTransform.position;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (fingerExhale)
        {
            baseTransform.position = _pointDownPos = eventData.position;
            pointDownEvent?.Invoke(baseTransform.name);
            return;
        }
        else
        {
            if (!FingerExhaleCheck(eventData))
            {
                _canHandle = false;
            }
        }

        _pointDownPos = eventData.position;
        pointDownEvent?.Invoke(baseTransform.name);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_canHandle) return;
        var dis = eventData.position - _pointDownPos;
        var clamp = Mathf.Clamp(dis.magnitude, 0f, maxRadius);
        var normalized = clamp * dis.normalized;
        handleTransform.localPosition = normalized;
        _dir = normalized.normalized;
        drawEvent?.Invoke(baseTransform.name);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!_canHandle)
        {
            _canHandle = true;
            return;
        }
        handleTransform.localPosition = Vector3.zero;
        _dir = Vector2.zero;
        if (fingerExhale) baseTransform.position = _baseRootPos;
        pointUpEvent?.Invoke(baseTransform.name);
    }

    protected virtual bool FingerExhaleCheck(PointerEventData eventData)
    {
        return (eventData.pointerCurrentRaycast.gameObject.name == handleTransform.name);
    }
}