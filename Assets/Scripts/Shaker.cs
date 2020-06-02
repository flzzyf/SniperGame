using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : Singleton<Shaker> {
    //开始晃动（幅度，频率，持续时间）
    public static void Shake(Transform target, float magnitude, float frequency, float duration) {
        instance.RealShake(target, magnitude, frequency, duration);
    }

    void RealShake(Transform target, float magnitude, float frequency, float duration) {
        StartCoroutine(ShakeCor(target, magnitude, frequency, duration));
    }

    IEnumerator ShakeCor(Transform target, float magnitude, float frequency, float duration) {
        Vector3 offset = Vector3.zero;

        Vector3 previousPos = target.position;

        for (float t = 0; t < duration; t += frequency) {
            offset.x = UnityEngine.Random.Range(-1, 1f) * magnitude;
            offset.y = UnityEngine.Random.Range(-1, 1f) * magnitude;

            target.position = previousPos + offset;

            yield return new WaitForSeconds(frequency);
        }

        target.position = previousPos;
    }
}
