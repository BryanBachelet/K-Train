using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Render.Camera
{
    public class CameraBehavior : MonoBehaviour
    {
        [SerializeField] private Transform cameraTrainTransform;
        [SerializeField] private Character.CharacterMouvement playerMove;
        [SerializeField] private Transform m_targetTransform;
        [SerializeField] private float m_distanceToTarget;
        [HideInInspector] public Vector3 m_offsetPos;

        private Vector3 m_cameraDirection;
        private Vector3 m_baseAngle;
        const int maxDirection = 8;
        const float angleValue = 360.0f / maxDirection;
        private int indexDirection = 0;

        private Vector3 m_finalPosition;
        private Vector3 m_finalRotation;

        private float m_nextAngle;
        private Vector3 m_nextRot;
        private float m_prevAngle;
        private Vector3 m_prevRot;
        private bool m_isLerping = false;
        private float m_lerpTime = 0.3f;
        private float m_lerpTimer = 0.0f;
        private bool attachedToTrain = false;


        private CameraEffect[] cameraEffects;
        public Transform sun;
        [SerializeField] private Texture2D[] m_cursorTex = new Texture2D[2];

        // -- Test Camera Zoom ---- 

        [Header("Camera Zoom parameter")]
        [SerializeField] private float m_maxDistance = 10;
        [SerializeField] private float m_minDistance = 2;
        [SerializeField] private Vector3 m_maxAngle;
        [SerializeField] private Vector3 m_minAngle;
        [SerializeField] private float m_currentLerpValue = 1;
        [SerializeField] private float m_inputZoomSensibility = 1.0f;
        [SerializeField] private bool m_activeCameraZoomDebug = false;
        [SerializeField] private Vector3 m_baseOffset;
        [SerializeField] private float m_valueMinToStartSlope = 0.8f;

        private float m_zoomInputGamepad = 0.0f;
        private bool m_IsGamepad = true;

        [Header("Camera Zoom High Block State parameters")]
        private bool m_isZoomBlock = false;
        [SerializeField] private float m_maxZoomBlock = 0.15f;
        [SerializeField] private float m_transitionDuration = 2;
        [SerializeField] private float m_minZoomBlock = .85f;
        private bool m_boolTest = false;
        private bool m_isDezoomingAutomatily;

        private float m_inputZoomValue;
        private float m_slopeAngle;
        private float m_prevSlopeAngle;
        private float m_nextSlopeAngle;

        [SerializeField] private float m_thresholdAngle = 4.0f;

        // -------------

        // -------- Test Rotation Camera Mouse ---------

        [Header("Camera Mouse Parameters")]
        [SerializeField] private float m_mousDeltaThreshold = 3.0f;
        [SerializeField] private bool m_activeDebugMouseRotation = false;
        [SerializeField] private float m_maxMouseDeltaSpeed = 500;
        [SerializeField] private float m_minMouseDeltaSpeed = 5.0f;
        [SerializeField] private float m_mouseSensibility = 1.0f;
        [SerializeField] private float m_maxAngularSpeed = 360;

        // ------------------------------


        // Free Rotation Variable
        [Header("Free Rotation Variables")]
        [SerializeField] private float m_angularSpeed = 10;
        [SerializeField] private AnimationCurve angularSpeedAcceleration;
        [SerializeField] private bool m_inverseCameraController = false;
        [SerializeField] private bool m_activateHeightDirectionMode = false;
        [SerializeField] private bool m_mouseInputActivate = true;
        [SerializeField] private bool m_rotationKeyboardActive = true;

        private float initialAngularSpeed;
        private float timeLastRotationInput;
        private float m_currentAngle;
        private bool m_isRotationInputPress;
        private float m_mouseDeltaValue;



        // Start is called before the first frame update
        void Start()
        {
            initialAngularSpeed = m_angularSpeed;
            cameraEffects = GetComponents<CameraEffect>();
            m_cameraDirection = transform.position - m_targetTransform.position;
            m_baseAngle = transform.rotation.eulerAngles;
            Cursor.lockState = CursorLockMode.Confined;
        }




        void Update()
        {
            if (m_IsGamepad)
            {

                m_inputZoomValue = m_inputZoomSensibility * m_zoomInputGamepad;
                if (m_activeCameraZoomDebug) Debug.Log("Zoom Input value = " + m_inputZoomValue);

                if (m_isDezoomingAutomatily) return;

                m_currentLerpValue += m_inputZoomValue;
                m_currentLerpValue = Mathf.Clamp(m_currentLerpValue, 0.0f, 1.0f);
                if (m_isZoomBlock) m_currentLerpValue = Mathf.Clamp(m_currentLerpValue, 0.0f, m_maxZoomBlock);
            }


            if (playerMove.mouvementState != Character.CharacterMouvement.MouvementState.Train)
            {
                if (m_isLerping)
                {
                    if (m_lerpTimer > m_lerpTime)
                    {
                        m_prevRot = m_nextRot;
                        m_prevAngle = m_nextAngle;
                        m_lerpTimer = 0.0f;
                        m_isLerping = false;
                    }
                    else
                    {
                        m_lerpTimer += Time.deltaTime;
                    }

                }

                // ------------ Camera zoom ----------
                CameraZoom();
                // ------------------------


                if (!m_activateHeightDirectionMode && m_isRotationInputPress) FreeRotation(m_mouseDeltaValue);

                SetCameraRotation();
                SetCameraPosition();
                Apply();
            }
            else
            {
                //transform.parent = cameraTrainTransform;
                transform.position = cameraTrainTransform.position;
                Vector3 directionsun = sun.position;
                directionsun.y = m_targetTransform.position.y;
                transform.LookAt(directionsun);
            }

        }

        #region CameraZoom 

        public void InputZoom(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                m_zoomInputGamepad = ctx.ReadValue<float>();
                if (!m_IsGamepad)
                {

                    m_inputZoomValue = m_inputZoomSensibility * m_zoomInputGamepad;
                    if (m_activeCameraZoomDebug) Debug.Log("Zoom Input value = " + m_inputZoomValue);

                    if (m_isDezoomingAutomatily) return;

                    m_currentLerpValue += m_inputZoomValue;
                    m_currentLerpValue = Mathf.Clamp(m_currentLerpValue, 0.0f, 1.0f);
                    if (m_isZoomBlock) m_currentLerpValue = Mathf.Clamp(m_currentLerpValue, 0.0f, m_maxZoomBlock);
                }

            }

            if (ctx.canceled)
            {
                m_zoomInputGamepad = 0.0f;
                //m_inputZoomValue = m_inputZoomSensibility * ctx.ReadValue<float>();
            }
        }
        private void CameraZoom()
        {
            m_prevSlopeAngle = m_slopeAngle;
            float angle = playerMove.GetSlope();
            if (angle > m_thresholdAngle)
            {
                m_nextSlopeAngle = angle;
            }

            m_slopeAngle = Mathf.Lerp(m_prevSlopeAngle, m_nextSlopeAngle, 0.2f);
            Vector3 slopeAngle = new Vector3(0, 0.0f, 0);

            if (m_currentLerpValue > m_valueMinToStartSlope) slopeAngle = new Vector3(m_slopeAngle, 0.0f, 0);
            m_baseAngle = Vector3.Lerp(m_maxAngle, m_minAngle, m_currentLerpValue) + slopeAngle;
            m_distanceToTarget = Mathf.Lerp(m_maxDistance, m_minDistance, m_currentLerpValue);
            m_cameraDirection = Quaternion.Euler(m_baseAngle) * -Vector3.forward;
        }

        IEnumerator DeZoomCamera()
        {

            // Setup a bool 
            m_isDezoomingAutomatily = true;
            float zoonDelta = m_currentLerpValue - m_maxZoomBlock;
            float speed = zoonDelta * (1 / m_transitionDuration);
            while (m_currentLerpValue > m_maxZoomBlock)
            {
                m_currentLerpValue -= speed * Time.deltaTime;
                yield return Time.deltaTime;
            }
            m_isDezoomingAutomatily = false;
            //Reset bool

        }

        IEnumerator ZoomCloseCamera()
        {
            m_isDezoomingAutomatily = true;
            float zoonDelta = m_minZoomBlock - m_currentLerpValue;
            float speed = zoonDelta * (1 / m_transitionDuration);
            while (m_currentLerpValue < m_minZoomBlock)
            {
                m_currentLerpValue += speed * Time.deltaTime;
                yield return Time.deltaTime;
            }
            m_isDezoomingAutomatily = false;
        }

        public void BlockZoom(bool state)
        {
            if (state == m_isZoomBlock) return;

            m_isZoomBlock = state;

            if (!m_isZoomBlock) return;

            StartCoroutine(DeZoomCamera());
        }


        #endregion

        public float GetAngle()
        {
            return m_currentAngle;
        }
        public void RotationInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && m_rotationKeyboardActive)
            {

                float value = ctx.ReadValue<float>();
                if (m_inverseCameraController) value = -1 * value;
                m_mouseDeltaValue = value;
                m_isRotationInputPress = true;
                timeLastRotationInput = Time.time;
                if (value > 0)
                {
                    if (m_activateHeightDirectionMode && !m_isLerping) ChangeRotation(true);

                }
                if (value < 0)
                {
                    if (m_activateHeightDirectionMode && !m_isLerping) ChangeRotation(false);
                }
            }

            if (ctx.canceled && m_rotationKeyboardActive)
            {
                m_isRotationInputPress = false;
                float value = ctx.ReadValue<float>();
            }
        }

        public void RotationAimInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && m_mouseInputActivate)
            {
                int value = 1;
                if (m_inverseCameraController) value = -1;

                m_mouseDeltaValue = value * m_mouseSensibility * ctx.ReadValue<Vector2>().x;



                if (Mathf.Abs(m_mouseDeltaValue) < m_mousDeltaThreshold) m_mouseDeltaValue = 0;
                //if (m_activeDebugMouseRotation) Debug.Log("Mouse Delta = " + m_mouseDeltaValue.ToString());

            }

            if (ctx.canceled && m_mouseInputActivate)
            {
                m_mouseDeltaValue = 0.0f;
                //if (m_activeDebugMouseRotation) Debug.Log("Mouse Delta = " + m_mouseDeltaValue.ToString());
            }
        }

        public void RotationMouseInput(InputAction.CallbackContext ctx)
        {
            if (ctx.performed && m_mouseInputActivate)
            {

                //Cursor.visible = false;
                Cursor.SetCursor(m_cursorTex[0], Vector2.zero, CursorMode.ForceSoftware);
                m_isRotationInputPress = true;

            }
            if (ctx.canceled && m_mouseInputActivate)
            {
                Cursor.SetCursor(m_cursorTex[1], Vector2.zero, CursorMode.ForceSoftware);
                m_isRotationInputPress = false;
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
            }
        }

        public Vector3 TurnDirectionForCamera(Vector3 direction)
        {
            return Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * direction;
        }


        private void ChangeRotation(bool state)
        {
            if (m_isLerping)
            {
                m_lerpTimer = m_lerpTime / 1.5f;
            }
            else m_lerpTimer = 0.0f;


            if (!m_isLerping)
            {
                int prevIndex = indexDirection;
                if (prevIndex + 1 == maxDirection)
                    prevIndex = -1;
                if (prevIndex - 1 == -maxDirection)
                    prevIndex = 1;

                m_prevAngle = angleValue * prevIndex;
                m_prevRot = new Vector3(0.0f, angleValue * prevIndex, 0.0f);
                m_isLerping = true;
            }

            if (state) indexDirection++;
            else indexDirection--;

            indexDirection = indexDirection % maxDirection;
            m_currentAngle = angleValue * indexDirection;
            m_nextRot = new Vector3(0.0f, angleValue * indexDirection, 0.0f);
            m_nextAngle = angleValue * indexDirection;
        }


        private void FreeRotation(float sign)
        {
            if (sign == 0) return;
            sign = Mathf.Sign(sign);
            float deltaInputMove = Time.time - timeLastRotationInput;

            if (deltaInputMove < 1)
            {
                m_angularSpeed = angularSpeedAcceleration.Evaluate(deltaInputMove);
            }
            else
            {
                m_angularSpeed = initialAngularSpeed;
            }
            float ratio = Mathf.Clamp(Mathf.Abs(m_mouseDeltaValue), m_minMouseDeltaSpeed, m_maxMouseDeltaSpeed) / m_maxMouseDeltaSpeed;

            float angularSpeed = m_maxAngularSpeed * ratio;
            m_prevAngle = m_currentAngle;
            m_prevRot = new Vector3(0.0f, m_currentAngle, 0.0f);

            m_currentAngle += sign * angularSpeed * Time.deltaTime;

            m_nextAngle = m_currentAngle;
            m_nextRot = new Vector3(0.0f, m_currentAngle, 0.0f);
            if (m_isLerping)
            {
                m_lerpTimer = m_lerpTime / 1.5f;
            }
            else m_lerpTimer = 0.0f;
        }



        private void SetCameraRotation()
        {
            m_finalRotation = m_baseAngle + Vector3.Lerp(m_prevRot, m_nextRot, m_lerpTimer / m_lerpTime);
            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalRotation += cameraEffects[i].GetEffectRot();
            }
        }
        private void SetCameraPosition()
        {
            m_finalPosition = m_targetTransform.position;
            m_finalPosition += Quaternion.Euler(0.0f, Mathf.Lerp(m_prevAngle, m_nextAngle, m_lerpTimer / m_lerpTime), 0.0f) * m_cameraDirection.normalized * m_distanceToTarget;

            for (int i = 0; i < cameraEffects.Length; i++)
            {
                m_finalPosition += cameraEffects[i].GetEffectPos();
            }

        }
        void Apply()
        {
            transform.position = m_finalPosition;
            transform.rotation = Quaternion.Euler(m_finalRotation);
        }
    }
}
