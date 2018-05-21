using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jp.yzroid.CsgUnitySweeper
{
    public class CameraController : MonoBehaviour
    {
        private Transform mTrans;

        void Awake()
        {
            mTrans = GetComponent<Transform>();
        }

        //-------------
        // 入力の監視 //
        //---------------------------------------------------------------------------------

        public void CheckInput()
        {
            Vector3 velocity = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) velocity.z += 1.0f;
            if (Input.GetKey(KeyCode.A)) velocity.x -= 1.0f;
            if (Input.GetKey(KeyCode.S)) velocity.z -= 1.0f;
            if (Input.GetKey(KeyCode.D)) velocity.x += 1.0f;
            if (velocity.magnitude > 0.0f)
            {
                Move(velocity);
                return;
            }
        }

        //--------
        // 移動 //
        //---------------------------------------------------------------------------------

        [SerializeField]
        private readonly float MOVE_SPEED = 8.0f;
        private float mMinX, mMaxX, mMinZ, mMaxZ; // カメラの可動範囲

        /// <summary>
        /// 難易度によってカメラの稼働範囲を設定する
        /// </summary>
        /// <param name="gameLevel"></param>
        public void SetLimit(int gameLevel)
        {
            switch (gameLevel)
            {
                case GameController.LEVEL_EASY:
                    mMinX = 3.5f;
                    mMaxX = 5.5f;
                    mMinZ = -2.0f;
                    mMaxZ = 2.0f;
                    break;
                case GameController.LEVEL_NORMAL:
                    mMinX = 5.0f;
                    mMaxX = 11.0f;
                    mMinZ = -2.0f;
                    mMaxZ = 9.0f;
                    break;
                default:
                    mMinX = 5.0f;
                    mMaxX = 25.0f;
                    mMinZ = -2.0f;
                    mMaxZ = 9.0f;
                    break;
            }
        }

        /// <summary>
        /// 移動アクション
        /// </summary>
        /// <param name="velocity"></param>
        private void Move(Vector3 velocity)
        {
            Vector3 current = mTrans.position;
            current.x = Mathf.Clamp(current.x + velocity.x * MOVE_SPEED * Time.deltaTime, mMinX, mMaxX);
            current.z = Mathf.Clamp(current.z + velocity.z * MOVE_SPEED * Time.deltaTime, mMinZ, mMaxZ);
            mTrans.position = current;
        }

    }
}
