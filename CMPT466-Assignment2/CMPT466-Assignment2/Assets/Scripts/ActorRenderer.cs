using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorRenderer : MonoBehaviour
{
    private Actor Actor;

    private List<Transform> JointSpheres;
    private List<Transform> Bones;
    private GameObject TargetBall;

    [Range(0.0f, 1.0f)]
    public float ActorScale = 0.13f;
    public float BoneWidth = 1f;
    public Color BoneColor = Color.gray;
    public Color JointColor = Color.red;
    public Material transparentMaterial;

    private Transform CreateJointObject(Joint joint)
    {
        Transform joint_ball = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        Destroy(joint_ball.GetComponent<SphereCollider>());
        joint_ball.parent = transform;
        joint_ball.GetComponent<Renderer>().material.color = JointColor;

        return joint_ball;
    }

    private void SetActorScaleAndBoneWidth()
    {
        ActorScale = 1.0f;
        var legJoint = Actor.Joints.Find(x => x.Name.Contains("Leg"));
        if (legJoint is not null)
        {
            ActorScale = 0.5f / Vector3.Magnitude(legJoint.LocalPosition);
            BoneWidth = Mathf.Ceil(Vector3.Magnitude(legJoint.LocalPosition) * 0.2f);
        }
    }

    private Transform CreateBoneObject(Joint joint)
    {
        Transform bone = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
        Destroy(bone.GetComponent<CapsuleCollider>());
        bone.parent = transform;
        bone.GetComponent<Renderer>().material.color = BoneColor;

        return bone;
    }

    private void InitializeSkeleton()
    {
        JointSpheres = new List<Transform>();
        Bones = new List<Transform>();

        JointSpheres.Add(CreateJointObject(Actor.GetRootJoint()));
        Bones.Add(null);

        foreach (Joint joint in Actor.Joints.Skip(1))
        {
            JointSpheres.Add(CreateJointObject(joint));
            Bones.Add(CreateBoneObject(joint));
        }
    }

    private void InitializeTarget()
    {
        TargetBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(TargetBall.transform.GetComponent<SphereCollider>());
        TargetBall.transform.parent = transform;
        Color color = Color.green;
        color.a = 0.9f;
        transparentMaterial.color = color;
        TargetBall.GetComponent<Renderer>().material = transparentMaterial;
        TargetBall.SetActive(false);
    }

    private void UpdateSkeletonPosition()
    {
        // Update each JointSphere's position
        for (int i = 0; i < Actor.Joints.Count; i++)
        {
            JointSpheres[i].position = Actor.Joints[i].GlobalPosition;
        }

        // Update each bone's position and rotation
        for (int i = 1; i < Actor.Joints.Count; i++)
        {
            Joint joint = Actor.Joints[i];
            Joint parentJoint = Actor.Joints[joint.ParentIdx];
            
            // Skip end sites (they don't have bones)
            if (joint.Name == "Site")
                continue;
                
            Vector3 startPos = parentJoint.GlobalPosition;
            Vector3 endPos = joint.GlobalPosition;
            Vector3 boneDirection = endPos - startPos;
            float boneLength = boneDirection.magnitude;
            
            if (boneLength > 0)
            {
                // Position the bone at the midpoint between parent and child
                Bones[i].position = (startPos + endPos) / 2;
                
                // Rotate the bone to align with the direction from parent to child
                // The default cylinder in Unity points along the y-axis
                Bones[i].rotation = Quaternion.FromToRotation(Vector3.up, boneDirection.normalized);
            }
        }

        // DO NOT REMOVE: Necessary to apply the required scaling
        ApplyActorScale();
    }

    private void UpdateTargetPosition()
    {
        TargetBall.transform.position = Actor.GetTargetPosition();
        TargetBall.transform.position *= ActorScale;
        TargetBall.transform.localScale = Vector3.one * 3.0f * BoneWidth * ActorScale;
        if (!TargetBall.activeSelf && Actor.ApplyIk)
        {
            TargetBall.SetActive(true);
        }
        if (!Actor.ApplyIk && TargetBall.activeSelf)
        {
            TargetBall.SetActive(false);
        }
    }

    private void ApplyActorScale()
    {
        if (JointSpheres.Count > 0)
        {
            JointSpheres[0].localScale = Vector3.one * 2.0f * BoneWidth * ActorScale;
            JointSpheres[0].position *= ActorScale;
        }

        for (int i = 1; i < Actor.Joints.Count; i++)
        {
            var joint = Actor.Joints[i];

            JointSpheres[i].localScale = Vector3.one * 2.0f * BoneWidth * ActorScale;
            JointSpheres[i].position *= ActorScale;

            var boneLength = Vector3.Magnitude(joint.LocalPosition);
            Bones[i].localScale = new Vector3(BoneWidth, boneLength / 2, BoneWidth) * ActorScale;
            Bones[i].position *= ActorScale;
        }
    }

    private void Start()
    {
        Actor = GetComponent<Actor>();
        SetActorScaleAndBoneWidth();
        InitializeSkeleton();
        InitializeTarget();
    }

    private void Update()
    {
        UpdateSkeletonPosition();
        UpdateTargetPosition();
    }
}
