using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jp.yzroid.CsgUnitySweeper
{
    public class GameTimer
    {

        private readonly int MAX_TIME = 3599; // 限界は59分59秒
        private int mCurrent;

        public void ResetTime()
        {
            mCurrent = 0;
        }

        public void IncTime()
        {
            mCurrent++;
            if (mCurrent > MAX_TIME) mCurrent = MAX_TIME;
        }

        public int GetTime()
        {
            return mCurrent;
        }

    }
}
