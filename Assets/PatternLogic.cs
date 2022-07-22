using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLogic : MonoBehaviour
{
    public List<ComplexPattern> phases = new List<ComplexPattern>() { new ComplexPattern() };
    protected int currentPhase = 0;

    void FixedUpdate()
    {
        //while active / enabled / later bool test, do logic
        doLogic();
    }


    protected virtual void doLogic()
    {
        if(!currentlyWaitingForDelay)
        {
            if(phases[currentPhase].isFinished())
            {
                phases[currentPhase].reset();
                currentPhase++;
                if(currentPhase >= phases.Count)
                    currentPhase = 0;
                currentlyWaitingForDelay = true;
                StartCoroutine(waitForDelay());
            }
            else
                phases[currentPhase].shootAllPatterns();
        }
    }

    protected bool currentlyWaitingForDelay = false;
    protected IEnumerator waitForDelay()
    {
        yield return new WaitForSeconds(phases[currentPhase].timeDelayAfterFinished);
        currentlyWaitingForDelay = false;
    }
}

[System.Serializable]
public class ComplexPattern
{
    public float timeDelayAfterFinished = 0.5f; //time after all patterns are finished before the next phase can occur

    public List<cPatternLoopInfo> simultanousPatterns = new List<cPatternLoopInfo>()
    {
        new cPatternLoopInfo()
    };

    public void reset()
    {
        for(int i = 0; i < simultanousPatterns.Count; i++)
        {
            simultanousPatterns[i].resetTimesShot();
        }
    }

    public void shootAllPatterns()
    {
        for(int i = 0; i < simultanousPatterns.Count; i++)
        {
            if(simultanousPatterns[i].timesShot() < simultanousPatterns[i].timesToShoot && !simultanousPatterns[i].bPattern.isRunning())
            {
                simultanousPatterns[i].bPattern.tryPattern();
                simultanousPatterns[i].incTimesShot();
            }
        }
    }

    public bool isFinished()
    {
        for(int i = 0; i < simultanousPatterns.Count; i++)
        {
            if(simultanousPatterns[i].timesShot() < simultanousPatterns[i].timesToShoot || simultanousPatterns[i].bPattern.isRunning())
                return false;
        }
        return true;
    }
}

[System.Serializable]
public class cPatternLoopInfo
{
    public int timesToShoot = 10;
    public BulletPattern bPattern = new BulletPattern();

    int timesAlreadyShot = 0;
    public void resetTimesShot() { timesAlreadyShot = 0; }
    public void incTimesShot() { timesAlreadyShot++; }
    public int timesShot() { return timesAlreadyShot; }
}