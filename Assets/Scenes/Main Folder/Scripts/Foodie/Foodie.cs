using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Foodie : MonoBehaviour
{

    [Header("-----FOODIE-----")]
    public FoodieMovement foodieMovement;
    public GameObject orderBubble;
    public Timer timerScript;
    public TextMeshProUGUI distractedText;
    public Collider2D foodieCollider;
    public Table table;
    public Vector3 tablePosition;

    [Header("-----ORDERING SETTINGS-----")]
    public int orderTime = 10;
    public Sprite order;

    [Header("-----DISTRACTION SETTINGS-----")]
    public int distractedTime = 2;
    public Timer distractionTimerScript;

    private void Awake()
    {
        stateMachine = new FoodieStateMachine();
        lineState = new FoodieLineState(this, stateMachine);
        orderState = new FoodieOrderState(this, stateMachine);
        leaveState = new FoodieLeaveState(this, stateMachine);
        eatState = new FoodieEatState(this, stateMachine);
        distractedState = new FoodieDistractedState(this, stateMachine);

        orderBubble.SetActive(false);
        orderBubble.GetComponentInChildren<SpriteRenderer>().sprite = order;

    }

    public void Start()
    {
        foodieMovement = GetComponent<FoodieMovement>();
        distractedText.enabled = false;

        stateMachine.Initialize(lineState);

        // https://docs.unity3d.com/ScriptReference/Physics.IgnoreLayerCollision.html
        Physics2D.IgnoreLayerCollision(7, 8); // foodies ignore collision with player
        Physics2D.IgnoreLayerCollision(7, 7); // foodies ignore collision with other foodies
    }

    public void Update()
    {
        stateMachine.currentFoodieState.Update();
        
    }

    public FoodieStateMachine stateMachine { get; set; }
    public FoodieLineState lineState { get; set; }
    public FoodieOrderState orderState { get; set; }
    public FoodieLeaveState leaveState { get; set; }
    public FoodieEatState eatState { get; set; }
    public FoodieDistractedState distractedState { get; set; }

    public void DestroyFoodie()
    {
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if foodies are in distraction radius --> become distracted
        if (collision.gameObject.tag == "DistractionCircle")
            stateMachine.ChangeState(distractedState);
    }

    // ---------------------------------------  OLD CODE  -------------------------------------------- //

    /*
    FoodieMovement foodieMovement;
    bool atFrontOfLine = false;
    public Vector3 seat;
    bool inSeat = false;
    bool orderTaken;

    GameObject orderBubble;
    Timer timerScript;
    public int timeBetweenActions = 2;

    bool inCycle = false;

    // Start is called before the first frame update
    void Start()
    {
        foodieMovement = GetComponent<FoodieMovement>();
        orderBubble = GameObject.Find("OrderBubble");
        orderBubble.SetActive(false);
        timerScript = FindObjectOfType<Timer>();

        

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Line Count: " + FoodieSystem.inst.line.Count);

        if (Input.GetKeyDown(KeyCode.H))
            GoToSeat();

        if (Input.GetKeyDown(KeyCode.J))
            OrderFood();

        if (Input.GetKeyDown(KeyCode.K))
            LeaveSeat();

        if (Input.GetKeyDown(KeyCode.I))
            StartCoroutine(DelaySitAndOrder(timeBetweenActions));
        
        if (!inCycle)
            StartCoroutine(DelaySitAndOrder(timeBetweenActions));
    }


    private void EnterLine()
    {
        Vector3 targetPosition;
        
        // if line is empty
        if (FoodieSystem.inst.line.Count == 0)
        {
            // move to the front of the line
            atFrontOfLine = true;
            targetPosition = FoodieSystem.inst.lineStartPosition;
        }
        else
        {
            // foodie goes into position based on the number of foodies in line
            int positionInLine = FoodieSystem.inst.line.Count;
            targetPosition = new Vector3(FoodieSystem.inst.lineStartPosition.x + positionInLine, FoodieSystem.inst.lineStartPosition.y);
        }
        Debug.Log("targetPosition" + targetPosition);
        FoodieSystem.inst.line.Enqueue(foodieMovement);
        foodieMovement.SetTargetPosition(targetPosition, FoodieSystem.inst.pathfinding); // moves foodie
    }

    // moves all foodies in line up in line
    private void MoveUpInLine()
    {
        // checks if foodie is in line
        if (FoodieSystem.inst.line.Contains(foodieMovement)) //&& FoodieSystem.inst.line.ElementAt(0).GetPosition().x > FoodieSystem.inst.lineStartPosition.x)
        {
            // moves foodie up one in line
            Vector3 moveUpPosition = new Vector3(transform.position.x - 1, transform.position.y);
            foodieMovement.SetTargetPosition(moveUpPosition, FoodieSystem.inst.pathfinding);

            if (moveUpPosition.x == FoodieSystem.inst.lineStartPosition.x)
                atFrontOfLine = true;
        }
    }



    private void GoToSeat()
    {
        // checks if there are available seats and if foodie is at the front of the line
        if (FoodieSystem.inst.availableSeats.Count > 0 && atFrontOfLine)
        {
            seat = FoodieSystem.inst.availableSeats.Dequeue(); // gets seat from available seats
            foodieMovement.SetTargetPosition(seat, FoodieSystem.inst.pathfinding); // moves to seat
            
            FoodieSystem.inst.line.Dequeue(); // removes foodie from line
            atFrontOfLine = false;
            
            inSeat = true;

            FoodieSystem.inst.lineUpdated = false; // resets lineUpdated

            
            
        }

        // BUG: lineUpdated isn't working -- foodies move anyway
        // prevents foodies in line from moving if the line has already moved
        if (FoodieSystem.inst.line.Count != 0 )//&& FoodieSystem.inst.line.ElementAt(0).GetPosition() != FoodieSystem.inst.lineStartPosition)
        {

            MoveUpInLine(); // moves foodies up in line
            FoodieSystem.inst.lineUpdated = true; // foodies have moved, so do not move again
            //Debug.Log(FoodieSystem.inst.line.ElementAt(0).GetPosition());
        }

    }

    private void LeaveSeat()
    {
        // checks if foodie is seated
        if (InSeat())
        {
            orderBubble.SetActive(false);
            inSeat = false; // removes foodie from seat
            FoodieSystem.inst.availableSeats.Enqueue(seat); // seat is available
            foodieMovement.SetTargetPosition(Vector3.zero, FoodieSystem.inst.pathfinding); // moves foodie out of restaurant


            //Destroy(gameObject); // destroying has to be on some kind of coroutine --> destroys instantly right now
        }
    }

    private void OrderFood()
    {
        if (InSeat())
        {
            orderBubble.SetActive(true);
            Debug.Log("Ordered Food");

            timerScript.SetMaxTime(timeBetweenActions);
        }
    }


    // parameter: function
    IEnumerator DelaySitAndOrder(int timeBetweenActions)
    {
        inCycle = true;
        EnterLine();
        yield return new WaitForSeconds(timeBetweenActions);
        GoToSeat();
        yield return new WaitForSeconds(timeBetweenActions);

        OrderFood();
        yield return new WaitForSeconds(timeBetweenActions);

        

        if (!orderTaken) // or if timer ran out
            LeaveSeat();

    }

    private bool InSeat()
    {
        return transform.position.x == seat.x && transform.position.y == seat.y && inSeat;
    } 

    // ---------------------------------------  IGNORE  --------------------------------------------

    // not currently in use -- keeping as skeleton code for if foodies can leave line out of impatience
    private void LeaveLine()
    {
        if (atFrontOfLine && FoodieSystem.inst.line.Count != 0)
        {
            FoodieSystem.inst.line.Dequeue();
            //Destroy(gameObject);
        }

        MoveUpInLine();
    }

    */


}
