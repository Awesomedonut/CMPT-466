using System.Collections.Generic;
using UnityEngine;

public static class InverseKinematics
{
    public static void ApplyIK(Actor actor, Vector3 targetPosition)
    {
        Joint endEffector = actor.GetIKEndEffector();
        if (endEffector == null)
            return;

    
        List<Joint> ikChain = new List<Joint>();
        Joint current = endEffector;
        
   
        int maxChainLength = 7;
        int chainLength = 0;
        Joint rootJoint = null;
        
        while (current != null && chainLength < maxChainLength)
        {
            ikChain.Add(current);
            
         
            if (current.Name.ToLower().Contains("spine") || current.Name.ToLower().Contains("chest"))
            {
                rootJoint = current;
                break;
            }
            
            current = current.GetParent();
            chainLength++;
        }
        

        if (ikChain.Count < 2)
            return;
            
  
        List<Vector3> originalPositions = new List<Vector3>();
        List<Quaternion> originalRotations = new List<Quaternion>();
        
        foreach (Joint joint in ikChain)
        {
            originalPositions.Add(joint.GlobalPosition);
            originalRotations.Add(joint.LocalQuaternion);
        }
        
     
        int maxIterations = 10;
        float targetThreshold = 0.1f;
        float totalChainLength = 0;
        
    
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < ikChain.Count; i++)
        {
            positions.Add(ikChain[i].GlobalPosition);
            
            if (i > 0)
            {
                totalChainLength += Vector3.Distance(positions[i], positions[i-1]);
            }
        }
        
   
        Vector3 rootPos = positions[positions.Count - 1];
        float distanceToTarget = Vector3.Distance(rootPos, targetPosition);
        
        if (distanceToTarget > totalChainLength)
        {
    
            Vector3 directionToTarget = (targetPosition - rootPos).normalized;
            
       
            for (int i = 0; i < positions.Count - 1; i++)
            {
                float distFromRoot = 0;
                for (int j = positions.Count - 1; j > i; j--)
                {
                    distFromRoot += Vector3.Distance(originalPositions[j], originalPositions[j-1]);
                }
                
                positions[i] = rootPos + directionToTarget * distFromRoot;
            }
        }
        else
        {
         
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
              
                if (Vector3.Distance(positions[0], targetPosition) < targetThreshold)
                    break;
                
             
                positions[0] = targetPosition;
                
                for (int i = 1; i < positions.Count; i++)
                {
                   
                    float distance = Vector3.Distance(positions[i], positions[i-1]);
                    
                 
                    Vector3 direction = (positions[i] - positions[i-1]).normalized;
                    
                 
                    float originalDistance = Vector3.Distance(originalPositions[i], originalPositions[i-1]);
                    
                 
                    positions[i] = positions[i-1] + direction * originalDistance;
                }
                
              
                positions[positions.Count - 1] = rootPos;
                
                for (int i = positions.Count - 2; i >= 0; i--)
                {
               
                    float distance = Vector3.Distance(positions[i], positions[i+1]);
                    
                  
                    Vector3 direction = (positions[i] - positions[i+1]).normalized;
                    
             float originalDistance = Vector3.Distance(originalPositions[i], originalPositions[i+1]);
                    
            
                    positions[i] = positions[i+1] + direction * originalDistance;
                }
                
             
                if (Vector3.Distance(positions[0], targetPosition) < targetThreshold)
                    break;
            }
        }
        
      
        for (int i = ikChain.Count - 1; i > 0; i--)
        {
            Joint joint = ikChain[i];
            Joint childJoint = ikChain[i-1];
            
          
            if (i == 0)
                continue;
        
            Vector3 originalDir = (originalPositions[i-1] - originalPositions[i]).normalized;
            Vector3 newDir = (positions[i-1] - positions[i]).normalized;
            
        
            Quaternion fromToRotation = Quaternion.FromToRotation(originalDir, newDir);
            
       
            float weight = 1.0f;
            
         
            if (joint.Name.ToLower().Contains("shoulder"))
            {
                weight = 0.8f;
            }
            else if (joint.Name.ToLower().Contains("elbow") || joint.Name.ToLower().Contains("wrist"))
            {
                weight = 1.0f;
            }
            else if (joint.Name.ToLower().Contains("spine") || joint.Name.ToLower().Contains("chest"))
            {
                weight = 0.3f;
            }
            
           
            Quaternion weightedRotation = Quaternion.Slerp(Quaternion.identity, fromToRotation, weight);
            
           
            if (joint.ParentIdx != -1)
            {
                Joint parent = joint.GetParent();
                Quaternion worldRotation = weightedRotation * joint.GlobalQuaternion;
                Quaternion parentInverse = Quaternion.Inverse(parent.GlobalQuaternion);
                joint.LocalQuaternion = parentInverse * worldRotation;
            }
            else
            {
                
                joint.LocalQuaternion = weightedRotation * joint.LocalQuaternion;
            }
            
          
            UpdateGlobalTransforms(actor.GetRootJoint());
            
            
            if (HasInvalidValues(joint) || HasInvalidValues(childJoint))
            {
               
                for (int j = 0; j < ikChain.Count; j++)
                {
                    ikChain[j].LocalQuaternion = originalRotations[j];
                }
                UpdateGlobalTransforms(actor.GetRootJoint());
                return;
            }
        }
    }
    
 
    private static bool HasInvalidValues(Joint joint)
    {
        return float.IsNaN(joint.GlobalPosition.x) || float.IsNaN(joint.GlobalPosition.y) || float.IsNaN(joint.GlobalPosition.z) ||
               float.IsInfinity(joint.GlobalPosition.x) || float.IsInfinity(joint.GlobalPosition.y) || float.IsInfinity(joint.GlobalPosition.z) ||
               float.IsNaN(joint.GlobalQuaternion.x) || float.IsNaN(joint.GlobalQuaternion.y) || 
               float.IsNaN(joint.GlobalQuaternion.z) || float.IsNaN(joint.GlobalQuaternion.w) ||
               float.IsInfinity(joint.GlobalQuaternion.x) || float.IsInfinity(joint.GlobalQuaternion.y) || 
               float.IsInfinity(joint.GlobalQuaternion.z) || float.IsInfinity(joint.GlobalQuaternion.w);
    }
    
   
    private static void UpdateGlobalTransforms(Joint joint)
    {
       
        if (joint.ParentIdx == -1) //root
        {
            
            joint.GlobalQuaternion = joint.LocalQuaternion;
            joint.GlobalPosition = joint.LocalPosition;
        }
        else
        { //child
       
            Joint parent = joint.GetParent();
            joint.GlobalQuaternion = parent.GlobalQuaternion * joint.LocalQuaternion;
            joint.GlobalPosition = parent.GlobalPosition + (parent.GlobalQuaternion * joint.LocalPosition);
        }
        
       
        foreach (Joint child in joint.GetChildren())
        {
            UpdateGlobalTransforms(child);
        }
    }
}
