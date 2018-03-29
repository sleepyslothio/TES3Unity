#define DEBUG_3DRUDDER

using UnityEngine;
using System;
using System.Threading;

namespace Unity3DRudder
{
    public class s3DRudderManager : ns3DRudder.CSdk
    {
        #region Properties
        public static readonly int _3DRUDDER_SDK_MAX_DEVICE = ns3DRudder.i3DR._3DRUDDER_SDK_MAX_DEVICE;
        public static readonly int _3DRUDDER_SDK_VERSION = ns3DRudder.i3DR._3DRUDDER_SDK_VERSION;

        // Instance
        private Thread thread;
        private static s3DRudderManager _instance;
        public static s3DRudderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new s3DRudderManager();
                }
                return _instance;
            }
        }

        // Event
        public EventRudder Events;
        private bool quit = false;
        private readonly int threadSleepMs = 100;

        // States for all rudders (MAX = 4)
        private Rudder[] rudders;
        private bool[] connected;
        #endregion

        private void Update()
        {
            while (!quit)
            {
                // Callback Events
                for (uint i = 0; i < rudders.Length; ++i)
                {
                    bool isConnected = rudders[i].IsDeviceConnected();
                    // FRONT
                    if (isConnected != connected[i])
                    {
                        if (isConnected)
                        {
                            Events.OnConnect(i);
                            connected[i] = true;
                        }
                        else
                        {
                            Events.OnDisconnect(i);
                            connected[i] = false;
                        }
                    }
                }
                // 10 FPS = 100 ms
                Thread.Sleep(threadSleepMs);
            }
        }
        #region Functions
        private s3DRudderManager()
        {
#if DEBUG_3DRUDDER
            Debug.Log("init s3DRudderManager");
#endif
            // Init SDK
            Init();

            // Init States
            rudders = new Rudder[_3DRUDDER_SDK_MAX_DEVICE];
            connected = new bool[_3DRUDDER_SDK_MAX_DEVICE];
            for (uint i = 0; i < rudders.Length; ++i)
            {
                rudders[i] = new Rudder(i, this);
                connected[i] = false;
            }
#if DEBUG_3DRUDDER
            // Show info
            Debug.LogFormat("SDK version : {0:X4}" , GetSDKVersion());
#endif            
            // Set events Connected & Disconnected
            Events = new EventRudder();
#if DEBUG_3DRUDDER
            Events.OnConnectEvent += (portNumber) => Debug.LogFormat("3dRudder {0} connected, firmware : {1:X4}", portNumber, GetVersion(portNumber));
            Events.OnDisconnectEvent += (portNumber) => Debug.LogFormat("3dRudder {0} disconnected, firmware : {1:X4}", portNumber, GetVersion(portNumber));
#endif
            SetEvents(Events);
        }

        public void ShutDown()
        {
#if DEBUG_3DRUDDER
            Debug.Log("shutdown s3DRudderManager");
#endif
            Dispose();
        }

        public override void Dispose()
        {
#if DEBUG_3DRUDDER
            Debug.Log("dispose s3DRudderManager");
#endif
            // kill thread
            quit = true;
            ClearEvents();
            // call sdk dispose            
            base.Dispose();
            // dispose rudders
            foreach (Rudder r in rudders)
                r.Dispose();
            // delete instance
            _instance = null;
            // Call garbage collector
            GC.SuppressFinalize(this);
            
        }
        #endregion

        #region SDK
        /// <summary>
        /// Set events and start thread
        /// </summary>
        /// <param EventRudder="e"></param>        
        private void SetEvents(EventRudder events)
        {
            if (thread == null)
            {
                thread = new Thread(Update);
                thread.Start();
            }
            Events = events;
        }

        /// <summary>
        /// Clear events and stop thread
        /// </summary>
        private void ClearEvents()
        {
            if (thread != null)
                thread.Abort();
            // dispose Event
            Events.Dispose();
            Events = null;
        }

        /// <summary>
        /// Returns rudder
        /// </summary>
        /// <param name="portNumber"></param>
        /// <returns>Rudder</returns>
        public Rudder GetRudder(int portNumber)
        {
            return rudders[portNumber];
        }
        #endregion
    }
}