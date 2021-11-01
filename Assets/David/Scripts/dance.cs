using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dance : MonoBehaviour
{
    List<Sprite> sprites;
    public Animator playerAnimator;
    private int direction;
    void Start()
    {
        sprites = new List<Sprite>();
        playerAnimator.enabled = false;
        sprites.Add(Resources.Load<Sprite>("cow_up"));
        sprites.Add(Resources.Load<Sprite>("cow_down"));
        sprites.Add(Resources.Load<Sprite>("cow_left"));
        sprites.Add(Resources.Load<Sprite>("cow_right"));
    }

    // Update is called once per frame
    void Update()
    {
        print(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("portal_in"))
        {
            //print("haha");
            playerAnimator.enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAnimator.enabled = true;
            playerAnimator.Play("portal_in");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerAnimator.enabled = true;
            playerAnimator.Play("portal_out");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            direction = 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            direction = 2;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = 3;
        }
        if (gameObject.GetComponent<SpriteRenderer>().sprite != sprites[direction])
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[direction];
        }
    }
}