using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class _playerController : MonoBehaviour
{

    Rigidbody rb;
    public float speed = 100f;
    public float sensitivity = 2f;
    Camera cam;
    public float maxStepHeigh = 0.25f;
    public float stairDetail = 10;
    public LayerMask stepMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 xMov = new Vector2(Input.GetAxisRaw("Horizontal") * transform.right.x, Input.GetAxisRaw("Horizontal") * transform.right.z);
        Vector2 zMov = new Vector2(Input.GetAxisRaw("Vertical") * transform.forward.x, Input.GetAxisRaw("Vertical") * transform.forward.z);

        Vector2 velocity = (xMov + zMov).normalized * speed * Time.deltaTime;

        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);

        //Rotation
        float yRot = Input.GetAxisRaw("Mouse X") * sensitivity;
        rb.rotation *= Quaternion.Euler(0, yRot, 0);

        float xRot = Input.GetAxisRaw("Mouse Y") * sensitivity;
        float x_rot = cam.transform.localEulerAngles.x;
        x_rot -= xRot;

        float eulerAngleX = cam.fieldOfView;

        //stair
        
    }
}
