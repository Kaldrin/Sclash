using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Button : MonoBehaviour {

    //Visual
    [SerializeField]
    bool
        spriteSwap = true,
        animSwap = false;
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Animator animator;
    [SerializeField]
    Sprite
        idle,
        over,
        active;



    //Functional
    [SerializeField]
    KeyCode[] activationKeys;
    [SerializeField]
    MonoBehaviour[] scripts;
    [SerializeField]
    string[] functionsNames;
    [SerializeField]
    float callDelay = 0.1f;


    string selectionState = "Idle";


	// Use this for initialization
	void Start () {
        if (spriteSwap)
            spriteRenderer.sprite = idle;
	}
	
	// Update is called once per frame
	void Update () {
        DetectKeys();

    }


    void DetectKeys()
    {
        for (int i = 0; i < activationKeys.Length; i++)
        {
            if (Input.GetKeyDown(activationKeys[i]))
            {
                callFunctions();
            }
        }
    }


    //Mouse
    private void OnMouseOver()
    {
        if (enabled)
        {
            if (selectionState == "Idle")
            {
                if (spriteSwap)
                    spriteRenderer.sprite = over;

                if (animSwap)
                {
                    animator.SetBool("Over", true);
                }


                selectionState = "Over";
            }
        }
    }

    private void OnMouseExit()
    {
        if (enabled)
        {
            if (selectionState == "Over")
            {
                if (spriteSwap)
                    spriteRenderer.sprite = idle;

                if (animSwap)
                {
                    animator.SetBool("Over", false);
                }



                selectionState = "Idle";
            }
        }
    }

    private void OnMouseDown()
    {
        if (enabled)
        {
            if (selectionState == "Over")
            {
                if (spriteSwap)
                    spriteRenderer.sprite = active;

                if (animSwap)
                {
                    animator.SetBool("Active", true);
                }


                selectionState = "Active";
            }
        }
    }

    private void OnMouseUp()
    {
        if (enabled)
        {
            if (selectionState == "Active")
            {
                if (spriteSwap)
                    spriteRenderer.sprite = idle;
                if (animSwap)
                {
                    animator.SetBool("Active", false);
                    animator.SetBool("Over", false);
                }


                selectionState = "Idle";

                callFunctions();
            }
        }
    }



    //Scripts
    void callFunctions()
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            if (functionsNames.Length >= i)
            {
                scripts[i].Invoke(functionsNames[i], callDelay);
            }
        }
    }
}
