using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DateManager : MonoBehaviour {

    //Display
    [SerializeField]
    TextMeshPro
        minuteText,
        hourText,
        dayText,
        monthText,
        yearText;

    //Play / Stop
    [SerializeField]
    bool isRunning;

    //Calculus
    float seconds = 0;
    [SerializeField]
    float secondsPerMinute = 1;
    [HideInInspector]
    public int
        minutes = 0,
        hours = 0,
        days = 0,
        months = 0,
        years = 0;

    int[]
        months31 = { 1, 3, 5, 7, 8, 10, 12 },
        months30 = { 4, 6, 9, 11 };

        
        

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateTime();

    }

    public void DisplayTime()
    {
        if (minutes < 10)
        {
            minuteText.SetText("0" + minutes.ToString());
        }
        else
            minuteText.SetText(minutes.ToString());


        if (hours < 10)
        {
            hourText.SetText("0" + hours.ToString());
        }
        else
            hourText.SetText(hours.ToString());


        if (days < 10)
        {
            dayText.SetText("0" + days.ToString());
        }
        else
            dayText.SetText(days.ToString());


        if (months < 10)
        {
            monthText.SetText("0" + months.ToString());
        }
        else
            monthText.SetText(months.ToString());


        if (years < 10)
        {
            yearText.SetText("000" + years.ToString());
        }
        else if (years < 100)
        {
            yearText.SetText("00" + years.ToString());
        }
        else if (years < 1000)
        {
            yearText.SetText("0" + years.ToString());
        }
        else
            yearText.SetText(years.ToString());
    }


    void UpdateTime()
    {
        if (isRunning)
        {
            seconds += Time.deltaTime;

            if (seconds >= secondsPerMinute)
            {
                seconds -= secondsPerMinute;
                minutes++;
            }
                
            if (minutes >= 60)
            {
                minutes -= 60;
                hours++;
            }

            if (hours >= 24)
            {
                hours -= 24;
                days++;
            }

            if (days >= GetCurrentMonthDays())
            {
                days = 1;
                months++;
            }

            if (months >= 12)
            {
                months = 1;
                years++;
            }

                DisplayTime();
        }
    }

    public int GetCurrentMonthDays()
    {
        if (CheckIntInArray(months, months30))
            return 30;
        else if (CheckIntInArray(months, months31))
            return 31;
        else if (months == 2)
        {
            if (years % 4 == 0)
                return 29;
            else
                return 28;
        }

        return 30;
    }

    public void SetTime(int minute, int hour, int day, int month, int year)
    {
        minutes = minute;
        hours = hour;
        days = day;
        months = month;
        years = year;
    }

    public void ResumeTime()
    {
        isRunning = true;
    }

    public void PauseTime()
    {
        isRunning = false;
    }




    //Secondary

    bool CheckIntInArray(int intToCheck, int[] intArray)
    {
        bool intInArray = false;

        for (int i = 0; i < intArray.Length; i++)
        {
            if (intToCheck == intArray[i])
            {
                intInArray = true;
            }
        }

        return intInArray;
    }
}
