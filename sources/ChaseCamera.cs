using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jp.yzroid.CsgUnitySweeper
{
    public class ChaseCamera : MonoBehaviour
    {

        [SerializeField]
        private Transform mTarget;
        private Transform mTrans;
        private Vector3 mDistance;
        
        void Start()
        {
            mTrans = GetComponent<Transform>();
            mDistance = mTarget.position - mTrans.position;
        }

        void Update()
        {
            mTrans.position = mTarget.position - mDistance;
        }

    }
}
