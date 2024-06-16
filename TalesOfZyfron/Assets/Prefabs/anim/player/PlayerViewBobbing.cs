using UnityEngine;

public class PlayerViewBobbing : MonoBehaviour
{
    public Transform cameraTransform;
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.05f;
    public float midpoint = 0.75f;

    private float timer = 0.0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        ViewBobbing();
    }

    void ViewBobbing()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;

            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                midpoint + translateChange,
                cameraTransform.localPosition.z);
        }
        else
        {
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                midpoint,
                cameraTransform.localPosition.z);
        }
    }
}