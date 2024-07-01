using Simulation.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Simulation.Objects.AcAgent
{
    /// <summary>
    /// Domain model of AC Agent movement
    ///
    /// To control AC, use this class
    /// </summary>
    public class AcAgentCore : MonoBehaviour, AttackableObject
    {
        /// Consts set by Unity Editor

        [Header("Status")]
        [SerializeField]
        private float _hpMax = 100f;

        [SerializeField]
        private Transform _centerPoint;

        [SerializeField]
        private Team _team;

        [Header("Running")]
        [SerializeField]
        private float maxSpeed = 5f;

        [SerializeField]
        private float dragAcceleration = 5f;

        [SerializeField]
        private float maxAcceleration = 15f;

        [SerializeField]
        private float minAcceleration = 6f;

        [SerializeField]
        private float rotationSpeedMax = 360f;

        [Header("Thruster")]
        [SerializeField]
        private float _thrusterMax = 1f;

        [SerializeField]
        private float initialThruster = 0f;

        [SerializeField]
        private float thrusterCoolSpeed = 0.5f;

        [SerializeField]
        private float thrusterCoolTime = 0.5f;

        [SerializeField]
        private GroundDetector groundDetector;

        [Header("Jumping")]
        [SerializeField]
        private float jumpPower = 5f;

        [SerializeField]
        private float thrusterConsumptionJumping = 0.2f;

        [SerializeField]
        private float justJumpedTime = 0.4f;

        [Header("Hovering")]
        [SerializeField]
        private float thrusterConsumptionSpeedHovering = 0.4f;

        [SerializeField]
        private float hoverPower = 1f;

        [Header("Camera")]
        [SerializeField]
        private AcCameraCore cameraAgent;

        [SerializeField]
        private float cameraRotationVerticalMax = 30f;

        [Header("Lockon")]
        [SerializeField]
        private float lockonDistance = 15f;

        [Header("Attack")]
        [SerializeField]
        private GunCore[] guns;

        [SerializeField]
        private float forwardOffsetUnlockedon = 20f;

        /// public properties
        //status
        public float hp { get; private set; }
        public float hpMax => _hpMax;
        public Vector3 centerPoint => _centerPoint.position;
        public Team team => _team;

        /// <summary>
        /// Reference to the controller that taking control of this Agent
        /// </summary>
        public Controller controller { get; private set; }

        //running
        public Vector3 acceleration { get; private set; } = Vector3.zero;
        public Vector3 velocity { get; private set; } = Vector3.zero;

        //camera
        public float cameraRotationHorizontal { get; private set; } = 0f;
        public float cameraRotationVertical { get; private set; } = 0f;

        //thruster
        public float thruster { get; private set; } = 1f;
        public float thrusterMax => _thrusterMax;

        //lockon
        public AttackableObject lockonTarget { get; private set; }

        /// <summary>
        /// Whether the agent is grounded, not jumping
        /// </summary>
        public bool isOnGround
        {
            get
            {
                bool collidingGround = groundDetector.isGrounded;
                bool movingUpward = GetComponent<Rigidbody>().velocity.y > 0;
                bool justJumped = Time.time - jumpedTime < justJumpedTime;

                return collidingGround && !movingUpward && !justJumped;
            }
        }

        /// private fields
        //status
        private UnityEvent<AttackableObject> onDeath = new UnityEvent<AttackableObject>();

        //thruster
        private float thrusterLastUsedTime = 0f;

        //jumping
        private float jumpedTime = 0f;

        /// Unity Callbacks

        private void Awake()
        {
            hp = hpMax;
            thruster = initialThruster;
        }

        private void Start()
        {
            InitializeCameraInformation();
        }

        private void Update()
        {
            MoveObject();
            RotateObject();
            cameraAgent.MoveCamera(cameraRotationHorizontal, cameraRotationVertical);
            UpdateThruster();
            lockonTarget = cameraAgent.LockOn(this, lockonDistance);
        }

        /// public methods

        public void ListenToDeath(UnityAction<AttackableObject> action)
        {
            onDeath.AddListener(action);
        }

        public void StopListeningToDeath(UnityAction<AttackableObject> action)
        {
            onDeath.RemoveListener(action);
        }

        //status

        /// <summary>
        /// Reset all status for new lifecycle within the game
        /// </summary>
        public void Restart()
        {
            hp = hpMax;
            thruster = initialThruster;
            acceleration = Vector3.zero;
            velocity = Vector3.zero;
            thrusterLastUsedTime = 0f;
            jumpedTime = 0f;

            Start();
        }

        /// <summary>
        /// Register Controller
        /// </summary>
        /// <param name="controller">Controller that taking control of this Agent</param>
        public void TakeControl(Controller controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Take damage
        ///
        /// Will destroy the object if hp is less than 0
        /// </summary>
        /// <param name="damage">damage taking</param>
        public void TakeDamage(float damage)
        {
            hp -= damage;

            if (hp <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Die
        ///
        /// Will call callbacks and disappear (not destroy)
        /// </summary>
        public void Die()
        {
            //call callbacks
            onDeath.Invoke(this);

            //Disappear
            //not destroying is for next lifecycle
            gameObject.SetActive(false);
        }

        // running

        /// <summary>
        /// Receive moving command.
        ///
        /// Will add acceleration for the physical computation
        /// </summary>
        /// <param name="direction">which direction to go. magnitude will be converted to 1</param>
        /// <param name="accelerationRatio">(0,1]</param>
        /// <param name="fromCamera">whether the direction is from camera</param>
        public void Move(Vector3 direction, float accelerationRatio, bool fromCamera)
        {
            //if direction is from camera...
            if (fromCamera)
            {
                //...convert `direction to world direction
                Quaternion rotation = Quaternion.Inverse(
                    Quaternion.Euler(0, cameraRotationHorizontal, 0)
                );

                direction = rotation * direction;
            }

            //range check
            //(0, 1]
            accelerationRatio = Mathf.Clamp(accelerationRatio, 0, 1);

            //compute acceleration
            float accelerationMagnitude = Mathf.Lerp(
                minAcceleration,
                maxAcceleration,
                accelerationRatio
            );
            Vector3 accelerationAdding = direction.normalized * accelerationMagnitude;

            //add acceleration
            acceleration += accelerationAdding;
        }

        //jumping / hovering

        /// <summary>
        /// Receive jump command.
        ///
        /// This will jump when grounded, hover when not.
        /// </summary>
        public void JumpOrHover()
        {
            //if the agent is on the ground...
            if (isOnGround)
            {
                //...jump
                Jump();
            }
            else
            {
                //...hover
                Hover();
            }
        }

        // camera rotation

        /// <summary>
        /// Receive horizon camera rotation command.
        /// </summary>
        /// <param name="ratio">
        /// ratio of rotation speed.
        /// [-1, 1].
        ///  Positive is rightside
        /// </param>
        public void RotateCameraHorizontal(float ratio)
        {
            //range check
            //[-1, 1]
            ratio = Mathf.Clamp(ratio, -1, 1);

            //compute rotation
            float rotation = ratio * rotationSpeedMax * Time.deltaTime;

            //rotate
            cameraRotationHorizontal += rotation;
        }

        /// <summary>
        /// Receive vertical camera rotation command.
        /// </summary>
        /// <param name="ratio">
        /// ratio of rotation speed.
        /// [-1, 1].
        /// Positive is upside
        /// </param>
        public void RotateCameraVertical(float ratio)
        {
            //range check
            //[-1, 1]
            ratio = Mathf.Clamp(ratio, -1, 1);

            //compute rotation
            float rotation = ratio * rotationSpeedMax * Time.deltaTime;

            //rotate
            cameraRotationVertical += rotation;

            //rotation limit
            cameraRotationVertical = Mathf.Clamp(
                cameraRotationVertical,
                -cameraRotationVerticalMax,
                cameraRotationVerticalMax
            );
        }

        //attacking

        /// <summary>
        /// Receive Attack command
        /// </summary>
        public void Attack()
        {
            ShootTarget();
        }

        /// private functions
        //status


        // running

        /// <summary>
        /// Move the agent object by acceleration
        /// </summary>
        private void MoveObject()
        {
            velocity += acceleration * Time.deltaTime;

            //drag
            velocity = DragVelocity(velocity);

            //limit speed
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            //move
            transform.position += velocity * Time.deltaTime;

            //reset
            acceleration = Vector3.zero;
        }

        /// <summary>
        /// Take drag into account of velocity
        ///
        /// avoid the velocity to be flipped by drag forever
        /// </summary>
        /// <param name="velocity">velocity without drag</param>
        /// <returns>velocity with drag computed</returns>
        private Vector3 DragVelocity(Vector3 velocity)
        {
            Vector3 accelerationDrag = -velocity.normalized * dragAcceleration;
            Vector3 velocityDirectlyComputed = velocity + accelerationDrag * Time.deltaTime;

            //if the drag acceleration is so strong that flipped the velocity...
            if (Vector3.Dot(velocity, velocityDirectlyComputed) < 0)
            {
                return Vector3.zero;
            }
            else
            {
                return velocityDirectlyComputed;
            }
        }

        //thruster

        /// <summary>
        /// Consumes thruster
        ///
        /// Use this to avoid forgetting thruster check/time stamp
        /// </summary>
        /// <returns>if has enough thruster</returns>
        private bool ConsumeThruster(float consumption)
        {
            //check thruster
            if (thruster < consumption)
            {
                return false;
            }

            //consume thruster
            thruster -= consumption;

            //set last used time
            thrusterLastUsedTime = Time.time;

            return true;
        }

        /// <summary>
        /// Thruster logic per frame
        /// </summary>
        private void UpdateThruster()
        {
            CoolThruster();
        }

        /// <summary>
        /// Cool thruster
        /// </summary>
        private void CoolThruster()
        {
            //if not used for a while...
            if (Time.time - thrusterLastUsedTime > thrusterCoolTime)
            {
                //...cool
                thruster += thrusterCoolSpeed * Time.deltaTime;

                //limit
                thruster = Mathf.Clamp(thruster, 0, thrusterMax);
            }
        }

        //jumping

        /// <summary>
        /// Jump by impulse force.
        ///
        /// Skipped if not enough thruster
        /// </summary>
        private void Jump()
        {
            //consume thruster
            if (!ConsumeThruster(thrusterConsumptionJumping))
            {
                //hadn't enough thruster
                return;
            }

            //jump
            Vector3 force = Vector3.up * jumpPower;
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            //timestamp
            jumpedTime = Time.time;
        }

        //hovering

        /// <summary>
        /// Hover by continuous force
        ///
        /// skipped if just jumped
        /// </summary>
        private void Hover()
        {
            //jump check
            if (Time.time - jumpedTime < justJumpedTime)
            {
                //...just jumped
                return;
            }

            //consume thruster
            if (!ConsumeThruster(thrusterConsumptionSpeedHovering * Time.deltaTime))
            {
                //...hadn't enough thruster
                return;
            }

            //hover
            Vector3 force = Vector3.up * hoverPower;
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
        }

        // camera rotation

        /// <summary>
        /// Recognize and set initial position
        /// </summary>
        private void InitializeCameraInformation()
        {
            //set initial rotation by agent's rotation
            cameraRotationHorizontal = transform.rotation.eulerAngles.y;
            cameraRotationVertical = transform.rotation.eulerAngles.x;
        }

        /// <summary>
        /// Rotate Object
        /// once per frame
        /// </summary>
        private void RotateObject()
        {
            //set by camera horizontal rotation
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                -cameraRotationHorizontal, //without minus, this will be opposite with Camera
                transform.rotation.eulerAngles.z
            );
        }

        //attacking

        /// <summary>
        /// Attack by shooting bullets
        /// </summary>
        private void ShootTarget()
        {
            Vector3 targetPosition;

            //if there is a target locking on...
            if (lockonTarget != null)
            {
                //...shoot it
                targetPosition = lockonTarget.centerPoint;
            }
            //if not...
            else
            {
                //...shoot forward
                targetPosition = transform.position + transform.right * forwardOffsetUnlockedon;
            }

            //shoot
            foreach (GunCore gun in guns)
            {
                gun.Fire(this, targetPosition);
            }
        }
    }
}
