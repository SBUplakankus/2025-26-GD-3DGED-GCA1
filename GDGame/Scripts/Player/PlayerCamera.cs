using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDEngine.Core.Components;
using GDEngine.Core.Entities;
using Microsoft.Xna.Framework;

namespace GDGame.Scripts.Player
{
    public class PlayerCamera
    {
        #region Fields
        private GameObject _cameraGO;
        private Camera _camera;
        private float _cameraFOV = 0.9f;
        private readonly Vector3 _startPos = new(0, 5, 0);
        private const int FAR_PLANE_LIMIT = 1000;
        #endregion

        #region Constructors
        public PlayerCamera(GameObject parent, float aspectRatio)
        {
            _cameraGO = new GameObject(AppData.CAMERA_NAME);
            _cameraGO.Transform.TranslateTo(_startPos);

            _camera = _cameraGO.AddComponent<Camera>();
            _camera.FarPlane = FAR_PLANE_LIMIT;
            _camera.AspectRatio = aspectRatio;
            _camera.FieldOfView = _cameraFOV;

            _cameraGO.AddComponent<MouseYawPitchController>();
            _cameraGO.Transform.SetParent(parent);
        }
        #endregion

        #region Accessors
        public GameObject CameraGO => _cameraGO;
        public Camera Cam => _camera;
        #endregion

        #region Methods

        #endregion

    }
}
