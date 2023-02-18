using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] public Rigidbody2D rb;
    public SpriteRenderer spriteBall;

    private Transform transformBalls;
    private Transform transformBricks;
    public BBManager bbManager;
    bool isdead = false;
    private void Start()
    {
        transformBalls = TransformBalls.Transform;
        transformBricks = TransformBricks.Transform;
    }

    private void OnEnable()
    { 
        double radian =  100 * 3.141592 / 180;
        Vector2 velocity = new Vector2(-350 * Mathf.Cos((float)radian), 350 * Mathf.Sin((float)radian));
        rb.velocity = velocity * 5.0f;
        isdead = false;
    }

    public void InitBall()
    {
        spriteBall.sprite = (Sprite)ResourceManager.instance.Load_Resource($"ball_{bbManager.nowBall}", typeof(Sprite));
    }

    //충돌 처리 (item)
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //아이템을 만났을 때.
        if (collision.gameObject.tag.Equals("Item"))
        {
            var item = collision.gameObject.GetComponent<Item>();

            // 공이 닿자마자 사라지는 Item들.
            if (item.type == 0 || item.type == 4)
            {
                // +1 시퀀스.

                if (item.type == 0)
                {
                    // 폰트 Color = (218,235,104)
                }
                else
                {
                    // 폰트 Color = Yellow.
                }
            }
            else if (item.type == 1 || item.type == 2 || item.type == 3)
            {
                if (item.isused == false)
                    item.isused = true;

                // Laser Horizontal
                if (item.type == 1)
                {
                    // laser effect sequence.
                    //var laser = GameObjectPoolManager.Instance.GetObject("Laser_H");
                    //laser.transform.SetParent(transformParticles);
                    //laser.transform.position = new Vector2(621, collision.transform.position.y);
                }
                // Laser Vertical
                else if (item.type == 2)
                {
                    // laser effect sequence.
                    //var laser = GameObjectPoolManager.Instance.GetObject("Laser_V");
                    //laser.transform.SetParent(transformParticles);
                    //laser.transform.position = new Vector2(collision.transform.position.x, 1104 + (int)EventType.START_Y);
                }
                // 점프 아이템.
                else if (item.type == 3)
                {
                    int rand_angle = Random.Range(0, 180);
                    double radian = rand_angle * 3.141592 / 180;


                        Vector2 velocity = new Vector2(-bbManager.ballSpeed * Mathf.Cos((float)radian), bbManager.ballSpeed * Mathf.Sin((float)radian));
                        this.rb.velocity = velocity * 5.0f;
                    
                }
            }
            item.GetItem();
        }
    }

    //충돌 처리.
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //벽돌을 만났을 때.
        if (collision.gameObject.tag.Equals("Brick"))
        {
            var brick = collision.gameObject.GetComponent<Brick>();
            // Touch Count 초기화.
            bbManager.touchCount = 0;
            brick.disCount(1);
        }

        else if (collision.gameObject.tag.Equals("Wall"))
        {
            // 좌우 공회전 점프대 설치 조건.
            if (this.transform.position.y > bbManager.physicsworld_offset / 2 + 130 &&
                Mathf.Abs(bbManager.pre_touch_position.y - this.transform.position.y) < 50 &&
                bbManager.pre_touch_position.x + this.transform.position.x <= 1262 &&
                bbManager.pre_touch_position.x + this.transform.position.x >= 1222)
            {
                bbManager.touchCount++;
            }

            if (bbManager.touchCount > 7)
            {
                bbManager.itemCount++;
                bbManager.touchCount = 0;
                //점프대 설치.
                //Item item = GameObjectPoolManager.Instance.GetObject("Item").GetComponent<Item>();
                //item.transform.SetParent(transformBricks);
                ////SetPosition
                //item.transform.position = new Vector3(621, this.transform.position.y);
                ////SetType
                //item.Initilize(3);
            }

            bbManager.pre_touch_position = this.transform.position;
        }

        //바닥 조건.
        else if (collision.gameObject.tag.Equals("Floor") && !isdead)
        {
            //bool is_find = false;
            isdead = true;
            bbManager.stop_ball_count++;



            //처음 도착한 Ball (기준점이 됨)
            if (bbManager.stop_ball_count == 1)
            {

                bbManager.firstStopBallPosition = new Vector3(this.transform.position.x, bbManager.physicsworld_offset / 2 + (int)EventType.START_Y + this.spriteBall.sprite.rect.height / 16, 4);

                bbManager.imageFakeBall.gameObject.SetActive(true);


                bbManager.imageFakeBall.transform.position = bbManager.firstStopBallPosition;
                bbManager.textBallCount.transform.position = new Vector2(bbManager.firstStopBallPosition.x, bbManager.firstStopBallPosition.y + 70f);

                //gameSceneUI.imageInGameMonster.rectTransform.DOAnchorPosX(dataManager.first_stop_ball_position.x + 120f, 0.2f);
                //gameSceneUI.imageArrow.transform.position = dataManager.first_stop_ball_position;

                //Pool에 넣기 전 마지막 볼이였는지 체크.
                if (transformBalls.childCount == 1 && bbManager.isGaming)
                {
                    bbManager.is_ballstop = true;
                    bbManager.SetBricks();
                }

                Destroy(gameObject);
            }
            else
            {
                this.transform.DOMove(bbManager.firstStopBallPosition, 0.2f).OnComplete(() =>
                {
                    if (transformBalls.childCount == 1 && bbManager.isGaming)
                    {
                        bbManager.is_ballstop = true;
                        bbManager.SetBricks();
                    }

                    Destroy(gameObject);
                });

            }
        }
    }

    // Laser Horizontal
    public void Break_H(float _positionY)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
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
