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
    public float zoomChangeAmount;

    [Header("Components")]
    [SerializeField] CinemachineVirtualCamera thecam;
    [SerializeField] GameObject camObject;
    [SerializeField] PolygonCollider2D bounds;
    [SerializeField] Camera mainCam;

    public static CameraMove I;

    Vector3 movement;
    public bool isFollowing = false;
    Vector2 lastMousePos;
    public Transform camtransform;
    [HideInInspector] public bool canMove;

    protected void Awake()
    {
        if (I == null)
            I = this;
        camObject.transform.position = new Vector3(MapGenerator.I.mapWidth/2,MapGenerator.I.mapHeight /2,-10); // position the camera in middle of scene

        thecam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = false;
        camtransform = transform;
    }

    public void resizeBounds(int width, int height)
    {
        bounds.gameObject.transform.localScale = new Vector2((width*2)-1, (height*2)-1);
        bounds.gameObject.transform.position = new Vector2(1,1);
        // we also need to relimit max fov so its not out of bounds somefucking how.
        maxFov = width / 5 + (width/50); // this was my first guess and it's pretty fuckin spot on
    }

    void Update()
    {
        if (isFollowing)
        {
            checkMouse();
            if (Input.GetKeyDown(Keybinds.Escape))
                unfollow();
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        checkMouse();

        if (Input.GetKeyDown(Keybinds.FasterCam))
            moveSpeed *= 1.25f;
        if (Input.GetKeyUp(Keybinds.FasterCam))
            moveSpeed /= 1.25f;

        Vector3 pos = mainCam.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.1f, 0.9f);
        pos.y = Mathf.Clamp(pos.y, 0.1f, 0.9f);
        
        if (canMove)
        {
            transform.position = mainCam.ViewportToWorldPoint(pos);
            transform.Translate(moveSpeed * Time.unscaledDeltaTime * movement);
        }
    }


    private void LateUpdate()
    {
        if(Input.GetMouseButtonDown(Keybinds.MiddleMouse))
        {
            var find = PawnManager.GetAll().Find(p => p.thisPawnMouseOver);
            if (find != null)
                follow(find.gameObject);
            else
                unfollow();
        }
    }

    public void follow(GameObject follow)
    {
        I.thecam.Follow = follow.transform;
        I.isFollowing = true;
    }

    public void unfollow()
    {
        I.isFollowing = false;
        I.thecam.Follow = I.camObject.transform;
    }

    private void checkMouse()
    {
        if (!Input.GetKey(Keybinds.changeSpeed))
            ZoomOrthoCamera(Input.GetAxis("Mouse ScrollWheel") * zoomChangeAmount);
        else
            moveSpeed += Input.GetAxis("Mouse ScrollWheel") * 10;
    }

    public void MoveTo(Vector2 pos) // todo : find a way to use this
    {
        camObject.transform.position = pos;
    }

    // Ortographic camera zoom towards a point (in world coordinates). Negative amount zooms in, positive zooms out
    void ZoomOrthoCamera(/*Vector3 zoomTowards, */float amount)
    {
        if (thecam.m_Lens.OrthographicSize <= minFov && amount > 0)
            return;
        if (UIManager.mouseOverUI)
            return;
        // Calculate how much we will have to move towards the zoomTowards position
        //float multiplier = (1.0f / thecam.m_Lens.OrthographicSize * amount);

        //thecam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset += (zoomTowards - transform.position) * multiplier;
        
        thecam.m_Lens.OrthographicSize -= amount;
        
        thecam.m_Lens.OrthographicSize = Mathf.Clamp(thecam.m_Lens.OrthographicSize, minFov, maxFov);

        canMove = thecam.m_Lens.OrthographicSize != maxFov;
    }

    public void ScreenShake()
    {
        StartCoroutine(nameof(_screenshake));
    }

    private IEnumerator _screenshake()
    {
        thecam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = true;
        yield return new WaitForSecondsRealtime(0.2f);
        thecam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().enabled = false;
    }
}