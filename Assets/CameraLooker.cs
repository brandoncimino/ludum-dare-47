using Packages.BrandonUtils.Runtime;
using Packages.BrandonUtils.Runtime.Enums;

using UnityEngine;

public class CameraLooker : MonoBehaviour {
    public Camera CameraToLookAt;
    private Transform TransformToLookAt {
        get {
            if (CameraToLookAt) {
                return CameraToLookAt.transform;
            }

            if (Camera.main && Camera.main != null) {
                return Camera.main.transform;
            }

            return null;
        }
    }

    public enum Direction {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

    public Direction ForwardDirection = Direction.Forward;

    [EditorInvocationButton]
    public void LookAtCamera() {
        var fromRot = Quaternion.FromToRotation(
            //GetLocalVector3(transform, ForwardDirection),
            GetGlobalVector3(ForwardDirection),
            TransformToLookAt.position - transform.position
        );

        transform.rotation = fromRot;
    }

    private static Vector3 GetGlobalVector3(Direction direction) {
        switch (direction) {
            case Direction.Forward:
                return Vector3.forward;
            case Direction.Backward:
                return Vector3.back;
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            default:
                throw EnumUtils.InvalidEnumArgumentException(nameof(direction), direction);
        }
    }

    private Vector3 GetLocalVector3(Transform self, Direction direction) {
        switch (direction) {
            case Direction.Forward:
                return self.forward;
            case Direction.Backward:
                return -self.forward;
            case Direction.Left:
                return -self.right;
            case Direction.Right:
                return self.right;
            case Direction.Up:
                return self.up;
            case Direction.Down:
                return -self.up;
            default:
                throw EnumUtils.InvalidEnumArgumentException(nameof(direction), direction);
        }
    }
}