using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballController : MonoBehaviour {

    public float MoveSpeed = 10;
    public Transform Ball;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;
    public Collider platformCollider;
    private float platformWidth;  // Width of the platform
    private float platformHeight;  // Height of the platform

    // variables
    private bool IsBallInHands = true;
    private bool IsBallFlying = false;
    private float T = 0;

    void Start()
    {
        // Get the platform width from its collider bounds
        platformWidth = platformCollider.bounds.size.x;
        platformHeight = platformCollider.bounds.size.z;
    }

    Vector3 GetRandomPlatformPosition()
    {
        // Get the bounds of the platform
        Vector3 platformCenter = platformCollider.bounds.center;
        float platformWidth = platformCollider.bounds.size.x;
        float platformHeight = platformCollider.bounds.size.z;

        // Generate a random position on the platform
        float x = Random.Range(platformCenter.x - platformWidth / 2f, platformCenter.x + platformWidth / 2f);
        float z = Random.Range(platformCenter.z - platformHeight / 2f, platformCenter.z + platformHeight / 2f);

        return new Vector3(x, transform.position.y, z);
    }
    // Update is called once per frame
    void Update() {

        // walking
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);
        // Check if the player is about to fall off the platform
        float playerX = transform.position.x;
        float playerY = transform.position.z;
        float halfPlayerWidth = GetComponent<Collider>().bounds.size.x / 2f;
        float halfPlayerHeight = GetComponent<Collider>().bounds.size.z / 2f;
        float platformLeftEdge = platformCollider.bounds.center.x - platformWidth / 2f;
        float platformRightEdge = platformCollider.bounds.center.x + platformWidth / 2f;
        float platformTopEdge = platformCollider.bounds.center.z + platformHeight / 2f;
        float platformBottomEdge = platformCollider.bounds.center.z - platformHeight / 2f;

        // Prevent player from moving off the left or right edges of the platform
        if (playerX - halfPlayerWidth < platformLeftEdge)
        {
            transform.position = new Vector3(platformLeftEdge + halfPlayerWidth, transform.position.y, transform.position.z);
        }
        else if (playerX + halfPlayerWidth > platformRightEdge)
        {
            transform.position = new Vector3(platformRightEdge - halfPlayerWidth, transform.position.y, transform.position.z);
        }

        // Prevent player from moving off the top or bottom edges of the platform
        if (playerY + halfPlayerHeight > platformTopEdge)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, platformTopEdge - halfPlayerHeight);
        }
        else if (playerY - halfPlayerHeight < platformBottomEdge)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, platformBottomEdge + halfPlayerHeight);
        }

        


        // ball in hands
        if (IsBallInHands) {

            // hold over head
            if (Input.GetKey(KeyCode.Space)) {
                Ball.position = PosOverHead.position;
                Arms.localEulerAngles = Vector3.right * 180;

                // look towards the target
                transform.LookAt(Target.parent.position);
            }

            // dribbling
            else {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }

            // throw ball
            if (Input.GetKeyUp(KeyCode.Space)) {
                IsBallInHands = false;
                IsBallFlying = true;
                T = 0;
            }
        }

        // ball in the air
        if (IsBallFlying) {
            T += Time.deltaTime;
            float duration = 0.66f;
            float t01 = T / duration;

            // move to target
            Vector3 A = PosOverHead.position;
            Vector3 B = Target.position;
            Vector3 pos = Vector3.Lerp(A, B, t01);

            // move in arc
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            Ball.position = pos + arc;

            // moment when ball arrives at the target
            if (t01 >= 1) {
                IsBallFlying = false;
                Ball.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

       
    }

    private void OnTriggerEnter(Collider other) {

        if (!IsBallInHands && !IsBallFlying) {

            IsBallInHands = true;
            Ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
