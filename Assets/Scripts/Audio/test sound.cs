//will delete this script after testing - Marissa

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testsound : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SfxManager.Instance.PlaySFX("test");
        }
    }
}
