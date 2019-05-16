using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class StatMonitor : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Monitor Frequency")]
    public float monitorFrequency = 1;
    [Header("UI Fields")]
    public Text rpm, distance;
    public Image keyUp, keyDown, keyLeft, keyRight, BL, BR, FL, FR;
    [Header("Monitor Objects")]
    public CarAgent car;
    public CarController CarController;
    public GameObject rangeFindersContainer, rangeFinderTexts;
    public Map map;
    public GameObject textInstance;
    private Image[] wheelMonitor;
    private WheelCollider[] wc = new WheelCollider[4];
    private RangeFinder[] rangefinders;
    private List<Text> rfs = new List<Text>();
    private float lastTime;
    private bool ready = true;
    void Start()
    {
        wheelMonitor = new Image[] { BL, BR, FL, FR };
        wc[0] = car.wheelBL;
        wc[1] = car.wheelBR;
        wc[2] = car.wheelFL;
        wc[3] = car.wheelFR;
        rangefinders = rangeFindersContainer.GetComponentsInChildren<RangeFinder>();
        for (int i = 0; i < rangefinders.Length; i++)
        {
            rfs.Add(Instantiate(textInstance, rangeFinderTexts.transform).GetComponent<Text>());
        }
        lastTime = Time.time;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rpm.text = string.Format("Speed: {0:0.00} kph", CarController.carSpeed * 3.6);
        distance.text = string.Format("Progress: {0:0.00} m", map.GetDistance());
        CheckInput();
        CheckWheel();
        StartCoroutine(CheckRangeFinders());
        StartCoroutine(RemoveLayout());
    }
    IEnumerator RemoveLayout(){
        yield return new WaitForSeconds(1f);
        rangeFinderTexts.GetComponent<GridLayoutGroup>().enabled = false;
    }

    void CheckWheel()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelCollider wheel = wc[i];
            WheelHit hit;
            if (wheel.GetGroundHit(out hit))
            {

                if (Mathf.Abs(hit.forwardSlip) > 0.5 || Mathf.Abs(hit.sidewaysSlip) > 0.5)
                {
                    wheelMonitor[i].color = Color.red;
                }
                else
                {
                    wheelMonitor[i].color = new Color(1f, 1 - Mathf.Abs(hit.forwardSlip), 1 - Mathf.Abs(hit.sidewaysSlip));
                }
            }
        }
    }

    void CheckInput()
    {
        if (car.forwardInput > 0)
        {
            keyUp.color = Color.red;
            keyDown.color = Color.white;
        }
        else if (car.forwardInput < 0)
        {
            keyUp.color = Color.white;
            keyDown.color = Color.red;
        }
        else
        {
            keyUp.color = Color.white;
            keyDown.color = Color.white;
        }
        if (car.sideInput < 0)
        {
            keyLeft.color = Color.red;
            keyRight.color = Color.white;
        }
        else if (car.sideInput > 0)
        {
            keyLeft.color = Color.white;
            keyRight.color = Color.red;
        }
        else
        {
            keyLeft.color = Color.white;
            keyRight.color = Color.white;
        }
    }
    IEnumerator CheckRangeFinders()
    {
        if (Time.time - lastTime < (1 / monitorFrequency) || !ready) yield return null;
        lastTime = Time.time;
        ready = false;
        for (int i = 0; i < rangefinders.Length; i++)
        {
            RangeFinder rf = rangefinders[i];
            if (rf.detected)
            {
                rfs[i].text = string.Format("<color=#008000ff><b>{0}: {1:0.00}</b></color>", rf.gameObject.name, rf.range);
            }
            else
            {
                rfs[i].text = string.Format("{0}: {1:0.00}", rf.gameObject.name, rf.range);
            }
        }
        ready = true;
    }
}
