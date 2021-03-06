﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterMenu : MonoBehaviour {

    int monster = 0;
    string aboutMrMonster = "Mr. Monster \n He loves to ride the elevators!";
    string aboutDrKhil = "Dr. Khil \n Psychologist that has his own television show, where he helps monster families with their problems. Be vary, if you don’t get him to the elevator in time he will convince someone to leave with him.";
    string aboutMonsterMonroe = "Monster Monroe \n She is a very famous model and actress in the monsterverse. She is so attractive that the timer on the monsters on her floor stops.";
    string aboutHunkiestHogan = "Hunkiest Hogan \n He is a semi-retired monster wrestler. If you don’t get him on the elevator on time he will shake the ground with his anger.";

    public void Left()
    {
        Image mrMonster = GameObject.Find("Monsters/MrMonster").GetComponent<Image>();
        Image drKhil = GameObject.Find("Monsters/DrKhil").GetComponent<Image>();
        Image monsterMonroe = GameObject.Find("Monsters/MonsterMonroe").GetComponent<Image>();
        Image hunkiestHogan = GameObject.Find("Monsters/HunkiestHogan").GetComponent<Image>();
        Text monsterText = GameObject.Find("MonsterText").GetComponent<Text>();
        if (monster != 0)
        {
            monster--;
        }
        if (monster == 2)
        {
            hunkiestHogan.enabled = false;
            monsterMonroe.enabled = true;
            monsterText.text = aboutMonsterMonroe;
            GameObject.Find("Monsters/Right").GetComponent<Image>().enabled = true;
        }
        else if (monster == 1)
        {
            monsterMonroe.enabled = false;
            drKhil.enabled = true;
            monsterText.text = aboutDrKhil;
        }
        else if (monster == 0)
        {
            drKhil.enabled = false;
            mrMonster.enabled = true;
            monsterText.text = aboutMrMonster;
            GameObject.Find("Monsters/Left").GetComponent<Image>().enabled = false;
        }
        else
        {

        }
        Debug.Log("Left");
    }

    public void Right()
    {
        Image mrMonster = GameObject.Find("Monsters/MrMonster").GetComponent<Image>();
        Image drKhil = GameObject.Find("Monsters/DrKhil").GetComponent<Image>();
        Image monsterMonroe = GameObject.Find("Monsters/MonsterMonroe").GetComponent<Image>();
        Image hunkiestHogan = GameObject.Find("Monsters/HunkiestHogan").GetComponent<Image>();
        Text monsterText = GameObject.Find("MonsterText").GetComponent<Text>();

        if (monster != 3)
        {
            monster++;
        }
        if(monster == 1)
        {
            mrMonster.enabled = false;
            drKhil.enabled = true;
            monsterText.text = aboutDrKhil;
            GameObject.Find("Monsters/Left").GetComponent<Image>().enabled = true;
        }
        else if(monster == 2)
        {
            drKhil.enabled = false;
            monsterMonroe.enabled = true;
            monsterText.text = aboutMonsterMonroe;
        }
        else if (monster == 3)
        {
            monsterMonroe.enabled = false;
            hunkiestHogan.enabled = true;
            monsterText.text = aboutHunkiestHogan;
            GameObject.Find("Monsters/Right").GetComponent<Image>().enabled = false;
        }
        else
        {

        }
        Debug.Log("Right");
    }
}
