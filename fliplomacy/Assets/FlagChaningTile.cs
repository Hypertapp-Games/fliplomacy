using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagChaningTile : MonoBehaviour
{
    [HideInInspector] public FloppyControll floppy;
    [HideInInspector] public int flagChaningType = 0;

    public List<FlagTile> flagTiles = new List<FlagTile>();
    public string direction;
    public GameObject FlagChaningSprite;
    GameObject flagChaning;
    Vector3 eular1;
    Vector3 eular2;

    public void SetUP()
    {
        floppy.EndJump += OnThingHappened;
        flagChaning = Instantiate(FlagChaningSprite, gameObject.transform.position, Quaternion.identity);
        flagChaning.transform.SetParent(gameObject.transform);
        eular1 = flagChaning.transform.localEulerAngles;
        if (flagChaningType == 1)
        {
            eular2 = flagChaning.transform.localEulerAngles + new Vector3(0, 0, -45);
            flagChaning.transform.localEulerAngles = eular2;
        }
        else
        {
            eular2 = flagChaning.transform.localEulerAngles + new Vector3(0, 0, -125);
            flagChaning.transform.localEulerAngles = eular2;
        }
        CheckDirectionWithFloppyFirstTime();
        
    }
    public void OnDestroy()
    {
        floppy.EndJump -= OnThingHappened;
    }
    bool rightdirection = false;
    void OnThingHappened()
    {
        Debug.Log(231);
        float distance = Vector2.Distance(gameObject.transform.position, floppy.gameObject.transform.position);
        //Debug.Log("hehe" + distance);
        if (distance == 1 || distance == 0)
        {
            if (!rightdirection)
            {
                StartCoroutine(0.15f.Tweeng((p) => flagChaning.transform.localEulerAngles = p, eular2, eular1));
                rightdirection = true;
            }

        }
        else
        {
            if(rightdirection)
            {
                StartCoroutine(0.15f.Tweeng((p) => flagChaning.transform.localEulerAngles = p,eular1, eular2));
                rightdirection = false;
            }
            
        }
    }
    public void CheckDirectionWithFloppyFirstTime()
    {
        float distance = Vector2.Distance(gameObject.transform.position, floppy.gameObject.transform.position);
        if (distance == 1 || distance == 0)
        {
            rightdirection = true;
            flagChaning.transform.localEulerAngles = eular1;
        }
        else
        {
            rightdirection = false;
            flagChaning.transform.localEulerAngles = eular2;
        }
    }

    public void ChangeFlag()
    {
        StartCoroutine(ChangeFlag(0.3f));
    }
    public IEnumerator ChangeFlag(float time)
    {
        var w = true;
        while (w)
        {
            int Condition = 0; 
            for (int i = 0; i < flagTiles.Count; i++)
            {
                if (!flagTiles[i].rotatingFlag)
                {
                    Condition++;
                }
            }
            if (Condition == flagTiles.Count)
            {
                for (int i = 0; i < flagTiles.Count; i++)
                {
                    flagTiles[i].ChangeFlag(direction);
                }

                StartCoroutine(FloppyReCanSwipe(time));
                w = false;
            }
            else
            {
                floppy.canswipe = false;
            }

            yield return null;
        }
    }

    public IEnumerator FloppyReCanSwipe(float time)
    {
        yield return new WaitForSeconds(time);
        floppy.canswipe = true;
    }
    //private List<Observer> _observers = new List<Observer>();

    //public void RegisterObserver(Observer observer)
    //{
    //    _observers.Add(observer);
    //}

    //public void UnregisterObserver(Observer observer)
    //{
    //    _observers.Remove(observer);
    //}

    //public void NotifyObservers()
    //{
    //    foreach (Observer observer in _observers)
    //    {
    //        observer.OnNotify();
    //    }
    //}
}
