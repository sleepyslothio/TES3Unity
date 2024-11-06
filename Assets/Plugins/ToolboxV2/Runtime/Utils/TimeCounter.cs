﻿using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.Utils
{
    public sealed class TimeCounter : MonoBehaviour
    {
        private float _elapsedTime = 0.0f;
        private float _minutes = 0.0f;
        private float _seconds = 0.0f;
        private float _miliseconds = 0.0f;
        private bool _done = false;
        private bool _timeAttackMode = true;
        private StringBuilder _builder = new StringBuilder();
        public ushort minutes = 5;
        public ushort seconds = 0;
        public ushort milliseconds = 0;
        public Text timeText;
        public bool startNow = false;
        public bool onlySeconds = false;

        public bool TimeAttackMode
        {
            get { return _timeAttackMode; }
            set
            {
                _timeAttackMode = value;
                timeText.enabled = value;
            }
        }

        public Action TimerCompleted = null;

        private void Start()
        {
            if (startNow)
            {
                ReStart();
            }
            else
            {
                _done = true;
            }
        }

        private void Update()
        {
            if (_done)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (_timeAttackMode)
            {
                if (_minutes <= 0 && _minutes <= 0 && _seconds <= 0)
                {
                    _done = true;
                    _minutes = 0.0f;
                    _seconds = 0.0f;
                    _miliseconds = 0.0f;

                    if (TimerCompleted != null)
                    {
                        TimerCompleted();
                    }
                }

                if (Time.timeScale > 0.0f && timeText.enabled)
                {
                    timeText.text = GetTime();
                }
            }
        }

        public void ReStart()
        {
            SetTime(minutes, seconds, milliseconds);
            _done = false;
        }

        public void SetTime(ushort minutes, ushort seconds, ushort miliseconds)
        {
            _minutes = minutes;
            _seconds = seconds;
            _miliseconds = miliseconds;
        }

        public float GetMaxTime()
        {
            return minutes * 60 + seconds;
        }

        public float GetElapsedTime()
        {
            return _elapsedTime;
        }

        public string GetElaspsedTime()
        {
            var time = TimeSpan.FromSeconds(_elapsedTime);
            var dMins = time.Minutes.ToString();
            var dSecs = time.Seconds.ToString();
            var dMs = time.Milliseconds.ToString();

            if (dMins.Length == 1)
            {
                dMins = string.Concat("0", dMins);
            }

            if (dSecs.Length == 1)
            {
                dSecs = string.Concat("0", dSecs);
            }
            else if (dSecs.Length > 2)
            {
                dSecs = string.Concat(dSecs[0], dSecs[1]);
            }

            if (onlySeconds)
            {
                return string.Format("{0}'", dSecs);
            }

            if (dMs.Length == 1)
            {
                dMs = string.Concat("0", dMs);
            }
            else if (dMs.Length > 2)
            {
                dMs = string.Concat(dMs[0], dMs[1]);
            }

            return string.Concat(dMins, "'", dSecs, "\"", dMs);
        }

        public string GetTime()
        {
            _builder.Length = 0;

            if (_miliseconds < 0)
            {
                if (_seconds <= 0)
                {
                    _minutes--;
                    _seconds = 59;
                }
                else if (_seconds >= 0)
                {
                    _seconds--;
                }

                _miliseconds = 100;
            }
            else if (_miliseconds >= 100)
            {
                if (_seconds >= 60)
                {
                    _minutes++;
                    _seconds = 0;
                }
                else if (_seconds <= 60)
                {
                    _seconds++;
                }

                _miliseconds = 0;
            }

            _miliseconds -= Time.deltaTime * 100.0f;

            if (_minutes < 10)
            {
                _builder.Append("0");
            }

            _builder.Append(_minutes);
            _builder.Append(":");

            if (_seconds < 10)
            {
                _builder.Append("0");
            }

            _builder.Append(_seconds);
            _builder.Append(":");

            if ((int)_miliseconds < 10)
            {
                _builder.Append("0");
            }

            _builder.Append((int)_miliseconds);

            return _builder.ToString();
        }
    }
}