using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInSnowBall : MonoBehaviour
{
    [Header("눈덩이에 박힌 Character 원래 자기 위치")]
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


    /// <summary>
    /// 눈덩이 터질시 Character 터치면서 위치 초기화
    /// </summary>
    public void CharacterAddForce()
    {
        if (m_rigid != null)
            m_rigid.isKinematic = false;
        else
            print("CharacterSnowBall에 Rigidbody가 없다.");


        Vector3 a_vec = ReturnForceVec();

        m_rigid.AddForce(a_vec, ForceMode.Impulse);
        //m_rigid.AddExplosionForce(1000f, this.transform.position, 10, 300);

        StartCoroutine(ResetPos());
    }

    /// <summary>
    /// 터치는 방향 벡터 반환 함수
    /// </summary>
    /// <returns></returns>
    private Vector3 ReturnForceVec()
    {
        float a_x = Random.Range(-100, 100);
        float a_y = Random.Range(40, 100);
        float a_z = Random.Range(-100, 100);

        return new Vector3(a_x, a_y, a_z);
    }

    /// <summary>
    /// 눈덩이에 박힌 Character 위치 초기화 코루틴함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetPos()
    {
        yield return new WaitForSeconds(1.5f);

        m_rigid.isKinematic = true;
        
        this.transform.position = m_originTf.position;
        this.transform.rotation = m_originTf.rotation;

        this.gameObject.SetActive(false);
    }
}
