// Jin Song
// student number 301470175

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using Unity.MLAgentsUtils;

[RequireComponent(typeof(JointDriveController))] // Required to set joint forces
public class CrawlerAgent : Agent
{
    [Header("Body Parts")][Space(10)]
    public Rigidbody body;
    public Rigidbody leg0Upper;
    public Rigidbody leg0Lower;
    public Rigidbody leg1Upper;
    public Rigidbody leg1Lower;
    public Rigidbody leg2Upper;
    public Rigidbody leg2Lower;
    public Rigidbody leg3Upper;
    public Rigidbody leg3Lower;

    JointDriveController m_JdController;
    Dictionary<Rigidbody, BodyPart> bodyPartsDict;

    private Vector3 targetPosition;

    public override void Initialize()
    {
        m_JdController = GetComponent<JointDriveController>();
        bodyPartsDict = m_JdController.bodyPartsDict;

        //Setup each body part
        m_JdController.SetupBodyPart(body);
        m_JdController.SetupBodyPart(leg0Upper);
        m_JdController.SetupBodyPart(leg0Lower);
        m_JdController.SetupBodyPart(leg1Upper);
        m_JdController.SetupBodyPart(leg1Lower);
        m_JdController.SetupBodyPart(leg2Upper);
        m_JdController.SetupBodyPart(leg2Lower);
        m_JdController.SetupBodyPart(leg3Upper);
        m_JdController.SetupBodyPart(leg3Lower);

        GameObject target = GameObject.Find("Target");
        if (target != null)
        {
            targetPosition = target.transform.position;
        }
        else
        {
            Debug.LogWarning("Target GameObject not found in the scene.");
        }
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
        sensor.AddObservation(body.rotation.eulerAngles);

        sensor.AddObservation(bodyPartsDict[leg0Upper].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[leg1Upper].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[leg2Upper].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[leg3Upper].getJointEulerAngle().x);

        sensor.AddObservation(bodyPartsDict[leg0Upper].getJointEulerAngle().y);
        sensor.AddObservation(bodyPartsDict[leg1Upper].getJointEulerAngle().y);
        sensor.AddObservation(bodyPartsDict[leg2Upper].getJointEulerAngle().y);
        sensor.AddObservation(bodyPartsDict[leg3Upper].getJointEulerAngle().y);
        
        sensor.AddObservation(bodyPartsDict[leg0Lower].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[leg1Lower].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[leg2Lower].getJointEulerAngle().x);
        sensor.AddObservation(bodyPartsDict[leg3Lower].getJointEulerAngle().x);

        sensor.AddObservation(bodyPartsDict[leg0Lower].getJointEulerAngle().y);
        sensor.AddObservation(bodyPartsDict[leg1Lower].getJointEulerAngle().y);
        sensor.AddObservation(bodyPartsDict[leg2Lower].getJointEulerAngle().y);
        sensor.AddObservation(bodyPartsDict[leg3Lower].getJointEulerAngle().y);

        sensor.AddObservation(bodyPartsDict[leg0Upper].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[leg1Upper].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[leg2Upper].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[leg3Upper].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[leg0Lower].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[leg1Lower].getJointAngleVelocity());
        sensor.AddObservation(bodyPartsDict[leg2Lower].getJointAngleVelocity());
        sensor.AddObservation(body.position);
        sensor.AddObservation(body.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        var i = -1;
        // Pick a new target joint rotation
        bodyPartsDict[leg0Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyPartsDict[leg1Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyPartsDict[leg2Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyPartsDict[leg3Upper].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyPartsDict[leg0Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyPartsDict[leg1Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyPartsDict[leg2Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyPartsDict[leg3Lower].SetJointTargetRotation(continuousActions[++i], 0, 0);

        AddReward(computeRewardTarget());
    }

    public float computeRewardTarget()
    {
        /***
         * Task 1: Compute reward for a crawler agent for target reaching
         */
        //return 0f;
        
        float dist = Vector3.Distance(body.position, targetPosition);
        float reward = 0f;




        if (dist < 0.1f) {

            reward = 1f;

            EndEpisode();


        } else {

            float prevDist = Vector3.Distance(body.position - body.velocity * Time.fixedDeltaTime, targetPosition);

            reward = (prevDist - dist) * 0.1f;
       
       
        }

        return reward;
    }
}
