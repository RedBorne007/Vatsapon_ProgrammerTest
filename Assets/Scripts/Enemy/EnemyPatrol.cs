using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyPatrol : MonoBehaviour
{
    [Tooltip("Determine to use debug mode or not")]
    [SerializeField] private bool isDebug = true;

    [Space]

    [Tooltip("Array of all patrol positions")]
    [SerializeField] private Vector3[] patrolPositions;

    private int currentIndex;

    public Vector3[] PatrolPositions => patrolPositions;
    public Vector3 Current => patrolPositions[currentIndex];

    // Function to change index to next position.
    public void Next()
    {
        if (currentIndex + 1 == patrolPositions.Length)
        {
            currentIndex = 0;
        }
        else
        {
            currentIndex++;
        }
    }

    // Function to set to the closet path from position.
    public void SetClosetPatrol(Vector3 position)
    {
        float minDistance = 99999f;

        for (int i = 0; i < patrolPositions.Length; i++)
        {
            Vector3 patrolPos = patrolPositions[i];
            float distance = Vector3.Distance(position, patrolPos);

            // If distance is shorter than the min distance, set that as min distance.
            if (distance < minDistance)
            {
                minDistance = distance;
                currentIndex = i;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyPatrol))]
public class EnemyPatrolEditor : Editor
{
    private EnemyPatrol enemyPatrol;

    private void OnEnable()
    {
        enemyPatrol = target as EnemyPatrol;
    }

    private void OnSceneGUI()
    {
        bool isDebug = serializedObject.FindProperty("isDebug").boolValue;

        if (isDebug)
        {
            Vector3[] positions = enemyPatrol.PatrolPositions;
            Handles.zTest = CompareFunction.LessEqual;

            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 startPos = positions[i];
                Vector3 endPos = i + 1 == positions.Length ? positions[0] : positions[i + 1];

                Handles.DrawDottedLine(startPos, endPos, 5f);

                EditorGUI.BeginChangeCheck();
                Vector3 pos = Handles.DoPositionHandle(positions[i], Quaternion.identity);
                Handles.Label(pos + (Vector3.up * 0.5f), $"Path #{i} {pos.x.ToString("F2")}, {pos.y.ToString("F2")}, {pos.z.ToString("F2")}");

                // If there's make changes in position handle, update value back to array.
                if (EditorGUI.EndChangeCheck())
                {
                    enemyPatrol.PatrolPositions[i] = pos;
                }
            }
        }
    }
}
#endif