using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBrain : MonoBehaviour
{
    Player player;
    public NeuralNetwork.NeuralNetwork Brain;
    public AiDecision lastDecision;
    public AiInput lastInput;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    public void SetBrain(NeuralNetwork.NeuralNetwork brain)
    {
        Brain = brain;
    }

    public AiDecision MakeDecision(AiInput input)
    {
        if (Brain == null) { Debug.Log("Brain missing"); }

        lastInput = input;

        lastDecision = (AiDecision)Brain.Think((float[])input);

        return lastDecision;
    }

    [System.Serializable]
    public struct AiInput
    {
        public float walkingCurrentDirection;
        public bool jumpOnCooldown;
        public Vector2 AIPosition;
        public float goalPosX;
        public float goalPosY;
        public float[] lookDirections;
        public bool[] isDeadZone;

        public AiInput(float n_Dir, bool n_jmpcool, Vector2 n_PlayerPosition, float n_flagx, float n_flagy, float[] ray, bool[] n_isDeadZone)
        {
            walkingCurrentDirection = n_Dir;
            jumpOnCooldown = n_jmpcool;
            AIPosition = n_PlayerPosition;
            goalPosX = n_flagx;
            goalPosY = n_flagy;
            lookDirections = ray;
            isDeadZone = n_isDeadZone;
        }

        public static explicit operator float[](AiInput obj)
        {
            List<float> converted = new List<float>();

            converted.Add(obj.walkingCurrentDirection);
            converted.Add(obj.jumpOnCooldown ? 1 : 0);
            converted.Add(obj.AIPosition.x);
            converted.Add(obj.AIPosition.y);
            converted.Add(obj.goalPosX);
            converted.Add(obj.goalPosY);
            converted.AddRange(obj.lookDirections);

            for(int i = 0; i < obj.isDeadZone.Length; i++)
            {
                if (obj.isDeadZone[i])
                {
                    converted.Add(1);
                }
                else
                {
                    converted.Add(0);
                }
            }

            return converted.ToArray();
        }
    }

    [System.Serializable]
    public struct AiDecision
    {
        public float Direction;
        public bool jumpNow;

        public static explicit operator AiDecision(float[] output)
        {
            AiDecision converted;

            converted.Direction = Mathf.Clamp(output[0], -1, 1);

            if (output[1] >= 0.5f)
            {
                converted.jumpNow = true;
            }
            else
            {
                converted.jumpNow = false;
            }

            return converted;
        }
    }
}
