using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dance : MonoBehaviour
{
    List<Sprite> sprites;
    public Animator playerAnimator;
    private Vector3 target;
    public float step;
    private int[] direction;
    void Start()
    {
        playerAnimator = gameObject.GetComponent<Animator>();
        target = this.gameObject.transform.position;
        step = 1f;
        direction = new int[] { 0, 0 };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            target = this.gameObject.transform.position + new Vector3(0, step, 0);
            direction[0] = 1;
            direction[1] = 0;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            target = this.gameObject.transform.position + new Vector3(0, -step, 0);
            direction[0] = -1;
            direction[1] = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            target = this.gameObject.transform.position + new Vector3(-step, 0, 0);
            direction[0] = 0;
            direction[1] = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            target = this.gameObject.transform.position + new Vector3(step, 0, 0);
            direction[0] = 0;
            direction[1] = 1;
        }

        if (this.gameObject.transform.position != target)
        {
            ani(direction);
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, 0.01f);
            playerAnimator.SetBool("arrive_target", false);
        }
        else
        {
            playerAnimator.SetBool("arrive_target", true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAnimator.SetBool("sleep", true);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerAnimator.SetBool("dance", true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerAnimator.SetBool("dance", false);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            playerAnimator.SetInteger("portal", 1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerAnimator.SetInteger("portal", 2);
        }
    }
    void ani(int[] direction)
    {
        playerAnimator.SetFloat("up_down", direction[0]);
        playerAnimator.SetFloat("left_right", direction[1]);
        playerAnimator.SetBool("sleep", false);
    }
}