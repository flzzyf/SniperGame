using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    //public float speed = 3;

    //private void Start() {
    //    targetPos = transform.position;
    //}

    void Update() {
        //isMoving = (targetPos != (Vector2)transform.position);
        GetComponent<Animator>().SetBool("Moving", isMoving);

        //if (isMoving) {
        //    MoveTo(targetPos);

        //    //Face(targetPos);
        //} else {
        //    StartCoroutine(DelaySetNextTarget());
        //}
    }

    //Vector2 targetPos;

    public bool isMoving;

    //void MoveTo(Vector2 pos) {
    //    if(Vector2.Distance(transform.position, pos) > speed * Time.fixedDeltaTime) {
    //        Vector2 dir = (pos - (Vector2)transform.position).normalized;

    //        transform.Translate(dir * speed * Time.fixedDeltaTime);
    //    } else {
    //        transform.position = pos;
    //    }
    //}

    bool facingRight;
    public void Face(Vector2 pos) {
        facingRight = pos.x > transform.position.x;
        GetComponent<SpriteRenderer>().flipX = facingRight;
    }

    //bool isWaiting;

    //IEnumerator DelaySetNextTarget() {
    //    if (!isWaiting) {
    //        isWaiting = true;

    //        yield return new WaitForSeconds(Random.Range(1, 3));

    //        isWaiting = false;

    //        int sign = Random.Range(0, 2) == 0 ? 1 : -1;
    //        targetPos = Vector2.right * Random.Range(1, 3) * sign;

    //        Face(targetPos);
    //    }
    //}
}
