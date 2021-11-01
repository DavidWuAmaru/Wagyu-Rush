using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cow_move : MonoBehaviour
{
    public int satiety;
    public int cow_freeze;
    private Vector3 target;
    public bool is_tagged;
    private GameObject[] all_portal;
    public bool is_portal;
    List<Sprite> sprites;
    public Animator playerAnimator;
    private int direction;
    private Collider2D portal_coll;
    public float step;
    private bool dance_stop;
    private bool transferring;

    void Start()
    {
        satiety = 10;
        cow_freeze = 0;
        target = this.gameObject.transform.position;
        is_tagged = false;
        is_portal = false;

        sprites = new List<Sprite>();
        playerAnimator.enabled = false;
        sprites.Add(Resources.Load<Sprite>("cow_up"));
        sprites.Add(Resources.Load<Sprite>("cow_down"));
        sprites.Add(Resources.Load<Sprite>("cow_left"));
        sprites.Add(Resources.Load<Sprite>("cow_right"));
        direction = 3;
        step = 1f;
        dance_stop = true;
    }

    // Update is called once per frame
    void Update()
    {
        print(playerAnimator.enabled);
        //move
        if (this.gameObject.transform.position == target && cow_freeze == 0 && !transferring)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                target = this.gameObject.transform.position + new Vector3(0, step, 0);
                direction = 0;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                target = this.gameObject.transform.position + new Vector3(0, -step, 0);
                direction = 1;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                target = this.gameObject.transform.position + new Vector3(-step, 0, 0);
                direction = 2;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                target = this.gameObject.transform.position + new Vector3(step, 0, 0);
                direction = 3;
            }
        }
        MoveToTarget();
        //¬O§_³Q¦B­á¡B¦©¹¡­¹«×
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            satiety -= 1;
            if (cow_freeze > 0)
            {
                cow_freeze -= 1;
            }
            else if(cow_freeze == 0 && !dance_stop)
            {
                playerAnimator.enabled = false;
                dance_stop = true;
            }
        }
        if (transferring)
        {
            /*TransferToAnotherPortal();
            transferring = false;
            is_portal = true;*/
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("portal_in"))
            {
                playerAnimator.Play("idle");
                playerAnimator.enabled = false;
                TransferToAnotherPortal();
                playerAnimator.enabled = true;
                playerAnimator.Play("portal_out");
            }
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("portal_out"))
            {
                playerAnimator.Play("idle");
                playerAnimator.enabled = false;
                transferring = false;
                is_portal = true;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.tag == "grass")
        {
            satiety += 5;
        }
        if (coll.gameObject.tag == "trap")
        {
            satiety -= 5;
        }
        if (coll.gameObject.tag == "earphone")
        {
            cow_freeze = 5;
            dance_stop = false;
            Earphone();
        }
        if (coll.gameObject.tag == "tag")
        {
            is_tagged = true;
        }
        if (coll.gameObject.tag == "portal")
        {
            portal_coll = coll;
            Portal();
        }
    }
    void MoveToTarget()
    {
        if (this.gameObject.transform.position != target)
        {
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, 0.1f);
        }
        if (gameObject.GetComponent<SpriteRenderer>().sprite != sprites[direction])
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[direction];
        }
    }
    void Earphone()
    {
        playerAnimator.enabled = true;
        playerAnimator.Play("dance_ani");
    }
    void Portal()
    {
        if (is_portal)
        {
            is_portal = false;
        }
        else
        {
            playerAnimator.enabled = true;
            playerAnimator.Play("portal_in");
            transferring = true;
        }
    }
    void TransferToAnotherPortal()
    {
        int portal_id = portal_coll.GetComponent<portal>().portal_id;
        all_portal = GameObject.FindGameObjectsWithTag("portal");
        for (int i = 0; i < all_portal.Length; i++)
        {
            if (all_portal[i].GetComponent<portal>().portal_id == portal_id && all_portal[i].gameObject.name != portal_coll.gameObject.name)
            {
                this.gameObject.transform.position = all_portal[i].transform.position;
                target = all_portal[i].transform.position;
                is_portal = true;
                break;
            }
        }
    }
}
