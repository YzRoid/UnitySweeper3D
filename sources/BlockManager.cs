using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace jp.yzroid.CsgUnitySweeper
{
    public class BlockManager : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPrefabBlock;

        // ブロックの辺のサイズ
        private readonly float BLOCK_SIZE = 1.0f;
        private readonly float BLOCK_SIZE_HALF = 0.5f;

        // ブロックをListで管理
        private List<BlockModel> mBlockList = new List<BlockModel>();

        //-----------------
        // ステージの生成 //
        //---------------------------------------------------------------------------------

        private readonly float CAMERA_OFFSET_Z = 6.5f;

        /// <summary>
        /// フィールドを生成する
        /// </summary>
        public void CreateField(int gameLevel)
        {
            // 前のブロックが存在する場合は全て破棄
            foreach(BlockModel model in mBlockList)
            {
                Destroy(model.gameObject);
            }
            mBlockList.Clear();

            // フラグの解除
            IsGameClear = false;
            IsGameOver = false;

            // ゲームレベルによってサイズと爆弾の数を決定
            int xLength;
            int yLength;
            int bombCount;
            switch (gameLevel)
            {
                case GameController.LEVEL_EASY:
                    xLength = 9;
                    yLength = 9;
                    bombCount = 10;
                    break;
                case GameController.LEVEL_NORMAL:
                    xLength = 16;
                    yLength = 16;
                    bombCount = 40;
                    break;
                default:
                    xLength = 30;
                    yLength = 16;
                    bombCount = 99;
                    break;
            }

            // ブロックを並べる
            InstantiateBlocks(xLength, yLength);

            // ブロックに爆弾を設置
            SetBombs(bombCount);

            // カメラを中心に設定
            float cameraX = xLength * BLOCK_SIZE / 2.0f;
            float cameraZ = yLength * BLOCK_SIZE / 2.0f - CAMERA_OFFSET_Z;
            float cameraY = 6.5f;
            Transform cameraTrans = Camera.main.transform;
            cameraTrans.position = new Vector3(cameraX, cameraY, cameraZ);
            cameraTrans.rotation = Quaternion.Euler(new Vector3(50.0f, 0.0f, 0.0f));
        }

        /// <summary>
        /// ブロックを並べる
        /// 同時に生成されたBlockModelをListに格納する
        /// </summary>
        /// <param name="xLength"></param>
        /// <param name="yLength"></param>
        private void InstantiateBlocks(int xLength, int yLength)
        {
            for(int y=0; y<yLength; y++)
            {
                for(int x=0; x<xLength; x++)
                {
                    GameObject blockGo = Instantiate(mPrefabBlock, new Vector3(x * BLOCK_SIZE + BLOCK_SIZE_HALF, BLOCK_SIZE_HALF, y * BLOCK_SIZE + BLOCK_SIZE_HALF), Quaternion.identity, transform);
                    BlockModel blockModel = blockGo.GetComponent<BlockModel>();
                    blockModel.SetPosition(x, y);
                    mBlockList.Add(blockModel);
                }
            }
        }

        /// <summary>
        /// ブロックに爆弾を設置する
        /// </summary>
        /// <param name="bombCount">配置する爆弾の数</param>
        private void SetBombs(int bombCount)
        {
            //TODO 後で両メソッドの処理時間を計測
            /*
            // LINQを使った場合
            for(int i=0; i<bombCount; i++)
            {
                var targetBlocks = mBlockList.Where(block => !block.HasMine);
                int rand = Random.Range(0, targetBlocks.Count());
                targetBlocks.ElementAt(rand).TestSetMine();
                //targetBlocks.ElementAt(rand).HasMine = true;
            }
            */

            // LINQを使わない場合（Listをシャッフルして先頭から爆弾を設置していく）
            int blockCount = mBlockList.Count;
            for (int i = 0; i < blockCount; i++)
            {
                BlockModel temp = mBlockList[i];
                int rand = UnityEngine.Random.Range(0, blockCount);
                mBlockList[i] = mBlockList[rand];
                mBlockList[rand] = temp;
            }
            for(int i=0; i<bombCount; i++)
            {
                mBlockList[i].HasBomb = true;
            }
        }

        //-----------------
        // ブロックの操作 //
        //---------------------------------------------------------------------------------

        /// <summary>
        /// 指定座標のブロックを取得する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private BlockModel GetBlock(int x, int y)
        {
            return mBlockList.FirstOrDefault(block => block.X == x && block.Y == y);
        }

        /// <summary>
        /// 指定座標の隣接1マスにあるブロックを取得する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private List<BlockModel> GetAroundBlocks(int x, int y)
        {
            List<BlockModel> result = new List<BlockModel>();

            // 定義済みデリゲート
            Action<int, int> action = (posX, posY) =>
            {
                BlockModel model = GetBlock(posX, posY);
                if (model != null) result.Add(model);
            };

            // 各座標をチェックしてリストに追加していく
            action(x - 1, y - 1);
            action(x - 1, y);
            action(x - 1, y + 1);
            action(x, y - 1);
            action(x, y + 1);
            action(x + 1, y - 1);
            action(x + 1, y);
            action(x + 1, y + 1);
            return result;
        }

        private void OpenBlock(BlockModel target)
        {
            // 対象がすでに開かれている or チェック済の場合は何もしない
            if (target.IsOpen || target.IsCheck) return;

            // 対象ブロックの隣接1マスにあるブロックを取得
            List<BlockModel> aroundBlocks = GetAroundBlocks(target.X, target.Y);

            // 周囲の爆弾の数を取得
            int bombCount = aroundBlocks.Count(block => block.HasBomb);

            // 対象ブロックを開く
            target.Open(bombCount);

            // 周囲に爆弾が0だった場合に限り、隣接するブロックを連鎖的に開いていく
            if(bombCount == 0)
            {
                foreach(BlockModel model in aroundBlocks)
                {
                    if (!model.HasBomb) OpenBlock(model);
                }
            }
        }

        private void CheckBlock(BlockModel target)
        {
            target.ChangeCheckFlg();
        }

        //------------------------------
        // ゲームクリアとゲームオーバー //
        //---------------------------------------------------------------------------------

        // ゲームクリアのフラグ
        public bool IsGameClear { get; private set; }

        // ゲームオーバーのフラグ
        public bool IsGameOver { get; private set; }

        /// <summary>
        /// ゲームオーバー
        /// 全ての爆弾を表示してゲームオーバーフラグを立てる
        /// </summary>
        private void GameOver(BlockModel target)
        {
            // 全ての爆弾ブロックに通常の爆弾を表示
            var bombBlocks = mBlockList.Where(block => block.HasBomb);
            foreach(BlockModel model in bombBlocks)
            {
                model.ShowBomb(false);
            }

            // ターゲットのみ特別な爆弾に表示を変更
            target.ShowBomb(true);

            // フラグを立てる
            IsGameOver = true;
        }

        /// <summary>
        /// 爆弾がなく開かれていないブロックが存在するか確認し、存在しないならばクリアフラグを立てる
        /// </summary>
        private void JudgeGameClear()
        {
            if (mBlockList.Any(block => !block.HasBomb && !block.IsOpen) == false)
            {
                IsGameClear = true;
            }
        }

        //-------------
        // 入力の監視 //
        //---------------------------------------------------------------------------------

        /// <summary>
        /// マウスクリックを監視
        /// </summary>
        public void CheckMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnRightClick();
            }
        }

        /// <summary>
        /// 左クリック
        /// 対象ブロックを開く
        /// </summary>
        private void OnLeftClick()
        {
            // UIにマウスポインターが重なっている場合はこちらの処理を無効
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                if(go.tag == GameController.TAG_BLOCK)
                {
                    // 対象が爆弾ブロックか判定
                    BlockModel target = go.GetComponent<BlockModel>();
                    if (target.HasBomb)
                    {
                        // チェック済ならば何もしない
                        if (target.IsCheck) return;

                        // チェックしていないなら開いてゲームオーバー
                        GameOver(target);
                    }else
                    {
                        // 爆弾でないならば一連の開く処理
                        OpenBlock(target);

                        // ゲームクリアの判定
                        JudgeGameClear();
                    }
                }
            }
        }

        /// <summary>
        /// 右クリック
        /// 対象ブロックのチェックフラグを切り替える
        /// </summary>
        private void OnRightClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                if (go.tag == GameController.TAG_BLOCK)
                {
                    CheckBlock(go.GetComponent<BlockModel>());
                }
            }
        }

    }
}
