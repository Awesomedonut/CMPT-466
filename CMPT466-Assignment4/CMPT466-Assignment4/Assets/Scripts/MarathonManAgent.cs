using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsUtils;

[RequireComponent(typeof(JointDriveController))] // Required to set joint forces
public class MarathonManAgent : Agent
{
    [Header("Body Parts")] 
    [Space(10)]
    public Rigidbody body;
    public Rigidbody left_thigh;
    public Rigidbody left_shin;
    public Rigidbody left_foot;
    public Rigidbody right_thigh;
    public Rigidbody right_shin;
    public Rigidbody right_foot;

    JointDriveController m_JdController;
    Dictionary<Rigidbody, BodyPart> bodyPartsDict;

    public override void Initialize()
    {
        m_JdController = GetComponent<JointDriveController>();
        bodyPartsDict = m_JdController.bodyPartsDict;

        //Setup each body part
        m_JdController.SetupBodyPart(body);
        m_JdController.SetupBodyPart(left_thigh);
        m_JdController.SetupBodyPart(left_shin);
        m_JdController.SetupBodyPart(left_foot);
        m_JdController.SetupBodyPart(right_thigh);
        m_JdController.SetupBodyPart(right_shin);
        m_JdController.SetupBodyPart(right_foot);
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // body's orientation
        sensor.AddObservation(body.rotation.eulerAngles);

        // joints angles
        sensor.AddObservation(bodyPartsDict[left_thigh].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[left_thigh].getJointEulerAngle().z);
        sensor.AddObservation(bodyPartsDict[left_shin].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[left_foot].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[right_thigh].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[right_shin].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[right_foot].getJointEulerAngle().x);

        // joints velocities
        sensor.AddObservation(bodyPartsDict[left_thigh].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[left_shin].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[left_foot].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[right_thigh].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[right_shin].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[right_foot].getJointAngleVelocity());

        // body's height and velocity
        sensor.AddObservation(body.position.y);
        sensor.AddObservation(body.velocity);

        // left and right foot's relative yz-position to body
        var left_foot_relPos = left_foot.position - body.position;
        var right_foot_relPos = right_foot.position - body.position;
        sensor.AddObservation(left_foot_relPos.y);
        sensor.AddObservation(left_foot_relPos.z);
        sensor.AddObservation(right_foot_relPos.y);
        sensor.AddObservation(right_foot_relPos.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        var i = 0;

        bodyPartsDict[left_thigh].SetJointTargetRotation(continuousActions[i++], 0, continuousActions[i++]);
        bodyPartsDict[left_shin].SetJointTargetRotation(continuousActions[i++], 0, 0);
        bodyPartsDict[left_foot].SetJointTargetRotation(continuousActions[i++], 0, 0);
        bodyPartsDict[right_thigh].SetJointTargetRotation(continuousActions[i++], 0, continuousActions[i++]);
        bodyPartsDict[right_shin].SetJointTargetRotation(continuousActions[i++], 0, 0);
        bodyPartsDict[right_foot].SetJointTargetRotation(continuousActions[i++], 0, 0);

        AddReward(computeRewardForward());
    }

    public float computeRewardForward()
    {
        /***
         * Task 2: Compute reward for a marathon man agent for a forward locomotion cycle
         */
        return 0f;
    }
}
