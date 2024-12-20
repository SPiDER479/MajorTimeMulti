using UnityEngine;
using UnityEngine.AI;
public enum SlimeState { Idle,Walk,Jump,Attack,Damage, Chase}
public class SlimeAI : MonoBehaviour
{
    private Transform player;

    public Face faces;
   
    public GameObject SmileBody;
    public SlimeState currentState; 
   
    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    public int damType;

    private int m_CurrentWaypointIndex;

    private bool move;
    private Material faceMaterial;
    private Vector3 originPos;
    
  
    
    public enum WalkType { Patroll ,ToOrigin, ToPlayer }
    
    
    private WalkType walkType;
    
    [SerializeField]private float speed;
    void Start()
    {
        originPos = transform.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        walkType = WalkType.Patroll;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        agent.speed = speed;
    }
    public void WalkToNextDestination()
    {
        currentState = SlimeState.Walk;
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        SetFace(faces.WalkFace);
    }
    public void CancelGoNextDestination() =>CancelInvoke(nameof(WalkToNextDestination));
    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }
    void Update()
    {
        switch (currentState)
        {
            case SlimeState.Idle:
                // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
                // StopAgent();
                // SetFace(faces.Idleface);
                //agent.isStopped = false;
                //agent.updateRotation = true;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (Random.Range(0.0f, 100.0f) < 0.5f)
                    {
                        agent.SetDestination(new Vector3(transform.position.x + Random.Range(-5f, 5f), 0,
                            transform.position.z + Random.Range(-5f, 5f)));
                    }
                }
                SetFace(agent.velocity == Vector3.zero ? faces.Idleface : faces.WalkFace);
                animator.SetFloat("Speed",agent.velocity.magnitude);
                break;

            case SlimeState.Walk:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;
                agent.isStopped = false;
                agent.updateRotation = true;
                if (walkType == WalkType.ToOrigin)
                {
                    agent.SetDestination(originPos);
                    // Debug.Log("WalkToOrg");
                    SetFace(faces.WalkFace);
                    // agent reaches the destination
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        walkType = WalkType.Patroll;

                        //facing to camera
                        transform.rotation = Quaternion.identity;

                        currentState = SlimeState.Idle;
                    }
                       
                }
                //Patroll
                else
                {
                    if (!waypoints[0]) return;
                     agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
                    // agent reaches the destination
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        currentState = SlimeState.Idle;
                        //wait 2s before go to next destionation
                        Invoke(nameof(WalkToNextDestination), 2f);
                    }

                }
                // set Speed parameter synchronized with agent root motion moverment
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;

            case SlimeState.Jump:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
                StopAgent();
                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");
                //Debug.Log("Jumping");
                break;

            case SlimeState.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                StopAgent();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
               // Debug.Log("Attacking");
                break;
            
            case SlimeState.Damage:
               // Do nothing when animtion is playing
               if(animator.GetCurrentAnimatorStateInfo(0).IsName("Damage0")
                    || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")
                    || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2") ) return;
                StopAgent();
                animator.SetTrigger("Damage");
                animator.SetInteger("DamageType", damType);
                SetFace(faces.damageFace);
                //Debug.Log("Take Damage");
                break;
            
            case SlimeState.Chase:
                //agent.isStopped = false;
                //agent.updateRotation = true;
                //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").transform;
                    agent.SetDestination(player.position);
                    SetFace(agent.velocity == Vector3.zero ? faces.Idleface : faces.WalkFace);
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                }
                break;
        }

    }
    private void StopAgent()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        agent.updateRotation = false;
    }
    // Animation Event
    public void AlertObservers(string message)
    {
        if (message.Equals("AnimationDamageEnded"))
        {
            // When Animation ended check distance between current position and first position 
            //if it > 1 AI will back to first position 

            float distanceOrg = Vector3.Distance(transform.position, originPos);
            if (distanceOrg > 1f)
            {
                walkType = WalkType.ToOrigin;
                currentState = SlimeState.Walk;
            }
            else currentState = SlimeState.Idle;

            //Debug.Log("DamageAnimationEnded");
        }
        if (message.Equals("AnimationAttackEnded"))
        {
            currentState = SlimeState.Idle;           
        }
        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = SlimeState.Idle;
        }
    }
    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}