using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TetrisBlock : MonoBehaviourPunCallbacks
{
    public Vector3 rotationPoint;
    private float previousTime;
    private float previousTimeHoldLeft;
    private float previousTimeHoldRight;
    public static float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] myGrid = new Transform[width, height];
    private static Transform[,] EnemyGrid = new Transform[width, height];
    const float blocksize = 1f;
    private float holdSpeed = 0.08f;
    [SerializeField]
    private AudioSource moveSound;
    [SerializeField]
    private AudioSource clearLineSound;
    [SerializeField]
    private float volume = 0.5f;
    [SerializeField]
    public ParticleSystem ps;
    static ParticleSystem Instace;
    static int tetrisCount = 0;
    public GameObject EnemyBlockGameObject;
    private Transform[,] grid;
    private Vector3 lastTransformPosition;
    private Quaternion lastTransformRotation;
    public static bool stopGame = false;

    static bool HoldOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        previousTime = Time.time;
        clearLineSound.volume = volume;
        moveSound.volume = volume;
        if (photonView.IsMine)
        {
            grid = myGrid;
        }
        else
        {
            grid = EnemyGrid;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || stopGame) {
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (Time.time - previousTimeHoldLeft > holdSpeed) {
                previousTimeHoldLeft = Time.time;
                transform.position += new Vector3(-blocksize, 0, 0);
                if (!InsideBoard()) {
                    transform.position += new Vector3(blocksize, 0, 0);
                }else {
                    moveSound.Play();
                }
            }
        }
        else {
            previousTimeHoldLeft = 0;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            if (Time.time - previousTimeHoldRight > holdSpeed) {
                previousTimeHoldRight = Time.time;
                transform.position += new Vector3(blocksize, 0, 0);
                if (!InsideBoard()) {
                    transform.position += new Vector3(-blocksize, 0, 0);
                }
                else {
                    moveSound.Play();
                }
            }
        }
        else {
            previousTimeHoldRight = 0;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && !HoldOnce) {
            SpawnBlock.Instance.holdBlock();
            photonView.RPC("UpdatePosition", RpcTarget.Others, transform.position);
            HoldOnce = true;
            this.enabled = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            while (InsideBoard()) {
                transform.position += new Vector3(0, -1, 0);
            }
            transform.position += new Vector3(0, 1, 0);
            HoldOnce = false;
            if (!AddToGrid())
            {
                this.enabled = false;
                return;
            }
            photonView.RPC("UpdatePosition", RpcTarget.Others, transform.position);
            photonView.RPC("UpdateGrid", RpcTarget.Others);
            CheckForLines();
            this.enabled = false;
            if (!CheckIfLost())
            {
                SpawnBlock.Instance.NewBlock();
            }
            else
            {

                Debug.Log("you lost");
                Lose();
            }
            return;
        }

        /*if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            transform.position += new Vector3(-blocksize, 0,0);
            if (!InsideBoard()){
                transform.position += new Vector3(blocksize, 0, 0);
            }
        }else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            transform.position += new Vector3(blocksize, 0, 0);
            if (!InsideBoard()) {
                transform.position += new Vector3(-blocksize, 0, 0);
            }
        }*/

        /*if (Input.GetKeyDown(KeyCode.I))
        {

            int emptyPosition = Random.Range(0, width);
            EnemyBlockLocal(emptyPosition);
            photonView.RPC("EnemyBlock", RpcTarget.Others, emptyPosition);

        }*/

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            transform.RotateAround(transform.TransformPoint(rotationPoint),new Vector3(0,0,1),90);
            if (!InsideBoardRotate()) {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }
            else {
                moveSound.Play();
            }
        }

            if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? holdSpeed : fallTime) ) {
            transform.position += new Vector3(0, -1, 0);
            if (!InsideBoard()) {
                transform.position += new Vector3(0, 1, 0);
                HoldOnce = false;
                if (!AddToGrid()){
                    this.enabled = false;
                    return;
                }
                photonView.RPC("UpdateGrid", RpcTarget.Others);
                CheckForLines();
                this.enabled = false;
                if (!CheckIfLost()) {
                    SpawnBlock.Instance.NewBlock();
                }
                else
                {
                    Debug.Log("you lost");
                    Lose();
                }
            }
            else {
                moveSound.Play();
            }
            previousTime = Time.time;
        }


        if (transform.position != lastTransformPosition) {
            photonView.RPC("UpdatePosition",RpcTarget.Others, transform.position);
        }
        if (transform.localRotation != lastTransformRotation) {
            photonView.RPC("UpdateRotation", RpcTarget.Others, transform.localRotation);
        }
        lastTransformPosition = transform.position;
        lastTransformRotation = transform.localRotation;
    }


    bool AddToGrid()
    {
        foreach (Transform children in transform) {
            int roundedX = Mathf.RoundToInt(children.transform.position.x - transform.parent.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y - transform.parent.position.y);
            if (roundedY > 19)
            {
                //Debug.Log("you lostAdd");
                Lose();
                return false;
            }
            grid[roundedX, roundedY] = children;
        }
        return true;
    }

    void AddToGridEnemy()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x - transform.parent.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y - transform.parent.position.y);

            grid[roundedX, roundedY] = children;
        }
    }


    bool InsideBoard()
    {
        foreach (Transform children in transform) {
            int roundedX = Mathf.RoundToInt(children.transform.position.x - transform.parent.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y - transform.parent.position.y);
     

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height) {
                return false;
            }
            if (grid[roundedX, roundedY] != null) {
                return false;
            }
        }
        return true;
    }

    bool InsideBoardRotate()
    {
        foreach (Transform children in transform) {
            int roundedX = Mathf.RoundToInt(children.transform.position.x - transform.parent.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y - transform.parent.position.y);

            if (roundedX >= width) {
                transform.position -= new Vector3(1,0,0);
                roundedX = Mathf.RoundToInt(children.transform.position.x - transform.parent.position.x);
            }
            if (roundedX < 0) {
                transform.position += new Vector3(1, 0, 0);
                roundedX = Mathf.RoundToInt(children.transform.position.x - transform.parent.position.x);
            }

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height) {
                return false;
            }
            if (grid[roundedX, roundedY] != null) {
                return false;
            }
        }
        return true;
    }

    void CheckForLines()
    {
        for (int i = height-1; i>= 0; i--) {
            if (HasLine(i)) {
                DeleteLine(i);
                RowDown(i);
                if (photonView.IsMine) {
                    tetrisCount++;
                    if (tetrisCount % 3 == 0) {
                        int emptyPosition = Random.Range(0, width);
                        EnemyBlockLocal(emptyPosition);
                        photonView.RPC("EnemyBlock", RpcTarget.Others, emptyPosition);
                    }
                }
            }
        }
    }
    bool HasLine(int i)
    {
        for (int j = 0; j< width; j++) {
            if (grid[j,i] == null) {
                return false;
            }
        }
        return true;
    }
    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++) {
            playParticle(new Vector3(j + transform.parent.position.x, i + transform.parent.position.y, 0), grid[j, i].gameObject);
            var parent = grid[j, i].gameObject.transform.parent.gameObject;
            if (parent.transform.childCount == 1) {
               // Debug.Log("DESTORUYTOYKERPGOKDFGP ODFGPODF GFDPOGKDFPOGKDFOPG");
                Destroy(grid[j, i].gameObject.transform.parent.gameObject);
            }
            else {
                grid[j, i].gameObject.transform.SetParent(null);
                Destroy(grid[j, i].gameObject);
            }
            grid[j, i] = null;
        }
        clearLineSound.Play();
    }

    void playParticle (Vector3 postion, GameObject block)
    {
        if (Instace == null) {
            Instace = Instantiate(ps, postion, Quaternion.identity);
        }
        Instace.transform.position = postion;
        Instace.startColor = block.GetComponent<SpriteRenderer>().color;
        Instace.Emit(Random.Range(7, 11));

    }

    bool CheckIfLost()
    {
        for (int j = 0; j < width; j++) {
            if (grid[j, 19] != null) {
                return true;
            }
        }
        return false;
    }

    void RowDown(int i)
    {
        for (int y = i; y < height; y++) {
            for (int j = 0; j < width; j++) {
                if (grid[j,y] != null) {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, blocksize, 0);
                }
            }
        }
    }

    void EnemyBlockLocal(int emptyPosition)
    {
        if (PhotonNetwork.PlayerList.Length != 2) {
            return;
        }

        for (int y = height - 1; y >= 0; y--) {
            for (int j = 0; j < width; j++) {
                if (EnemyGrid[j, y] != null) {
                    EnemyGrid[j, y + 1] = EnemyGrid[j, y];
                    EnemyGrid[j, y] = null;
                    EnemyGrid[j, y + 1].transform.position += new Vector3(0, blocksize, 0);
                }
            }
        }

        var boards = FindObjectsOfType<BoardPosition>();
        foreach (var board in boards)
        {
            if (!board.GetComponent<PhotonView>().IsMine)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j != emptyPosition) {
                        var temp = Instantiate(EnemyBlockGameObject, Vector3.zero, Quaternion.identity, board.transform);
                        temp.transform.localPosition = new Vector3(j, 0, 0);
                        EnemyGrid[j, 0] = temp.transform.GetChild(0);
                    }
                }
            }
            else
            {
                continue;
            }
        }


    }

    public void Lose()
    {
        stopGame = true;
        var boards = FindObjectsOfType<BoardPosition>();
        foreach (var board in boards)
        {
            if (board.GetComponent<PhotonView>().IsMine)
            {
                board.lose();
            }
            else
            {
                board.win();
            }
        }
        photonView.RPC("winPun", RpcTarget.Others);

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("BackToLobby", 2.0f);
        }
    }

    public void Win()
    {
        stopGame = true;
        var boards = FindObjectsOfType<BoardPosition>();
        foreach (var board in boards)
        {
            if (board.GetComponent<PhotonView>().IsMine)
            {
                board.win();
            }
            else
            {
                board.lose();
            }
        }
        photonView.RPC("losePun", RpcTarget.Others);
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("BackToLobby", 2.0f);
        }
    }
    public void BackToLobby()
    {
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    public void winPun()
    {
        stopGame = true;
        var boards = FindObjectsOfType<BoardPosition>();
        foreach (var board in boards)
        {
            if (board.GetComponent<PhotonView>().IsMine)
            {
                board.win();
            }
            else
            {
                board.lose();
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("BackToLobby", 2.0f);
        }
    }

    [PunRPC]
    public void losePun()
    {
        stopGame = true;
        var boards = FindObjectsOfType<BoardPosition>();
        foreach (var board in boards)
        {
            if (board.GetComponent<PhotonView>().IsMine)
            {
                board.lose();
            }
            else
            {
                board.win();
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("BackToLobby", 2.0f);
        }
    }



    [PunRPC]
    void setParent(int viewID)
    {
        var parent = PhotonNetwork.GetPhotonView(viewID);
        transform.SetParent(parent.transform);
    }
    [PunRPC]
    void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    [PunRPC]
    void UpdateRotation(Quaternion newRotation)
    {
        transform.localRotation = newRotation;
    }
    [PunRPC]
    void UpdateGrid()
    {
        AddToGrid();
        CheckForLines();
        this.enabled = false;
    }
    [PunRPC]
    void EnemyBlock(int emptyPosition)
    {
        for (int y = height - 1; y >= 0; y--) {
            for (int j = 0; j < width; j++) {
                if (myGrid[j, y] != null) {
                    myGrid[j, y + 1] = myGrid[j, y];
                    myGrid[j, y] = null;
                    myGrid[j, y + 1].transform.position += new Vector3(0, blocksize, 0);
                }
            }
        }

        var boards = FindObjectsOfType<BoardPosition>();
        foreach (var board in boards)
        {
            if (board.GetComponent<PhotonView>().IsMine)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j != emptyPosition)
                    {
                        var temp = Instantiate(EnemyBlockGameObject, Vector3.zero, Quaternion.identity, board.transform);
                        temp.transform.localPosition = new Vector3(j, 0, 0);
                        myGrid[j, 0] = temp.transform.GetChild(0);
                    }
                }
                var tetrisPieces = FindObjectsOfType<TetrisBlock>();
                foreach (var tetrisBlock in tetrisPieces)
                {
                    if (tetrisBlock.GetComponent<PhotonView>().IsMine && tetrisBlock.enabled == true)
                    {
                        if (!tetrisBlock.InsideBoard())
                        {
                            tetrisBlock.transform.position += new Vector3(0, 1, 0);
                        }
                    }
                }
            }
            else
            {
                continue;
            }
        }
    }




}
