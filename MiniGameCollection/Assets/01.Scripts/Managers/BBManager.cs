using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BBManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private int nowStage = 1;

    [SerializeField] Text textNowStage;
    public Text textBallCount;

    [Header("Parent Transform")]
    [SerializeField] private Transform transformBricks;
    [SerializeField] private Transform transformBalls;
    [SerializeField] private Transform transformParticle;

    [Header("Object")]
    [SerializeField] private GameObject objectItem;
    [SerializeField] private GameObject[] objectBrick;
    [SerializeField] private GameObject objectBall;

    public Image imageFakeBall;
    // Ingame Variance
    public int ballSpeed = 0;
    public int nowBall = 1;
    public int touchCount = 0;
    public int stop_ball_count = 0;
    public int itemCount = 0;
    public bool isGaming = false;
    public bool is_ballstop = false;
    public Vector2 firstStopBallPosition;
    public Vector2 pre_touch_position;
    public int ball_sprite;

    private int nowBallCount = 0;
    private int addBallCount = 0;
    private int shootBallCount = 0;
    private int first_arrive_tag = 0;
    private float touch_distance = 0.0f;
    private bool is_arrow = false;
    private bool is_ready = false;

    private int brick_count = 0;
    private Vector2 startBallPosition;
    private Vector2 start_ball_position = new Vector2(488,234);

    private Vector2 start_point;
    private Vector2 end_point;
    private bool is_pause = false;

    public float physicsworld_offset { get; private set; } = 628f;
    private float brick_offset = 13.5f;
    private float brick_width = 162f;
    #region Unity Method
    private void Awake()
    {
        InitStage();
    }
    #endregion

    private void InitStage()
    {
        if (PlayerPrefs.HasKey("BB_Stage") == false)
        {
            PlayerPrefs.SetInt("BB_Stage", 0);
            nowStage = PlayerPrefs.GetInt("BB_Stage");
        }
        else
        {
            nowStage = PlayerPrefs.GetInt("BB_Stage");
        }

        // Brick Setting.
        if (nowStage == 0)
            SetBricks();
        //else
        //dataManager.GetBricksFromDB();
        InitialValue();
        is_ballstop = true;
        isGaming = true;
    }

    public void InitialValue()
    {
        is_ballstop = false;
        nowBallCount = 1;
        addBallCount = 0;
        shootBallCount = 0;
        brick_offset = 13.5f;
        brick_width = 162.0f;
        brick_count = 0;
        itemCount = 0;
        nowStage = nowBallCount - 1;
        physicsworld_offset = 628;
        is_arrow = false;
        first_arrive_tag = -11;
        ballSpeed = 350;
        //top_point = 0;
        is_pause = false;
        isGaming = false;
        is_ready = false;
    }

    public void DownBricksLayer()
    {
        string point_numWithCommas = nowStage.ToString();
        int point_insertPosition = point_numWithCommas.Length - 3;

        while (point_insertPosition > 0)
        {
            point_numWithCommas.Insert(point_insertPosition, ",");
            point_insertPosition -= 3;
        }

        textNowStage.text = point_numWithCommas.ToString();

        Sequence bricksMovemnet = DOTween.Sequence();
        for (int i = transformBricks.childCount - 1; i >= 0; i--)
        {
            bricksMovemnet.Join(transformBricks.transform.GetChild(i).DOMove(new Vector2(transformBricks.transform.GetChild(i).transform.position.x, transformBricks.transform.GetChild(i).transform.position.y - 175.5f), 0.5f));
            //if (transformBricks.transform.GetChild(i).GetComponent<Brick>() != null)
            //transformBricks.transform.GetChild(i).GetComponent<Brick>().setColorBrick(dataManager.now_stage);
        }

        //GameOver Check.
        if (nowStage != 0)
            StartCoroutine(DelayAction(0.6f, CheckGameOver));
    }

    //Game Over Check
    public void CheckGameOver()
    {
        for (int i = transformBricks.childCount - 1; i >= 0; i--)
        {
            float positionY = transformBricks.transform.GetChild(i).transform.position.y;
            if (positionY < 450f && transformBricks.transform.GetChild(i).GetComponent<Item>() != null)
            {
                Destroy(transformBricks.transform.GetChild(i).gameObject);
            }
            else if (positionY < 450f && transformBricks.transform.GetChild(i).GetComponent<Brick>() != null)
            {
                //game over
                isGaming = false;

                int is_continue = PlayerPrefs.GetInt("is_continue");
                if (is_continue == 1)
                    GameOver();
                else
                    StopGame();

                break;
            }
            else
            {
                //vector_item.pushBack(bricksLayer->getChildren().at(i));
                //is_on_magnet = false;
            }
        }
    }

    public void StartShoot()
    {
        stop_ball_count = 0;
        shootBallCount = 0;
        touchCount = 0;
        start_ball_position = firstStopBallPosition;
        start_ball_position.x = Mathf.Clamp(start_ball_position.x, 30, 1212); // 1212 =  visiableSize.width -30
        pre_touch_position = new Vector2(0, 0);
        addBallCount = 0;
        first_arrive_tag = -11;

        SetAndShootBall();
    }

    public void SetBricks()
    {
        nowStage++;

        //이미 사용한 아이템 Release Pool
        for (int i = transformBricks.childCount - 1; i >= 0; i--)
        {
            if (transformBricks.GetChild(i).GetComponent<Item>() != null && transformBricks.GetChild(i).GetComponent<Item>().isused == true)
            {
                Destroy(transformBricks.GetChild(i).GetComponent<Item>().gameObject);
            }
        }

        nowBallCount = nowBallCount + addBallCount;


        //초기 세팅 필요.
        textBallCount.text = $"x{nowBallCount}";
        textBallCount.transform.position = new Vector2(firstStopBallPosition.x, firstStopBallPosition.y + 70f);

        int rand_plus_item = Random.Range(0, 7);
        // 8칸 중 한 칸을 비운 채 생성.
        for (int i = 0; i < 7; i++)
        {
            // item 소환 로직.
            if (rand_plus_item == i) // Item Type 0  => 한개는 
            {
                itemCount++;
                Item item = Instantiate(objectItem, transformBricks).GetComponent<Item>(); // GameObjectPoolManager.Instance.GetObject("Item").GetComponent<Item>();
                //SetPosition
                item.transform.position = new Vector3(brick_offset + brick_width / 2 + (brick_offset + brick_width) * i, 2208 - (brick_offset + brick_width / 2) - physicsworld_offset / 2 + (int)EventType.START_Y, 2f);
                //SetType
                item.Initilize(0);
            }
            else
            {
                int rand = Random.Range(0, 45 + nowStage / 4);

                if (rand < 4)
                {
                    itemCount++;
                    Item item = Instantiate(objectItem, transformBricks).GetComponent<Item>();
                    //SetPosition
                    item.transform.position = new Vector3(brick_offset + brick_width / 2 + (brick_offset + brick_width) * i, 2208 - (brick_offset + brick_width / 2) - physicsworld_offset / 2 + (int)EventType.START_Y, 2f);

                    item.Initilize(rand +1);
                }
                else if(rand >20)
                {
                    brick_count++;
                    int rand_brick = Random.Range(0, 25);

                    if (rand_brick < 4)
                    {
                        Brick brick = Instantiate(objectBrick[rand_brick], transformBricks).GetComponent<Brick>(); //GameObjectPoolManager.Instance.GetObject($"Brick_{rand_brick + 1}").GetComponent<Brick>();
                        //SetPosition
                        brick.transform.position = new Vector3(brick_offset + brick_width / 2 + (brick_offset + brick_width) * i, 2208 - (brick_offset + brick_width / 2) - physicsworld_offset / 2 + (int)EventType.START_Y, 2f);
                        //SetType
                        brick.Initilize(rand_brick, nowStage);
                    }
                    else
                    {
                        // 10스테이지 마다 보스 Brick 소환 로직.
                        if (nowStage % 10 == 0)
                        {
                            int rand_brick_double = Random.Range(0, 2);

                            if (rand_brick_double == 0)
                            {
                                Brick brick = Instantiate(objectBrick[4], transformBricks).GetComponent<Brick>();
                                //SetPosition
                                brick.transform.position = new Vector3(brick_offset + brick_width / 2 + (brick_offset + brick_width) * i, 2208 - (brick_offset + brick_width / 2) - physicsworld_offset / 2 + (int)EventType.START_Y, 2f);
                                //SetType
                                brick.Initilize(5, nowStage);
                            }
                            else
                            {
                                Brick brick = Instantiate(objectBrick[4], transformBricks).GetComponent<Brick>();
                                //SetPosition
                                brick.transform.position = new Vector3(brick_offset + brick_width / 2 + (brick_offset + brick_width) * i, 2208 - (brick_offset + brick_width / 2) - physicsworld_offset / 2 + (int)EventType.START_Y, 2f);
                                //SetType
                                brick.Initilize(4, nowStage);
                            }
                        }
                        else
                        {
                            Brick brick = Instantiate(objectBrick[4], transformBricks).GetComponent<Brick>();
                            //SetPosition
                            brick.transform.position = new Vector3(brick_offset + brick_width / 2 + (brick_offset + brick_width) * i, 2208 - (brick_offset + brick_width / 2) - physicsworld_offset / 2 + (int)EventType.START_Y, 2f);
                            //SetType
                            brick.Initilize(4, nowStage);
                        }
                    }
                }
            }
        }
        //생성 후 한칸 내려줌.
        DownBricksLayer();
    }

    public void GameOver()
    {
        PlayerPrefs.SetInt("is_continue", 0);

        //GameScenePopupManager.Instance.popupResult.ShowResult();
    }

    public void StopGame()
    {
        //GameScenePopupManager.Instance.popupContinue.Show();

        PlayerPrefs.SetInt("is_continue", 1);
        for (int i = transformBricks.childCount - 1; i >= 0; i--)
        {

            float positionY = transformBricks.GetChild(i).transform.position.y;

            if (positionY < 450f)
                Destroy(transformBricks.GetChild(i).gameObject);
        }
    }

    public void SetAndShootBall()
    {
        Debug.Log("Shoot");

        shootBallCount++;
        textBallCount.text = $"x{nowBallCount - shootBallCount}";

        ball_sprite = nowBall;

        var ball = Instantiate(objectBall, transformBalls).GetComponent<Ball>();
        ball.transform.localPosition = new Vector3(start_ball_position.x, start_ball_position.y, 5);
        ball.bbManager = this;
        ball.InitBall();

        if (ball.GetComponent<CircleCollider2D>() == null)
            ball.gameObject.AddComponent<CircleCollider2D>();
        

        int angle = (int)GetDegree(start_point, end_point);
        if (angle < 15)
            angle = 15;
        else if (angle > 165)
            angle = 165;

        double radian = angle * 3.141592 / 180;


            Vector2 velocity = new Vector2(-ballSpeed * Mathf.Cos((float)radian), ballSpeed * Mathf.Sin((float)radian));
            ball.rb.velocity = velocity * 5.0f;

        //if (shoot_ball_count == 1)
        //{
        //    imageInGameMonster.transform.DORotate(new Vector3(0, 0, -10), 0.1f);
        //}
        //if (now_ball_count == 1)
        //{
        //    imageInGameMonster.transform.DORotate(new Vector3(0, 0, 0), 0.1f).SetDelay(0.1f);
        //}

        if (shootBallCount == nowBallCount)
            ShootBallEnd();
        else
        {
            StartCoroutine(DelayAction(0.07f, SetAndShootBall));
        }
    }

    private void ShootBallEnd()
    {
        if (shootBallCount == 0)
            imageFakeBall.gameObject.SetActive(false);

        //if (nowBallCount != 1)
        //{
        //    imageInGameMonster.transform.DORotate(new Vector3(0, 0, 0), 0.1f);
        //}
    }

    public void AddBall()
    {
        addBallCount++;
    }

    public void GetCoin()
    {

    }

    public double GetDegree(Vector2 _from, Vector2 _to)
    {
        return Mathf.Atan2(_from.y - _to.y, _to.x - _from.x) * 180 / 3.1415926535;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 point = eventData.position;
        Debug.Log("Begin Drag");
        if (is_ballstop && is_pause == false && (point.y < 2208 - physicsworld_offset / 2 + 105))
        {
            start_point = point;
            end_point = point;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 point;
        Debug.Log("On Drag");
        if (eventData == null)
            point = end_point;
        else
            point = eventData.position;

        if (is_ballstop && !is_pause && (point.y < 2208 - physicsworld_offset / 2 + 105))
        {
            if (eventData != null)
                end_point = point;

            touch_distance = Mathf.Sqrt(Mathf.Pow(start_point.x - end_point.x, 2) + Mathf.Pow(start_point.y - end_point.y, 2));

            if (touch_distance > 40 && (end_point.y < start_point.y))
            {
                if (is_arrow == false)
                {
                    is_arrow = true;

                    //imageArrow.gameObject.SetActive(true);
                    //imageStartPoint.gameObject.SetActive(true);
                    //imageHelpLine.gameObject.SetActive(true);

                    //imageStartPoint.rectTransform.position = new Vector2(dataManager.start_point.x, dataManager.start_point.y);

                }

                int angle = (int)GetDegree(start_point, end_point);

                if (angle < 15)
                    angle = 15;
                else if (angle > 165)
                    angle = 165;

                //imageHelpLine.rectTransform.localScale = new Vector2(((float)dataManager.touch_distance / 400), ((float)dataManager.touch_distance / 400));
                //imageArrow.rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle);
                //imageHelpLine.rectTransform.rotation = Quaternion.Euler(0f, 0f, -angle);

            }

            else
            {
                if (is_arrow)
                {
                    //imageStartPoint.gameObject.SetActive(false);
                    //imageArrow.gameObject.SetActive(false);
                    //imageHelpLine.gameObject.SetActive(false);
                    is_arrow = false;
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 point;
        Debug.Log("End Drag");
        if (eventData == null)
            point = end_point;
        else
            point = eventData.position;

        if (is_ballstop && is_pause == false && (point.y < 2208 - physicsworld_offset / 2 + 105))
        {
            if (is_arrow)
            {
                is_ballstop = false;
                is_arrow = false;

                //imageStartPoint.gameObject.SetActive(false);
                //imageArrow.gameObject.SetActive(false);
                //imageHelpLine.gameObject.SetActive(false);

                //if (imageHand.gameObject.activeSelf)
                //    imageHand.gameObject.SetActive(false);

                StartShoot();
            }
        }
    }

    private IEnumerator DelayAction(float _time, System.Action _action)
    {
        yield return new WaitForSeconds(_time);
        _action();
        yield break;
    }
}
