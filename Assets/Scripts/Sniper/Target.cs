using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    public bool isMoving {
        set {
            GetComponent<Animator>().SetBool("Moving", value);
        }
    }

    bool facingRight;

    public void Face(int dir) {
        facingRight = dir > 0;
        GetComponent<SpriteRenderer>().flipX = facingRight;
    }

}
