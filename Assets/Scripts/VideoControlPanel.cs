using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AVPro functionality
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;
using System;

public class VideoControlPanel : MonoBehaviour
{

    // Variables
    public MediaPlayer _mediaPlayer;
    public GameObject introUI, controlsUI, creditsUI, goToCastleUI, loadingUI;
    public GameObject exitCreditsButton;
    public GameObject crowdSound, insideCastle;
    public GameObject log;
    public GameObject videoSphere;

    float current_time;

    private bool isReadyToPlay;
    private bool isFrameReady;

    // Use this for initialization
    void Start()
    {
        isFrameReady = isReadyToPlay = false;
        controlsUI.SetActive(false);
        creditsUI.SetActive(false);
        goToCastleUI.SetActive(false);
        loadingUI.SetActive(false);
        introUI.SetActive(true);
        _mediaPlayer.Events.AddListener(OnVideoEvent);
    }

    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                mp.Control.Play();
                mp.Control.Stop();
                mp.Control.Rewind();
                break;
            
            case MediaPlayerEvent.EventType.FirstFrameReady:
                onFrameReady();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this._mediaPlayer != null && this._mediaPlayer.Control != null)
        {
            current_time = this._mediaPlayer.Control.GetCurrentTimeMs();
            // Hide "go to castle" button when already inside castle
            if (current_time >= 35000)
            {
                goToCastleUI.SetActive(false);
                if (_mediaPlayer.Control.IsPlaying())
                {
                    if (!insideCastle.GetComponent<GvrAudioSource>().isPlaying)
                        insideCastle.GetComponent<GvrAudioSource>().Play();
                    if (!crowdSound.GetComponent<GvrAudioSource>().isPlaying)
                        crowdSound.GetComponent<GvrAudioSource>().Play();
                }
            }

            // Show Credit screen at end of video
            if (current_time >= 85000)
            {
                creditsUI.SetActive(true);
                controlsUI.SetActive(false);
                exitCreditsButton.SetActive(false);
            }
        }
    }

    // Play button
    public void PlayButton_OnClicked()
    {
        if (!_mediaPlayer.Control.IsPlaying())
        {
            _mediaPlayer.Control.Play();
            crowdSound.GetComponent<GvrAudioSource>().Play();
        }
    }

    // Pause button
    public void PauseButton_OnClicked()
    {
        if (_mediaPlayer.Control.IsPlaying())
            _mediaPlayer.Control.Pause();
        crowdSound.GetComponent<GvrAudioSource>().Pause();
        insideCastle.GetComponent<GvrAudioSource>().Pause();
    }

    // Restart button
    public void RestartButton_Clicked()
    {
        _mediaPlayer.Control.Stop();
        _mediaPlayer.Control.Rewind();
        goToCastleUI.SetActive(true);
        crowdSound.GetComponent<GvrAudioSource>().Stop();
        insideCastle.GetComponent<GvrAudioSource>().Stop();
    }

    // "Let's Go" button
    public void IntroButton_OnClicked()
    {
        introUI.SetActive(false);
        isReadyToPlay = true;
        if (!isFrameReady)
            loadingUI.SetActive(true);
        else
            showControlsOnFrameReady();
    }

    private void showControlsOnFrameReady()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Play();

        controlsUI.SetActive(true);
        goToCastleUI.SetActive(true);
    }

    // View credits button
    public void ViewCreditsButton_OnClicked()
    {
        creditsUI.SetActive(true);
        controlsUI.SetActive(false);
    }

    // Back button on Credits panel
    public void CreditsBackButton_OnClicked()
    {
        creditsUI.SetActive(false);
        controlsUI.SetActive(true);
    }

    // Go to castle button
    public void GoToCastleButton_OnClicked()
    {
        crowdSound.GetComponent<GvrAudioSource>().Stop();

        // Variable for video time showing desired scene (castle close-up view) 
        float timeslot_1 = 35000;

        // Hide/show buttons after button click
        goToCastleUI.SetActive(false);

        // Fast-forward movie to specific time
        _mediaPlayer.Control.Seek(timeslot_1);

        _mediaPlayer.Control.Play();
    }

    private void onFrameReady()
    {
        loadingUI.SetActive(false);
        videoSphere.transform.localScale += new Vector3(1, 1);
        isFrameReady = true;
        if(isReadyToPlay)
            showControlsOnFrameReady();
    }
}
