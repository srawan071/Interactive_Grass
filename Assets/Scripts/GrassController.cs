using UnityEngine;

// This script control the values in shader
public class GrassController : MonoBehaviour
{
    public float ExternalForceStrength = 0.25f;
    public float EaseInTime = 0.15f;
    public float EaseOutTime = 0.15f;
    public float VelocityThreshold = 5f;
    private int _externalForce = Shader.PropertyToID("_ExternalForce");

    //Manually moving grass according to player velocity.
    public void InfluenceGrass(Material mat, float velocity)
    {
        mat.SetFloat(_externalForce, velocity);
    }
}
