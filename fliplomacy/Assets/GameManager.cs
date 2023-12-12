using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
[Serializable]
public struct TileData
{
    public int x;
    public int y;
    public string typeTile;
    public GameObject ob;

    public TileData(int x, int y, string typeTile, GameObject ob)
    {
        this.x = x;
        this.y = y;
        this.typeTile = typeTile;
        this.ob = ob;
    }
}
public class GameManager : MonoBehaviour
{
    //[Serializable]
    

    public GameManagerReuse gameManagerReuse;
    public GameObject aCell;
    public List<Sprite> sprite = new List<Sprite>();

    public Transform allCell;
    public GameObject Floppy;
    FloppyControll floppyControll;

    public GameObject Flags;
    public AnimationCurve flagscurve;
    List<TileData> FlagsAllCell = new List<TileData>();

    public GameObject Disappearing;
    List<DisappearingTile> DisappearingObj = new List<DisappearingTile>();// dung cho Disappearing tile

    public GameObject Wormhole;
    List<TileData> WormholeAllCell = new List<TileData>();

    List<Color> WormholeColor = new List<Color>();
    List<float> WormholeSpeed= new List<float>();


    public GameObject FlagChaningDoc;
    List<TileData> FlagChaningDocAllCells = new List<TileData>();
    public GameObject FlagChaningNgang;
    List<TileData> FlagChaningNgangAllCells = new List<TileData>();


    public GameObject MovingTileSprite;
    List<MovingTileGroup> movingTileGroups = new List<MovingTileGroup>();
    List<TileData> MovingTilesAllCell = new List<TileData>();
    public AnimationCurve movingTileGroupsCurve;

    public GameObject BombTileSprite;
    List<TileData> allBomb = new List<TileData>();

    public GameObject BombMarketSprite;
    List<TileData> allBombMarked = new List<TileData>();

    [SerializeField] private TileData[,] _allCells;
    [SerializeField] private TileData floppyPosition = new TileData(0,0,"0",null);

    public Object LevelSave;
    private void Start()
    {
        floppyControll = Floppy.GetComponent<FloppyControll>();
       
        //CreateLevel();
    }
    public void ClearLevelHaveAnim()
    {
        ClearData();
        floppyControll.StopAllAnim();
        StartCoroutine(0.5f.Tweeng((p) => allCell.localScale = p, allCell.localScale, new Vector3(0, 0, 0)));
        StartCoroutine(ClearLevelHaveAnim1());
    }
    public IEnumerator ClearLevelHaveAnim1()
    {
        yield return new WaitForSeconds(0.5f);
        floppyPosition = new TileData(0, 0, "0", null);
        Floppy.transform.position = new Vector3(floppyPosition.x, floppyPosition.y, -1);
        floppyControll.ClearLevel();
        allCell.gameObject.GetComponent<CellsManager>().ClearLevel();
        GetComponent<CheckWinCondition>().flagList.Clear();

    }

    public void ClearLevelAndReloadSceneHaveAnim()
    {
        ClearData();
        floppyControll.StopAllAnim();
        StartCoroutine(0.5f.Tweeng((p) => allCell.localScale = p, allCell.localScale, new Vector3(0, 0, 0)));
        StartCoroutine(ClearLevelAndReloadSceneHaveAnim1());
    }
    public IEnumerator ClearLevelAndReloadSceneHaveAnim1()
    {
        yield return new WaitForSeconds(0.5f);
        floppyPosition = new TileData(0, 0, "0", null);
        Floppy.transform.position = new Vector3(floppyPosition.x, floppyPosition.y, -1);
        floppyControll.ClearLevel();
        allCell.gameObject.GetComponent<CellsManager>().ClearLevel();
        GetComponent<CheckWinCondition>().flagList.Clear();
        CreateLevel();

    }
    public void ClearLevel()
    {
        ClearData();
        floppyPosition = new TileData(0, 0, "0", null);
        Floppy.transform.position = new Vector3(floppyPosition.x, floppyPosition.y, -1);
        floppyControll.ClearLevel();
        allCell.gameObject.GetComponent<CellsManager>().ClearLevel();
        GetComponent<CheckWinCondition>().flagList.Clear();
    }

