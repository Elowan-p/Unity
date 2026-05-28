using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    float _playerSensivity;
    
    public void setSensivity(float value){
        _playerSensivity = value;
        Debug.Log(_playerSensivity);
    }
}
