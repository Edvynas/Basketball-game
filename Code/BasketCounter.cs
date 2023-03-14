using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BasketCounter : MonoBehaviour
{
    public int score = 0;
    [SerializeField] TMP_Text scoreText;
    public GameObject ballPrefab; // Reference to the ball prefab
    public Collider platformCollider; // Reference to the platform collider

    private float platformWidth;  // Width of the platform
    private float platformHeight;  // Height of the platform

    void Start()
    {
        // Get the platform width and height from its collider bounds
        platformWidth = platformCollider.bounds.size.x;
        platformHeight = platformCollider.bounds.size.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Get the position of the ball and hoop
            Vector3 ballPos = other.transform.position;
            Vector3 hoopPos = transform.position;

            // Check if the ball is above the hoop
            if (ballPos.y > hoopPos.y)
            {
                Debug.Log("Basket made");
                score++;
                scoreText.text = "Score: " + score.ToString();

                TeleportBall(other.gameObject);
            }
        }
    }

    private void TeleportBall(GameObject ball)
    {
        float halfBallWidth = ball.GetComponent<Collider>().bounds.size.x / 2f;
        float halfBallHeight = ball.GetComponent<Collider>().bounds.size.z / 2f;
        float platformLeftEdge = platformCollider.bounds.center.x - platformWidth / 2f;
        float platformRightEdge = platformCollider.bounds.center.x + platformWidth / 2f;
        float platformTopEdge = platformCollider.bounds.center.z + platformHeight / 2f;
        float platformBottomEdge = platformCollider.bounds.center.z - platformHeight / 2f;

        // Generate a random position within the platform's bounds
        float randomX = Random.Range(platformLeftEdge + halfBallWidth, platformRightEdge - halfBallWidth);
        float randomY = Random.Range(platformBottomEdge + halfBallHeight, platformTopEdge - halfBallHeight);

        // Teleport the ball to the random position
        ball.transform.position = new Vector3(randomX, ball.transform.position.y, randomY);
    }
}
