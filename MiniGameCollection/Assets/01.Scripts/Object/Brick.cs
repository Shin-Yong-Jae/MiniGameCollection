using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Brick : MonoBehaviour
{
    public SpriteRenderer spriteBrick;

    int max;
    public int count;
    public TextMeshPro textCount;

    public int type; // 0~3 세모,4 = stage%10 일반 몬스터 ,5 = stage %10 보스 몬스(hp 2배 color 보라색)

    //생성시 정보 Initialize
    public void Initilize(int _type, int _nowStage)
    {
        var t = TransformBricks.Transform;

        type = _type;
        //boss
        if (_type == 5)
        {
            count = _nowStage * 2;
            max = _nowStage * 2;
        }
        else
        {
            count = _nowStage;
            max = _nowStage;
        }

        textCount.text = count.ToString();
    }

    public void setColorBrick(int _max)
    {
        if (type == 5)
        {
            spriteBrick.color = new Color(100f / 255f, (float)(_max - count / 2) / _max * 255.0f / 255f, 255f / 255f);
            textCount.color = new Color(100f / 255f, (float)(_max - count / 2) / _max * 255.0f / 255f, 255f / 255f);
        }
        else
        {
            spriteBrick.color = new Color(255f / 255f, (float)(_max - count) / _max * 255.0f / 255f, 100f / 255f);
            textCount.color = new Color(255f / 255f, (float)(_max - count) / _max * 255.0f / 255f, 34f / 255f);
        }
    }

    public void disCount(int _discount)
    {
        count -= _discount;
        textCount.text = count.ToString();

        setColorBrick(max);

        //벽돌 사망.
        if (count <= 0)
        {
            Destroy(this);
        }
    }
}


