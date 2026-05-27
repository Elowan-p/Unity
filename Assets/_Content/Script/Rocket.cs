using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [System.Serializable]
    public class References
    {
        public Rigidbody Rigidbody;
    }
    
    [System.Serializable]
    public class Settings
    {
        public float Speed = 20;
        public float Duration = 5;
        public bool LaunchOnEnable;
    }

    [System.Serializable]
    public class State
    {
        public bool Launched = false;
        public float FlightTime = 0;
    }
    
    [SerializeField] private References _references;
    [SerializeField] private Settings _settings;
    [SerializeField] private State _state;
    

    void Start()
    {
        
    }
    [ContextMenu("Launch")]
    public void Launch()
    {
        _state.Launched = true;
    }
    
    void OnEnable()
    {
        if (_settings.LaunchOnEnable)
        {
            Launch();
        }
    }

    void Update()
    {
        if (_state.Launched && _state.FlightTime < _settings.Duration)
        {
            Vector3 force = transform.up * (_settings.Speed * Time.deltaTime * 100);
            
            _references.Rigidbody.AddForce(force);
            
            _state.FlightTime += Time.deltaTime;
        }

    }


}
