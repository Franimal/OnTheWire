using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Wheel;

    private Quaternion _startRotation;
    private Quaternion _targetRotation;
    private float _startRotationTime;
    private float _endRotationTime;

    public float Speed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_targetRotation != null)
        {
            var t = (Time.time - _startRotationTime) / (_endRotationTime - _startRotationTime);
            transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, t);
        }

        // Rotate wheel based on Speed
        Wheel.Rotate(new Vector3(0, 0, 1), Speed * -1000 * Time.deltaTime);
    }

    internal void RotateTowards(Quaternion rotation, float totalTimeSeconds)
    {
        _startRotation = transform.rotation;
        _targetRotation = rotation;

        if(_targetRotation.x < 0)
        {
            _targetRotation.x = (_targetRotation.x - transform.rotation.x) / 2f;
        }


        if (_targetRotation.z < 0)
        {
            _targetRotation.z = (_targetRotation.z - transform.rotation.z) / 1.5f;
        }

        _startRotationTime = Time.time;
        _endRotationTime = _startRotationTime + totalTimeSeconds;
    }
}
