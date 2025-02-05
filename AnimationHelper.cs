using UnityEngine;

namespace Utils.Animation
{
    public static class AnimationHelper
    {
        public static void SetActiveObject(GameObject obj, bool state)
        {
            obj.SetActive(state);
        }

        public static void MoveObject(GameObject obj, Vector3 position)
        {
            obj.transform.position = position;
        }

        public static void ScaleObject(GameObject obj, float scale)
        {
            obj.transform.localScale = Vector3.one * scale;
        }

        public static void RotateObject(GameObject obj, float angle)
        {
            obj.transform.Rotate(Vector3.up, angle);
        }
    }
}
