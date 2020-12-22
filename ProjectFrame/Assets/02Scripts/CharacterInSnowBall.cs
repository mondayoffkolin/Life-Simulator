using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInSnowBall : MonoBehaviour
{
    [SerializeField] private Transform m_originTf = null;

    private Rigidbody m_rigid = null;


    private void Awake()
    {
        m_originTf = this.transform;

        m_rigid = this.GetComponent<Rigidbody>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void CharacterAddForce()
    {
        if (m_rigid != null)
            m_rigid.isKinematic = false;
        else
            print("CharacterSnowBall에 Rigidbody가 없다.");


        Vector3 a_vec = ReturnForceVec();

        m_rigid.AddForce(a_vec, ForceMode.VelocityChange);

        StartCoroutine(ResetPos());
    }

    private Vector3 ReturnForceVec()
    {
        float a_x = Random.Range(-50, 50);
        float a_y = Random.Range(40, 50);
        float a_z = Random.Range(-50, 50);

        return new Vector3(a_x, a_y, a_z);
    }

    private IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(1.5f);

        m_rigid.isKinematic = true;
        
        this.transform.position = m_originTf.position;
        this.transform.rotation = m_originTf.rotation;

        this.gameObject.SetActive(false);
    }
}
