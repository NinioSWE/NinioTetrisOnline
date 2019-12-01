using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnBlock : MonoBehaviour
{
    public string[] blocks;
    public TetrisPreview[] blockPreviewHolder;
    public Sprite[] previewSprites;
    private Queue<int> blockList = new Queue<int>();
    public PhotonView pv;
    private GameObject blockInUse;
    private GameObject blockInStore;
    private int blockInStoreNumber;
    private int blockInUseNumber;
    public static SpawnBlock Instance { get; private set; }
    // Start is called before the first frame update


    void Start()
    {
        if (Instance == null && pv.IsMine) {
            Instance = this;
        }
        for (int i = 0; i < 6; i++) {
            int temp = Bag.PickPiece();
            blockList.Enqueue(temp);
            blockPreviewHolder[i].changeSprite(previewSprites[temp]);

        }
        if (pv.IsMine) {
            NewBlock();
        }
    }

    public void holdBlock()
    {
        if (blockInStore == null) {
            blockInStore = blockInUse;
            blockInStore.transform.position = Vector3.up * 100;
            Debug.Log("image number ? " + blockInUseNumber);
            blockInStoreNumber = blockInUseNumber;
            HoldBlockPreview.Instance.tp.changeSprite(previewSprites[blockInStoreNumber]);
            Debug.Log(previewSprites[blockInUseNumber].name);
            NewBlock();
        }else {
            //switch values
            GameObject temp = blockInStore;
            blockInStore = blockInUse;
            blockInUse = temp;
            int temp2 = blockInStoreNumber;
            blockInStoreNumber = blockInUseNumber;
            blockInUseNumber = temp2;
            HoldBlockPreview.Instance.tp.changeSprite(previewSprites[blockInStoreNumber]);
            blockInStore.transform.position = Vector3.up * 100;
            blockInUse.transform.position = transform.position;
            blockInUse.transform.rotation = Quaternion.identity;
            blockInUse.GetComponent<TetrisBlock>().enabled = true;
        }
    }

    public void updatePreviewList(int newblock)
    {
        for (int i = 0; i < 5; i++) {
            blockPreviewHolder[i].changeSprite(blockPreviewHolder[i+1].GetComponent<SpriteRenderer>().sprite);
        }
        blockPreviewHolder[5].changeSprite(previewSprites[newblock]); 
    }

    public void NewBlock()
    {
        if (!pv.IsMine) {
            return;
        }
        int next = blockList.Peek();
        blockInUseNumber = next;
        blockList.Dequeue();
        blockInUse = PhotonNetwork.Instantiate(blocks[next], transform.position, Quaternion.identity);
        blockInUse.transform.SetParent(transform.parent);
        blockInUse.GetComponent<PhotonView>().RPC("setParent", RpcTarget.Others,pv.ViewID);
        Debug.Log("image number  = " +blockInUseNumber);
        //Instantiate(blocks[next],transform.position,Quaternion.identity ,transform.parent);
        int temp = Bag.PickPiece();
        blockList.Enqueue(temp);
        updatePreviewList(temp);
    }

    static class Bag
    {
        static int[] pieces = {0,1,2,3,4,5,6};
        static int pick = 7;
        public static int PickPiece()
        {
            if (pick == pieces.Length) {
                pick = 0;
                pieces.Shuffle();
            }
            return pieces[pick++];
        }
    }

}
static class RandomExtensions
{
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1) {
            int k = UnityEngine.Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
