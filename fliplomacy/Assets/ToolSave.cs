using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ToolSave : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject allCell;
    public List<ACell> cells = new List<ACell>();

    public GameObject Flags;
    public GameObject Disappearing;
    public GameObject Wormhole;
    public GameObject FlagChaningDoc;
    public GameObject FlagChaningNgang;
    public GameObject MovingTileSprite;
    public GameObject BombTileSprite;
    public GameObject BombMarketSprite;
    public GameObject NonTile;

    public GameObject selectedPanel;
    private Camera cam;

   


    void Start()
    {
        cam = Camera.main;
        //print(cam.name);
        for (int i = 0; i < allCell.transform.childCount; i++)
        {
            cells.Add((allCell.transform.GetChild(i).gameObject.GetComponent<ACell>()));
        }
    }

    private GameObject objInRay1;
    private GameObject objInRay2;

    public Color color1;
    public Color color2;
    public Color color3;
    public GameObject selectedCell;
    public GameObject selectedCellTemp;
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(cam.transform.position, mousePos - cam.transform.position, Color.blue);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool rayCastDown = Physics.Raycast(ray, out hit, 100);
        if (rayCastDown)
        {
           objInRay1 = hit.transform.gameObject;
          
            if (objInRay1 != objInRay2) //|| (objInRay1 == null && objInRay2 == null))
            {
                if (objInRay1 != selectedCell)
                {
                    objInRay1.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color2;
                }
                

                
                if (objInRay2 != null && objInRay2 != selectedCell)
                {
                    objInRay2.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color1;
                }

                objInRay2 = objInRay1;
            }
        }
        else
        {
            if (objInRay1 != null && objInRay1 != selectedCell)
            {
                objInRay1.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color1;
            }

            objInRay1 = null;

            if (objInRay2 != null && objInRay2 != selectedCell)
            {
                objInRay2.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color1;
            }

            objInRay2 = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //selectedCell = null;
            if (objInRay1 != null)
            {
                selectedCell = objInRay1;
            }
            
            if (selectedCell != null)
            {
                selectedCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color3;
                if (selectedCellTemp != selectedCell)
                {
                    if (selectedCellTemp != null)
                    {
                        selectedCellTemp.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color1;
 
                    }
                    selectedCellTemp = selectedCell;
                }
            }
           // selectedPanel.gameObject.SetActive(true);
        }
    }
    int secondInWormHoleID = 1;
    
    public void CheckSumWormHole()
    {
        int NumberWormHole = -1;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].cellID[0] == "3"[0])
            {
                NumberWormHole++;
                secondInWormHoleID = 1+ (int)NumberWormHole/2;
            }
        }
        selectedCell.GetComponent<ACell>().cellID = 3.ToString()+secondInWormHoleID.ToString();
    }
    //string a = "1";
    //string b = "2";
    //var c = a + b;
    //int d = Int32.Parse(c);
    //Debug.Log(d);
    //Debug.Log(c[0]);

    public void OnFlagCilck()
    {
        AddTileFunc(1.ToString(), Flags);
    }
    public void OnDisappearingCilck()
    {
    
        AddTileFunc(2.ToString(), Disappearing);
    }
    public void OnWormholeCilck()
    {
        AddTileFunc(3.ToString(), Wormhole);
        CheckSumWormHole();
    }

    public int ChangingTileID = 0;
    public TMP_Dropdown ChangingTileDirection;
    
    public void OnFlafChangingCilck()
    {
        string id = 4.ToString() + ChangingTileDirection.value.ToString()+ ChangingTileID;
        ChangingTileID++;
        if(id[1] == '0')
        {
            AddTileFunc(id, FlagChaningDoc);
        }
        else
        {
            AddTileFunc(id, FlagChaningNgang);
        }
        
    }

    public TMP_Dropdown MovingTileDirection;
    public TMP_InputField numberOfList;
    public TMP_InputField ShowPosition;
    public TMP_Dropdown MovingDirectionUporBack;
    int MovingTileID = 0;
    public void OnMovingCilck()
    {
        AddTileFunc(5.ToString(), MovingTileSprite);
        GenerateMovingTile();
    }
    public void GenerateMovingTile()
    {
        List<ACell> allMovingTile = new List<ACell>();
        int numberoflist = Int32.Parse(numberOfList.text);
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].cellID == "5")
            {
                for (int j = 0; j < numberoflist; j++)
                {
                    for (int k = 0; k < cells.Count; k++)
                    { 
                        if(MovingTileDirection.value == 0)
                        {
                            var cx = cells[k].idX;
                            var cy = cells[k].idY;
                            if (cx == cells[i].idX && cy == cells[i].idY - j)
                            {
                                allMovingTile.Add(cells[k]);
                            }
                        }
                        else
                        {
                            var cx = cells[k].idX;
                            var cy = cells[k].idY;
                            if (cx == cells[i].idX + j && cy == cells[i].idY )
                            {
                                allMovingTile.Add(cells[k]);
                            }
                        }
                    }
                }
                break;
            }
        }
        for (int i = 0; i < allMovingTile.Count; i++)
        {
            //550104
            string id = "5" + numberOfList.text + (Int32.Parse(ShowPosition.text) - 1).ToString() + MovingDirectionUporBack.value.ToString() + MovingTileID.ToString()+i.ToString() ;
            if (i == Int32.Parse(ShowPosition.text)-1)
            {
                AddTileFuncServe(id, MovingTileSprite, allMovingTile[i].gameObject);
            }
            else
            {
                AddTileFuncServe(id, NonTile, allMovingTile[i].gameObject);
            }
        }
        MovingTileID++;
        //selectedCell.GetComponent<ACell>().cellID = 3.ToString() + secondInWormHoleID.ToString();
    }
    public TMP_InputField BombID;
    public TMP_Dropdown BombMarkedID;

    List<string> BombIDpOption = new List<string> ();
    public void OnBombCilck()
    {
        var id = 6 + BombID.text;
        AddTileFunc(id, BombTileSprite);

        BombIDpOption.Add("BombID " + BombID.text);
        var BombIDpOptiontemp = BombIDpOption.Distinct().ToList();
        BombIDpOptiontemp.Add("1111");
        BombMarkedID.ClearOptions();
        BombMarkedID.AddOptions(BombIDpOptiontemp);

    }
    public void OnBombMarkedCilck()
    {
        if(BombMarkedID.options.Count > 0)
        {
            var bid = BombMarkedID.options[BombMarkedID.value].text[7].ToString();
            Debug.Log(bid) ;
            var id = 7 + bid;
            AddTileFunc(id, BombMarketSprite);
        }
        
    }

    public void OnNoneTileClick()
    {
        AddTileFunc(20.ToString(), NonTile);
    }
    public void OnDefaultTileClick()
    {
        
        selectedCell.GetComponent<ACell>().cellID = "0";
        if (selectedCell.transform.childCount > 1)
        {
            Destroy(selectedCell.transform.GetChild(1).gameObject);
        }
    }
    public void OnSaveClick()
    {

        allCell.GetComponent<AllCellDate>().Save();
    }
    public void AddTileFunc(string id, GameObject sprite)
    {
        if(selectedCell != null)
        {
            selectedCell.GetComponent<ACell>().cellID = id;
            if (selectedCell.transform.childCount > 1)
            {
                Destroy(selectedCell.transform.GetChild(1).gameObject);
                Instantiate(sprite, selectedCell.transform.position, quaternion.identity).transform.SetParent(selectedCell.transform);
            }
            else
            {
                Instantiate(sprite, selectedCell.transform.position, quaternion.identity).transform.SetParent(selectedCell.transform);
            }
        }
    }
    public void AddTileFuncServe( string id, GameObject sprite , GameObject objNeedChange)
    {
        if (objNeedChange != null)
        {
            objNeedChange.GetComponent<ACell>().cellID = id;
            if (objNeedChange.transform.childCount > 1)
            {
                Destroy(objNeedChange.transform.GetChild(1).gameObject);
                Instantiate(sprite, objNeedChange.transform.position, quaternion.identity).transform.SetParent(objNeedChange.transform);
            }
            else
            {
                Instantiate(sprite, objNeedChange.transform.position, quaternion.identity).transform.SetParent(objNeedChange.transform);
            }
        }
    }
}
