using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using DG;
using DG.Tweening;

public class Probability : MonoBehaviour
{
    public Image trunPlate;
    public Button startBtn;
    private int curSeed = 1;
    private AnimationCurve _curve;
    private void Start()
    {
        #region 洗牌
        List<int> intList = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            intList.Add(i+1);
        }
        Shuffle(ref intList);
        for (int i = 0; i < intList.Count; i++)
        {
           // Debug.Log(intList[i]);
        }
        #endregion

        _curve = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.098f,0.004f),
            new Keyframe(0.875f,0.987f),
            new Keyframe(0.919f,1f),
            new Keyframe(0.96f,1f),
            new Keyframe(1,1)
            );
    }

    void Shuffle(ref List<int> nums)
    {
        for (int i = 0; i < nums.Count; i++)
        {
            int temp = nums[i];
            int seed = Random.Range(0, nums.Count);
            nums[i] = nums[seed];
            nums[seed] = temp;
        }
    }

    public void StartTurnPlate()
    {
        int seed = Random.Range(1, 13);
        if (curSeed == seed)
            return;
        curSeed = seed;
        int rotateNum =Random.Range(8, 12);
        float angle = 360 - trunPlate.transform.Find(seed.ToString()).transform.localEulerAngles.z + 360 * rotateNum;
        float time = Random.Range(4, 6) + (seed - 1) /12;
        Debug.Log($"开始转盘,结果为{seed},旋转角度为{angle},旋转时间为{time}");
        
        DOTween.To(() =>trunPlate.transform.localEulerAngles, x => trunPlate.transform.localEulerAngles = x, new Vector3(0,0,angle), time).SetEase(_curve);
    }
}
