using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;


namespace GameStateManagement.GameObjects
{
    class Camera
    {
        public Matrix View;
        public Matrix Projection;

        private  Vector3 cameraZPlane = new Vector3(0, 0, 10.0f);

        public Camera(Vector3 playerPosition)
        {
            View = Matrix.CreateLookAt(playerPosition + cameraZPlane, playerPosition, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);
        }

        public void Update(Vector3 playerPosition)
        {
            View = Matrix.CreateLookAt(playerPosition + cameraZPlane, playerPosition, Vector3.Up);
        }
    }
}
