using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace jp.yzroid.CsgUnitySweeper
{
    public class SceneMain : MonoBehaviour
    {

        private GameController mGame;

        [SerializeField]
        private CameraController mCamera;
        [SerializeField]
        private BlockManager mBlock;
        [SerializeField]
        private UiManager mUi;

        void Awake()
        {
            // ゲーム全体の初期化
            mGame = GameController.Instance;
            mGame.Init();

            // 初期レベルはイージー
            mGame.GameLevel = GameController.LEVEL_EASY;

            // タイマーの生成
            mTimer = new GameTimer();
        }

        //-------------
        // 状態と更新 //
        //---------------------------------------------------------------------------------

        private enum STATE
        {
            LOADING = 0,
            WAIT_START,
            PLAY,
            RESULT
            
        }
        private STATE mState = STATE.LOADING;

        void Update()
        {
            switch (mState)
            {
                // ステージ生成
                case STATE.LOADING:
                    LoadStage();
                    break;
                // スタートボタンが押されるまで何もしないで待機（取り外してOK）
                case STATE.WAIT_START:
                    break;
                // プレイ中：ゲームの終了条件を監視し、終了でない場合はプレイヤーの入力を受け付ける
                case STATE.PLAY:
                    if (mBlock.IsGameClear)
                    {
                        EndGame(true);
                        return;
                    }
                    if (mBlock.IsGameOver)
                    {
                        EndGame(false);
                        return;
                    }
                    mBlock.CheckMouseInput();
                    mCamera.CheckInput();
                    break;
                // 結果表示中:
                case STATE.RESULT:
                    break;
            }
        }

        //---------------------
        // ゲームの開始と終了 //
        //---------------------------------------------------------------------------------

        private void StartGame()
        {
            mUi.RenewStartText("RESET", "red");
            StartCoroutine("RenewTime");
            mState = STATE.PLAY;
        }

        private void ResetGame()
        {
            StopAllCoroutines();
            mTimer.ResetTime();
            mUi.RenewTimeText(mTimer.GetTime());
            mUi.RenewStartText("LOADING", "black");
            mUi.HideResultText();
            mState = STATE.LOADING;
        }

        private void EndGame(bool clearFlg)
        {
            StopAllCoroutines();
            if (clearFlg)
            {
                mUi.ShowResultText("GAME CLEAR!");
            }
            else
            {
                mUi.ShowResultText("GAME OVER");
            }
            mState = STATE.RESULT;
        }

        //-----------------
        // ステージの生成 //
        //---------------------------------------------------------------------------------

        private void LoadStage()
        {
            int gameLevel = mGame.GameLevel;
            mBlock.CreateField(gameLevel);
            mCamera.SetLimit(gameLevel);
            mUi.RenewStartText("START", "blue");
            mState = STATE.WAIT_START;
        }

        //------------
        // 時間管理 //
        //---------------------------------------------------------------------------------

        private GameTimer mTimer;

        private IEnumerator RenewTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                mTimer.IncTime();
                mUi.RenewTimeText(mTimer.GetTime());
            }
        }

        //-----------
        // 入力待機 //
        //---------------------------------------------------------------------------------

        public void OnStart()
        {
            switch (mState)
            {
                case STATE.WAIT_START:
                    StartGame();
                    break;
                case STATE.PLAY:
                case STATE.RESULT:
                    ResetGame();
                    break;
            }
        }

        public void OnSelectLevel(Dropdown dropdown)
        {
            switch (dropdown.value)
            {
                case 0:
                    mGame.GameLevel = GameController.LEVEL_EASY;
                    break;
                case 1:
                    mGame.GameLevel = GameController.LEVEL_NORMAL;
                    break;
                case 2:
                    mGame.GameLevel = GameController.LEVEL_HARD;
                    break;
            }
            ResetGame();
        }

    }
}
