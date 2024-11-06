using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.VisualizerSample;
#if UNITY_VISIONOS
using UnityEngine.InputSystem.XR;
#endif
using Wacki;

namespace Demonixis.ToolboxV2.XR
{
    public class HandTrackingManager : MonoBehaviour
    {
        public enum HandGestures
        {
            Relax,
            Grab,
            PinchIndex,
            PinchMiddle,
            PinchRing,
            PinchLittle
        }

        public enum FingerPinch
        {
            Index = 0,
            Middle,
            Ring,
            Little
        }

        private const int NumHands = 2;
        private const int NumFingers = 5;
        private const float LowThreshold = 0.3f;
        private const float HighThreshold = 0.7f;
        private const int ThumbFingerIndex = 0;
        private const int IndexFingerIndex = 1;
        private const int MiddleFingerIndex = 2;
        private const int RingFingerIndex = 3;
        private const int LittleFingerIndex = 4;

        private XRHandSubsystem _handSubsystem;
        private readonly float _pinchThreshold = 0.02f;

        private readonly bool[] _trackingState = new bool[NumHands];
        private readonly float[] _leftFingersValues = new float[NumFingers];
        private readonly float[] _rightFingersValues = new float[NumFingers];
        private Dictionary<HandGestures, bool> _leftGestures;
        private Dictionary<HandGestures, bool> _rightGestures;
        private Transform[] _leftFingerProximals = new Transform[NumFingers];
        private Transform[] _rightFingerProximals = new Transform[NumFingers];

        [SerializeField] private Transform _origin;
        [SerializeField] private XRHandSkeletonDriver _leftSkeleton;
        [SerializeField] private XRHandSkeletonDriver _rightSkeleton;
        [SerializeField] IUILaserPointer _laserPointer;
        [SerializeField] private GameObject[] _motionControllerGameObjects;
        [SerializeField] private GameObject[] _handTrackingGameObjects;

        public event Action<bool, bool> HandTrackingEnableChanged;
        public event Action<HandGestures, bool, bool, bool> GestureChanged;

        public bool Tracked(bool left)
        {
            return _trackingState[left ? 0 : 1];
        }

        private void EnsureStarted()
        {
            if (_leftGestures != null) return;

            if (!XRManager.Enabled)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            _leftGestures = InitializeGestureArray();
            _rightGestures = InitializeGestureArray();

            var joins = _leftSkeleton.jointTransformReferences;
            PopulateFingers(ref _leftFingerProximals, joins);

            joins = _rightSkeleton.jointTransformReferences;
            PopulateFingers(ref _rightFingerProximals, joins);

            var showHands = true;
#if UNITY_VISIONOS
            showHands = false;
#endif
            var visualizer = GetComponent<HandVisualizer>();
            visualizer.drawMeshes = showHands;

#if !UNITY_VISIONOS
            var leftEvent = _leftSkeleton.GetComponent<XRHandTrackingEvents>();
            leftEvent.trackingChanged.AddListener(OnLeftHandTrackingChanged);

            var rightEvent = _rightSkeleton.GetComponent<XRHandTrackingEvents>();
            rightEvent.trackingChanged.AddListener(OnRightHandTrackingChanged);
#endif
        }

        private void Start()
        {
            EnsureStarted();

            var loader = XRManager.GetXRLoader();
            if (loader != null)
                _handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();

#if UNITY_VISIONOS
            OnHandTrackingChanged(true, true);
            OnHandTrackingChanged(false, true);
#endif
        }

        private void Update()
        {
            TryCheckGesturesForHand(true);
            TryCheckGesturesForHand(false);
        }

        private void OnLeftHandTrackingChanged(bool tracked)
        {
#if !UNITY_VISIONOS
            OnHandTrackingChanged(true, tracked);
#endif
        }

        private void OnRightHandTrackingChanged(bool tracked)
        {
#if !UNITY_VISIONOS
            OnHandTrackingChanged(false, tracked);
#endif
        }

        private void OnHandTrackingChanged(bool leftHand, bool tracked)
        {
            EnsureStarted();

            var index = leftHand ? 0 : 1;

            _trackingState[index] = tracked;
            _motionControllerGameObjects[index].SetActive(!tracked);
            _handTrackingGameObjects[index].SetActive(tracked);

            if (!leftHand)
            {
                _laserPointer.AllowExternalPressInput = tracked;
                _laserPointer.ExternalPressInputValue = false;
            }

            HandTrackingEnableChanged?.Invoke(leftHand, tracked);
        }

        private void OnGestureChanged(HandGestures gestures, bool leftHand, bool previewGestureState,
            bool newGestureState)
        {
            GestureChanged?.Invoke(gestures, leftHand, previewGestureState, newGestureState);
        }

        private bool CheckGesture(HandGestures gestures, ref float[] array)
        {
            if (gestures == HandGestures.Relax)
            {
                // Don't take the Thumb
                for (var i = IndexFingerIndex; i < array.Length; i++)
                {
                    if (array[i] > LowThreshold)
                        return false;
                }

                return true;
            }

            if (gestures == HandGestures.Grab)
            {
                // Don't take the Thumb
                for (var i = IndexFingerIndex; i <= MiddleFingerIndex; i++)
                {
                    if (array[i] < HighThreshold)
                        return false;
                }

                return true;
            }

            return false;
        }

