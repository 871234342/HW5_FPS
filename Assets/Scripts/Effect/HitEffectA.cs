using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectA : MonoBehaviour
{
    private void OnDestroy()
    {
        //Debug.Log(Time.time);
        Destroy(transform.parent.gameObject);
    }
}
