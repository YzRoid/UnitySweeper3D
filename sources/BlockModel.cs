using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jp.yzroid.CsgUnitySweeper
{
    public class BlockModel : MonoBehaviour
    {
        [SerializeField]
        private Material mOpenedMaterial;
        [SerializeField]
        private NumberChanger mNumChanger;

        //-------------
        // ポジション //
        //---------------------------------------------------------------------------------

        public int X { get; private set; }
        public int Y { get; private set; }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        //----------
        // フラグ //
        //---------------------------------------------------------------------------------

        // 爆弾ブロックの場合はtrue
        public bool HasBomb { get; set; }

        // 開かれたブロックの場合はtrue
        public bool IsOpen { get; private set; }

        // チェック済ブロックの場合はtrue
        public bool IsCheck { get; private set; }

        //-------------
        // アクション //
        //---------------------------------------------------------------------------------

        /// <summary>
        /// ブロックを開く
        /// </summary>
        public void Open(int aroundBombs)
        {
            IsOpen = true;
            mNumChanger.ChangeNumber(aroundBombs);
            GetComponent<Renderer>().material = mOpenedMaterial;
        }

        /// <summary>
        /// チェック済フラグを反転させる
        /// それによってチェックマークを表示・非表示にする
        /// </summary>
        public void ChangeCheckFlg()
        {
            if (IsOpen) return;
            IsCheck = !IsCheck;
            if (IsCheck)
            {
                mNumChanger.ChangeUvToCheck();
            }else
            {
                mNumChanger.ChangeUvToBlank();
            }
        }

        /// <summary>
        /// 爆弾を表示する
        /// フラグが立っている場合は特別な爆弾を表示する
        /// </summary>
        /// <param name="flg"></param>
        public void ShowBomb(bool flg)
        {
            if (flg)
            {
                mNumChanger.ChangeUvToBombB();
            }
            else
            {
                mNumChanger.ChangeUvToBombA();
            }
        }

    }
}
