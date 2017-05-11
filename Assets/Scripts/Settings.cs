using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    bool sound = true;
    bool music = true;
    GameControllerTest gameController;

	public void Sound()
    {
        sound = !sound;
        gameController = GameObject.Find("GameController").GetComponent<GameControllerTest>();
        gameController.scoreOn = !gameController.scoreOn;
        GameObject.Find("Sound/Background/Checkmark").GetComponent<Image>().enabled = sound;
    }

    public void Music()
    {
        music = !music;
        Camera.main.GetComponent<AudioSource>().mute = !music;
        GameObject.Find("Music/Background/Checkmark").GetComponent<Image>().enabled = music;
    }
}
