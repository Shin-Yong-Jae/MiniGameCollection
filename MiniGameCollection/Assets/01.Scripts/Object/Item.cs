using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour
{
    public SpriteRenderer spriteItem;

    public int type; // 0 = + item, 1 = horizontal lazer, 2 = vertical lazer, 3 = jump item. 

    private BBManager bbManager;
    private Transform transformBricks;
    public bool isused = false;
    public void Start()
    {
        bbManager = FindObjectOfType<BBManager>();
        transformBricks = FindObjectOfType<TransformBricks>().gameObject.transform;
    }

    public void OnEnable()
    {
        isused = false;
    }


    //생성시 정보 Initialize
    public void Initilize(int _type)
    {
        type = _type;

        if (_type != 4)
            spriteItem.sprite = (Sprite)ResourceManager.instance.Load_Resource($"item_{type}", typeof(Sprite));
        else
            spriteItem.sprite = (Sprite)ResourceManager.instance.Load_Resource("game_coin", typeof(Sprite));
    }

    public void GetItem()
    {
        switch (type)
        {
            case 0:
                bbManager.AddBall();
                Destroy(this);
                break;
            case 1:
                Break_H(this.transform.position.y);

                //아이템 밟히는 이펙트 시퀀스.
                spriteItem.transform.DOScale(new Vector2(1.1f, 1.1f), 0.05f).OnComplete(() =>
                spriteItem.transform.DOScale(new Vector2(1.0f, 1.0f), 0.05f));
                break;
            case 2:
                Break_V(this.transform.position.x);

                //아이템 밟히는 이펙트 시퀀스.
                spriteItem.transform.DOScale(new Vector2(1.1f, 1.1f), 0.05f).OnComplete(() =>
                spriteItem.transform.DOScale(new Vector2(1.0f, 1.0f), 0.05f));
                break;
            case 3:
                //아이템 밟히는 이펙트 시퀀스.
                spriteItem.transform.DOScale(new Vector2(1.1f, 1.1f), 0.05f).OnComplete(() =>
                spriteItem.transform.DOScale(new Vector2(1.0f, 1.0f), 0.05f));
                break;
            case 4:
                bbManager.GetCoin();
                Destroy(this);
                break;
        }
    }

    // Laser Horizontal
    public void Break_H(float _positionY)
    {
        for (int i = transformBricks.childCount - 1; i >= 0; i--)
        {
            float positionY = transformBricks.GetChild(i).transform.position.y;

            if (transformBricks.GetChild(i).tag.Equals("Brick") && positionY + 10 > _positionY && _positionY > positionY - 10)
            {
                transformBricks.GetChild(i).GetComponent<Brick>().disCount(1);
            }
        }
    }

    // Laser Vertical
    public void Break_V(float _positionX)
    {
        for (int i = transformBricks.childCount - 1; i >= 0; i--)
        {
            float positionX = transformBricks.GetChild(i).transform.position.x;

            if (transformBricks.GetChild(i).tag.Equals("Brick") && positionX + 10 > _positionX && _positionX > positionX - 10)
            {
                transformBricks.GetChild(i).GetComponent<Brick>().disCount(1);
            }
        }
    }
}
