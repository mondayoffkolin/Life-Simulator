using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] LayerMask m_doorlayerMask;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 a_mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(a_mousePos);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_doorlayerMask))
            {
                print("Door hit");

                GameObject a_obj = hit.transform.gameObject;
                a_obj.GetComponent<DoorProduction>().DoorOpen();
            }
        }
    }
}
