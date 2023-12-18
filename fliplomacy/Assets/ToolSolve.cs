using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class ToolSolve : MonoBehaviour
{
    [SerializeField] private TileData[,] _allCells;
    public GameObject aCell;
    public GameObject floppy;
    public List<Sprite> sprite = new List<Sprite>();
    public Object LevelSave;
    
    private List<TileData> flagTiles = new List<TileData>();
    
    private List<TileData> disappearingTiles = new List<TileData>();
    
    private List<TileData> wormholeTiles = new List<TileData>();
    
    private List<TileData> flagChaningTiles = new List<TileData>();
    private Dictionary<int, List<TileData>> changedFlagGroup = new Dictionary<int, List<TileData>>();
    
    private List<TileData> movingTiles = new List<TileData>();
    private Dictionary<int, List<TileData>> movingTileGroup = new Dictionary<int, List<TileData>>();
    
    private List<TileData> bombTiles = new List<TileData>();
    
    private List<TileData> bombMarkedTiles = new List<TileData>();
    private Dictionary<int, List<TileData>> bombMarkedTileGroup = new Dictionary<int, List<TileData>>();

    private void Start()
    {
        var cellsdata = LevelSave.GetComponent<AllCellDate>().cellsData;
        _allCells = new TileData[5, 5];
        
        for (int i = 0; i < cellsdata.Count; i++)
        {
            _allCells[cellsdata[i].idX, cellsdata[i].idY].typeTile = cellsdata[i].cellID;
        }

        floppyPosition.ob = floppy;

        CreateLevel();
        BackTracking(counting);
    }
   
    void CreateLevel()
    {
        for (int i = 0; i < _allCells.GetLength(0); i++)
        {
            for (int j = 0; j < _allCells.GetLength(1); j++)
            {
               _allCells[i, j].x = i;
               _allCells[i, j].y = j;
    
                //aCell.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite[UnityEngine.Random.Range(0, sprite.Count)];
                var tile = Instantiate(aCell, new Vector3(i, j, 0), Quaternion.identity);
                tile.transform.SetParent(gameObject.transform);
    
                _allCells[i, j].ob = tile;
                char tileType = _allCells[i, j].typeTile[0];
                switch (tileType)
                {
                    case '0':
                        _allCells[i, j].typeTile = "2";
                        disappearingTiles.Add(_allCells[i, j]);
                        break;
                    case '1':
                        flagTiles.Add(_allCells[i, j]);
                        _allCells[i, j].typeTile = "10";
                        break;
                    case '2':
                        disappearingTiles.Add(_allCells[i, j]);
                        break;
                    case '3':
                        wormholeTiles.Add(_allCells[i, j]);
                        break;
                    case '4':
                        flagChaningTiles.Add(_allCells[i, j]);
                        break;
                    case '5':
                        movingTiles.Add(_allCells[i, j]);
                        break;
                    case '6':
                        bombTiles.Add(_allCells[i, j]);
                        break;
                    case '7':
                        bombMarkedTiles.Add(_allCells[i, j]);
                        break;
                    default:
                        break;
                }
            }
        }
        SortMovingTileIntoMovingTileGroup();
        SortFlagChangedIntoGroup();
        SortBombMarkedIntoGroup();
        LoadTileVisual();
    }

    
    [SerializeField] private TileData floppyPosition = new TileData(0,0,"0",null);
    private TileData disappearingTile = new TileData(0,0,"20",null);
    bool FloppyMove(int direction_X, int direction_Y)
    {
        var nextX = floppyPosition.x + direction_X;
        var nextY = floppyPosition.y + direction_Y;
        List<TileData> allFlagChanging = new List<TileData>();

        bool canMove = false;
        if (CheckTileJumpOn(direction_X, direction_Y,ref nextX, ref nextY, allFlagChanging, ref disappearingTile))
        {
            floppyPosition.x = nextX;
            floppyPosition.y = nextY;
            floppyPosition.ob.transform.position = new Vector3(floppyPosition.x, floppyPosition.y);
            canMove = true;
        }

        LoadTileVisual();
        return canMove;
    }
    
    bool CheckTileJumpOn(int direction_X , int direction_Y, ref int tileX,  ref int tileY, List<TileData> allFlagChanging, ref TileData disappearingTile)
    {
        bool onMovingTile = false;
        if (tileX < 5 && tileX >= 0 && tileY >= 0 && tileY < 5) // && _allCells[nextX, nextY].typeTile != 20)
        {
            TileData tile = _allCells[tileX, tileY];
            char tileType = tile.typeTile[0];
            
            if (tileType == '1')
            {
                allFlagChanging.Add(_allCells[tileX,tileY]);
                tileX += direction_X;
                tileY += direction_Y;
                return CheckTileJumpOn(direction_X, direction_Y, ref tileX, ref tileY, allFlagChanging, ref disappearingTile);
            }
            
            if (ISTileCanMoveOn(tile.typeTile))
            {
                if (allFlagChanging.Count > 0)
                {
                    for (int i = 0; i < allFlagChanging.Count; i++)
                    {
                        FlagTileChangeTypeTile(allFlagChanging[i]);
                    }
                    allFlagChanging.Clear();
                }

                if (disappearingTile.typeTile != "20")
                {
                    TileHidingChangeTypeTile(disappearingTile);
                   // Debug.Log("disappearingTile");
                }
                
                switch (tileType)
                {
                    case '1':
                        break;
                    case '2':
                        disappearingTile = _allCells[tileX, tileY];
                        break;
                    case '3':
                        var connectedTile = FindConnectedWormhole(tile);
                        tileX = connectedTile.x;
                        tileY = connectedTile.y;
                        break;
                    case '4':
                        //Debug.Log("changingflag");
                        ChangeConnectFlag(tile);
                        break;
                    case '5':
                        onMovingTile = true;
                        break;
                    case '6':
                        ChangeConnectBombMarked(tile);
                        break;
                    case '7':
                        break;
                    default:
                        break;
                }
                MovingTileChangeTypeTile();
                if (onMovingTile)
                {
                    var ar = MovingTileOn(tile.typeTile[4].ToString());
                    tileX = ar[0];
                    tileY = ar[1];
                }

                return true;
            }
        }

        return false;
    }

    

    void FlagTileChangeTypeTile(TileData flagTileData)
    {
        var newTypeTile = _allCells[flagTileData.x,flagTileData.y].typeTile == "10" ? "11" : "10";
        ChangeTypeTile(flagTileData, newTypeTile);
    }

    void TileHidingChangeTypeTile(TileData tileData)
    {
        var newTypeTile = "20";
        if (tileData.typeTile != newTypeTile)
        {
            ChangeTypeTile(tileData, newTypeTile);
        }
    }

    TileData FindConnectedWormhole(TileData tileData)
    {
        TileData connectedWormhole = wormholeTiles
            .FirstOrDefault(wormholeTile => wormholeTile.ob != tileData.ob && wormholeTile.typeTile == tileData.typeTile);

        return connectedWormhole.ob != null ? connectedWormhole : tileData;
    }

    void ChangeConnectFlag(TileData tileData)
    {
        var changingID = Int32.Parse(tileData.typeTile[2].ToString());
        var flagGroup = changedFlagGroup[changingID];
        for (int i = 0; i < flagGroup.Count; i++)
        {
            FlagTileChangeTypeTile(flagGroup[i]);
        }
    }

    void ChangeConnectBombMarked(TileData tileData)
    {
        var bombID = Int32.Parse(tileData.typeTile[1].ToString());
        var maredGroup = bombMarkedTileGroup[bombID];
        for (int i = 0; i < maredGroup.Count; i++)
        {
            TileHidingChangeTypeTile(maredGroup[i]);
        }
    }
    void MovingTileChangeTypeTile() // index 2 in typetile
    {
        for (int i = 0; i < movingTileGroup.Count; i++)
        {
            var mtg = movingTileGroup.ElementAt(i);
            mtg.Value[0] = _allCells[mtg.Value[0].x, mtg.Value[0].y];
            
            bool tileMovingBack = Int32.Parse(mtg.Value[0].typeTile[3].ToString()) == 1;
            int tileShowing = Int32.Parse(mtg.Value[0].typeTile[2].ToString());
            
            if (!tileMovingBack)
            {
                if(tileShowing >= mtg.Value.Count - 1) // set xem co phai dang o muc lon nhat co the tien len khong
                {
                    tileMovingBack = true; // neu co thi phai lui lai
                    tileShowing--;
                }
                else
                {
                    tileShowing++; // neu khong tiep tuc tien len
                }
            }
            else
            {
                if (tileShowing <= 0) // set xem co phai dang o muc nho nhat co the lui xuong khong
                {
                    tileMovingBack = false; // neu co thi phai tien len
                    tileShowing++;
                }
                else
                {
                    tileShowing--; // neu khong tiep tuc lui xuong
                }
            }

            int tileDirection = tileMovingBack? 1 : 0;
            
            char[] charArray = mtg.Value[0].typeTile.ToCharArray();
            
            charArray[2] = tileShowing.ToString()[0];
            charArray[3] = tileDirection.ToString()[0];
            
            for (int j = 0; j < mtg.Value.Count; j++)
            {
                charArray[5] = j.ToString()[0];
                string newTypeTile = new string(charArray);
                ChangeTypeTile(mtg.Value[j], newTypeTile);
            }
        }
    }
    int[] MovingTileOn(string movingTileOnID) 
    {
        for (int i = 0; i < movingTileGroup.Count; i++)
        {
            var mtg = movingTileGroup.ElementAt(i);
            mtg.Value[0] = _allCells[mtg.Value[0].x, mtg.Value[0].y];
            var movingtileID = mtg.Value[0].typeTile[4].ToString();
            if (movingTileOnID == movingtileID)
            {
                var tile = mtg.Value.FirstOrDefault(tile => _allCells[tile.x, tile.y].typeTile[2] == _allCells[tile.x, tile.y].typeTile[5]);
                return new[] { _allCells[tile.x, tile.y].x, _allCells[tile.x, tile.y].y };
            }
        }

        return new[]{ 100, 100};
    }

    void ChangeTypeTile(TileData tileData, string newTypeTile)
    {
        int tileData_x = tileData.x;
        int tileData_y = tileData.y;
        _allCells[tileData_x, tileData_y].typeTile = newTypeTile;
    }

    bool ISTileCanMoveOn(string typeTile)
    {
        if (typeTile == "20")
        {
            return false;
        }
        else if (typeTile[0] == '5')
        {
            int tileShowing = Int32.Parse(typeTile[2].ToString());
            int tileID = Int32.Parse(typeTile[5].ToString());
            if (tileShowing != tileID)
            {
                return false;
            }
        }
        return true;
    }
    void SortBombMarkedIntoGroup()
    {
        foreach (var bomb in bombTiles)
        {
            int bombId = Int32.Parse(bomb.typeTile[1].ToString());

            var targetTiles = bombMarkedTiles
                .Where(tile => tile.typeTile[1].ToString() == bombId.ToString())
                .ToList();

            if (targetTiles.Any())
            {
                bombMarkedTileGroup.TryAdd(bombId,new List<TileData>(targetTiles));
            }
        }
    }
    void SortFlagChangedIntoGroup()
    {
        foreach (var flagChangingTile in flagChaningTiles)
        {
            int flagChangingDirection = Int32.Parse(flagChangingTile.typeTile[1].ToString());
            int flagChangingID = Int32.Parse(flagChangingTile.typeTile[2].ToString());

            var targetTiles = flagTiles
                .Where(tile =>
                    (flagChangingDirection == 0 && tile.x == _allCells[flagChangingTile.x, flagChangingTile.y].x) ||
                    (flagChangingDirection == 1 && tile.y == _allCells[flagChangingTile.x, flagChangingTile.y].y))
                .ToList();

            if (targetTiles.Any())
            {
                changedFlagGroup.TryAdd(flagChangingID,new List<TileData>(targetTiles));
                //Debug.Log("changedFlagGroup"+changedFlagGroup[flagChangingID].Count);
            }
        }
    }

    void SortMovingTileIntoMovingTileGroup()
    {
        for (int i = 0; i < movingTiles.Count; i++)
        {
            var movingTileID = Int32.Parse(movingTiles[i].typeTile[4].ToString());
            if (!movingTileGroup.TryAdd(movingTileID, new List<TileData> { movingTiles[i] }))
            {
                movingTileGroup[movingTileID].Add(movingTiles[i]);
            }
        }
        SortValueInMovingTileGroupByIdMarked();
    }

    void SortValueInMovingTileGroupByIdMarked()
    {
        for (int i = 0; i < movingTileGroup.Count; i++)
        {
            var mtg = movingTileGroup.ElementAt(i);
            var sortedTiles = mtg.Value.OrderBy(t => Int32.Parse(t.typeTile[5].ToString())).ToList();
            movingTileGroup[movingTileGroup.ElementAt(i).Key] = sortedTiles;
        }
    }
     void LoadTileVisual()
    {
        for (int i = 0; i < _allCells.GetLength(0); i++)
        {
            for (int j = 0; j < _allCells.GetLength(1); j++)
            {
                var obj = _allCells[i, j].ob;
                if (obj == null)
                {
                    Debug.Log("obj"+1);

                }
                List<Transform> child = obj.transform.Cast<Transform>().ToList();
                child.ForEach(child => child.gameObject.SetActive(false));
                string tileType = _allCells[i, j].typeTile;
                char firstCharTileType = _allCells[i, j].typeTile[0];
                switch (firstCharTileType)
                {
                    case '0':
                        FindingObjectAndShow(child, "normal");
                        break;
                    case '1':
                        string flagName = tileType == "10" ? "blueflag" : "greenflag";
                        FindingObjectAndShow(child, flagName);
                        break;
                    case '2':
                        if (tileType.Length == 1)
                        {
                            FindingObjectAndShow(child, "disappearing");
                        }
                        break;
                    case '3':
                        FindingObjectAndShow(child, "wormhole");
                        break;
                    case '4':
                        string flagChangingType = tileType.Substring(0, 2);
                        string flagChangingName = flagChangingType == "40" ? "doc" : "ngang";
                        FindingObjectAndShow(child, flagChangingName);
                        break;
                    case '5':
                        int tileShowing = Int32.Parse(tileType[2].ToString());
                        int tileID = Int32.Parse(tileType[5].ToString());
                        if (tileShowing == tileID)
                        {
                            FindingObjectAndShow(child, "moving");
                        }
                        break;
                    case '6':
                        FindingObjectAndShow(child, "bomb");
                        break;
                    case '7':
                        FindingObjectAndShow(child, "bombmarked");
                        break;
                    default:
                        break;
                }
            }
        }
        floppyPosition.ob.transform.position = new Vector3(floppyPosition.x, floppyPosition.y);    }
     
     void FindingObjectAndShow(List<Transform> child,string nameObject)
     {
         Transform obj = child.FirstOrDefault(achild => achild.gameObject.name == nameObject);
         if (obj != null)
         {
             obj.gameObject.SetActive(true);
         }
     }

    private void Update()
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
    }
    
    public void OnSwipeLeft() {
        FloppyMove(-1,0);
    }
    
    public void OnSwipeRight() {
        FloppyMove(1, 0);
    }
    
    public void OnSwipeTop() {
        FloppyMove(0,1);
    }
    
    public void OnSwipeBottom() {
        FloppyMove(0,-1);
        
    }

    private int counting = 0;
    
    public bool BackTracking(int countingg)
    {
        countingg++;
        if (Solved())
        {
            Debug.Log("Solved()");
            return true;
        }

        TileData[,] tempAllCells = _allCells;
        TileData tempFloppyPosition = floppyPosition;
        TileData tempDisappearingTile = disappearingTile;

        List<string> directions = new List<string>();
        
        for (int i = 1; i <= 4; i++)
        {
            switch (i)
            {
                case 1:
                    if (CheckCanMove(-1, 0))
                    {
                        directions.Add("left");
                    }
                    break;
                case 2:
                    if (CheckCanMove(1, 0))
                    {
                        directions.Add("right");
                    }
                    break;
                case 3:
                    if (CheckCanMove(0, 1))
                    {
                        directions.Add("top");
                    }
                    break;
                case 4:
                    if (CheckCanMove(0, -1))
                    {
                        directions.Add("bottom");
                    }
                    break;
            }
        }

        if (directions.Count == 0)
        {
            Debug.Log("directions = 0");
            return false;
        }
        foreach (string direction in directions)
        {
            switch (direction)
            {
                case "left":
                    Debug.Log("counting " + countingg +": left");
                    FloppyMove(-1, 0);
                    
                    if (BackTracking(countingg))
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("back");
                        _allCells = tempAllCells;
                        floppyPosition = tempFloppyPosition;
                        disappearingTile = tempDisappearingTile;
                        LoadTileVisual();
                    }
                    break;
                case "right":
                    Debug.Log("counting " + countingg +": right");
                    FloppyMove(1, 0);
                    
                    if (BackTracking(countingg))
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("back");
                        _allCells = tempAllCells;
                        floppyPosition = tempFloppyPosition;
                        disappearingTile = tempDisappearingTile;
                        LoadTileVisual();
                    }

                    break;
                case "top":
                    Debug.Log("counting " + countingg +": top");
                    FloppyMove(0, 1);

                    if (BackTracking(countingg))
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("back");
                        _allCells = tempAllCells;
                        floppyPosition = tempFloppyPosition;
                        disappearingTile = tempDisappearingTile;
                        LoadTileVisual();
                    }

                    break;
                case "bottom":
                    Debug.Log("counting " + countingg +": bottom");
                    FloppyMove(0, -1);

                    if (BackTracking(countingg))
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("back");
                        _allCells = tempAllCells;
                        floppyPosition = tempFloppyPosition;
                        disappearingTile = tempDisappearingTile;
                        LoadTileVisual();
                    }
                    break;
            }
        }

        return false;
    }
    public bool Solved()
    {
        for (int i = 0; i < flagTiles.Count; i++)
        {
            if (_allCells[flagTiles[i].x, flagTiles[i].y].typeTile == "10")
            {
                return false;
            }
        }
        return true;
    }
    bool CheckCanMove(int direction_X, int direction_Y)
    {
        var nextX = floppyPosition.x + direction_X;
        var nextY = floppyPosition.y + direction_Y;
        bool canMove = CheckTileJumpOn(direction_X, direction_Y, ref nextX, ref nextY);
        return canMove;
    }
    bool CheckTileJumpOn(int direction_X , int direction_Y, ref int tileX,  ref int tileY)
    {
        if (tileX < 5 && tileX >= 0 && tileY >= 0 && tileY < 5) // && _allCells[nextX, nextY].typeTile != 20)
        {
            TileData tile = _allCells[tileX, tileY];
            char tileType = tile.typeTile[0];
            
            if (tileType == '1')
            {
                tileX += direction_X;
                tileY += direction_Y;
                return CheckTileJumpOn(direction_X, direction_Y, ref tileX, ref tileY);
            }
            
            if (ISTileCanMoveOn(tile.typeTile))
            {
                return true;
            }
        }
        return false;
    }
}
