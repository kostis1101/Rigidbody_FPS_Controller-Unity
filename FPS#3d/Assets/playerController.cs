using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class playerController : MonoBehaviour
{
    public float normalSpeed = 700f;
    public float sprintSpeed = 7.5f;
    public float sneakSpeed = 400;
    public float sneakStrechingFactor;
    public GameObject antiStrechingObj;
    float speed;
    public float sensitivity = 1f;
    public Camera cam;
    Rigidbody rb;
    public Transform jumpRay;
    public float jumpForce;

    public float maxStepHeight = 0.25f;
    public int stepDetail = 1;
    public LayerMask stepMask;

    public Animator animator;

    public GameObject gunHolder;

    pauseMenu _pauseMenu;

    private void Start()
    {
        _pauseMenu = FindObjectOfType<pauseMenu>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 100;
    }

    void Update()
    {
        Vector2 xMov = new Vector2(Input.GetAxisRaw("Horizontal") * transform.right.x, Input.GetAxisRaw("Horizontal") * transform.right.z);
        Vector2 zMov = new Vector2(Input.GetAxisRaw("Vertical") * transform.forward.x, Input.GetAxisRaw("Vertical") * transform.forward.z);

        if (xMov != Vector2.zero || zMov != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.localScale = new Vector3(1, sneakStrechingFactor, 1);
            antiStrechingObj.transform.localScale = new Vector3(1, 1 / sneakStrechingFactor, 1);
            speed = sneakSpeed;
            animator.speed = 1 / 1.3f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            speed = sprintSpeed;
            animator.speed = 1.3f;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            antiStrechingObj.transform.localScale = new Vector3(1, 1, 1);
            speed = normalSpeed;
            animator.speed = 1;
        }

        //Rotation
        float yRot = Input.GetAxisRaw("Mouse X") * sensitivity;
        rb.rotation *= Quaternion.Euler(0, yRot, 0);

        if (!_pauseMenu.isPaused)
        {
            float xRot = Input.GetAxisRaw("Mouse Y") * sensitivity;
            float x_rot = cam.transform.rotation.eulerAngles.x;
            x_rot -= xRot;

            //cam.transform.rotation = Quaternion.Euler(x_rot, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z);

            float camEulerAngleX = cam.transform.localEulerAngles.x;

            camEulerAngleX -= xRot * sensitivity;

            if (camEulerAngleX < 180)
            {
                camEulerAngleX += 360;
            }

            camEulerAngleX = Mathf.Clamp(camEulerAngleX, 270, 450);

            cam.transform.localEulerAngles = new Vector3(camEulerAngleX, 0, 0);
        }

        //Jumping
        bool isGrounded = Physics.BoxCastAll(jumpRay.transform.position, new Vector3(.5f, .1f, .5f), Vector3.down, Quaternion.identity, .1f).Length > 1;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (!isGrounded)
        {
            animator.SetBool("isWalking", false);
        }

        //scope
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("isScoping", true);
            //animator.SetBool("isSprinting", false);
            animator.speed = 1;
            speed = sneakSpeed;
        }
        else
        {
            animator.SetBool("isScoping", false);
        }

        //shoot
        if (Input.GetMouseButton(0))
        {
            shoot();
        }

        Vector2 velocity = (xMov + zMov).normalized * speed * Time.deltaTime;

        //stairs
        bool isFirstStepCheck = false;
        bool canMove = true;

        for (int i = stepDetail; i >= 1; i--)
        {
            Collider[] c = Physics.OverlapBox(transform.position + new Vector3(0, i * maxStepHeight / stepDetail - transform.localScale.y, 0), new Vector3(1.05f, maxStepHeight / stepDetail / 2, 1.05f), Quaternion.identity, stepMask);

            if (velocity != Vector2.zero)
            {
                if (c.Length > 0 && i == stepDetail)
                {
                    isFirstStepCheck = true;
                    if (!isGrounded)
                    {
                        Debug.Log("stack");
                        canMove = false;
                    }
                }

                if (c.Length > 0 && !isFirstStepCheck)
                {
                    transform.position += new Vector3(0, i * maxStepHeight / stepDetail, 0);
                    break;
                }
            }
        }

        //gunHolder.transform.position += new Vector3(0, -rb.velocity.y, 0);
        if(canMove)
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);
    }

    public void changeSensitivity(float _sensitivity)
    {
        sensitivity = _sensitivity;
    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(jumpRay.transform.position, new Vector3(.5f, .1f, .5f));

        for (int i = stepDetail; i >= 1; i--)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, i * maxStepHeight / stepDetail - transform.localScale.y, 0), new Vector3(1.05f, maxStepHeight / stepDetail, 1.05f));
        }
    }

    void shoot()
    {
        RaycastHit hit;

        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 200f);
        if (hit.collider == null)
            return;
        Rigidbody rbHit = hit.collider.gameObject.GetComponent<Rigidbody>();

        if (rbHit != null)
        {
            rbHit.AddForceAtPosition(cam.transform.forward * 10, hit.point);
        }

        print("shoot");
    }
}
