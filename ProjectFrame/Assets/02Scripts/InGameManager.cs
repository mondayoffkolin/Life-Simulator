using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager uniqueInstance;

    public enum eGameStage
    {
        Ready,
        Start,
        Play,
        End,

        None
    }

    public static eGameStage m_curGameState;

    // Start is called before the first frame update
    void Start()
    {
        uniqueInstance = this;

        m_curGameState = eGameStage.Ready;
    }
}
