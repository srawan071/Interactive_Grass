using System.Collections;
using UnityEngine;

public class Grass : MonoBehaviour
{
    private GrassController _grassController;
    private GameObject _player;
    private Rigidbody2D _playerRigidBody;

    private float _initialVelocity;
    private float _finalVelocity;
    private int _externalForce = Shader.PropertyToID("_ExternalForce");

    private bool _isEaseInRunning;
    private bool _isEaseOutRunning;

    private Material _material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerRigidBody= _player.GetComponent<Rigidbody2D>();
        _grassController = FindAnyObjectByType<GrassController>();

        _material = GetComponent<SpriteRenderer>().material;
        _initialVelocity = _material.GetFloat("_ExternalForce");
    }

    private IEnumerator EaseIn( float velocity)
    {
      //  Debug.Log("EaseIn");
        _isEaseInRunning = true;
        float elapsedTime = 0;
        while(elapsedTime<_grassController.EaseInTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(_initialVelocity, velocity, (elapsedTime / _grassController.EaseInTime));
            _grassController.InfluenceGrass(_material,lerpedAmount);
            yield return null;
        }
        _isEaseInRunning
            = false;
    }
    private IEnumerator EaseOut()
    {
      //  Debug.Log("EaseOut");
        _isEaseOutRunning = true;
        float currentForce= _material.GetFloat(_externalForce);
        float elapsedTime = 0;
        while (elapsedTime < _grassController.EaseOutTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(currentForce, _initialVelocity, (elapsedTime / _grassController.EaseOutTime));
            _grassController.InfluenceGrass( _material,lerpedAmount);
            yield return null;
        }
        _isEaseOutRunning= false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_player.tag))
        {
            if(!_isEaseInRunning&& Mathf.Abs(_playerRigidBody.velocity.x) > Mathf.Abs(_grassController.VelocityThreshold)){
                StartCoroutine(EaseIn(_playerRigidBody.velocity.x * _grassController.ExternalForceStrength));
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(_player.tag))
        {
            if (Mathf.Abs(_finalVelocity) > Mathf.Abs(_grassController.VelocityThreshold) && Mathf.Abs(_playerRigidBody.velocity.x) < Mathf.Abs(_grassController.VelocityThreshold))
            {
                StartCoroutine(EaseOut());

            }
            else if (Mathf.Abs(_finalVelocity) < Mathf.Abs(_grassController.VelocityThreshold) && Mathf.Abs(_playerRigidBody.velocity.x) > Mathf.Abs(_grassController.VelocityThreshold))
            {
                StartCoroutine(EaseIn(_playerRigidBody.velocity.x * _grassController.ExternalForceStrength));
            }
            else if(!_isEaseInRunning && !_isEaseOutRunning&& Mathf.Abs(_playerRigidBody.velocity.x) > Mathf.Abs(_grassController.VelocityThreshold))
            {
                _grassController.InfluenceGrass(_material,_playerRigidBody.velocity.x*_grassController.ExternalForceStrength);
            }
            _finalVelocity = _playerRigidBody.velocity.x;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy)
        {


            if (collision.CompareTag(_player.tag))
            {
                StartCoroutine(EaseOut());
            }
        }
    }
}
