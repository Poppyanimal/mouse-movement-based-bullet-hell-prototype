using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPattern : MonoBehaviour
{
    public Rigidbody2D bullet;
    public PatternData patternDat = new PatternData();
    bool currentlyRunning = false;
    
    void doPattern()
    {
        currentlyRunning = true;
        if(patternDat.autoSettings.doAutoPattern)
        {
            float angleModifier = 0f;
            if(patternDat.trackTarget && patternDat.targetToTrack != null)
            {
                angleModifier -= KiroLib.angleToTarget(patternDat.targetToTrack.position, gameObject.transform.position);
            }

            Vector2 startingVelocity = patternDat.autoSettings.startingVelocity;

            if(isCrazyMathDerivative)
            {
                angleModifier *= crazyMathAngleMultiplication();
                angleModifier += crazyMathAngleAddition();
                startingVelocity *= crazyMathVelocityMultiplication();
                startingVelocity += crazyMathVelocityAddition();
            }

            float angleBetween = 360f/patternDat.autoSettings.bulletQuantity;
            for(int i = 0; i < patternDat.autoSettings.bulletQuantity; i++)
            {
                createBullet(angleBetween*i + angleModifier, startingVelocity);
            }
            StartCoroutine(activityDecay(0.5f));
        }
        else
        {
            StartCoroutine(activityDecay(patternDat.patternSettings.activityDuration));
            StartCoroutine(doSimplePatternLogic(patternDat, 0));
        }
    }

    void createBullet(float angle, Vector2 vel, Rigidbody2D bulletToUse, float angularVelocity)
    {
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0,0,-angle);
        Rigidbody2D newBullet = Instantiate(bulletToUse, transform.position, rotation);
        newBullet.velocity = KiroLib.rotateVector2(angle, vel);
        newBullet.angularVelocity = angularVelocity;
    }

    void createBullet(float angle, Vector2 vel)
    {
        createBullet(angle, vel, bullet, 0f);
    }

    void createBullet(float angle, Vector2 vel, float angularVelocity)
    {
        createBullet(angle, vel, bullet, angularVelocity);
    }

    IEnumerator activityDecay(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        currentlyRunning = false;
    }

    IEnumerator doSimplePatternLogic(PatternData settings, int curPatID)
    {
        if(curPatID < settings.patternSettings.sPatternList.Count)
        {
            simplePatternInfo curPat = settings.patternSettings.sPatternList[curPatID];

            float angleModifier = curPat.angleOffset;
            SinPatternAngleInfo sAngInfo = curPat.SinInfo.SinAngleInfo;

            if(((curPat.TrackingOverrideChoice == simplePatternInfo.TrackingOverride.Ignore && patternDat.trackTarget)
                || curPat.TrackingOverrideChoice == simplePatternInfo.TrackingOverride.Yes_Tracking) && patternDat.targetToTrack != null)
            {
                angleModifier -= KiroLib.angleToTarget(patternDat.targetToTrack.position, gameObject.transform.position);
            }
            if(sAngInfo.useSinAngleOffset)
            {
                angleModifier += sAngInfo.amplitude*Mathf.Sin(curPat.SinInfo.getTimeSinceInit()/sAngInfo.period + sAngInfo.timeOffset);
            }

            Vector2 startingVelocity = curPat.startingVelocity;
            SinPatternVelocityInfo velInfo = curPat.SinInfo.SinVelocityInfo;

            if(velInfo.useSinVelocity == SinPatternVelocityInfo.SinVelocityUsage.x || velInfo.useSinVelocity == SinPatternVelocityInfo.SinVelocityUsage.both)
            {
                startingVelocity += new Vector2(velInfo.amplitude.x*Mathf.Sin(curPat.SinInfo.getTimeSinceInit()/velInfo.period.x + velInfo.timeOffset.x), 0);
            }
            if(velInfo.useSinVelocity == SinPatternVelocityInfo.SinVelocityUsage.y || velInfo.useSinVelocity == SinPatternVelocityInfo.SinVelocityUsage.both)
            {
                startingVelocity += new Vector2(0, velInfo.amplitude.y*Mathf.Sin(curPat.SinInfo.getTimeSinceInit()/velInfo.period.y + velInfo.timeOffset.y));
            }

            patternDrift driftInfo = settings.patternSettings.sPatternList[curPatID].driftInfo;
            if(driftInfo.doAngleDrift)
            {
                angleModifier += driftInfo.getCurrentDrift();
                driftInfo.drift();
            }

            float angularVelocityModifier = curPat.angularVelocity;

            if(isCrazyMathDerivative)
            {
                angleModifier *= crazyMathAngleMultiplication();
                angleModifier += crazyMathAngleAddition();
                startingVelocity *= crazyMathVelocityMultiplication();
                startingVelocity += crazyMathVelocityAddition();
                angularVelocityModifier *= crazyMathAngularVelocityMultiplication();
                angularVelocityModifier += crazyMathAngularVelocityAddition();
            }


            for(int i = 0; i < curPat.bulletQuantity; i++)
            {
                if(curPat.bulletOverride == null)
                    createBullet(angleModifier + curPat.angleBetweenBullets * i, startingVelocity, angularVelocityModifier);
                else
                    createBullet(angleModifier + curPat.angleBetweenBullets * i, startingVelocity, curPat.bulletOverride, angularVelocityModifier);
            }

            if(settings.patternSettings.timeBetweenBullets > 0)
                yield return new WaitForSeconds(settings.patternSettings.timeBetweenBullets);
            StartCoroutine(doSimplePatternLogic(settings, curPatID + 1));
        }
    }

    public bool isRunning()
    {
        return currentlyRunning;
    }

    public void tryPattern()
    {
        if(!currentlyRunning)
            doPattern();
    }




    protected bool isCrazyMathDerivative = false;
    protected virtual float crazyMathAngleAddition()
    {
        return 0f;
    }

    protected virtual float crazyMathAngleMultiplication()
    {
        return 1f;
    }

    protected virtual Vector2 crazyMathVelocityAddition()
    {
        return Vector2.zero;
    }

    protected virtual Vector2 crazyMathVelocityMultiplication()
    {
        return new Vector2(1, 1);
    }

    protected virtual float crazyMathAngularVelocityAddition()
    {
        return 0f;
    }

    protected virtual float crazyMathAngularVelocityMultiplication()
    {
        return 1f;
    }

}
