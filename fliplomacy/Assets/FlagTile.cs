using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;

public class FlagTile : MonoBehaviour
{
    [HideInInspector] public GameObject flag;
    int flagStatus = 0;
    //private Color _color1;
    private Color _color;
    public void SetUP()
    {
        Instantiate(flag, gameObject.transform.position, Quaternion.identity).transform.SetParent(gameObject.transform);
        // SetColor();
        // gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>()
        //     .color = _color;

    }
    public void ChangeFlag()
    {
        //var flag = gameObject.transform.GetChild(1);
        //if (flagStatus == 0)
        //{
        //    flagStatus = 1;
        //    flag.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //    flag.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        //}
        //else
        //{
        //    flagStatus = 0;
        //    flag.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        //    flag.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        //}
        // var flag = gameObject.transform.GetChild(1);
        // StartCoroutine(0.1f.Tweeng((p) => flag.gameObject.transform.GetChild(0).gameObject.transform.localEulerAngles = p,
        //     flag.gameObject.transform.GetChild(0).gameObject.transform.localEulerAngles,
        //     flag.gameObject.transform.GetChild(0).gameObject.transform.localEulerAngles + new Vector3(0, 180, 0)));
        SetColor();
        TweenColor();

    }

    public void SetColor()
    {
        var flag = gameObject.transform.GetChild(1);
        if (flagStatus == 0)
        {
            flagStatus = 1;
            _color = flag.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color;
           // _color2 = flag.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color;
        }
        else
        {
            flagStatus = 0;
            _color = flag.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color;
            //_color2 = flag.gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color;
        }
    }
    private void TweenColor()
    {
        var flagsprite = gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
        var spriteRenderer = flagsprite.GetComponent<SpriteRenderer>();
        System.Action<ITween<Color>> updateColor = (t) =>
        {
            spriteRenderer.color = t.CurrentValue;
        };
        flagsprite.gameObject.Tween("ColorCircle", spriteRenderer.color, 
            _color, 0.1f, TweenScaleFunctions.QuadraticEaseOut, updateColor);
    }
}
