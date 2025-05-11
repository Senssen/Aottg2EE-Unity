using UnityEngine;
using Photon.Pun;
using Settings;
using Characters;
using ApplicationManagers;
using GameManagers;
using Map;
using UnityEngine.SceneManagement;
using Projectiles;
using Effects;
using UnityEngine.UIElements;

class CannoneerCannon : MonoBehaviour
{
    [Header("Cannon Parts")]
    [SerializeField] private Transform Barrel;
    [SerializeField] private Transform BarrelEnd;
    [SerializeField] private Transform HumanMount;
    private LineRenderer CannonLine;

    private PhotonView PV;
    private GameObject Hero;
    private Human Human;
    protected GeneralInputSettings _input;
    protected HumanInputSettings _humanInput;
    protected InteractionInputSettings _interactionInput;

    private float currentRot = 0f;
    private readonly float RotateSpeed = 30f;
    private readonly float BallSpeed = 300f;
    private float timer = 0f;

    private readonly float GRAVITY = -20f;

    private void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
        Hero = PhotonExtensions.GetPlayerFromID(PV.Owner.ActorNumber);
        Human = Hero.GetComponent<Human>();
        CannonLine = BarrelEnd.GetComponent<LineRenderer>();

        if (PV.IsMine) {
            CannonLine.enabled = true;
            CannonLine.textureMode = LineTextureMode.Tile;
        } else {
            CannonLine.enabled = false;
        }
    }

    void Start()
    {
        Hero.transform.position = HumanMount.transform.position;
        Hero.transform.SetParent(HumanMount.transform);
        Human.MountState = HumanMountState.MapObject;
        Human.MountedTransform = HumanMount.transform;

        if (PV.IsMine)
        {
            _input = SettingsManager.InputSettings.General;
            _humanInput = SettingsManager.InputSettings.Human;
            _interactionInput = SettingsManager.InputSettings.Interaction;
        }
    }

    void Update()
    {
        if (Human == null) {
            Destroy(gameObject);
        }

        if (timer > 0f)
            timer -= Time.deltaTime;

        DrawLine();
        Controls();
    }

    void Shoot()
    {
        if (!PV.IsMine)
            return;

        if (timer <= 0f) {
            timer = 2f;
            Vector3 position = BarrelEnd.transform.position;
            Vector3 velocity = BarrelEnd.forward.normalized * BallSpeed;
            Vector3 gravity = new Vector3(0, GRAVITY, 0);

            EffectSpawner.Spawn(EffectPrefabs.Boom2, position, gameObject.transform.rotation, 0.5f);
            ProjectileSpawner.Spawn(ProjectilePrefabs.CannonBall, position, Quaternion.Euler(Vector3.zero), velocity, gravity, 6.0f, Human.GetComponent<PhotonView>().ViewID, Human.Team);
        }
    }

    public void UnMount()
    {
        PV.RPC("UnMountCannoneer", RpcTarget.All, Human.MountedTransform);
    }

    private void Controls()
    {
        if (_interactionInput.Interact.GetKeyDown()) { 
            UnMount();
        }
        
        if (_humanInput.AttackDefault.GetKeyDown()) { 
            Shoot(); 
        }

        if (_input.Forward.GetKey())
        {
            if (currentRot <= 32f)
            {
                currentRot += Time.deltaTime * RotateSpeed;
                Barrel.Rotate(new Vector3(Time.deltaTime * RotateSpeed, 0f, 0f));
            }
        }
        else if (_input.Back.GetKey() && (currentRot >= -18f))
        {
            currentRot += Time.deltaTime * -RotateSpeed;
            Barrel.Rotate(new Vector3(Time.deltaTime * -RotateSpeed, 0f, 0f));
        }
        if (_input.Left.GetKey())
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * -RotateSpeed, 0f));
        }
        else if (_input.Right.GetKey())
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * RotateSpeed, 0f));
        }
    }

    private void DrawLine()
    {
        if (!PV.IsMine)
            return;

        Vector3 gravityVector = new Vector3(0f, GRAVITY, 0f);
        Vector3 position = BarrelEnd.position;
        Vector3 velocity = BarrelEnd.forward * 300f;

        float timeStep = 40f / velocity.magnitude;

        CannonLine.startWidth = 0.5f;
        CannonLine.endWidth = 40f;
        CannonLine.positionCount = 100;
        
        float totalLength = 0f;
        for (int i = 1; i < CannonLine.positionCount; i++)
        {
            totalLength += Vector3.Distance(CannonLine.GetPosition(i - 1), CannonLine.GetPosition(i));
        }
        float textureRepeatCount = totalLength / 1f;
        CannonLine.material.mainTextureScale = new Vector2(textureRepeatCount, 1f);

        for (int i = 0; i < 100; i++)
        {
            CannonLine.SetPosition(i, position);
            position += velocity * timeStep + 0.5f * timeStep * timeStep * gravityVector;
            velocity += gravityVector * timeStep;
        }
    }

    [PunRPC]
    private void UnMountCannoneer(Transform humanMount, PhotonMessageInfo info)
    {
        Hero.transform.parent = null; //RPC
        Human.MountState = HumanMountState.None;
        Human.MountedTransform = humanMount;
        Destroy(gameObject);
    }
}