using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//瞄准圈
public class AimCircle : MonoBehaviour {

    //设置半径
    public void SetRadius(float radius) {
        transform.localScale = Vector3.one * radius * 2;
    }

    public void SetColor(Color color, float duration = .3f) {
        GetComponent<SpriteRenderer>().DOColor(color, duration);
    }

    public void MoveTo(Vector2 pos) {
        transform.DOKill();
        transform.DOMove(pos, .2f);
    }

}