        private bool IsPinching(bool left, FingerPinch index)
        {
            if (_handSubsystem == null) return false;

            var hand = left ? _handSubsystem.leftHand : _handSubsystem.rightHand;
            if (!hand.isTracked) return false;

            var indexJoint = index switch
            {
                FingerPinch.Index => XRHandJointID.IndexTip,
                FingerPinch.Middle => XRHandJointID.MiddleTip,
                FingerPinch.Ring => XRHandJointID.RingTip,
                FingerPinch.Little => XRHandJointID.LittleTip,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };

            var thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
            var indexTip = hand.GetJoint(indexJoint);

            if (TryToWorldPose(thumbTip, _origin, out var thumbPos) &&
                TryToWorldPose(indexTip, _origin, out var indexPos))
            {
                var distance = Vector3.Distance(thumbPos, indexPos);
                if (distance < _pinchThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryToWorldPose(XRHandJoint joint, Transform origin, out Vector3 result)
        {
            var xrOriginPose = new Pose(origin.position, origin.rotation);
            if (joint.TryGetPose(out Pose jointPose))
            {
                result = jointPose.GetTransformedBy(xrOriginPose).position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        public bool TryToWorldPose(XRHandJoint joint, Transform origin, out Vector3 position, out Quaternion rotation)
        {
            var xrOriginPose = new Pose(origin.position, origin.rotation);
            if (joint.TryGetPose(out Pose jointPose))
            {
                var pose = jointPose.GetTransformedBy(xrOriginPose);
                position = pose.position;
                rotation = pose.rotation;
                return true;
            }

            position = Vector3.zero;
            rotation = Quaternion.identity;
            return false;
        }

        private void TryCheckGesturesForHand(bool left)
        {
            if (!_trackingState[left ? 0 : 1]) return;

            var gestureArray = left ? _leftGestures : _rightGestures;
            var fingerValues = left ? _leftFingersValues : _rightFingersValues;
            var proximalArray = left ? _leftFingerProximals : _rightFingerProximals;

            // Check
            for (var i = 0; i < proximalArray.Length; i++)
            {
                var boneRotX = Mathf.Abs(proximalArray[i].localEulerAngles.x);
                if (boneRotX > 90) boneRotX = 0;
                var boneRate = boneRotX / 90.0f;

                if (left)
                    _leftFingersValues[i] = boneRate;
                else
                    _rightFingersValues[i] = boneRate;
            }

            // Relax
            var newRelaxGesture = CheckGesture(HandGestures.Relax, ref fingerValues);
            var oldRelaxGesture = gestureArray[HandGestures.Relax];
            if (newRelaxGesture != oldRelaxGesture)
            {
                gestureArray[HandGestures.Relax] = newRelaxGesture;
                OnGestureChanged(HandGestures.Relax, left, oldRelaxGesture, newRelaxGesture);
            }

            // Grab
            var newGrabGesture = CheckGesture(HandGestures.Grab, ref fingerValues);
            var oldGrabGesture = gestureArray[HandGestures.Grab];
            if (newGrabGesture != oldGrabGesture)
            {
                gestureArray[HandGestures.Grab] = newGrabGesture;
                OnGestureChanged(HandGestures.Grab, left, oldGrabGesture, newGrabGesture);
            }

            // Pinch
            CheckPinch(left, FingerPinch.Index);
            CheckPinch(left, FingerPinch.Middle);
            CheckPinch(left, FingerPinch.Ring);
            CheckPinch(left, FingerPinch.Little);
        }

        private void CheckPinch(bool left, FingerPinch pinchTarget)
        {
            var gestureTarget = pinchTarget switch
            {
                FingerPinch.Index => HandGestures.PinchIndex,
                FingerPinch.Middle => HandGestures.PinchMiddle,
                FingerPinch.Ring => HandGestures.PinchRing,
                FingerPinch.Little => HandGestures.PinchLittle
            };

            var gestureArray = left ? _leftGestures : _rightGestures;
            var newPinchGesture = IsPinching(left, pinchTarget);
            var oldPinchGesture = gestureArray[gestureTarget];

            if (newPinchGesture != oldPinchGesture)
            {
                gestureArray[gestureTarget] = newPinchGesture;
                OnGestureChanged(gestureTarget, left, oldPinchGesture, newPinchGesture);

                if (pinchTarget == FingerPinch.Index && !left && newPinchGesture)
                {
                    _laserPointer.ExternalPressInputValue = true;
                }
            }
        }

        private static Dictionary<HandGestures, bool> InitializeGestureArray()
        {
            var names = Enum.GetNames(typeof(HandGestures));
            var arr = new Dictionary<HandGestures, bool>(names.Length);

            for (var i = 0; i < names.Length; i++)
                arr.Add((HandGestures)i, false);

            return arr;
        }

        private static void PopulateFingers(ref Transform[] proximalArray, List<JointToTransformReference> joins)
        {
            foreach (var join in joins)
            {
                if (join.xrHandJointID == XRHandJointID.ThumbProximal)
                    proximalArray[ThumbFingerIndex] = join.jointTransform;
                else if (join.xrHandJointID == XRHandJointID.IndexProximal)
                    proximalArray[IndexFingerIndex] = join.jointTransform;
                else if (join.xrHandJointID == XRHandJointID.MiddleProximal)
                    proximalArray[MiddleFingerIndex] = join.jointTransform;
                else if (join.xrHandJointID == XRHandJointID.RingProximal)
                    proximalArray[RingFingerIndex] = join.jointTransform;
                else if (join.xrHandJointID == XRHandJointID.LittleProximal)
                    proximalArray[LittleFingerIndex] = join.jointTransform;
            }
        }
    }
}