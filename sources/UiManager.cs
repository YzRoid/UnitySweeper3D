using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace jp.yzroid.CsgUnitySweeper
{
    public class UiManager : MonoBehaviour
    {

        //---------------------------
        //  スタートボタンのテキスト //
        //---------------------------------------------------------------------------------

        [SerializeField]
        private Text mStartText;

        public void RenewStartText(string str, string color)
        {
            mStartText.text = "<color=" + color + ">" + str + "</color>";
        }

        //------------------
        //  経過時間の表示 //
        //---------------------------------------------------------------------------------

        [SerializeField]
        private Text mTimeText;

        public void RenewTimeText(int time)
        {
            int minutes = time / 60;
            int seconds = time % 60;
            mTimeText.text = string.Format("TIME: {0:00}:{1:00}", minutes, seconds);


            //mTimeText.text = "TIME: " + time;

            //DateTime test = DateTime.Parse(time.ToString());
            //mTimeText.text = test.ToString("TIME: mm:ss");
        }

        //-------------------
        // 結果表示・非表示 //
        //---------------------------------------------------------------------------------

        [SerializeField]
        private Text mResultText;

        public void ShowResultText(string str)
        {
            mResultText.text = str;
            mResultText.gameObject.SetActive(true);
        }

        public void HideResultText()
        {
            mResultText.gameObject.SetActive(false);
        }

    }

}
