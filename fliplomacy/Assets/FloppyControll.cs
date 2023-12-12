using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class FloppyControll : MonoBehaviour
{
    public float timeInAAnimLoop = 1f;
    [HideInInspector] public GameObject floopySprite;
    public event Action EndJump;

    public AnimationCurve Curve;
    //private GameObject floppyOnStart;
    //private GameObject floppyOnJump;
    private void Awake()
    {
        canswipe = false;

    }
    public void SetUp()
    {
        
        var sprite = gameObject.transform.GetChild(0).gameObject;
        sprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        floopySprite = Instantiate(sprite,  new Vector3(0,0,-2), quaternion.identity);
        floopySprite.gameObject.SetActive(true);

    }
    public void ClearLevel()
    {
        canswipe = false;
        floppyInWormHole = false;
        inMovingTile = false;
        stillInMovingTile = false;
        StopIdelAnim();
        if (floopySprite != null)
        {
            Destroy(floopySprite);
        }
        
    }
    IEnumerator floppyMove;
    IEnumerator floppyScale;
    public IEnumerator RePlayFloppyIdelAnim()
    {
        StartCoroutine("FloppyIdelAnim");
        while (true)
        {
            yield return new WaitForSeconds(timeInAAnimLoop * 2);
            StartCoroutine("FloppyIdelAnim");
        }
    }
    

    public IEnumerator FloppyIdelAnim()
    {
        FloopySpriteScale(timeInAAnimLoop, new Vector3(0.4f, 0.4f, 0.4f));

        yield return new WaitForSeconds(timeInAAnimLoop);

        FloopySpriteScale(timeInAAnimLoop, new Vector3(0.5f, 0.5f, 0.5f));
    }
    public void StartIdelAnim()
    {
        StartCoroutine("RePlayFloppyIdelAnim");
    }
    public void StopIdelAnim()
    {
        StopCoroutine("FloppyIdelAnim");
        StopCoroutine("RePlayFloppyIdelAnim");

    }
    public bool canswipe = true;

    [Obsolete]
    public void StartJumpAnim()
    {
        floopySprite.transform.position = gameObject.transform.position -  new Vector3(0,0,2);

        StartCoroutine("Shake");
        //StopIdelAnim();


        //FloopySpriteScale(0.1f, new Vector3(0.2f, 0.2f, 0.2f));

        //EffectWhenEndMove(0.6f,0.2f);
    }

    public void FloppyReScale()
    {
        //StartCoroutine(EndStartJumpAnim());
    }

    IEnumerator EndStartJumpAnim()
    {
        FloopySpriteScale(0.1f, new Vector3(0.5f, 0.5f, 0.5f));

        yield return new WaitForSeconds(0.1f);
        StartIdelAnim();
        Debug.Log("EndStartJumpAnim");



    }
    public bool floppyInWormHole = false;
    public bool inMovingTile = false;
    public Vector3 movingTilePos = new Vector3();
    public float wormholeStartPosX;
    public float wormholeStartPosY;

    [Obsolete]
    public void JumpAnim()
    {
        canswipe = false;
        if (!floopySprite.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            floopySprite.transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log("ShowTail");
        }
        StartCoroutine(0.03f.DelayedAction(StartJump));
    }

    public void StartJump()
    {
        //yield return new WaitForSeconds(0.03f);
        StopCoroutine("Shake");
        Jump();
    }

    public void Jump()
    {
        Debug.Log(1);
        StopIdelAnim();
        FloopySpriteScale(0.17f, new Vector3(0.5f, 0.5f, 0.5f));

        if (!floppyInWormHole)
        {
            if (!inMovingTile) 
            {
                FloopySpriteMove(gameObject.transform.position - new Vector3(0,0,2)); 
            }
            else
            {
                FloopySpriteMove(movingTilePos - new Vector3(0, 0, 2));
            }

            StartCoroutine(FloppySpriteEndJumpAnim(0.2f));
        }
        else
        {
            floopySprite.transform.GetChild(0).gameObject.SetActive(false);
            FloopySpriteMove(new Vector3(wormholeStartPosX, wormholeStartPosY, floopySprite.transform.position.z));
            //StartCoroutine(TeleportThroughWormholes());
            StartCoroutine(0.2f.DelayedAction(TeleportThroughWormholes));
        }
    }

    void TeleportThroughWormholes()
    {
        //yield return new WaitForSeconds(0.2f);
        var floopySpriteWormHoleClone = Instantiate(floopySprite, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2), quaternion.identity);

        StartCoroutine(0.5f.Tweeng((p) => floopySpriteWormHoleClone.transform.localScale = p,
            new Vector3(0f, 0f, 0f),
            new Vector3(0.5f, 0.5f, 0.5f)));

        FloopySpriteScale(0.5f, new Vector3(0f, 0f, 0f));

        //StartCoroutine(EndTeleport(floopySpriteWormHoleClone));
        
    }
    IEnumerator EndTeleport(GameObject clone)
    {
        yield return new WaitForSeconds(0.5f);
        floppyInWormHole = false;
        canswipe = true;
        Debug.LogError("EndTeleport  set can Swipe true");
        floopySprite.transform.position = gameObject.transform.position - new Vector3(0, 0, 2);
        floopySprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(clone);
        StartIdelAnim();
        EventEndJump();
    }


    [Obsolete]
    public void EffectWhenEndMove(float size, float lifetime, ParticleSystem.MinMaxCurve rotate)
    {
        GameObject waveEfect = WavePool.SharedInstance.GetPooledObject();
        if (waveEfect != null)
        {
            waveEfect.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
            waveEfect.GetComponent<ParticleSystem>().startSize = size;
            waveEfect.GetComponent<ParticleSystem>().startLifetime = lifetime;
            var main = waveEfect.GetComponent<ParticleSystem>().main;
            main.startRotation = rotate;
            waveEfect.gameObject.SetActive(true);
        }
    }
    public void EventEndJump()
    {
        EndJump?.Invoke();
    }

    public bool stillInMovingTile = false;
    [Obsolete]
    IEnumerator FloppySpriteEndJumpAnim(float time)
    {
        StartCoroutine(CreateEffect(time - 0.1f));
        yield return new WaitForSeconds(time);
        StartIdelAnim();
        if (!inMovingTile)
        {
            canswipe = true;
            Debug.LogError("FloppySpriteEndJumpAnim  set can Swipe true");
            if (!floppyInWormHole)
            {
                EventEndJump();
            }

        }
        else
        {
            FloopySpriteMove(gameObject.transform.position - new Vector3(0, 0, 2));
            StartCoroutine(WaitMovingTileEnd());

            StartCoroutine(WaitMovingTile());
        }

        if (stillInMovingTile)
        {
            StartCoroutine(WaitMovingTileEnd());

            StartCoroutine(WaitMovingTile());
        }
    }


    [HideInInspector] public string swipeDirection;
    public int minCurve = 20;
    IEnumerator CreateEffect(float time)
    {
        yield return new WaitForSeconds(time);
        var rotateEffect = new ParticleSystem.MinMaxCurve();
        if (swipeDirection == "Top")
        {
            rotateEffect = new ParticleSystem.MinMaxCurve((0 -minCurve)*Mathf.Deg2Rad, (0 +minCurve)*Mathf.Deg2Rad);
        }
        if (swipeDirection == "Right")
        {
            rotateEffect = new ParticleSystem.MinMaxCurve((90-minCurve)*Mathf.Deg2Rad, (90+minCurve)*Mathf.Deg2Rad);
        }
        if (swipeDirection == "Bottom")
        {
            rotateEffect = new ParticleSystem.MinMaxCurve((180-minCurve)*Mathf.Deg2Rad, (180+minCurve)*Mathf.Deg2Rad);
        }
        if (swipeDirection == "Left")
        {
            rotateEffect = new ParticleSystem.MinMaxCurve((-90-minCurve)*Mathf.Deg2Rad, (-90 +minCurve)*Mathf.Deg2Rad);
        }
        if (!inMovingTile)
        {
            // Effect
            EffectWhenEndMove(1.1f,0.5f, rotateEffect);
        }
    }
    public IEnumerator WaitMovingTile()
    {
        yield return new WaitForSeconds(0.2f);
        canswipe = true;
        Debug.LogError("WaitMovingTile  set can Swipe true");
        EventEndJump();
    }

    public IEnumerator WaitMovingTileEnd()
    {
        yield return new WaitForSeconds(0.2f);
        StartIdelAnim();
        inMovingTile = false;
        stillInMovingTile = false;
    }
    void FloopySpriteMove(Vector3 pos)
    {
        if (floppyMove != null)
        {
            StopCoroutine(floppyMove);
        }
        floppyMove = TweengMove(0.17f, floopySprite.transform.position, pos);
        StartCoroutine(floppyMove);
    }
    void FloopySpriteScale(float time, Vector3 scale)
    {
        if (floppyScale != null)
        {
            StopCoroutine(floppyScale);
        }
        floppyScale = TweengScale(time, floopySprite.transform.localScale, scale);
        StartCoroutine(floppyScale);
    }
    public void StopAllAnim()
    {
        floopySprite.gameObject.SetActive(false);
        var sprite = gameObject.transform.GetChild(0).gameObject;
        sprite.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        var clone =Instantiate(sprite, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -2), quaternion.identity);
        clone.gameObject.SetActive(true);
        StartCoroutine(0.5f.Tweeng((p) => clone.transform.localScale = p, clone.transform.localScale, new Vector3(0, 0, 0)));
    }
    public IEnumerator TweengScale(float duration, Vector3 aa, Vector3 zz)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT && floopySprite != null)
        {
            float t = (Time.time - sT) / duration;
             floopySprite.transform.localScale = Vector3.Lerp(aa, zz, Curve.Evaluate(Mathf.SmoothStep(0f, 1f, t)) );
       
            yield return null;
        }
        if(floopySprite != null)
        {
            floopySprite.transform.localScale = zz;
        }
         
    }
    public IEnumerator TweengMove(float duration, Vector3 aa, Vector3 zz)
    {
        float sT = Time.time;
        float eT = sT + duration;

        while (Time.time < eT && floopySprite != null)
        {
            float t = (Time.time - sT) / duration;
            floopySprite.transform.position = Vector3.Lerp(aa, zz, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        if (floopySprite != null)
        {
            floopySprite.transform.position = zz;
        }
       
    }

    public float durationShake;
    public float magntiudeShake;
    public IEnumerator Shake()
    {
        Vector3 originalPos = floopySprite.transform.position;
        float elapsed = 0.0f;
        while (elapsed < durationShake && floopySprite != null)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magntiudeShake;
            float y = UnityEngine.Random.Range(-1f, 1f) * (magntiudeShake/2);
            floopySprite.transform.position = new Vector3(originalPos.x +x, originalPos.y+y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (floopySprite != null)
        {
            floopySprite.transform.position = originalPos;
        }
       
    }

}
