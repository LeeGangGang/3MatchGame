using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    float _rotSpeed = 270f;
    [SerializeField] GameObject _loadingImg = null;

    // Update is called once per frame
    void Update()
    {
        _loadingImg.transform.Rotate(new Vector3(0, 0, _rotSpeed * Time.deltaTime));
    }
}
