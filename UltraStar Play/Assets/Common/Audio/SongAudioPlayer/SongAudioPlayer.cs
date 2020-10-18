﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniInject;
using UnityEngine;
using UniRx;

public class SongAudioPlayer : MonoBehaviour
{
    // The playback position increase in milliseconds from one frame to the next to be counted as "jump".
    // An event is fired when jumping forward in the song.
    private const int MinForwardJumpOffsetInMillis = 500;

    [InjectedInInspector]
    public AudioSource audioPlayer;

    // The last frame in which the position in the song was calculated
    private int positionInSongInMillisFrame;

    private readonly Subject<double> playbackStoppedEventStream = new Subject<double>();
    public ISubject<double> PlaybackStoppedEventStream
    {
        get
        {
            return playbackStoppedEventStream;
        }
    }

    private readonly Subject<double> playbackStartedEventStream = new Subject<double>();
    public ISubject<double> PlaybackStartedEventStream
    {
        get
        {
            return playbackStartedEventStream;
        }
    }

    private readonly Subject<double> positionInSongEventStream = new Subject<double>();
    public ISubject<double> PositionInSongEventStream
    {
        get
        {
            return positionInSongEventStream;
        }
    }

    public IObservable<Pair<double>> JumpBackInSongEventStream
    {
        get
        {
            return positionInSongEventStream.Pairwise().Where(pair => pair.Previous > pair.Current);
        }
    }

    public IObservable<Pair<double>> JumpForwardInSongEventStream
    {
        get
        {
            // The position will increase in normal playback. A big increase however, can always be considered as "jump".
            // Furthermore, when not currently playing, then every forward change can be considered as "jump".
            return positionInSongEventStream.Pairwise().Where(pair =>
            {
                return (pair.Previous + MinForwardJumpOffsetInMillis) < pair.Current
                    || (!IsPlaying && pair.Previous < pair.Current);
            });
        }
    }

    // The current position in the song in milliseconds.
    private double positionInSongInMillis;
    public double PositionInSongInMillis
    {
        get
        {
            if (audioPlayer == null || audioPlayer.clip == null)
            {
                return 0;
            }
            // The samples of an AudioClip change concurrently,
            // even when they are queried in the same frame (e.g. Update() of different scripts).
            // For a given frame, the position in the song should be the same for all scripts,
            // which is why the value is only updated once per frame.
            if (positionInSongInMillisFrame != Time.frameCount)
            {
                positionInSongInMillisFrame = Time.frameCount;
                positionInSongInMillis = 1000.0f * (double)audioPlayer.timeSamples / (double)audioPlayer.clip.frequency;
            }
            return positionInSongInMillis;
        }

        set
        {
            if (DurationOfSongInMillis <= 0)
            {
                return;
            }

            double newPositionInSongInMillis = value;
            if (newPositionInSongInMillis < 0)
            {
                newPositionInSongInMillis = 0;
            }
            else if (newPositionInSongInMillis > DurationOfSongInMillis - 1)
            {
                newPositionInSongInMillis = DurationOfSongInMillis - 1;
            }

            positionInSongInMillis = newPositionInSongInMillis;
            int newTimeSamples = (int)(audioPlayer.clip.frequency * positionInSongInMillis / 1000.0);
            audioPlayer.timeSamples = newTimeSamples;

            positionInSongEventStream.OnNext(positionInSongInMillis);
        }
    }

    public double DurationOfSongInMillis { get; private set; }

    public double PositionInSongInPercent
    {
        get
        {
            if (DurationOfSongInMillis <= 0)
            {
                return 0;
            }

            return PositionInSongInMillis / DurationOfSongInMillis;
        }
    }

    public double CurrentBeat
    {
        get
        {
            if (audioPlayer.clip == null)
            {
                return 0;
            }
            else
            {
                double millisInSong = PositionInSongInMillis;
                double result = BpmUtils.MillisecondInSongToBeat(SongMeta, millisInSong);
                if (result < 0)
                {
                    result = 0;
                }
                return result;
            }
        }
    }

    public bool IsPlaying
    {
        get
        {
            return audioPlayer.isPlaying;
        }
    }

    public AudioClip AudioClip
    {
        get
        {
            return audioPlayer.clip;
        }
    }

    public bool HasAudioClip
    {
        get
        {
            return audioPlayer.clip != null;
        }
    }

    private SongMeta SongMeta { get; set; }

    public float PlaybackSpeed
    {
        get
        {
            return audioPlayer.pitch;
        }
        set
        {
            // Playback speed cannot be set randomly. Allowed (and useful) is a range of 0.5 to 1.5.
            float newPlaybackSpeed = value;
            if (newPlaybackSpeed < 0.5f)
            {
                newPlaybackSpeed = 0.5f;
            }
            else if (newPlaybackSpeed > 1.5f)
            {
                newPlaybackSpeed = 1.5f;
            }

            // Setting the pitch of an AudioPlayer will change tempo and pitch.
            audioPlayer.pitch = newPlaybackSpeed;

            // A Pitch Shifter effect on an AudioMixerGroup can be used to compensate the pitch change of the AudioPlayer,
            // such that only the change of the tempo remains.
            // See here for details: https://answers.unity.com/questions/25139/how-i-can-change-the-speed-of-a-song-or-sound.html
            // See here for how the pitch value of the Pitch Shifter effect is made available for scripting: https://learn.unity.com/tutorial/audio-mixing#5c7f8528edbc2a002053b506
            audioPlayer.outputAudioMixerGroup.audioMixer.SetFloat("PitchShifter.Pitch", 1 + (1 - newPlaybackSpeed));
        }
    }

    void Update()
    {
        if (IsPlaying)
        {
            positionInSongEventStream.OnNext(PositionInSongInMillis);
        }
    }

    public void Init(SongMeta songMeta)
    {
        this.SongMeta = songMeta;

        string songPath = SongMetaUtils.GetAbsoluteSongAudioPath(songMeta);
        AudioClip audioClip = AudioManager.Instance.GetAudioClip(songPath);
        if (audioClip != null)
        {
            audioPlayer.clip = audioClip;
            DurationOfSongInMillis = 1000.0 * audioClip.samples / audioClip.frequency;
        }
        else
        {
            audioPlayer.clip = null;
            DurationOfSongInMillis = 0;
        }
    }

    public void PauseAudio()
    {
        if (audioPlayer.isPlaying)
        {
            audioPlayer.Pause();
            playbackStoppedEventStream.OnNext(PositionInSongInMillis);
        }
    }

    public void PlayAudio()
    {
        if (HasAudioClip && !audioPlayer.isPlaying)
        {
            audioPlayer.Play();
            playbackStartedEventStream.OnNext(PositionInSongInMillis);
        }
    }
}
