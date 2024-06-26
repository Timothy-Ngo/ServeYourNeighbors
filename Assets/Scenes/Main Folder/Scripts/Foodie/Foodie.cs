// Author: Helen Truong
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Foodie : MonoBehaviour
{

    [Header("-----FOODIE-----")]
    public FoodieMovement foodieMovement;
    public Collider2D foodieCollider;
    private SpriteRenderer sr;
    public Sprite foodieSprite;
    public Collider2D kidnapCollider;

    [Header("-----UI-----")]
    public GameObject orderBubble;
    public Timer timerScript;
    public TextMeshProUGUI distractedText;
    public Image timerBar;
    public Gradient gradient;

    [Header("-----TABLE-----")]
    public Table table;
    public Vector3 tablePosition;


    [Header("-----FOODIE SIGHT-----")]
    public GameObject foodieSight;
    public SpriteRenderer sightSR;
    public bool isScared;

    [Header("-----ORDERING SETTINGS-----")]
    public float orderTime = 28;

    public float timeAtOrderTaken;
    public Sprite order;
    public GameObject orderPrefab;
    public string orderName;

    [Header("-----EATING SETTINGS-----")]
    public int eatingTime = 12;

    [Header("-----DISTRACTION SETTINGS-----")]
    public int distractedTime = 2;
    public Timer distractionTimerScript;

    public bool isTutorial = false;

    private void Awake()
    {
        stateMachine = new FoodieStateMachine();
        lineState = new FoodieLineState(this, stateMachine);
        orderState = new FoodieOrderState(this, stateMachine);
        leaveState = new FoodieLeaveState(this, stateMachine);
        eatState = new FoodieEatState(this, stateMachine);
        distractedState = new FoodieDistractedState(this, stateMachine);
        kidnappedState = new FoodieKidnappedState(this, stateMachine);

        orderBubble.SetActive(false);
        orderBubble.GetComponentInChildren<SpriteRenderer>().sprite = order;
    }

    public void Start()
    {
        foodieMovement = GetComponent<FoodieMovement>();
        distractedText.enabled = false;
        sightSR.enabled = false;
        sr = GetComponent<SpriteRenderer>();
        foodieSprite = sr.sprite;
        timerBar = timerScript.gameObject.GetComponent<Image>();

        stateMachine.Initialize(lineState);

        // https://docs.unity3d.com/ScriptReference/Physics.IgnoreLayerCollision.html
        Physics2D.IgnoreLayerCollision(7, 8); // foodies ignore collision with player
        Physics2D.IgnoreLayerCollision(7, 7); // foodies ignore collision with other foodies
        Physics2D.IgnoreLayerCollision(2, 9); // foodies sight ignores collision with distraction area affect
    }

    void Update()
    {
        stateMachine.currentFoodieState.Update();

        if (sightSR.enabled != FoodieSystem.inst.sightToggleEnabled)
        {
            sightSR.enabled = FoodieSystem.inst.sightToggleEnabled;
        }
        
        timerBar.color = gradient.Evaluate(timerBar.fillAmount); // https://youtu.be/Po6lOgPjQwY?si=UXduAud6JiIlRMby&t=348
      
    }

    [SerializeField] public FoodieStateMachine stateMachine { get; set; }
    public FoodieLineState lineState { get; set; }
    public FoodieOrderState orderState { get; set; }
    public FoodieLeaveState leaveState { get; set; }
    public FoodieEatState eatState { get; set; }
    public FoodieDistractedState distractedState { get; set; }
    public FoodieKidnappedState kidnappedState { get; set; }

    public void DestroyFoodie()
    {
        GameLoop.inst.DecrementFoodieCountText();
        Destroy(gameObject);
    }
    public void HideUI()
    {
        orderBubble.SetActive(false);
        timerScript.gameObject.SetActive(false);
        distractedText.gameObject.SetActive(false);
        distractionTimerScript.gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if foodies are in distraction radius --> become distracted
        if (collision.gameObject.tag == "DistractionCircle" && stateMachine.currentFoodieState != distractedState && stateMachine.currentFoodieState != lineState)
        {
            Debug.Log("in range of distraction circle");
            stateMachine.ChangeState(distractedState);
        }
    }

    private void OnMouseOver()
    {
        sightSR.enabled = true;
    }

    private void OnMouseExit()
    {
        if (!FoodieSystem.inst.sightToggleEnabled)
            sightSR.enabled = false;
    }

    public void ActivateTutorial()
    {
        isTutorial = true;
    }

    public void DeactivateTutorial()
    {
        isTutorial = false;
    }



}
