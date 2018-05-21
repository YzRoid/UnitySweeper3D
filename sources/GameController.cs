using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jp.yzroid.CsgUnitySweeper
{
    public class GameController : MonoBehaviour
    {

        // 初期化タイミングでインスタンスを生成（スレッドセーフ）
        private static readonly GameController mInstance = new GameController();

        // コンストラクタをprivateにすることによって他クラスからnewできないようにする
        private GameController() { }

        // 他クラスからこのインスタンスを参照する
        public static GameController Instance
        {
            get
            {
                return mInstance;
            }
        }

        public void Init()
        {
            // フレームレートの設定
            Application.targetFrameRate = 30;
        }

        //--------
        // 定数 //
        //----------------------------------------------------------------------------------------

        // タグ
        public static string TAG_BLOCK = "Block";

        // ゲームレベル
        public const int LEVEL_EASY = 1, LEVEL_NORMAL = 2, LEVEL_HARD = 3;

        //-----------------
        // ゲームのレベル //
        //----------------------------------------------------------------------------------------

        public int GameLevel { get; set; }

    }
}
