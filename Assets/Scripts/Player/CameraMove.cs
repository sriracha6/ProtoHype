using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using PawnFunctions;

// when you start, we have to place the camera in the middle of the screen so it's not shit
public class CameraMove : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    float minFov = 15f;
    [SerializeField]
    float maxFov = 90f;
    [SerializeField] float moveSpeed = 25f;

    [Header("Components")]
    [SerializeField] CinemachineVirtualCamera thecam;
    [SerializeField] GameObject camObject;
    [SerializeField] PolygonCollider2D bounds;
    [SerializeField] Camera mainCam;

    Vector3 movement;
    public static bool isFollowing = false;
    Vector2 lastMousePos;
    static CinemachineVirtualCamera _thecam;

    protected void Awake()
    {
        camObject.transform.position = new Vector3(MapGenerator.mapW/2,MapGenerator.mapH/2,-10); // position the camera in middle of scene
        _thecam = thecam; // >:(
    }

    protected void Start()                                  
    {
        resizeBounds(MapGenerator.mapW,MapGenerator.mapH);
    }

    public void resizeBounds(int width, int height)
    {
        bounds.gameObject.transform.localScale = new Vector2(width, height);
        bounds.gameObject.transform.position = Vector2.zero;
    }

    void Update()
    {
        if (isFollowing)
        {
            checkMouse();
            if (Input.GetKeyDown(KeyCode.Escape)) // todo: keybinds (maybe)
            {
                isFollowing = false;
                thecam.Follow = transform;
            }
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        checkMouse();

        if (Input.GetButtonDown("FasterCam"))
            moveSpeed = 40f;
        if (Input.GetButtonUp("FasterCam"))
            moveSpeed = 32.5f;

        Vector3 pos = mainCam.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.1f, 0.9f);
        pos.y = Mathf.Clamp(pos.y, 0.1f, 0.9f);
        transform.position = mainCam.ViewportToWorldPoint(pos);

        camObject.transform.Translate(moveSpeed * Time.unscaledDeltaTime * movement);
    }


    private void LateUpdate()
    {
        if(Input.GetMouseButtonDown(2)) // todo keybinds
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(Input.mousePosition),
                mainCam.transform.position - mainCam.ScreenToWorldPoint(Input.mousePosition), 10);

            if(hit.transform.GetComponent<Pawn>() != null)
                follow(hit.transform.gameObject);
        }
    }

    public static void follow(GameObject follow)
    {
        _thecam.Follow = follow.transform;
        isFollowing = true;
    }

    private void checkMouse()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (lastMousePos == (Vector2)Input.mousePosition)
                ZoomOrthoCamera(mainCam.ScreenToWorldPoint(lastMousePos), 1);
            else
            {
                lastMousePos = Input.mousePosition;
                ZoomOrthoCamera(mainCam.ScreenToWorldPoint(lastMousePos), 1);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (lastMousePos == (Vector2)Input.mousePosition)
                ZoomOrthoCamera(mainCam.ScreenToWorldPoint(Input.mousePosition), -1);
            else
            {
                lastMousePos = Input.mousePosition;
                ZoomOrthoCamera(mainCam.ScreenToWorldPoint(Input.mousePosition), -1);
            }
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public void MoveTo(Vector2 pos) // todo : find a way to use this
    {
        camObject.transform.position = pos;
    }

    // Ortographic camera zoom towards a point (in world coordinates). Negative amount zooms in, positive zooms out
    // TODO: when reaching zoom limits, stop camera movement as well
    void ZoomOrthoCamera(Vector3 zoomTowards, float amount)
    {
        if (thecam.m_Lens.OrthographicSize <= minFov && amount > 0)
            return;
        // Calculate how much we will have to move towards the zoomTowards position
        float multiplier = (1.0f / thecam.m_Lens.OrthographicSize * amount);

        transform.position += (zoomTowards - transform.position) * multiplier;
        transform.position = new Vector3(transform.position.x,transform.position.y,-10f);
        
        thecam.m_Lens.OrthographicSize -= amount;
        
        thecam.m_Lens.OrthographicSize = Mathf.Clamp(thecam.m_Lens.OrthographicSize, minFov, maxFov);
    }
}