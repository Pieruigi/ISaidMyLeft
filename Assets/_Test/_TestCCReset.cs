using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _TestCCReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) { ResetPlayer(); }
    }

    void ResetPlayer()
    {
        var cc = GetComponent<CharacterController>();
        cc.transform.position = Vector3.zero;   
        cc.transform.rotation = Quaternion.identity;
    }
}
