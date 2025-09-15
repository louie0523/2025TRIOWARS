using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class LeaderManager : MonoBehaviour
{
    public static LeaderManager instance;

    public List<Unit> units = new List<Unit>();
    public Unit currentLeaderUnit;
    public int currentLeaderIndex;
    public CinemachineVirtualCamera cam;
    public List<Vector3> UnitOffset = new List<Vector3>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetPlayersLeader();
    }

    public void SetPlayersLeader()
    {
        if(units.Count > 0)
        {

        } else
        {
            Unit[] temp = GameObject.FindObjectsOfType<Unit>();
            foreach(Unit unit in temp)
            {
                units.Add(unit);
            }
            units.Sort((x, y) => x.unitData.playerType.CompareTo(y.unitData.playerType));
            ChangeLeader(currentLeaderIndex);

        }
    }

    public void ChangeLeader(int index)
    {
        if(units.Count <= 0)
        {
            return;
        }

        if(currentLeaderUnit != null)
        {
            LeaderController leaderController = currentLeaderUnit.GetComponent<LeaderController>();
            CharacterController characterController = currentLeaderUnit.GetComponent<CharacterController>();
            Destroy(leaderController);
            Destroy(characterController);
        }

        currentLeaderIndex = index;

        currentLeaderUnit = units[index];


        //cam.Follow = currentLeaderUnit.transform;
        //cam.LookAt = currentLeaderUnit.transform;

        currentLeaderUnit.AddComponent<LeaderController>();
        CharacterController cc =  currentLeaderUnit.AddComponent<CharacterController>();
        cc.center = new Vector3(0, 1, 0);

        int count = 0;
        for(int i = 0; i < units.Count; i++)
        {
            if(i == currentLeaderIndex)
            {
                units[i].Leader = null;
                units[i].navMeshAgent.isStopped = true;
                units[i].LeaderOffset = Vector3.zero;
            } else
            {
                units[i].Leader = currentLeaderUnit.transform;
                units[i].navMeshAgent.isStopped = false;
                units[i].LeaderOffset = UnitOffset[count];
                count++;
            }
        }
    }


} 
