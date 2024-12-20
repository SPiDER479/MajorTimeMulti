using UnityEngine;
public class StateAI : MonoBehaviour
{
    public float chaseDistance;
    private GameObject player;
    public GameObject mainSlime;
    private void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;
        
        if (Vector3.Distance(mainSlime.transform.position, player.transform.position) < chaseDistance) 
            ChangeStateTo(SlimeState.Chase);
        else
            ChangeStateTo(SlimeState.Idle);
    }
    private void ChangeStateTo(SlimeState state)
    {
        if (!mainSlime) return;
        if (state == mainSlime.GetComponent<SlimeAI>().currentState) return;
        mainSlime.GetComponent<SlimeAI>().currentState = state ;
    }
}

