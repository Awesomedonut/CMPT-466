using UnityEngine;

public static class ForwardKinematics
{
    public static void UpdateJointPositions(Actor actor, float[] frameData)
    {
        int index = 0;
        foreach (Joint joint in actor.Joints)
        {
            Vector3 position = Vector3.zero;
            Vector3 rotation = Vector3.zero;

            if (joint.Channels != null)
            {
                foreach (BVHChannel channel in joint.Channels)
                {
                    float value = frameData[index++];
                    
                    switch (channel)
                    {
                        case BVHChannel.Xposition:
                            position.x = value;
                            break;
                        case BVHChannel.Yposition:
                            position.y = value;
                            break;
                        case BVHChannel.Zposition:
                            position.z = value;
                            break;
                        case BVHChannel.Xrotation:
                            rotation.x = value;
                            break;
                        case BVHChannel.Yrotation:
                            rotation.y = value;
                            break;
                        case BVHChannel.Zrotation:
                            rotation.z = value;
                            break;
                    }
                }
            }

            joint.LocalQuaternion = Quaternion.Euler(rotation);

            if (joint.Parent == null)
            {
                joint.GlobalPosition = position;
                joint.GlobalQuaternion = joint.LocalQuaternion;
            }
            else
            {
                joint.GlobalQuaternion = joint.Parent.GlobalQuaternion * joint.LocalQuaternion;
                joint.GlobalPosition = joint.Parent.GlobalPosition + 
                    (joint.Parent.GlobalQuaternion * joint.LocalOffset);
            }
        }
    }
}