    public void ClearData()
    {
        FlagsAllCell.Clear();
        DisappearingObj.Clear();
        FlagChaningDocAllCells.Clear();
        FlagChaningNgangAllCells.Clear();
        WormholeAllCell.Clear();
        MovingTilesAllCell.Clear();
        allBomb.Clear();
        allBombMarked.Clear();
        foreach(MovingTileGroup mtv in movingTileGroups)
        {
            mtv.ClearLevel();
        }
    }
    public void CreateLevel()
    {
        allCell.transform.localScale = new Vector3(1, 1, 1);
        Floppy.transform.localScale = new Vector3(1, 1, 1);
        floppyControll.SetUp();
        movingTileGroups = new List<MovingTileGroup>(gameManagerReuse.movingTileGroups);
        WormholeColor = new List<Color>(gameManagerReuse.WormholeColor);
        WormholeSpeed = new List<float>(gameManagerReuse.WormholeSpeed);
        _allCells = new TileData[5, 5];
        var cellsdata = LevelSave.GetComponent<AllCellDate>().cellsData;
        for (int i = 0; i < cellsdata.Count; i++)
        {
            _allCells[cellsdata[i].idX, cellsdata[i].idY].typeTile = cellsdata[i].cellID;
        }
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                _allCells[i, j].x = i;
                _allCells[i, j].y = j;

                aCell.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite[UnityEngine.Random.Range(0, sprite.Count)];
                var tile = Instantiate(aCell, new Vector3(i, j, 0), quaternion.identity);
                tile.transform.SetParent(allCell);

                _allCells[i, j].ob = tile;

                if (_allCells[i, j].typeTile == "1")
                {
                    var tileFlag = tile.AddComponent<FlagTile>();
                    GetComponent<CheckWinCondition>().flagList.Add(tileFlag);
                    tileFlag.flagSprite = Flags;
                    tileFlag.curve = flagscurve;
                    tileFlag.SetUP();
                    FlagsAllCell.Add(_allCells[i, j]);
                }
                else if (_allCells[i, j].typeTile == "2")
                {
                    var tileDisappearing = tile.AddComponent<DisappearingTile>();
                    tileDisappearing.DisappearingSprite = Disappearing;
                    tileDisappearing.SetUP();
                }
                else if (_allCells[i, j].typeTile[0] == '3')
                {
                    var tileWormhole = tile.AddComponent<WormholeTile>();
                    tileWormhole.WormholeSprite = Wormhole;
                    WormholeAllCell.Add(_allCells[i, j]);
                }
                else if (_allCells[i, j].typeTile == "40")
                {
                    var tileFlagChaning = tile.AddComponent<FlagChaningTile>();
                    tileFlagChaning.FlagChaningSprite = FlagChaningDoc;
                    tileFlagChaning.flagChaningType = 1;
                    tileFlagChaning.floppy = floppyControll;
                    tileFlagChaning.SetUP();
                    tileFlagChaning.direction = "Top";
                    FlagChaningDocAllCells.Add(_allCells[i, j]);
                    
                    //FlagChaningDocObj.Add(_allCells[i, j].ob);
                }
                else if (_allCells[i, j].typeTile == "41")
                {
                    var tileFlagChaning = tile.AddComponent<FlagChaningTile>();
                    tileFlagChaning.FlagChaningSprite = FlagChaningNgang;
                    tileFlagChaning.flagChaningType = 2;
                    tileFlagChaning.floppy = floppyControll;
                    tileFlagChaning.direction = "Left";
                    tileFlagChaning.SetUP();
                    FlagChaningNgangAllCells.Add(_allCells[i, j]);
                    
                    //FlagChaningNgangObj.Add(_allCells[i, j].ob);
                }
                else if (_allCells[i, j].typeTile[0] == '5')
                {
                    MovingTilesAllCell.Add(_allCells[i, j]);
                    
                }
                else if (_allCells[i, j].typeTile[0] == '6')
                {
                    var tileBomb = tile.AddComponent<BombTile>();
                    tileBomb.BombSprite = BombTileSprite;
                    //tileFlagChaning.flagChaningType = 2;
                    tileBomb.SetUP();
                    allBomb.Add(_allCells[i, j]);
                    
                    //FlagChaningNgangObj.Add(_allCells[i, j].ob);
                }
                else if (_allCells[i, j].typeTile[0] == '7')
                {
                    
                    var tileBombMarker = tile.AddComponent<BombMarkedTile>();
                    tileBombMarker.BombMarkedSprite = BombMarketSprite;
                    //tileFlagChaning.flagChaningType = 2;
                    tileBombMarker.SetUP();
                    allBombMarked.Add(_allCells[i, j]);
                    //FlagChaningNgangObj.Add(_allCells[i, j].ob);
                }
                else if (_allCells[i, j].typeTile == "20")
                {
                    _allCells[i, j].ob.gameObject.SetActive(false);
                }

            }
        }

        if (FlagChaningDocAllCells.Count != 0)
        {
            for (int j = 0; j < FlagChaningDocAllCells.Count; j++)
            {
                for (int i = 0; i < FlagsAllCell.Count; i++)
                {
                    if (FlagsAllCell[i].x == _allCells[FlagChaningDocAllCells[j].x, FlagChaningDocAllCells[j].y].x)
                    {
                        FlagChaningDocAllCells[j].ob.GetComponent<FlagChaningTile>().flagTiles
                            .Add(FlagsAllCell[i].ob.GetComponent<FlagTile>());
                        //FlagsAllCell[i].ob.GetComponent<FlagTile>().ChangeFlag("Top");
                    }
                }
            }
            
        }
        if (FlagChaningNgangAllCells.Count != 0)
        {
            for (int j = 0; j < FlagChaningNgangAllCells.Count; j++)
            {
                for (int i = 0; i < FlagsAllCell.Count; i++)
                {
                    if (FlagsAllCell[i].y == _allCells[FlagChaningNgangAllCells[j].x, FlagChaningNgangAllCells[j].y].y)
                    {
                        FlagChaningNgangAllCells[j].ob.GetComponent<FlagChaningTile>().flagTiles
                            .Add(FlagsAllCell[i].ob.GetComponent<FlagTile>());
                        //FlagsAllCell[i].ob.GetComponent<FlagTile>().ChangeFlag("Top");
                    }
                }
            }
            
        }
        

        if (WormholeAllCell.Count != 0)
        {
            for(int i = 0;i < WormholeAllCell.Count; i++)
            {
                var Wormhole1 = WormholeAllCell[i];
                for (int j = i+1; j < WormholeAllCell.Count; j++)
                {
                    var Wormhole2 = WormholeAllCell[j];
                    if (Wormhole1.typeTile[1] == Wormhole2.typeTile[1])
                    {
                        var w1tile = Wormhole1.ob.GetComponent<WormholeTile>();
                        w1tile.ConnetedObject = Wormhole2.ob;
                        w1tile.mainHole = true;
                        var _color = WormholeColor[UnityEngine.Random.Range(0, WormholeColor.Count)];
                        w1tile.color = _color;
                        WormholeColor.Remove(_color);
                        var whSpeed = UnityEngine.Random.Range(0, WormholeSpeed.Count);
                        w1tile.rotateSpeed = WormholeSpeed[whSpeed];
                        WormholeSpeed.Remove(WormholeSpeed[whSpeed]);


                        var w2tile = Wormhole2.ob.GetComponent<WormholeTile>();
                        w2tile.ConnetedObject = Wormhole1.ob;
                        w2tile.mainHole = false;


                        w2tile.SetUP();
                        w1tile.SetUP();
                        
                    }
                }
            }
            //WormholeObj[0].GetComponent<WormholeTile>().ConnetedObject = WormholeObj[1].gameObject;
            //WormholeObj[1].GetComponent<WormholeTile>().ConnetedObject = WormholeObj[0].gameObject;
        }
        if (MovingTilesAllCell.Count != 0)
        {
            //movingTileGroups
            //int d = Int32.Parse(c);
            for (int i = 0; i < MovingTilesAllCell.Count; i++)
            {
                var movingTileID = Int32.Parse(MovingTilesAllCell[i].typeTile[4].ToString());
                var movingTileStt = Int32.Parse(MovingTilesAllCell[i].typeTile[5].ToString());
                //movingTileGroups[movingTileID].allMovingTile.Add(MovingTilesAllCell[i].ob);
                movingTileGroups[movingTileID].arrayTile[movingTileStt] = MovingTilesAllCell[i].ob;
                movingTileGroups[movingTileID].CurrentStep = Int32.Parse(MovingTilesAllCell[i].typeTile[2].ToString());
                movingTileGroups[movingTileID].UporBack = Int32.Parse(MovingTilesAllCell[i].typeTile[3].ToString());
                movingTileGroups[movingTileID].Curve = movingTileGroupsCurve;


            }
            for(int i = 0; i< movingTileGroups.Count; i++)
            {
                //movingTileGroups[i].CurrentStep = arrayTile[0]
                movingTileGroups[i].MovingTileSprite = MovingTileSprite;
                movingTileGroups[i].SetUp();
                for(int j = 0; j < movingTileGroups[i].arrayTile.Length; j++)
                {
                    if(movingTileGroups[i].arrayTile[j] != null)
                    {
                       int x = (int)movingTileGroups[i].arrayTile[j].transform.position.x;
                       int y = (int)movingTileGroups[i].arrayTile[j].transform.position.y;
                        if (j != movingTileGroups[i].CurrentStep)
                        {
                            _allCells[x, y].typeTile = 20.ToString();
                        }
                        else
                        {
                            _allCells[x, y].typeTile = 5.ToString();
                        }
                    }
                }
            }
            

            ////550104
            ///string id = "5" + numberOfList.text + (Int32.Parse(ShowPosition.text) - 1).ToString() + MovingDirectionUporBack.value.ToString() + MovingTileID.ToString() + i.ToString();
            //WormholeObj[1].GetComponent<WormholeTile>().ConnetedObject = WormholeObj[0].gameObject;
        }
        List<MovingTileGroup> listOut = (from element in movingTileGroups

                                         where element.arrayTile[0] != null

                                         select (MovingTileGroup)element).ToList();
        movingTileGroups = new List<MovingTileGroup>(listOut);
        if(movingTileGroups.Count > 0)
        {
            //floppyControll.haveMovingTile = true;
        }

        if (allBombMarked.Count != 0)
        {
            for(int i = 0; i < allBombMarked.Count; i++)
            {
                var BombId = Int32.Parse(allBombMarked[i].typeTile[1].ToString());
                for (int j = 0; j < allBomb.Count; j++)
                {
                    var bombBombId = Int32.Parse(allBomb[j].typeTile[1].ToString());
                    if (bombBombId == BombId)
                    {
                        allBomb[j].ob.GetComponent<BombTile>().ConnetedBombMarked.Add(allBombMarked[i].ob);
                    }
                }
            }
           // allBombTile[0].GetComponent<BombTile>().ConnetedBombMarked = allBombMarketTile;
        }

        allCell.gameObject.GetComponent<CellsManager>().setUp();
        GetComponent<CheckWinCondition>().RegisterEndJump();
    }
    public void MovingtileGroup()
    {

        for (int i = 0; i < movingTileGroups.Count; i++)
        {
            //movingTileGroups[i].MovingTileSprite = MovingTileSprite;
            //movingTileGroups[i].SetUp();
            for (int j = 0; j < movingTileGroups[i].arrayTile.Length; j++)
            {
                if (movingTileGroups[i].arrayTile[j] != null)
                {
                    //movingTileGroups[i].MovingTileTravel();
                    StartCoroutine(WaitFloppyEndJump(movingTileGroups[i]));
                    int x = (int)movingTileGroups[i].arrayTile[j].transform.position.x;
                    int y = (int)movingTileGroups[i].arrayTile[j].transform.position.y;
                    if (j != movingTileGroups[i].CurrentStep)
                    {
                        _allCells[x, y].typeTile = 20.ToString();
                        //movingTileGroups[i].arrayTile[j].gameObject.SetActive(false);
                    }
                    else
                    {
                        _allCells[x, y].typeTile = 5.ToString();
                        //movingTileGroups[i].arrayTile[j].gameObject.SetActive(true);
                    }
                }
            }
        }
    }
    public bool FloppyInMovingTileGroup = false;
    public IEnumerator WaitFloppyEndJump(MovingTileGroup mvt)
    {
        float time = 0;
        if (FloppyInMovingTileGroup)
        {
            time = 0.2f;
        }
        else
        {
            time = 0;
        }
        yield return new WaitForSeconds(time);
        mvt.MovingTileTravel();
        FloppyInMovingTileGroup = false;
    }
    public void MovingtileGroupAfterJump()
    {
        for (int i = 0; i < movingTileGroups.Count; i++)
        {
            GameObject[] arrayOut = (from element in movingTileGroups[i].arrayTile

                            where element != null

                              select (GameObject)element).ToArray();
            if (movingTileGroups[i].UporBack == 0) // neu dang tien len
            {
                if(movingTileGroups[i].CurrentStep >= arrayOut.Length-1) // set xem co phai dang o muc lon nhat co the tien len khong
                {
                    movingTileGroups[i].UporBack = 1; // neu co thi phai lui lai
                    movingTileGroups[i].CurrentStep--;
                }
                else
                {
                    movingTileGroups[i].CurrentStep++; // neu khong tiep tuc tien len
                }
            }
            else if (movingTileGroups[i].UporBack == 1) // neu dang lui xuong
            {
                if (movingTileGroups[i].CurrentStep <= 0) // set xem co phai dang o muc nho nhat co the lui xuong khong
                {
                    movingTileGroups[i].UporBack = 0; // neu co thi phai tien len
                    movingTileGroups[i].CurrentStep++;
                }
                else
                {
                    movingTileGroups[i].CurrentStep--; // neu khong tiep tuc lui xuong
                }
            }

        }
        MovingtileGroup();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnSwipeLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnSwipeRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnSwipeTop();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnSwipeBottom();
        }
        if ((Input.GetKeyDown(KeyCode.H)))
        {
            ClearLevel();
            CreateLevel();
        }
        //
    }
    [HideInInspector] public string swipeDirection;
    public void OnSwipeLeft() {
        swipeDirection = "Left";
        FloppyMove(-1,0);
    }

    public void OnSwipeRight() {
        swipeDirection = "Right";
        FloppyMove(1, 0);
    }

    public void OnSwipeTop() {
        swipeDirection = "Top";
        FloppyMove(0,1);
    }

    public void OnSwipeBottom() {
        swipeDirection = "Down";
        FloppyMove(0,-1);
    }

    private void FloppyMove(int x, int y)
    {

        var nextX = floppyPosition.x + x;
        var nextY = floppyPosition.y + y;
        List<GameObject> flagsInStep = new List<GameObject>();// dung cho flag tile

        if (nextX < 5 && nextX >= 0 && nextY >= 0 && nextY < 5)// && _allCells[nextX, nextY].typeTile != 20)
        {
            if(_allCells[floppyPosition.x, floppyPosition.y].typeTile[0] == '5')
            {
                FloppyInMovingTileGroup = true;
                floppyControll.stillInMovingTile = true;
                Debug.LogError(" FloppyInMovingTileGroup = true");
            }
            if (_allCells[nextX, nextY].typeTile == 1.ToString()) // flagtile
            {
                FlagTileFunction(nextX, nextY, flagsInStep, x, y, floppyPosition.x, floppyPosition.y);

            }
            else if (_allCells[nextX, nextY].typeTile == 2.ToString()) //Disappearing
            {
                DisappearingCheckOnIt(nextX, nextY);
            }
            else if (_allCells[nextX, nextY].typeTile == 20.ToString())
            {
                //DisappearingHideTileFunction(nextX, nextY, x, y, floppyPosition.x, floppyPosition.x);
                // cai nay dung trong truong hop muon nhay qua o da bi an
                // con khong thi khong lam gi ca
            }
            else if (_allCells[nextX, nextY].typeTile[0] == '6') // BombTile
            {
                BombTileCheckOnIt(nextX, nextY);
            }
            else if (_allCells[nextX, nextY].typeTile == 40.ToString()) // Change flag theo chieu doc
            {
                FlagChangeTileCheckOnIt(nextX, nextY, "Top");
            }
            else if (_allCells[nextX, nextY].typeTile == 41.ToString()) // Change flag theo chieu ngang
            {
                FlagChangeTileCheckOnIt(nextX, nextY, "Left");

            }
            else
            {
                FloppyJump(nextX, nextY);
            }
            
        }

    }
    public void FloppyMove()
    {

        if (_allCells[floppyPosition.x, floppyPosition.y].typeTile == 2.ToString()) //Disappearing
        {
            DisappearingCheckOnIt();
        }
        else if (_allCells[floppyPosition.x, floppyPosition.y].typeTile[0] == '6') // BombTile
        {
            BombTileCheckOnIt();
        }
        else if (_allCells[floppyPosition.x, floppyPosition.y].typeTile == 40.ToString()) // Change flag theo chieu doc
        {
            FlagChangeTileCheckOnIt("Top");
        }
        else if (_allCells[floppyPosition.x, floppyPosition.y].typeTile == 41.ToString()) // Change flag theo chieu ngang
        {
            FlagChangeTileCheckOnIt("Left");

        }

    }

    public void FloppyJump(int nextX, int nextY)
    {
        Debug.LogWarning("MovingtileGroupAfterJump");
        floppyPosition.x = nextX;
        floppyPosition.y = nextY;
        Floppy.transform.position = new Vector3(floppyPosition.x, floppyPosition.y, -1);
        HideDisappearingTileAfterJumpOut();

        if (_allCells[nextX, nextY].typeTile[0] == '3') // Sau khi jum moi check wormHole
        {
            var conneted = _allCells[nextX, nextY].ob.GetComponent<WormholeTile>().ConnetedObject.transform.position;

            Floppy.transform.position = new Vector3(conneted.x, conneted.y, -1);
            floppyControll.wormholeStartPosX = floppyPosition.x;
            floppyControll.wormholeStartPosY = floppyPosition.y;
            floppyControll.floppyInWormHole = true;

            //floppyControll.TeleAnim(floppyPosition.x, floppyPosition.y);

            floppyPosition.x = (int)conneted.x;
            floppyPosition.y = (int)conneted.y;
        }
        if (movingTileGroups.Count > 0) // Sau khi jum moi check moving tile
        {
            bool inMoving = false;
            var mtg = new MovingTileGroup();
            for (int i = 0; i < movingTileGroups.Count; i++)
            {
                int tileX = (int)movingTileGroups[i].arrayTile[movingTileGroups[i].CurrentStep].transform.position.x;
                int tileY = (int)movingTileGroups[i].arrayTile[movingTileGroups[i].CurrentStep].transform.position.y;

                if (floppyPosition.x == tileX && floppyPosition.y == tileY)
                {
                    inMoving = true;
                    floppyControll.inMovingTile = true;
                    FloppyInMovingTileGroup = true;
                    mtg = movingTileGroups[i];
                    floppyControll.movingTilePos = new Vector3(mtg.arrayTile[mtg.CurrentStep].transform.position.x, mtg.arrayTile[mtg.CurrentStep].transform.position.y, 0);
                }
            }
            MovingtileGroupAfterJump();
           
            if (inMoving)
            {
                Floppy.transform.position = new Vector3(mtg.arrayTile[mtg.CurrentStep].transform.position.x, mtg.arrayTile[mtg.CurrentStep].transform.position.y, -1);
                floppyPosition.x = (int)mtg.arrayTile[mtg.CurrentStep].transform.position.x;
                floppyPosition.y = (int)mtg.arrayTile[mtg.CurrentStep].transform.position.y;
            }
        }
        floppyControll.JumpAnim();
        
    }
    public void FlagTileFunction(int nextX, int nextY, List<GameObject> flagsInStep,
        int x, int y, int startFloppyPositionx, int startFloppyPositiony)
    {
        if (_allCells[nextX, nextY].typeTile == 1.ToString()) // flagtile
        {
            flagsInStep.Add(_allCells[nextX, nextY].ob);
            nextX += x;
            nextY += y;
            if (nextX < 5 && nextX >= 0 && nextY >= 0 && nextY < 5)
            {
                if (_allCells[nextX, nextY].typeTile == 1.ToString())
                {
                    FlagTileFunction(nextX, nextY, flagsInStep, x, y, startFloppyPositionx, startFloppyPositiony);
                }
                else if (_allCells[nextX, nextY].typeTile != 1.ToString())
                {
                    if (_allCells[nextX, nextY].typeTile != 20.ToString())
                    {
                        FloppyJump(nextX, nextY);

                        for (int i = 0; i < flagsInStep.Count; i++)
                        {
                            var flagFunc = flagsInStep[i].GetComponent<FlagTile>();
                            flagFunc.ChangeFlag(swipeDirection);

                        }
                    }
                    if (_allCells[nextX, nextY].typeTile == 2.ToString())
                    {
                        DisappearingCheckOnIt();
                    }
                    else if (_allCells[nextX, nextY].typeTile[0] == '6') // BombTile
                    {
                        BombTileCheckOnIt();
                    }
                    else if (_allCells[nextX, nextY].typeTile == 40.ToString()) // Change flag theo chieu doc
                    {
                        FlagChangeTileCheckOnIt( "Top");
                    }
                    else if (_allCells[nextX, nextY].typeTile == 41.ToString()) // Change flag theo chieu ngang
                    {
                        FlagChangeTileCheckOnIt( "Left");

                    }
                }

            }
            else
            {
                FloppyJump(startFloppyPositionx, startFloppyPositiony);
                Debug.Log("log gi do o day");
                //endstep
            }
        }
    }

    public void DisappearingCheckOnIt(int nextX, int nextY)
    {
        FloppyJump(nextX, nextY);
        _allCells[nextX, nextY].typeTile = 20.ToString();
        var disappearingTileFunc = _allCells[nextX, nextY].ob.GetComponent<DisappearingTile>();
        disappearingTileFunc.HaveObjectOn();
        Debug.Log("DisappearingOnTriger");
        DisappearingObj.Add(disappearingTileFunc);
    }
    public void DisappearingCheckOnIt()
    {
        _allCells[floppyPosition.x, floppyPosition.y].typeTile = 20.ToString();
        var disappearingTileFunc = _allCells[floppyPosition.x, floppyPosition.y].ob.GetComponent<DisappearingTile>();
        disappearingTileFunc.HaveObjectOn();
        Debug.Log("DisappearingOnTriger");
        DisappearingObj.Add(disappearingTileFunc);
    }
    public void BombTileCheckOnIt(int nextX, int nextY)
    {
        FloppyJump(nextX, nextY);
        StartCoroutine(WaitEndJumpToDoBombFunc(nextX, nextY));
        
    }
    public void BombTileCheckOnIt()
    {
        StartCoroutine(WaitEndJumpToDoBombFunc(floppyPosition.x, floppyPosition.y));

    }
    public IEnumerator WaitEndJumpToDoBombFunc(int nextX, int nextY)
    {
        yield return new WaitForSeconds(0.2f);
        var connetedBombMarked = _allCells[nextX, nextY].ob.GetComponent<BombTile>().ConnetedBombMarked;
        for (int i = 0; i < connetedBombMarked.Count; i++)
        {
            connetedBombMarked[i].GetComponent<BombMarkedTile>().Hiding();
            int mx = (int)connetedBombMarked[i].transform.position.x;
            int my = (int)connetedBombMarked[i].transform.position.y;
            _allCells[mx, my].typeTile = 20.ToString();

        }
    }
    public void FlagChangeTileCheckOnIt(int nextX, int nextY, string direction)
    {
        FloppyJump(nextX, nextY);
        StartCoroutine(WaitEndJumpToDoFlagChangeFunc(nextX,nextY,direction));
    }
    public void FlagChangeTileCheckOnIt( string direction)
    {
        StartCoroutine(WaitEndJumpToDoFlagChangeFunc(floppyPosition.x, floppyPosition.y, direction));
    }
    public IEnumerator WaitEndJumpToDoFlagChangeFunc(int nextX, int nextY, string direction)
    {
        yield return new WaitForSeconds(0.19f);
        _allCells[nextX, nextY].ob.GetComponent<FlagChaningTile>().ChangeFlag();
    }
    public void HideDisappearingTileAfterJumpOut()
    {
        if(DisappearingObj.Count > 0)
        {
            DisappearingObj[0].HideObject();
            DisappearingObj.Clear();
        }
    }

    public void StartSolve_Button()
    {
        
    }
    void GameSolve()
    {
        
    }

}
