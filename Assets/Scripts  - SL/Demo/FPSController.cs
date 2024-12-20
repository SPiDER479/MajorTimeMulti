using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using MT;
public class FPSController : PortalTraveller {

    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);
    public float rotationSmoothTime = 0.1f;

    CharacterController controller;
    Camera cam;
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;
    
    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    bool jumping;
    float lastGroundedTime;
    bool disabled;

    [SerializeField] private Transform gun;
    [SerializeField] private GameObject airColumn;
    //private int itemSlot;

    private int syncTimer;
    private int syncTimerMax;
     
    private bool canShoot;
    
    public UIManager uiManager;
    public Inventory inventory;
    public RPCHandlers rpcHandlers;
    
    void Start () {
        inventory = GetComponent<Inventory>();
        rpcHandlers = GetComponent<RPCHandlers>();
        if (!IsOwner) return; //
        
        uiManager = FindObjectOfType<UIManager>();
        inventory = GetComponent<Inventory>();
        // inventory.currItemSlot = 0;
        
        cam = Camera.main;
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController> ();

        yaw = transform.eulerAngles.y;
        pitch = cam.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;

        canShoot = true;
        //itemSlot = 1;
        
        Portal[] portals = FindObjectsOfType<Portal>();
        foreach (Portal portal in portals)
        {
            portal.playerCam = cam;
        }
    }

    void Update ()
    {
        if (!IsOwner) return; //
        
        if (Input.GetKeyDown (KeyCode.P)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break ();
        }
        if (Input.GetKeyDown (KeyCode.O)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = !disabled;
        }
        if (disabled) return;

        CheckItemSlot();
        Interact();
        MoveAndCam();
        Shoot();
        Vacuum();
    }
    private void Interact()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Note"));
        foreach (Collider target in targetsInRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < 45)
            {
                float distToTarget = Vector3.Distance(transform.position, targetTransform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, LayerMask.GetMask("Ground")))
                    target.gameObject.GetComponent<Note>().ReadNote();
            }
        }
    }
    private void CheckItemSlot()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     inventory.currItemSlot = 0;
        //     GetComponent<Inventory>().ChangeSlots(inventory.currItemSlot);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     inventory.currItemSlot = 1;
        //     GetComponent<Inventory>().ChangeSlots(inventory.currItemSlot);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     inventory.currItemSlot = 2;
        //     GetComponent<Inventory>().ChangeSlots(inventory.currItemSlot);
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     inventory.currItemSlot = 3;
        //     GetComponent<Inventory>().ChangeSlots(inventory.currItemSlot);
        // }
    }
    private void Vacuum()
    {
        if (!Input.GetMouseButton(0))
        {
            airColumn.SetActive(false);
            return;
        }
        airColumn.SetActive(true);
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("Pickable"));
        foreach (Collider target in targetsInRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < 45)
            {
                float distToTarget = Vector3.Distance(transform.position, targetTransform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, LayerMask.GetMask("Ground")))
                {
                    if (target.gameObject.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
                    {
                        if (networkObject.IsOwnedByServer)
                        {
                            PullSlime(target.gameObject, gun.localToWorldMatrix.GetPosition() - targetTransform.position);
                        }
                    }
                    
                    //target.gameObject.GetComponent<Pull>().pull(gun.localToWorldMatrix.GetPosition() - targetTransform.position);
                    if (distToTarget < 2)
                        //GetComponent<Inventory>().AddItem(target.gameObject, itemSlot);
                        rpcHandlers.AddSlimeToInventoryServerRpc(target.gameObject, inventory.currItemSlot);
                }
            }
        }
    }

    private void PullSlime(GameObject slime, Vector3 force)
    {
        // Call the server RPC to handle pulling the slime
        rpcHandlers.PullSlimeServerRpc(slime, force);
    }
    private void Shoot()
    {
        if (Input.GetMouseButton(1) && canShoot && GetComponent<Inventory>().item[inventory.currItemSlot] != null) 
        {
            //ShootServerRpc(inventory.currItemSlot, transform.position + transform.forward, Quaternion.identity);
            //RequestSpawnObject(transform.position + transform.forward, Quaternion.identity);
            Debug.Log("Shoot");
            rpcHandlers.SpawnObjectServerRpc( OwnerClientId.ToString(), GetComponent<Inventory>().item[inventory.currItemSlot].GetComponent<NetworkObject>().NetworkObjectId.ToString());
            //GameObject obj = Instantiate(GetComponent<Inventory>().item[inventory.currItemSlot], transform.position + transform.forward, Quaternion.identity);
            //obj.SetActive(true);
            
            //obj.GetComponent<Rigidbody>().AddForce(10 * transform.forward,ForceMode.Impulse);
            canShoot = false;
            
            StartCoroutine(ResetShooting());
        }
    }
    
    // Client-side method to request spawning the object
    public void RequestSpawnObject(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (IsClient) // Ensure the request is coming from a client
        {
            //SpawnObjectServerRpc(spawnPosition, spawnRotation, ); // Call the server RPC
        }
    }
    
    IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(3f);
        canShoot = true;
    }
    private void MoveAndCam()
    {
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

        Vector3 inputDir = new Vector3 (input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection (inputDir);

        float currentSpeed = (Input.GetKey (KeyCode.LeftShift)) ? runSpeed : walkSpeed;
        Vector3 targetVelocity = worldInputDir * currentSpeed;
        velocity = Vector3.SmoothDamp (velocity, targetVelocity, ref smoothV, smoothMoveTime);

        verticalVelocity -= gravity * Time.deltaTime;
        velocity = new Vector3 (velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move (velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below) {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        if (Input.GetKeyDown (KeyCode.Space)) {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f)) {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }

        float mX = Input.GetAxisRaw ("Mouse X");
        float mY = Input.GetAxisRaw ("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt (mX * mX + mY * mY);
        if (mMag > 5) {
            mX = 0;
            mY = 0;
        }

        yaw += mX * mouseSensitivity;
        pitch -= mY * mouseSensitivity;
        pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);

        transform.eulerAngles = Vector3.up * smoothYaw;
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
    }
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle (smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (velocity));
        Physics.SyncTransforms ();
    }

}