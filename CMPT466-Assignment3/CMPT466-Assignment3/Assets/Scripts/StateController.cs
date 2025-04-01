using UnityEngine;

public class StateController : MonoBehaviour
{
    DiceRigidbody WhiteDie;
    BallRigidbody WhiteBall;
    Rigidbody YellowDie;
    Rigidbody YellowBall;
    bool _simPaused;
    bool _advanceFrame;
    bool _advanceFrameDone;

    SimState _YellowDieSavedState;
    SimState _YellowBallSavedState;

    struct SimState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 linear_velocity;
        public Vector3 angular_velocity;
    }

    void Start()
    {
        WhiteDie = GameObject.Find("WhiteDice").GetComponent<DiceRigidbody>();
        WhiteBall = GameObject.Find("WhiteBall").GetComponent<BallRigidbody>();
        YellowDie = GameObject.Find("YellowDice").GetComponent<Rigidbody>();
        YellowBall = GameObject.Find("YellowBall").GetComponent<Rigidbody>();
        _simPaused = false;
        WhiteDie.simPaused = _simPaused;
        WhiteBall.simPaused = _simPaused;
        ResetSimulation();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ResetSimulation();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            _simPaused = !_simPaused;
            if (_simPaused)
            {
                PauseSimulation();
            }
            else
            {
                ResumeSimulation();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && _simPaused)
        {
            _advanceFrame = true;
            _advanceFrameDone = false;
            WhiteDie.AdvanceTimeStep();
            WhiteBall.AdvanceTimeStep();
        }
    }

    void ResetSimulation()
    {
        WhiteDie.ResetState();
        WhiteBall.ResetState();
        YellowDie.position = WhiteDie.initialPosition + 2 * Vector3.right;
        YellowDie.rotation = Quaternion.Euler(WhiteDie.initialRotation);
        YellowDie.velocity = WhiteDie.initialVelocity;
        YellowDie.angularVelocity = Vector3.zero;
        YellowBall.position = WhiteBall.initialPosition + 2 * Vector3.right;
        YellowBall.rotation = Quaternion.Euler(WhiteBall.initialRotation);
        YellowBall.velocity = WhiteBall.initialVelocity;
        YellowBall.angularVelocity = Vector3.zero;
        SaveState();
    }

    void PauseSimulation()
    {
        WhiteDie.simPaused = true;
        WhiteBall.simPaused = true;
        SaveState();
        YellowDie.isKinematic = true;
        YellowBall.isKinematic = true;
    }

    void ResumeSimulation()
    {
        WhiteDie.simPaused = false;
        WhiteBall.simPaused = false;
        YellowDie.isKinematic = false;
        YellowBall.isKinematic = false;
        LoadState();
    }

    void SaveState()
    {
        _YellowDieSavedState.position = YellowDie.position;
        _YellowDieSavedState.rotation = YellowDie.rotation;
        _YellowDieSavedState.linear_velocity = YellowDie.velocity;
        _YellowDieSavedState.angular_velocity = YellowDie.angularVelocity;
        _YellowBallSavedState.position = YellowBall.position;
        _YellowBallSavedState.rotation = YellowBall.rotation;
        _YellowBallSavedState.linear_velocity = YellowBall.velocity;
        _YellowBallSavedState.angular_velocity = YellowBall.angularVelocity;
    }

    void LoadState()
    {
        YellowDie.position = _YellowDieSavedState.position;
        YellowDie.rotation = _YellowDieSavedState.rotation;
        YellowDie.velocity = _YellowDieSavedState.linear_velocity;
        YellowDie.angularVelocity = _YellowDieSavedState.angular_velocity;

        YellowBall.position = _YellowBallSavedState.position;
        YellowBall.rotation = _YellowBallSavedState.rotation;
        YellowBall.velocity = _YellowBallSavedState.linear_velocity;
        YellowBall.angularVelocity = _YellowBallSavedState.angular_velocity;
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            WhiteDie = GameObject.Find("WhiteDice").GetComponent<DiceRigidbody>();
            WhiteBall = GameObject.Find("WhiteBall").GetComponent<BallRigidbody>();
            YellowDie = GameObject.Find("YellowDice").GetComponent<Rigidbody>();
            YellowBall = GameObject.Find("YellowBall").GetComponent<Rigidbody>();
            WhiteDie.transform.position = WhiteDie.initialPosition;
            WhiteDie.transform.rotation = Quaternion.Euler(WhiteDie.initialRotation);
            WhiteBall.transform.position = WhiteBall.initialPosition;
            WhiteBall.transform.rotation = Quaternion.Euler(WhiteBall.initialRotation);
            YellowDie.transform.position = WhiteDie.initialPosition + 2 * Vector3.right;
            YellowDie.transform.rotation = Quaternion.Euler(WhiteDie.initialRotation);
            YellowBall.transform.position = WhiteBall.initialPosition + 2 * Vector3.right;
            YellowBall.transform.rotation = Quaternion.Euler(WhiteBall.initialRotation);
        }
#endif
    }

    void FixedUpdate()
    {
        if (_simPaused && _advanceFrame)
        {
            if (!_advanceFrameDone)
            {
                YellowDie.isKinematic = false;
                YellowBall.isKinematic = false;
                LoadState();
                _advanceFrameDone = true;
            }
            else
            {
                SaveState();
                YellowDie.isKinematic = true;
                YellowBall.isKinematic = true;
                _advanceFrame = false;
                _advanceFrameDone = false;
            }
        }
    }
}
